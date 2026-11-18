#!/usr/bin/env python3
"""Share identical streamed texture bytes in version-locked UnityFS containers.

Texture2D/Sprite objects and animation data are retained. Only Texture2D stream
offsets, duplicate pixel extents, and the enclosing archive tables may change.
Untouched compressed blocks are copied verbatim, including existing LZ4HC.
The canonical Player build runs recompress_unityfs.py through the matching
Unity Editor after sharing and splash processing. Its full-byte verification
normalizes rewritten system-LZ4 blocks to HC without changing texture data.
"""
import argparse
import bisect
import collections
import hashlib
import json
import shutil
import struct
import sys
import tempfile
from datetime import datetime, timezone
from pathlib import Path

sys.dont_write_bytecode = True
from macos_splash_postprocess import PlayerArchive, VERSION, require


def sha_file(path):
    digest = hashlib.sha256()
    with path.open('rb') as stream:
        for data in iter(lambda: stream.read(4 * 1024 * 1024), b''):
            digest.update(data)
    return digest.hexdigest()


def serialized_objects(data):
    require(len(data) >= 64 and struct.unpack_from('>I', data, 8)[0] == 22
            and data[16] == 0 and struct.unpack_from('>Q', data, 24)[0] == len(data),
            'Unsupported serialized-file header')
    end = data.index(b'\0', 48)
    require(data[48:end].decode() == VERSION, 'Serialized-file Unity version mismatch')
    position = end + 1
    data_start = struct.unpack_from('>Q', data, 32)[0]

    def read(fmt):
        nonlocal position
        values = struct.unpack_from('<' + fmt, data, position)
        position += struct.calcsize('<' + fmt)
        return values[0] if len(values) == 1 else values

    read('i')  # target platform
    tree = read('B')
    types = []
    count = read('i')
    require(0 < count < 4096 and tree in (0, 1), 'Invalid serialized type table')
    for _ in range(count):
        class_id = read('i')
        read('B')
        read('h')
        position += 16 + (16 if class_id == 114 else 0)
        types.append(class_id)
        if tree:
            nodes, strings = read('ii')
            require(0 < nodes < 100000 and 0 <= strings < 16000000, 'Invalid type tree')
            position += nodes * 32 + strings
            dependencies = read('i')
            require(0 <= dependencies < 100000, 'Invalid type dependencies')
            position += dependencies * 4
    count = read('i')
    require(0 <= count < 1000000, 'Invalid serialized object count')
    objects = []
    for _ in range(count):
        position = (position + 3) // 4 * 4
        identifier, offset, size, type_index = read('qQIi')
        require(0 <= type_index < len(types) and data_start + offset + size <= len(data),
                'Invalid serialized object bounds')
        objects.append((identifier, types[type_index], data_start + offset, size))
    require(position <= data_start, 'Serialized object table exceeds metadata')
    return objects


