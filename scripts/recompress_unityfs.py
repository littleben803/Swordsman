#!/usr/bin/env python3
"""Recompress UnityFS with the matching Editor codec; verify every decoded byte."""
import argparse
import hashlib
import json
import shutil
import struct
import sys
import tempfile
from pathlib import Path

sys.dont_write_bytecode = True
from macos_splash_postprocess import PlayerArchive, require
from share_texture_storage import sha_file


def native_hc(data):
    sys.stdout.buffer.write(struct.pack('<I', len(data)) + data)
    sys.stdout.buffer.flush()
    header = sys.stdin.buffer.read(4)
    require(len(header) == 4, 'Unity HC worker closed before response')
    size = struct.unpack('<I', header)[0]
    require(0 < size <= len(data) + len(data) // 255 + 32, 'Invalid HC response size')
    result = sys.stdin.buffer.read(size)
    require(len(result) == size, 'Truncated HC response')
    return result


def recompress(archive, output):
    blocks = []
    with tempfile.TemporaryFile(dir=output.parent) as payloads, archive.path.open('rb') as source:
        for _, physical, raw, packed, flags in archive.blocks:
            source.seek(physical)
            original = source.read(packed)
            require(len(original) == packed, 'Truncated original block')
            kind = flags & 0x3f
            if kind == 3:
                payload = original
            else:
                decoded = archive.codec.decode(original, raw, kind)
                encoded = native_hc(decoded)
                kind = 3 if len(encoded) < raw else 0
                payload = encoded if kind else decoded
                require(archive.codec.decode(payload, raw, kind) == decoded, 'HC block round-trip mismatch')
            payloads.write(payload)
            blocks.append((raw, len(payload), flags & ~0x3f | kind))
        table = bytes(16) + struct.pack('>I', len(blocks))
        table += b''.join(struct.pack('>IIH', *block) for block in blocks) + archive.directory
        packed_table = native_hc(table)
        flags = archive.flags & ~0x3f | 3
        header = bytearray(archive.header)
        struct.pack_into('>QIII', header, archive.fields_offset, 0, len(packed_table), len(table), flags)
        with output.open('xb') as target:
            target.write(header)
            target.write(bytes(archive.aligned_header_size - len(header)))
            if not flags & 0x80:
                target.write(packed_table)
            if flags & 0x200:
                target.write(bytes((-target.tell()) % 16))
            payloads.seek(0)
            shutil.copyfileobj(payloads, target, 4 * 1024 * 1024)
            if flags & 0x80:
                target.write(packed_table)
            size = target.tell()
            target.seek(archive.fields_offset)
            target.write(struct.pack('>Q', size))


def content(archive):
    digest = hashlib.sha256()
    kinds = {}
    size = 0
    with archive.path.open('rb') as stream:
        for _, physical, raw, packed, flags in archive.blocks:
            stream.seek(physical)
            decoded = archive.codec.decode(stream.read(packed), raw, flags & 0x3f)
            digest.update(decoded)
            size += len(decoded)
            kind = str(flags & 0x3f)
            kinds[kind] = kinds.get(kind, 0) + 1
    return dict(decodedBytes=size, decodedSHA256=digest.hexdigest(), blockKinds=kinds)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--input', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--report', type=Path, required=True)
    parser.add_argument('--replace', action='store_true')
    parser.add_argument('--unity-native-hc', action='store_true', required=True,
                        help='Binary stdio protocol; invoke through SwordsmanMacBuild only.')
    args = parser.parse_args()
    source, target = args.input.resolve(), args.output.resolve()
    require(source != target, 'Recompression verification requires distinct archives')
    require(not target.exists(), 'Unfinished recompression output exists')
    before_sha = sha_file(source)
    before = PlayerArchive(source)
    recompress(before, target)
    # No more native requests. All remaining work is independent byte verification.
    sys.stdout.buffer.write(struct.pack('<I', 0))
    sys.stdout.buffer.flush()
    after_sha = sha_file(target)
    after = PlayerArchive(target)
    require(before.nodes == after.nodes, 'Recompression changed node names, offsets or sizes')
    # Directory equality also covers node flags and ordering, beyond the name map.
    require(before.directory == after.directory, 'Recompression changed the node directory')
    a, b = content(before), content(after)
    require(a['decodedBytes'] == b['decodedBytes'] and a['decodedSHA256'] == b['decodedSHA256'],
            'Recompression changed decoded archive bytes')
    require(set(b['blockKinds']) <= {'0', '3'}, 'Final archive contains non-HC compressed blocks')
    require(sha_file(source) == before_sha and sha_file(target) == after_sha,
            'Archive changed during verification')
    result = dict(status='passed', input=str(source), output=str(target),
                  beforeBytes=source.stat().st_size, afterBytes=target.stat().st_size,
                  beforeSHA256=before_sha, afterSHA256=after_sha,
                  nodes=len(before.nodes), before=a, after=b, replaced=args.replace)
    args.report.parent.mkdir(parents=True, exist_ok=True)
    args.report.write_text(json.dumps(result, indent=2) + '\n')
    if args.replace:
        target.replace(source)
    print('SWORDSMAN_LZ4HC_VERIFIED nodes=%d decodedBytes=%d saved=%d report=%s' %
          (result['nodes'], a['decodedBytes'], result['beforeBytes'] - result['afterBytes'], args.report), file=sys.stderr, flush=True)


if __name__ == '__main__':
    main()
