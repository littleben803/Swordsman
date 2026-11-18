#!/usr/bin/env python3
"""Offline experiment only: system-LZFSE blocks exposing an uncompressed UnityFS view.

Never writes a source Bundle or application. Outputs must stay outside Assets.
Each block falls back to the original LZ4/raw bytes if LZFSE is larger.
"""
import argparse
import bisect
import ctypes
import hashlib
import json
import random
import platform
import resource
import struct
import sys
import time
from collections import OrderedDict
from pathlib import Path

sys.dont_write_bytecode = True
from macos_splash_postprocess import PlayerArchive, SystemLz4, require
from share_texture_storage import sha_file

MAGIC = b'SWLZF001'
HEADER = struct.Struct('<8siiqq')
ROW = struct.Struct('<qqiiii')


class ExperimentStream:
    def __init__(self, path, archive, prefix, index):
        self.file = path.open('rb')
        self.archive, self.prefix, self.index = archive, prefix, index
        self.starts = [r[0] for r in index]
        self.cache = OrderedDict()

    def block(self, i):
        if i in self.cache:
            self.cache.move_to_end(i)
            return self.cache[i]
        _, physical, raw, packed, codec, _ = self.index[i]
        self.file.seek(physical)
        b = self.file.read(packed)
        if codec == 1:
            output = ctypes.create_string_buffer(raw)
            n = self.archive.codec.library.compression_decode_buffer(output, raw, b, len(b), None, 0x801)
            require(n == raw, 'LZFSE block decode failed')
            b = output.raw
        else:
            b = self.archive.codec.decode(b, raw, 2 if codec == 2 else 0)
        self.cache[i] = b
        while len(self.cache) > 128:
            self.cache.popitem(last=False)
        return b

    def read(self, offset, length):
        result = bytearray()
        if offset < len(self.prefix):
            n = min(length, len(self.prefix) - offset)
            result.extend(self.prefix[offset:offset + n])
            offset, length = offset + n, length - n
        while length and offset < self.starts[-1] + self.index[-1][2]:
            i = bisect.bisect_right(self.starts, offset) - 1
            r = self.index[i]
            n = min(length, r[0] + r[2] - offset)
            require(n > 0, 'Invalid virtual block mapping')
            result.extend(self.block(i)[offset - r[0]:offset - r[0] + n])
            offset, length = offset + n, length - n
        return bytes(result)


def virtual_prefix(a):
    table = bytes(16) + struct.pack('>I', len(a.blocks))
    table += b''.join(struct.pack('>IIH', raw, raw, flags & ~63) for _, _, raw, _, flags in a.blocks)
    table += a.directory
    prefix_size = a.aligned_header_size + len(table)
    length = prefix_size + sum(b[2] for b in a.blocks)
    header = bytearray(a.header)
    struct.pack_into('>QIII', header, a.fields_offset, length, len(table), len(table), 0x40)
    return bytes(header) + bytes(a.aligned_header_size - len(header)) + table, length


