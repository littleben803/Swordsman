#if UNITY_EDITOR_OSX
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

// Explicit Editor experiment only. No runtime resolver/build hook or default-path registration.
public static class SwordsmanLzfseValidation
{
    [Serializable] public class Vector { public long offset; public int length, actualLength; public string sha256; }
    [Serializable] public class Vectors { public Vector[] vectors; }
    [Serializable] public class TextureInput { public string name; public long size; }
    [Serializable] public class Inputs { public TextureInput[] textures; }
    [Serializable] public class AssetResult
    {
        public string path, geometrySHA256, renderedSHA256;
        public int width, height, sprites;
        public double loadMs, renderHashMs;
    }
    [Serializable] public class Report
    {
        public bool passed;
        public string mode, unity;
        public bool nativeIO;
        public int workers, managedBufferBytes;
        public double ioMs, decodeMs, copyMs;
        public int allAssets, randomReads, cacheLimitBytes;
        public double streamInitMs, openMs, assetLoadMs, renderHashMs;
        public long processPeakAtStart, processPeakBytes, unityAllocatedAtStart, unityAllocatedEnd;
        public long streamReads, streamSeeks, cacheHits, cacheMisses, cachePeakBytes, decodedBytes, packedBytesRead;
        public AssetResult[] assets;
    }

    static string Argument(string key)
    {
        string[] args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(args, key);
        if (i < 0 || i + 1 >= args.Length) throw new ArgumentException("Missing " + key);
        return args[i + 1];
    }
    static int Setting(string key, int fallback)
    {
        string[] args = Environment.GetCommandLineArgs(); int i = Array.IndexOf(args, key);
        return i < 0 ? fallback : int.Parse(args[i + 1], System.Globalization.CultureInfo.InvariantCulture);
    }
    static void Require(bool value, string message) { if (!value) throw new InvalidDataException(message); }
    static string Hash(byte[] data)
    {
        using (var sha = SHA256.Create()) return BitConverter.ToString(sha.ComputeHash(data)).Replace("-", "").ToLowerInvariant();
    }
    [DllImport("libc", EntryPoint = "getrusage")] static extern int GetRusage(int who, IntPtr data);
    static long PeakResidentBytes()
    {
        IntPtr data = Marshal.AllocHGlobal(1024);
        try { Require(GetRusage(0, data) == 0, "getrusage failed"); return Marshal.ReadInt64(data, 32); }
        finally { Marshal.FreeHGlobal(data); }
    }

