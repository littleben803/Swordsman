#!/usr/bin/env python3
"""Version-locked macOS build postprocessing; no third-party dependencies.

Read object locations with the matching Editor's binary2text. Decode only the
verified 2021.3.16f1c1 field prefixes; never reuse offsets from another build.
Compressed Players use macOS libcompression to rewrite only affected LZ4 blocks.
The canonical Player builder subsequently normalizes both archives to LZ4HC
with the matching Editor's native codec, verifies decoded bytes, and re-signs.
"""
import argparse
import ctypes
import hashlib
import json
import math
import re
import struct
import subprocess
import tempfile
from datetime import datetime, timezone
from pathlib import Path

VERSION = '2021.3.16f1c1'
SCENES = ['Assets/Scenes/level%d.unity' % i for i in range(4)]


def require(condition, message):
    if not condition:
        raise ValueError(message)


class SystemLz4:
    """Raw LZ4 via macOS libcompression; no third-party Python dependency."""
    def __init__(self):
        self.library = ctypes.CDLL('/usr/lib/libcompression.dylib')
        for name in ('compression_encode_buffer', 'compression_decode_buffer'):
            function = getattr(self.library, name)
            function.argtypes = [ctypes.c_void_p, ctypes.c_size_t, ctypes.c_char_p,
                                 ctypes.c_size_t, ctypes.c_void_p, ctypes.c_int]
            function.restype = ctypes.c_size_t

    def decode(self, data, size, kind):
        if kind == 0:
            require(len(data) == size, 'Uncompressed archive block size mismatch')
            return data
        require(kind in (2, 3), 'Only uncompressed/LZ4 UnityFS blocks are supported')
        output = ctypes.create_string_buffer(size)
        actual = self.library.compression_decode_buffer(output, size, data, len(data), None, 0x101)
        require(actual == size, 'LZ4 archive block decode failed')
        return output.raw

    def encode(self, data):
        output = ctypes.create_string_buffer(len(data) + len(data) // 255 + 32)
        size = self.library.compression_encode_buffer(output, len(output), data, len(data), None, 0x101)
        require(size > 0, 'LZ4 archive block encode failed')
        return output.raw[:size]


class PlayerArchive:
    """Read UnityFS nodes and replace one same-size node, copying other blocks verbatim."""
    def __init__(self, path):
        self.path = path
        self.codec = SystemLz4()
        with path.open('rb') as stream:
            def string():
                result = bytearray()
                while len(result) < 4096:
                    value = stream.read(1)
                    require(value, 'Truncated UnityFS header')
                    if value == b'\0':
                        return bytes(result)
                    result.extend(value)
                raise ValueError('UnityFS header string exceeds limit')
            require(string() == b'UnityFS', 'Unsupported compressed Player signature')
            version = struct.unpack('>I', stream.read(4))[0]
            require(version in (6, 7, 8), 'Unsupported UnityFS version')
            string()
            require(string().decode() == VERSION, 'Compressed Player Unity version mismatch')
            self.fields_offset = stream.tell()
            size, compressed, uncompressed, self.flags = struct.unpack('>QIII', stream.read(20))
            require(size == path.stat().st_size and self.flags & 0x40,
                    'Invalid UnityFS size or non-combined block table')
            self.header_size = stream.tell()
            stream.seek(0)
            self.header = stream.read(self.header_size)
            if version >= 7:
                stream.seek((stream.tell() + 15) // 16 * 16)
            self.aligned_header_size = stream.tell()
            table_start = size - compressed if self.flags & 0x80 else stream.tell()
            stream.seek(table_start)
            table = self.codec.decode(stream.read(compressed), uncompressed, self.flags & 0x3f)
            self.data_start = self.aligned_header_size if self.flags & 0x80 else stream.tell()
            if self.flags & 0x200:
                self.data_start = (self.data_start + 15) // 16 * 16
        require(table[:16] == bytes(16), 'Nonzero UnityFS content hash is unsupported')
        count = struct.unpack_from('>I', table, 16)[0]
        require(0 < count < 1000000, 'Invalid UnityFS block count')
        self.blocks = []
        position = 20
        logical = 0
        physical = self.data_start
        for _ in range(count):
            raw, packed, flags = struct.unpack_from('>IIH', table, position)
            require(0 < raw <= 16 * 1024 * 1024 and packed > 0 and flags & 0x3f in (0, 2, 3),
                    'Invalid or unsupported UnityFS block')
            self.blocks.append((logical, physical, raw, packed, flags))
            logical += raw
            physical += packed
            position += 10
        require(physical <= (table_start if self.flags & 0x80 else size), 'Archive blocks exceed file')
        self.directory = table[position:]
        node_count = struct.unpack_from('>I', table, position)[0]
        position += 4
        self.nodes = {}
        for _ in range(node_count):
            offset, length, flags = struct.unpack_from('>QQI', table, position)
            position += 20
            end = table.index(b'\0', position)
            name = table[position:end].decode()
            require(name not in self.nodes and offset + length <= logical, 'Invalid archive node')
            self.nodes[name] = (offset, length)
            position = end + 1
        require(position == len(table), 'Unexpected UnityFS directory suffix')

    def read_node(self, name):
        offset, length = self.nodes[name]
        result = bytearray()
        with self.path.open('rb') as stream:
            for logical, physical, raw, packed, flags in self.blocks:
                if logical < offset + length and logical + raw > offset:
                    stream.seek(physical)
                    block = self.codec.decode(stream.read(packed), raw, flags & 0x3f)
                    result.extend(block[max(0, offset - logical):min(raw, offset + length - logical)])
        require(len(result) == length, 'Archive node read size mismatch')
        return bytes(result)

    def replace_node(self, name, content, evidence):
        offset, length = self.nodes[name]
        require(len(content) == length, 'Archive node replacement must preserve length')
        replacements = {}
        records = []
        with self.path.open('rb') as source:
            for index, (logical, physical, raw, packed, flags) in enumerate(self.blocks):
                if logical >= offset + length or logical + raw <= offset:
                    continue
                source.seek(physical)
                original = self.codec.decode(source.read(packed), raw, flags & 0x3f)
                first, last = max(0, offset - logical), min(raw, offset + length - logical)
                changed = original[:first] + content[logical + first - offset:logical + last - offset] + original[last:]
                if changed == original:
                    continue
                require(changed[:first] == original[:first] and changed[last:] == original[last:],
                        'Archive replacement changed another node')
                encoded = self.codec.encode(changed)
                kind = 2 if len(encoded) < raw else 0
                payload = encoded if kind else changed
                require(self.codec.decode(payload, raw, kind) == changed, 'LZ4 round-trip mismatch')
                replacements[index] = (payload, flags & ~0x3f | kind)
                source_bytes = self.read_block(index)
                (evidence / ('archive-block-%d.before' % index)).write_bytes(source_bytes)
                records.append(dict(index=index, logicalOffset=logical, rawSize=raw,
                                    beforeCompressedSize=len(source_bytes), afterCompressedSize=len(payload),
                                    changedBytes=sum(a != b for a, b in zip(original, changed))))
        table = bytearray(bytes(16) + struct.pack('>I', len(self.blocks)))
        for index, (_, _, raw, packed, flags) in enumerate(self.blocks):
            if index in replacements:
                payload, flags = replacements[index]
                packed = len(payload)
            table.extend(struct.pack('>IIH', raw, packed, flags))
        table.extend(self.directory)
        encoded_table = self.codec.encode(bytes(table))
        flags = self.flags & ~0x3f | 2
        header = bytearray(self.header)
        struct.pack_into('>QIII', header, self.fields_offset, 0, len(encoded_table), len(table), flags)
        temporary = self.path.with_name(self.path.name + '.postprocess-tmp')
        require(not temporary.exists(), 'Archive temporary output already exists')
        try:
            with self.path.open('rb') as source, temporary.open('xb') as target:
                target.write(header)
                target.write(bytes(self.aligned_header_size - len(header)))
                if not flags & 0x80:
                    target.write(encoded_table)
                if flags & 0x200:
                    target.write(bytes((-target.tell()) % 16))
                for index in range(len(self.blocks)):
                    target.write(replacements[index][0] if index in replacements else self.read_block(index, source))
                if flags & 0x80:
                    target.write(encoded_table)
                final_size = target.tell()
                target.seek(self.fields_offset)
                target.write(struct.pack('>Q', final_size))
            require(PlayerArchive(temporary).read_node(name) == content, 'Rewritten archive verification failed')
            temporary.replace(self.path)
        finally:
            temporary.unlink(missing_ok=True)
        return records

    def read_block(self, index, stream=None):
        if stream is None:
            with self.path.open('rb') as source:
                return self.read_block(index, source)
        _, physical, _, packed, _ = self.blocks[index]
        stream.seek(physical)
        data = stream.read(packed)
        require(len(data) == packed, 'Truncated archive block payload')
        return data


class Reader:
    def __init__(self, data, start, end):
        self.data, self.pos, self.end = data, start, end

    def take(self, count):
        require(count >= 0 and self.pos + count <= self.end, 'Object prefix exceeds its boundary')
        start = self.pos
        self.pos += count
        return self.data[start:self.pos]

    def align(self):
        self.take((-self.pos) % 4)

    def integer(self):
        return struct.unpack('<i', self.take(4))[0]

    def string(self):
        size = self.integer()
        require(0 <= size <= 65536, 'Invalid string length')
        value = self.take(size).decode('utf-8')
        self.align()
        return value

    def strings(self):
        count = self.integer()
        require(0 <= count <= 1024, 'Invalid string array length')
        result = [self.string() for _ in range(count)]
        self.align()
        return result


def locate_fields(data, objects):
    require(len(data) >= 64 and struct.unpack_from('>I', data, 8)[0] == 22,
            'Unsupported serialized-file format')
    require(data[16] == 0, 'Only little-endian player data is supported')
    require(struct.unpack_from('>Q', data, 24)[0] == len(data), 'Serialized-file size mismatch')
    version_end = data.index(b'\0', 48)
    require(data[48:version_end].decode('ascii') == VERSION, 'Unsupported Unity version')
    data_start = struct.unpack_from('>Q', data, 32)[0]

    def reader(class_id):
        matches = [offset for cls, offset in objects if cls == class_id]
        require(len(matches) == 1, 'Expected exactly one object for class %d' % class_id)
        start = matches[0]
        end = min([offset for _, offset in objects if offset > start] + [len(data)])
        require(data_start <= start < end <= len(data), 'Invalid object bounds')
        return Reader(data, start, end)

    player = reader(129)
    player.take(16)  # productGUID
    require(all(v in (0, 1) for v in player.take(3)), 'Unexpected Android bool prefix')
    player.align()
    require(player.integer() in range(6), 'Unexpected orientation prefix')
    require(player.integer() in range(4), 'Unexpected target-device prefix')
    require(player.take(1)[0] in (0, 1), 'Unexpected on-demand prefix')
    player.align()
    require(0 <= player.integer() <= 1000, 'Unexpected accelerometer prefix')
    player.string()  # companyName
    require(player.string() == 'Swordsman', 'Unexpected product: refusing unrelated app')
    player.take(12 + 8)  # defaultCursor + cursorHotspot
    colors = struct.unpack('<4f', player.take(16))
    require(all(math.isfinite(v) and 0 <= v <= 1 for v in colors), 'Invalid splash color')
    show_screen = player.pos
    require(player.take(1)[0] in (0, 1), 'Invalid splash-screen bool')
    show_logo = player.pos
    require(player.take(1)[0] in (0, 1), 'Invalid splash-logo bool')
    player.align()
    opacity, animation, style, draw_mode = struct.unpack('<fiii', player.take(16))
    require(math.isfinite(opacity) and 0 <= opacity <= 1 and animation in (0, 1)
            and style in (0, 1) and draw_mode in (0, 1), 'Unexpected splash prefix layout')

    build = reader(141)
    require(build.strings() == SCENES, 'Scene order differs from the supported four-scene build')
    build.strings()  # preloadedPlugins
    build.strings()  # enabledVRDevices
    build.strings()  # buildTags
    build.take(16)  # buildGUID
    pro = build.pos
    flags = build.take(16)
    require(all(v in (0, 1) for v in flags), 'Unexpected build bool prefix')
    require(build.string() == VERSION, 'BuildSettings layout/version mismatch')
    return [('m_ShowUnitySplashScreen', show_screen, 0),
            ('m_ShowUnitySplashLogo', show_logo, 0), ('BuildSettings.hasPROVersion', pro, 1)]


def run(args):
    app = args.app.resolve()
    data_folder = app / 'Contents/Resources/Data'
    path = data_folder / 'globalgamemanagers'
    archive_path = data_folder / 'data.unity3d'
    require(path.is_file() != archive_path.is_file(), 'Expected exactly one loose/compressed Player layout')
    archive = PlayerArchive(archive_path) if archive_path.is_file() else None
    original = archive.read_node('globalgamemanagers') if archive else path.read_bytes()
    stamp = datetime.now(timezone.utc).strftime('%Y%m%dT%H%M%S%fZ')
    evidence = args.evidence.resolve() / (app.stem + '-' + stamp)
    evidence.mkdir(parents=True)
    dump = evidence / 'objects.txt'
    with tempfile.TemporaryDirectory(prefix='inspect-', dir=evidence) as temporary:
        inspect_path = Path(temporary) / 'globalgamemanagers'
        inspect_path.write_bytes(original)
        subprocess.run([str(args.binary2text), str(inspect_path), str(dump), '-detailed'], check=True,
                       stdout=subprocess.PIPE, stderr=subprocess.PIPE, timeout=60)
    objects = [(int(cls), int(offset)) for cls, offset in re.findall(
        r'Object #\d+ \(ClassID: (\d+)\) at byte (\d+) without type tree', dump.read_text())]
    fields = locate_fields(original, objects)
    state = tuple(original[offset] for _, offset, _ in fields)
    require(state in ((1, 1, 0), (0, 0, 1)), 'Unexpected partial field state: refusing to patch')
    if args.verify_only:
        require(state == (0, 0, 1), 'Splash fields are not in the expected final state')
        subprocess.run(['/usr/bin/codesign', '--verify', '--deep', '--strict', str(app)],
                       check=True, timeout=300)
        print('SWORDSMAN_MAC_SPLASH verified output=' + str(app), flush=True)
        return

    patched = bytearray(original)
    changes = []
    for name, offset, target in fields:
        changes.append(dict(field=name, offset=offset, before=patched[offset], after=target))
        patched[offset] = target
    expected = {offset for _, offset, target in fields if original[offset] != target}
    require({i for i, (a, b) in enumerate(zip(original, patched)) if a != b} == expected,
            'Unexpected byte changes')
    (evidence / 'globalgamemanagers.before').write_bytes(original)
    record = dict(unity=VERSION, app=str(app), changes=changes, size=len(original),
                  layout='unityfs-lz4' if archive else 'loose',
                  before_sha256=hashlib.sha256(original).hexdigest(),
                  after_sha256=hashlib.sha256(patched).hexdigest(), status='pending')
    report = evidence / 'result.json'
    report.write_text(json.dumps(record, indent=2) + '\n')
    try:
        if archive:
            record['archiveChangedBlocks'] = archive.replace_node('globalgamemanagers', patched, evidence)
            require(sum(block['changedBytes'] for block in record['archiveChangedBlocks']) == len(expected),
                    'Unexpected archive logical byte changes')
        else:
            path.write_bytes(patched)
        subprocess.run(['/usr/bin/codesign', '--force', '--sign', '-', str(app)],
                       check=True, timeout=300)
        subprocess.run(['/usr/bin/codesign', '--verify', '--deep', '--strict', str(app)],
                       check=True, timeout=300)
        final = PlayerArchive(archive_path).read_node('globalgamemanagers') if archive else path.read_bytes()
        require(final == patched, 'Final data verification failed')
    except Exception:
        if archive:
            rollback = evidence / 'rollback'
            rollback.mkdir(exist_ok=True)
            PlayerArchive(archive_path).replace_node('globalgamemanagers', original, rollback)
        else:
            path.write_bytes(original)
        record['status'] = 'failed-original-data-restored-do-not-distribute'
        report.write_text(json.dumps(record, indent=2) + '\n')
        raise
    record['status'] = 'passed'
    report.write_text(json.dumps(record, indent=2) + '\n')
    print('SWORDSMAN_MAC_SPLASH patched bytes=%d evidence=%s' % (len(expected), report), flush=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--app', type=Path, required=True)
    parser.add_argument('--binary2text', type=Path, required=True)
    parser.add_argument('--evidence', type=Path, required=True)
    parser.add_argument('--verify-only', action='store_true')
    run(parser.parse_args())
