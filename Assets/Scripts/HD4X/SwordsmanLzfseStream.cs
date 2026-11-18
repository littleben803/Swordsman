#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

// Seekable, bounded-cache view over the system-LZFSE HD Bundle container.
    public sealed class SwordsmanLzfseStream : Stream
    {
        static void Require(bool valid, string message) { if (!valid) throw new InvalidDataException(message); }
        struct Block { public long start, fileOffset; public int raw, packed, codec; }
        sealed class Cached : IDisposable
        {
            public byte[] data; public LinkedListNode<int> node; public bool ready;
            public GCHandle pin;
            public void Dispose() { if (pin.IsAllocated) pin.Free(); }
        }
        sealed class Decoder : IDisposable
        {
            public byte[] packed; public GCHandle pin; public IntPtr scratch;
            public Decoder(int size, bool native)
            {
                packed = new byte[size];
                if (native)
                {
                    pin = GCHandle.Alloc(packed, GCHandleType.Pinned);
                    ulong bytes = Math.Max(ScratchSize(0x801).ToUInt64(), ScratchSize(0x101).ToUInt64());
                    Require(bytes <= 16 * 1024 * 1024, "Invalid native scratch size");
                    if (bytes > 0) scratch = Marshal.AllocHGlobal((int)bytes);
                }
            }
            public void Dispose()
            {
                if (pin.IsAllocated) pin.Free();
                if (scratch != IntPtr.Zero) { Marshal.FreeHGlobal(scratch); scratch = IntPtr.Zero; }
            }
        }
        readonly FileStream file;
        readonly byte[] prefix;
        readonly Block[] blocks;
        readonly Decoder[] decoders;
        readonly long length;
        readonly bool native, profile;
        readonly int descriptor;
        readonly object gate = new object();
        readonly Dictionary<int, Cached> cache = new Dictionary<int, Cached>();
        readonly LinkedList<int> lru = new LinkedList<int>();
        long position, cachedBytes; bool disposed;
        public readonly int CacheLimit;
        public long Reads, Seeks, Hits, Misses, CachePeak, DecodedBytes, PackedBytes, IoTicks, DecodeTicks, CopyTicks;
        [DllImport("/usr/lib/libcompression.dylib", EntryPoint = "compression_decode_buffer")]
        static extern UIntPtr Decode([Out] byte[] destination, UIntPtr capacity, byte[] source, UIntPtr size, IntPtr scratch, int algorithm);
        [DllImport("/usr/lib/libcompression.dylib", EntryPoint = "compression_decode_buffer")]
        static extern UIntPtr DecodePointer(IntPtr destination, UIntPtr capacity, IntPtr source, UIntPtr size, IntPtr scratch, int algorithm);
        [DllImport("/usr/lib/libcompression.dylib", EntryPoint = "compression_decode_scratch_buffer_size")]
        static extern UIntPtr ScratchSize(int algorithm);
        [DllImport("libc", EntryPoint = "pread", SetLastError = true)]
        static extern IntPtr Pread(int descriptor, IntPtr output, UIntPtr count, long offset);

        public SwordsmanLzfseStream(string path, int cacheLimit, bool native, int workers, bool profile)
        {
            Require(workers >= 1 && workers <= 8 && (workers == 1 || native), "Invalid decoder worker settings");
            this.native = native; this.profile = profile; CacheLimit = cacheLimit;
            file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 65536);
            decoders = new Decoder[workers];
            try
            {
                var reader = new BinaryReader(file);
                Require(System.Text.Encoding.ASCII.GetString(reader.ReadBytes(8)) == "SWLZF001", "Bad experiment header");
                int count = reader.ReadInt32(), prefixSize = reader.ReadInt32();
                length = reader.ReadInt64(); long dataOffset = reader.ReadInt64();
                Require(count > 0 && count < 1000000 && prefixSize > 0 && prefixSize < 16000000, "Invalid index size");
                Require(dataOffset == 32L + prefixSize + count * 32L && dataOffset <= file.Length, "Invalid payload offset");
                prefix = reader.ReadBytes(prefixSize); Require(prefix.Length == prefixSize, "Truncated prefix");
                blocks = new Block[count]; long logical = prefixSize, physical = dataOffset;
                for (int i = 0; i < count; i++)
                {
                    var b = new Block { start = reader.ReadInt64(), fileOffset = reader.ReadInt64(), raw = reader.ReadInt32(), packed = reader.ReadInt32(), codec = reader.ReadInt32() };
                    int reserved = reader.ReadInt32();
                    Require(b.start == logical && b.fileOffset == physical && b.raw > 0 && b.raw <= CacheLimit &&
                        b.packed > 0 && b.packed <= 32 * 1024 * 1024 && b.codec >= 0 && b.codec <= 2 && reserved == 0, "Invalid block index");
                    blocks[i] = b; logical += b.raw; physical += b.packed;
                }
                Require(logical == length && physical == file.Length, "Invalid container bounds");
                int maximum = blocks.Max(b => b.packed);
                for (int i = 0; i < workers; i++) decoders[i] = new Decoder(maximum, native);
                if (native) descriptor = file.SafeFileHandle.DangerousGetHandle().ToInt32();
            }
            catch { foreach (var d in decoders) if (d != null) d.Dispose(); file.Dispose(); throw; }
        }
        int Find(long offset)
        {
            int lo = 0, hi = blocks.Length - 1;
            while (lo < hi) { int mid = (lo + hi + 1) / 2; if (blocks[mid].start <= offset) lo = mid; else hi = mid - 1; }
            return lo;
        }
        Cached Reserve(int i)
        {
            Misses++; Block b = blocks[i]; Cached item = null;
            while (cache.Count > 0 && cachedBytes + b.raw > CacheLimit)
            {
                int oldest = lru.First.Value; Cached recycled = cache[oldest];
                cachedBytes -= recycled.data.Length; cache.Remove(oldest); lru.RemoveFirst();
                if (recycled.data.Length == b.raw && item == null) item = recycled;
                else recycled.Dispose();
            }
            if (item == null)
            {
                item = new Cached { data = new byte[b.raw], node = new LinkedListNode<int>(i) };
                if (native) item.pin = GCHandle.Alloc(item.data, GCHandleType.Pinned);
            }
            item.node.Value = i; item.ready = false; lru.AddLast(item.node); cache.Add(i, item);
            cachedBytes += item.data.Length; CachePeak = Math.Max(CachePeak, cachedBytes); return item;
        }
        void Fill(int i, Cached item, Decoder decoder)
        {
            Block b = blocks[i]; long tick = profile ? System.Diagnostics.Stopwatch.GetTimestamp() : 0;
            int done = 0;
            if (native)
            {
                while (done < b.packed)
                {
                    long n = Pread(descriptor, IntPtr.Add(decoder.pin.AddrOfPinnedObject(), done), (UIntPtr)(b.packed - done), b.fileOffset + done).ToInt64();
                    if (n <= 0) throw new IOException("Experimental pread failed: " + Marshal.GetLastWin32Error()); done += (int)n;
                }
            }
            else
            {
                file.Position = b.fileOffset;
                while (done < b.packed) { int n = file.Read(decoder.packed, done, b.packed - done); if (n == 0) throw new EndOfStreamException(); done += n; }
            }
            if (profile) System.Threading.Interlocked.Add(ref IoTicks, System.Diagnostics.Stopwatch.GetTimestamp() - tick);
            System.Threading.Interlocked.Add(ref PackedBytes, b.packed);
            tick = profile ? System.Diagnostics.Stopwatch.GetTimestamp() : 0;
            if (b.codec != 0)
            {
                ulong count = native ? DecodePointer(item.pin.AddrOfPinnedObject(), (UIntPtr)b.raw, decoder.pin.AddrOfPinnedObject(), (UIntPtr)b.packed,
                    decoder.scratch, b.codec == 1 ? 0x801 : 0x101).ToUInt64() :
                    Decode(item.data, (UIntPtr)b.raw, decoder.packed, (UIntPtr)b.packed, IntPtr.Zero, b.codec == 1 ? 0x801 : 0x101).ToUInt64();
                Require(count == (ulong)b.raw, "Native block decode failed");
            }
            else Buffer.BlockCopy(decoder.packed, 0, item.data, 0, b.raw);
            if (profile) System.Threading.Interlocked.Add(ref DecodeTicks, System.Diagnostics.Stopwatch.GetTimestamp() - tick);
            System.Threading.Interlocked.Add(ref DecodedBytes, b.raw); item.ready = true;
        }
        byte[] GetBlock(int i)
        {
            Cached item;
            if (cache.TryGetValue(i, out item))
            { Require(item.ready, "Incomplete prefetched block"); Hits++; lru.Remove(item.node); lru.AddLast(item.node); return item.data; }
            item = Reserve(i); Fill(i, item, decoders[0]); return item.data;
        }
        void Prefetch(int count)
        {
            if (decoders.Length <= 1 || count < 131072) return;
            long end = Math.Min(length, position + Math.Min(count, 4 * 1024 * 1024));
            int first = Find(position), last = Find(end - 1);
            if (blocks[last].start + blocks[last].raw - blocks[first].start > CacheLimit) return;
            var missing = new List<int>();
            for (int i = first; i <= last; i++)
            {
                Cached hit;
                if (cache.TryGetValue(i, out hit)) { Require(hit.ready, "Incomplete cache entry"); lru.Remove(hit.node); lru.AddLast(hit.node); }
                else missing.Add(i);
            }
            if (missing.Count < 2) return;
            var items = new Cached[missing.Count];
            for (int i = 0; i < missing.Count; i++) items[i] = Reserve(missing[i]);
            System.Threading.Tasks.Parallel.For(0, Math.Min(decoders.Length, missing.Count), worker =>
            {
                for (int j = worker; j < missing.Count; j += decoders.Length) Fill(missing[j], items[j], decoders[worker]);
            });
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (offset < 0 || count < 0 || offset > buffer.Length - count) throw new ArgumentOutOfRangeException();
            lock (gate)
            {
                if (disposed) throw new ObjectDisposedException("SwordsmanLzfseStream");
                Reads++; int copied = 0;
                while (count > 0 && position < length)
                {
                    if (position < prefix.Length)
                    {
                        int n = (int)Math.Min(count, prefix.Length - position);
                        Buffer.BlockCopy(prefix, (int)position, buffer, offset, n); position += n; offset += n; count -= n; copied += n; continue;
                    }
                    Prefetch(count);
                    int index = Find(position); Block b = blocks[index]; byte[] data = GetBlock(index);
                    int take = (int)Math.Min(count, b.start + b.raw - position); Require(take > 0, "Virtual read made no progress");
                    long tick = profile ? System.Diagnostics.Stopwatch.GetTimestamp() : 0;
                    Buffer.BlockCopy(data, (int)(position - b.start), buffer, offset, take);
                    if (profile) CopyTicks += System.Diagnostics.Stopwatch.GetTimestamp() - tick;
                    position += take; offset += take; count -= take; copied += take;
                }
                return copied;
            }
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (gate)
            {
                if (disposed) throw new ObjectDisposedException("SwordsmanLzfseStream");
                long next = checked(offset + (origin == SeekOrigin.Begin ? 0 : origin == SeekOrigin.Current ? position : origin == SeekOrigin.End ? length : throw new ArgumentException("origin")));
                if (next < 0) throw new IOException("Negative seek"); position = next; Seeks++; return next;
            }
        }
        public override long Position { get { lock (gate) return position; } set { Seek(value, SeekOrigin.Begin); } }
        public override long Length { get { return length; } }
        public override bool CanRead { get { return !disposed; } }
        public override bool CanSeek { get { return !disposed; } }
        public override bool CanWrite { get { return false; } }
        public override void Flush() { }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        protected override void Dispose(bool disposing)
        {
            if (disposing) lock (gate)
            {
                if (!disposed) { foreach (var item in cache.Values) item.Dispose(); foreach (var decoder in decoders) decoder.Dispose(); cache.Clear(); lru.Clear(); file.Dispose(); disposed = true; }
            }
            base.Dispose(disposing);
        }
    }
#endif