    public static void Run()
    {
        string mode = Argument("-swordsman-lzfse-mode");
        string output = Path.GetFullPath(Argument("-swordsman-lzfse-report"));
        var evidence = new DirectoryInfo(Path.GetDirectoryName(output));
        Require(evidence.Name.StartsWith("lzfse-validation-", StringComparison.Ordinal) && evidence.Parent.Name == "Logs",
            "Experiment report must stay in Logs/lzfse-validation-*");
        var report = new Report { mode = mode, unity = Application.unityVersion,
            processPeakAtStart = PeakResidentBytes(), unityAllocatedAtStart = Profiler.GetTotalAllocatedMemoryLong() };
        int workers = Setting("-swordsman-lzfse-workers", 1);
        bool native = Setting("-swordsman-lzfse-native", 0) != 0;
        bool profile = Setting("-swordsman-lzfse-profile", 0) != 0;
        int managedBuffer = Setting("-swordsman-lzfse-buffer", 65536);
        report.nativeIO = native; report.workers = workers; report.managedBufferBytes = managedBuffer;
        LzfseStream stream = null;
        AssetBundle bundle = null;
        try
        {
            if (mode != "baseline")
            {
                var initTimer = System.Diagnostics.Stopwatch.StartNew();
                stream = new LzfseStream(Argument("-swordsman-lzfse-input"), 16 * 1024 * 1024, native, workers, profile);
                report.streamInitMs = initTimer.Elapsed.TotalMilliseconds;
            }
            if (mode == "random")
            {
                var vectors = JsonUtility.FromJson<Vectors>(File.ReadAllText(Argument("-swordsman-lzfse-vectors")));
                foreach (var vector in vectors.vectors)
                {
                    stream.Seek(vector.offset, SeekOrigin.Begin);
                    byte[] bytes = new byte[vector.length];
                    int count = stream.Read(bytes, 0, bytes.Length);
                    Require(count == vector.actualLength, "Random read length differs at " + vector.offset);
                    if (count != bytes.Length) Array.Resize(ref bytes, count);
                    Require(Hash(bytes) == vector.sha256, "Random read bytes differ at " + vector.offset);
                    report.randomReads++;
                }
                stream.Seek(-17, SeekOrigin.End);
                Require(stream.Position == stream.Length - 17, "End seek failed");
                stream.Seek(7, SeekOrigin.Current);
                Require(stream.Position == stream.Length - 10, "Relative seek failed");
                bool negativeRejected = false;
                try { stream.Seek(-1, SeekOrigin.Begin); } catch (IOException) { negativeRejected = true; }
                Require(negativeRejected, "Negative seek accepted");
            }
            else
            {
                Require(mode == "baseline" || mode == "candidate", "Unknown experiment mode");
                var timer = System.Diagnostics.Stopwatch.StartNew();
                bundle = mode == "baseline" ? AssetBundle.LoadFromFile(Argument("-swordsman-lzfse-source"))
                    : AssetBundle.LoadFromStream(stream, 0, (uint)managedBuffer);
                report.openMs = timer.Elapsed.TotalMilliseconds;
                Require(bundle != null, "Bundle did not load");
                string[] all = bundle.GetAllAssetNames().OrderBy(x => x, StringComparer.Ordinal).ToArray();
                report.allAssets = all.Length;
                Require(all.Length == 1819, "Unexpected HD asset count");
                var inputs = JsonUtility.FromJson<Inputs>(File.ReadAllText(Argument("-swordsman-lzfse-textures")));
                var largest = inputs.textures.OrderByDescending(t => t.size).Take(3).Select(t => t.name).ToArray();
                var chosen = new HashSet<string>(StringComparer.Ordinal);
                foreach (string name in largest)
                {
                    string match = all.FirstOrDefault(p => string.Equals(Path.GetFileNameWithoutExtension(p), name, StringComparison.OrdinalIgnoreCase));
                    Require(match != null, "Missing largest texture " + name); chosen.Add(match);
                }
                for (int i = 0; i < 29; i++) chosen.Add(all[(int)((i + 0.5) * all.Length / 29)]);
                var results = new List<AssetResult>();
                foreach (string path in chosen.OrderBy(x => x, StringComparer.Ordinal))
                {
                    timer.Restart();
                    Texture2D texture = bundle.LoadAsset<Texture2D>(path);
                    Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>(path);
                    double elapsed = timer.Elapsed.TotalMilliseconds;
                    Require(texture != null, "Missing texture " + path);
                    string geometry = string.Join("\n", sprites.OrderBy(x => x.name, StringComparer.Ordinal).Select(s =>
                        s.name + "|" + JsonUtility.ToJson(s.rect) + "|" + JsonUtility.ToJson(s.pivot) + "|" +
                        JsonUtility.ToJson(s.border) + "|" + s.pixelsPerUnit.ToString("R", System.Globalization.CultureInfo.InvariantCulture)).ToArray());
                    var item = new AssetResult { path = path, width = texture.width, height = texture.height,
                        sprites = sprites.Length, geometrySHA256 = Hash(System.Text.Encoding.UTF8.GetBytes(geometry)), loadMs = elapsed };
                    report.assetLoadMs += elapsed;
                    if (results.Count < 8)
                    {
                        timer.Restart(); item.renderedSHA256 = RenderHash(texture);
                        item.renderHashMs = timer.Elapsed.TotalMilliseconds; report.renderHashMs += item.renderHashMs;
                    }
                    results.Add(item);
                }
                report.assets = results.ToArray();
            }
            report.processPeakBytes = PeakResidentBytes();
            report.unityAllocatedEnd = Profiler.GetTotalAllocatedMemoryLong();
            if (stream != null)
            {
                report.cacheLimitBytes = stream.CacheLimit; report.streamReads = stream.Reads; report.streamSeeks = stream.Seeks;
                report.cacheHits = stream.Hits; report.cacheMisses = stream.Misses; report.cachePeakBytes = stream.CachePeak;
                report.decodedBytes = stream.DecodedBytes; report.packedBytesRead = stream.PackedBytes;
                report.ioMs = stream.IoTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
                report.decodeMs = stream.DecodeTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
                report.copyMs = stream.CopyTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
            }
            report.passed = true;
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output, JsonUtility.ToJson(report, true));
            Debug.Log("SWORDSMAN_LZFSE_VALIDATION passed mode=" + mode + " random=" + report.randomReads +
                " assets=" + (report.assets == null ? 0 : report.assets.Length) + " peakRSS=" + report.processPeakBytes);
        }
        finally { if (bundle != null) bundle.Unload(true); if (stream != null) stream.Dispose(); }
    }

    static string RenderHash(Texture2D texture)
    {
        RenderTexture previous = RenderTexture.active;
        bool srgb = GL.sRGBWrite;
        var target = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        Texture2D read = null;
        try
        {
            GL.sRGBWrite = false; Graphics.Blit(texture, target); RenderTexture.active = target;
            read = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, true);
            read.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0, false);
            return Hash(read.GetRawTextureData());
        }
        finally
        {
            RenderTexture.active = previous; GL.sRGBWrite = srgb;
            if (read != null) UnityEngine.Object.DestroyImmediate(read);
            RenderTexture.ReleaseTemporary(target);
        }
    }

    sealed class LzfseStream : Stream
    {
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

        public LzfseStream(string path, int cacheLimit, bool native, int workers, bool profile)
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
                if (disposed) throw new ObjectDisposedException("LzfseStream");
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
                if (disposed) throw new ObjectDisposedException("LzfseStream");
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
}
#endif