def texture_record(node, identifier, data, offset, size, archive):
    body = data[offset:offset + size]
    length = struct.unpack_from('<i', body)[0]
    require(0 <= length <= 65536, 'Invalid texture name')
    name = body[4:4 + length].decode()
    prefix = (4 + length + 3) // 4 * 4
    values = struct.unpack_from('<8i', body, prefix)
    end = body.find(b'.resS')
    if end < 0:
        return None  # Inline data is kept byte-for-byte and is not shared.
    end += 5
    start = end
    while start > 0 and 32 <= body[start - 1] < 127:
        start -= 1
    require(start >= 16, 'Invalid texture streaming prefix')
    path = body[start:end].decode()
    stream = path.rsplit('/', 1)[-1]
    stream_offset, stream_size, path_size = struct.unpack_from('<QII', body, start - 16)
    require(path_size == len(path) and stream in archive.nodes and stream_size > 0
            and stream_offset + stream_size <= archive.nodes[stream][1],
            'Invalid texture streaming bounds')
    require(values[2] > 0 and values[3] > 0 and values[4] == stream_size,
            'Unexpected texture payload layout')
    # Ignore the name and physical location only. Every sampling/format flag stays in the key.
    settings = body[prefix:start - 16] + struct.pack('<I', stream_size) + body[(end + 3) // 4 * 4:]
    return dict(key=node + ':' + str(identifier), node=node, identifier=identifier,
                name=name, width=values[2], height=values[3], format=values[6],
                stream=stream, offset=stream_offset, size=stream_size,
                offsetField=offset + start - 16,
                settingsHash=hashlib.sha256(settings).hexdigest())


def decoded_blocks(archive, start, end):
    with archive.path.open('rb') as source:
        for logical, physical, raw, packed, flags in archive.blocks:
            if logical + raw <= start or logical >= end:
                continue
            source.seek(physical)
            yield logical, archive.codec.decode(source.read(packed), raw, flags & 0x3f)


def inventory(path):
    archive = PlayerArchive(path)
    textures = []
    metadata = {}
    classes = collections.Counter()
    for node in archive.nodes:
        if node.endswith('.resS'):
            continue
        data = archive.read_node(node)
        normalized = bytearray(data)
        for identifier, class_id, offset, size in serialized_objects(data):
            classes[class_id] += 1
            if class_id == 28:
                texture = texture_record(node, identifier, data, offset, size, archive)
                if texture:
                    textures.append(texture)
                    field = texture['offsetField']
                    normalized[field:field + 8] = bytes(8)
        metadata[node] = dict(size=len(data), normalizedSha256=hashlib.sha256(normalized).hexdigest())
    require(classes[43] == 0, 'Mesh streaming is not supported by this texture-only optimizer')
    by_stream = collections.defaultdict(lambda: collections.defaultdict(list))
    for texture in textures:
        by_stream[texture['stream']][(texture['offset'], texture['size'])].append(texture)
    for stream, entries in by_stream.items():
        regions = sorted(entries)
        require(all(first + size <= following for (first, size), (following, _) in zip(regions, regions[1:])),
                'Partially overlapping texture extents are unsupported')
        start, length = archive.nodes[stream]
        index = 0
        buffer = bytearray()
        for logical, data in decoded_blocks(archive, start, start + length):
            cursor = max(0, start - logical)
            end = min(len(data), start + length - logical)
            while cursor < end and index < len(regions):
                offset, size = regions[index]
                absolute = logical + cursor - start
                if absolute < offset:
                    cursor += min(end - cursor, offset - absolute)
                    continue
                take = min(end - cursor, offset + size - absolute)
                require(take > 0, 'Unexpected texture extent order')
                buffer.extend(data[cursor:cursor + take])
                cursor += take
                if len(buffer) == size:
                    digest = hashlib.sha256(buffer).hexdigest()
                    for texture in entries[(offset, size)]:
                        texture['pixelSha256'] = digest
                    index += 1
                    buffer.clear()
        require(index == len(regions) and not buffer, 'Incomplete texture inventory')
    return archive, textures, metadata, dict(classes)


class RemovedRanges:
    def __init__(self, ranges):
        self.ranges = sorted(ranges)
        require(all(end <= following for (_, end), (following, _) in zip(self.ranges, self.ranges[1:])),
                'Removal ranges overlap')
        self.starts = [start for start, _ in self.ranges]
        self.ends = [end for _, end in self.ranges]
        self.prefix = [0]
        for start, end in self.ranges:
            require(start < end, 'Empty removal range')
            self.prefix.append(self.prefix[-1] + end - start)

    def before(self, offset):
        return self.prefix[bisect.bisect_right(self.ends, offset)]

    def intersecting(self, start, end):
        first = bisect.bisect_right(self.ends, start)
        last = bisect.bisect_left(self.starts, end)
        return self.ranges[first:last]


def plan(archive, textures):
    groups = collections.defaultdict(list)
    for texture in textures:
        key = (texture['stream'], texture['size'], texture['pixelSha256'], texture['settingsHash'])
        groups[key].append(texture)
    canonical = {}
    retained = collections.defaultdict(set)
    all_regions = collections.defaultdict(set)
    for members in groups.values():
        first = min(members, key=lambda t: (t['offset'], t['key']))
        for texture in members:
            canonical[texture['key']] = first
            all_regions[texture['stream']].add((texture['offset'], texture['offset'] + texture['size']))
        retained[first['stream']].add((first['offset'], first['offset'] + first['size']))
    local = {stream: RemovedRanges(regions - retained[stream]) for stream, regions in all_regions.items()}
    global_ranges = []
    for stream, removed in local.items():
        start = archive.nodes[stream][0]
        global_ranges.extend((start + first, start + last) for first, last in removed.ranges)
    removed = RemovedRanges(global_ranges)
    patches = []
    for texture in textures:
        first = canonical[texture['key']]
        new_offset = first['offset'] - local[first['stream']].before(first['offset'])
        if texture['offset'] != new_offset:
            field = archive.nodes[texture['node']][0] + texture['offsetField']
            patches.append((field, struct.pack('<Q', new_offset)))
    patches.sort()
    return removed, patches, sum(len(g) > 1 for g in groups.values())


def directory_rows(archive, removed):
    data = archive.directory
    count = struct.unpack_from('>I', data)[0]
    position = 4
    result = bytearray(struct.pack('>I', count))
    for _ in range(count):
        offset, size, flags = struct.unpack_from('>QQI', data, position)
        position += 20
        end = data.index(b'\0', position)
        name = data[position:end]
        new_offset = offset - removed.before(offset)
        new_size = size - removed.before(offset + size) + removed.before(offset)
        result.extend(struct.pack('>QQI', new_offset, new_size, flags) + name + b'\0')
        position = end + 1
    require(position == len(data), 'Unexpected node directory suffix')
    return bytes(result)


def rewrite(archive, output, removed, patches):
    patch_starts = [offset for offset, _ in patches]
    blocks = []
    copied = changed = dropped = 0
    with tempfile.TemporaryFile(dir=output.parent) as payloads, archive.path.open('rb') as source:
        for logical, physical, raw, packed, flags in archive.blocks:
            end = logical + raw
            deletions = removed.intersecting(logical, end)
            first = max(0, bisect.bisect_left(patch_starts, logical) - 1)
            last = bisect.bisect_left(patch_starts, end)
            edits = [(offset, value) for offset, value in patches[first:last] if offset + 8 > logical]
            source.seek(physical)
            encoded = source.read(packed)
            require(len(encoded) == packed, 'Truncated source block')
            if not deletions and not edits:
                payloads.write(encoded)
                blocks.append((raw, packed, flags))
                copied += 1
                continue
            data = bytearray(archive.codec.decode(encoded, raw, flags & 0x3f))
            for offset, value in edits:
                a, b = max(offset, logical), min(offset + 8, end)
                data[a - logical:b - logical] = value[a - offset:b - offset]
            chunks = []
            position = 0
            for start, stop in deletions:
                a, b = max(0, start - logical), min(raw, stop - logical)
                chunks.append(data[position:a])
                position = b
            chunks.append(data[position:])
            data = b''.join(chunks)
            if not data:
                dropped += 1
                continue
            # libcompression's raw encoder declines very short tails. UnityFS
            # permits an uncompressed block, preserving those bytes exactly.
            encoded = archive.codec.encode(data) if len(data) >= 256 else data
            kind = 2 if len(data) >= 256 and len(encoded) < len(data) else 0
            encoded = encoded if kind else data
            payloads.write(encoded)
            blocks.append((len(data), len(encoded), flags & ~0x3f | kind))
            changed += 1
        table = bytes(16) + struct.pack('>I', len(blocks))
        table += b''.join(struct.pack('>IIH', *block) for block in blocks)
        table += directory_rows(archive, removed)
        encoded_table = archive.codec.encode(table)
        flags = archive.flags & ~0x3f | 2
        header = bytearray(archive.header)
        struct.pack_into('>QIII', header, archive.fields_offset, 0, len(encoded_table), len(table), flags)
        with output.open('xb') as target:
            target.write(header)
            target.write(bytes(archive.aligned_header_size - len(header)))
            if not flags & 0x80:
                target.write(encoded_table)
            if flags & 0x200:
                target.write(bytes((-target.tell()) % 16))
            payloads.seek(0)
            shutil.copyfileobj(payloads, target, 4 * 1024 * 1024)
            if flags & 0x80:
                target.write(encoded_table)
            length = target.tell()
            target.seek(archive.fields_offset)
            target.write(struct.pack('>Q', length))
    return dict(copiedBlocks=copied, rewrittenBlocks=changed, removedBlocks=dropped)


def equivalent(before, after):
    _, old_textures, old_metadata, old_classes = before
    _, new_textures, new_metadata, new_classes = after
    require(old_metadata == new_metadata, 'Non-offset serialized data changed')
    require(old_classes == new_classes, 'Serialized object counts changed')
    def normalize(textures):
        return {t['key']: {k: v for k, v in t.items() if k not in ('offset', 'offsetField')}
                for t in textures}
    require(normalize(old_textures) == normalize(new_textures), 'Texture pixels or properties changed')


def optimize(source, output, report):
    require(source.is_file() and source.resolve() != output.resolve() and not output.exists(),
            'Input/output must be distinct and output must not exist')
    output.parent.mkdir(parents=True, exist_ok=True)
    before = inventory(source)
    archive, textures, metadata, classes = before
    removed, patches, groups = plan(archive, textures)
    temporary = output.with_name(output.name + '.verifying')
    require(not temporary.exists(), 'Unfinished optimizer output exists')
    try:
        if removed.ranges:
            blocks = rewrite(archive, temporary, removed, patches)
            after = inventory(temporary)
            equivalent(before, after)
            remaining, _, _ = plan(after[0], after[1])
            require(not remaining.ranges, 'Optimization is not idempotent')
        else:
            shutil.copyfile(source, temporary)
            blocks = dict(copiedBlocks=len(archive.blocks), rewrittenBlocks=0, removedBlocks=0)
        result = dict(status='passed', unity=VERSION, input=str(source.resolve()),
                      output=str(output.resolve()), beforeBytes=source.stat().st_size,
                      afterBytes=temporary.stat().st_size, rawBytesShared=removed.prefix[-1],
                      duplicateGroups=groups, physicalExtentsRemoved=len(removed.ranges),
                      textureCount=len(textures), objectClasses=classes, offsetFieldsChanged=len(patches),
                      inputSHA256=sha_file(source), outputSHA256=sha_file(temporary),
                      normalizedMetadata=metadata, textures=textures, blocks=blocks)
        temporary.replace(output)
        report.parent.mkdir(parents=True, exist_ok=True)
        report.write_text(json.dumps(result, indent=2) + '\n')
        print('SWORDSMAN_TEXTURE_STORAGE passed textures=%d rawShared=%d saved=%d report=%s' %
              (len(textures), removed.prefix[-1], result['beforeBytes'] - result['afterBytes'], report), flush=True)
        return result
    finally:
        temporary.unlink(missing_ok=True)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--input', type=Path)
    parser.add_argument('--output', type=Path)
    parser.add_argument('--report', type=Path)
    parser.add_argument('--app', type=Path)
    parser.add_argument('--evidence', type=Path)
    args = parser.parse_args()
    if args.app:
        require(args.evidence is not None and not any((args.input, args.output, args.report)),
                'App mode requires evidence and cannot be combined with file mode')
        stamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%fZ')
        evidence = args.evidence.resolve() / (args.app.stem + '-' + stamp)
        data = args.app.resolve() / 'Contents/Resources/Data'
        inputs = [data / 'data.unity3d', data / 'StreamingAssets/SwordsmanHD4X/swordsman-hd4x']
        outputs = [evidence / 'work' / path.name for path in inputs]
        results = [optimize(source, output, evidence / (source.name + '.json'))
                   for source, output in zip(inputs, outputs)]
        # Both archives are fully verified before either built application file is replaced.
        for source, output, result in zip(inputs, outputs, results):
            require(sha_file(source) == result['inputSHA256'], 'Application input changed during verification')
        for source, output in zip(inputs, outputs):
            output.replace(source)
        print('SWORDSMAN_TEXTURE_STORAGE_APP passed saved=%d evidence=%s' %
              (sum(r['beforeBytes'] - r['afterBytes'] for r in results), evidence), flush=True)
    else:
        require(all((args.input, args.output, args.report)) and args.evidence is None,
                'File mode requires input, output, and report')
        optimize(args.input.resolve(), args.output.resolve(), args.report.resolve())


if __name__ == '__main__':
    main()