def build(args):
    started = time.monotonic()
    source, output = args.input.resolve(), args.output.resolve()
    root = Path(__file__).resolve().parents[1]
    canonical = root / 'Builds/macOS/Swordsman-ChapterTests-AllChapters.app/Contents/Resources/Data/StreamingAssets/SwordsmanHD4X'
    experimental = output.is_relative_to(root / 'Builds/CompressionValidation')
    production = args.production and output.parent == canonical and output.name == 'swordsman-hd4x.swlzfse' and source.parent == canonical and source.name == 'swordsman-hd4x'
    require(experimental or production, 'Output must be an experiment or the canonical macOS app Bundle')
    require(args.report.resolve().is_relative_to(root / 'Logs'), 'Report must stay in Logs')
    require(source.is_file() and not output.exists(), 'Source missing or experimental output already exists')
    source_sha = sha_file(source)
    a = PlayerArchive(source)
    prefix, virtual_length = virtual_prefix(a)
    data_offset = HEADER.size + len(prefix) + len(a.blocks) * ROW.size
    temporary = output.with_suffix(output.suffix + '.verifying')
    require(not temporary.exists(), 'Unfinished experiment exists')
    output.parent.mkdir(parents=True, exist_ok=True)
    index, counts, digest = [], {}, hashlib.sha256()
    compress_seconds = 0.0
    try:
        with temporary.open('xb+') as target, source.open('rb') as original:
            target.write(HEADER.pack(MAGIC, len(a.blocks), len(prefix), virtual_length, data_offset))
            target.write(prefix)
            target.write(bytes(len(a.blocks) * ROW.size))
            for i, (logical, physical, raw, packed, flags) in enumerate(a.blocks):
                original.seek(physical)
                old = original.read(packed)
                data = a.codec.decode(old, raw, flags & 63)
                digest.update(data)
                buffer = ctypes.create_string_buffer(raw + raw // 2 + 65536)
                tick = time.monotonic()
                size = a.codec.library.compression_encode_buffer(buffer, len(buffer), data, raw, None, 0x801)
                compress_seconds += time.monotonic() - tick
                require(size > 0, 'System LZFSE encoding failed')
                encoded = buffer.raw[:size]
                decoded = ctypes.create_string_buffer(raw)
                actual = a.codec.library.compression_decode_buffer(decoded, raw, encoded, size, None, 0x801)
                require(actual == raw and decoded.raw == data, 'LZFSE round-trip mismatch')
                codec = 1
                if len(encoded) >= len(old):
                    encoded = old
                    codec = 0 if flags & 63 == 0 else 2
                index.append((len(prefix) + logical, target.tell(), raw, len(encoded), codec, 0))
                target.write(encoded)
                counts[str(codec)] = counts.get(str(codec), 0) + 1
                if i % 8192 == 0:
                    print('LZFSE blocks %d/%d' % (i, len(a.blocks)), flush=True)
            target.seek(HEADER.size + len(prefix))
            for row in index:
                target.write(ROW.pack(*row))
        reader = ExperimentStream(temporary, a, prefix, index)
        check = hashlib.sha256()
        for i in range(len(index)):
            check.update(reader.block(i))
        require(check.digest() == digest.digest(), 'Full decoded payload differs')
        rng = random.Random(20261116)
        vectors = []
        starts = [b[0] for b in a.blocks]
        with source.open('rb') as original:
            def reference(offset, length):
                result = bytearray()
                if offset < len(prefix):
                    n = min(length, len(prefix) - offset)
                    result.extend(prefix[offset:offset + n])
                    offset, length = offset + n, length - n
                pos = offset - len(prefix)
                while length and pos < virtual_length - len(prefix):
                    i = bisect.bisect_right(starts, pos) - 1
                    logical, physical, raw, packed, flags = a.blocks[i]
                    original.seek(physical)
                    b = a.codec.decode(original.read(packed), raw, flags & 63)
                    n = min(length, logical + raw - pos)
                    result.extend(b[pos - logical:pos - logical + n])
                    pos, length = pos + n, length - n
                return bytes(result)
            cases = [(0, 128), (len(prefix) - 31, 100), (virtual_length - 31, 100), (virtual_length, 1), (virtual_length + 7, 8)]
            cases += [(len(prefix) + p - 1, 4) for p in starts[1::max(1, len(starts) // 128)]]
            cases += [(rng.randrange(virtual_length), rng.choice([0, 1, 4, 127, 4096, 65536, 262143])) for _ in range(1024)]
            for offset, length in cases:
                expected = reference(offset, length)
                require(reader.read(offset, length) == expected, 'Random virtual read mismatch')
                vectors.append(dict(offset=offset, length=length, actualLength=len(expected), sha256=hashlib.sha256(expected).hexdigest()))
        reader.file.close()
        require(sha_file(source) == source_sha, 'Source Bundle changed')
        result = dict(status='passed', input=str(source), output=str(output), sourceSHA256=source_sha,
                      beforeBytes=source.stat().st_size, afterBytes=temporary.stat().st_size,
                      payloadDecodedBytes=sum(b[2] for b in a.blocks), payloadSHA256=digest.hexdigest(),
                      virtualLength=virtual_length, prefixBytes=len(prefix), indexBytes=len(index) * ROW.size,
                      blockCount=len(index), codecBlocks=counts, randomReadCases=len(vectors),
                      encodeSeconds=compress_seconds, totalSeconds=time.monotonic() - started,
                      outputSHA256=sha_file(temporary), nodes={n:list(v) for n,v in a.nodes.items()})
        output.parent.mkdir(parents=True, exist_ok=True)
        temporary.replace(output)
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(json.dumps(result, indent=2) + '\n')
        args.report.with_name('random-vectors.json').write_text(json.dumps(dict(vectors=vectors), indent=2) + '\n')
        print(json.dumps(result), flush=True)
    finally:
        temporary.unlink(missing_ok=True)


def verify_only(args):
    # Also used under Rosetta: exercise the x86_64 system codec with the same bytes.
    from types import SimpleNamespace
    started = time.monotonic()
    manifest = json.loads(args.manifest.read_text())
    require(sha_file(args.output) == manifest['outputSHA256'], 'Experimental container SHA differs')
    with args.output.open('rb') as stream:
        magic, count, prefix_size, virtual_length, data_offset = HEADER.unpack(stream.read(HEADER.size))
        require(magic == MAGIC and data_offset == HEADER.size + prefix_size + count * ROW.size, 'Invalid experiment header')
        prefix = stream.read(prefix_size)
        index = [ROW.unpack(stream.read(ROW.size)) for _ in range(count)]
    reader = ExperimentStream(args.output, SimpleNamespace(codec=SystemLz4()), prefix, index)
    digest = hashlib.sha256()
    for i in range(count):
        digest.update(reader.block(i))
    require(digest.hexdigest() == manifest['payloadSHA256'], 'Full decoded payload differs')
    vectors = json.loads(args.manifest.with_name('random-vectors.json').read_text())['vectors']
    for v in vectors:
        data = reader.read(v['offset'], v['length'])
        require(len(data) == v['actualLength'] and hashlib.sha256(data).hexdigest() == v['sha256'], 'Random read differs')
    reader.file.close()
    result = dict(passed=True, architecture=platform.machine(), payloadSHA256=digest.hexdigest(),
                  decodedBytes=manifest['payloadDecodedBytes'], randomReads=len(vectors),
                  seconds=time.monotonic() - started, processPeakRSS=resource.getrusage(resource.RUSAGE_SELF).ru_maxrss)
    args.report.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result), flush=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--input', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    parser.add_argument('--report', type=Path, required=True)
    parser.add_argument('--verify-only', action='store_true')
    parser.add_argument('--manifest', type=Path)
    parser.add_argument('--production', action='store_true', help='Allow only the canonical macOS app Bundle path')
    args = parser.parse_args()
    if args.verify_only:
        require(args.manifest is not None, 'Verification requires manifest')
        verify_only(args)
    else:
        build(args)
