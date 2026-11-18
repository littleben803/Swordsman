using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static partial class SwordsmanHd4xBuild
{
    const int HdCacheVersion = 2;
    const string HdCacheRoot = "Library/SwordsmanHD4X";
    const string HdCacheAssets = "Assets/SceneDependencyCacheSwordsmanHD";
    const string HdCacheManifest = HdCacheRoot + "/build-cache.json";

    [Serializable]
    sealed class HdCacheEntry
    {
        public string path;
        public string inputHash;
        public string imageHash;
        public string metaHash;
    }

    [Serializable]
    sealed class HdCacheState
    {
        public int version;
        public string unityVersion;
        public string recipeHash;
        public string inputsHash;
        public string bundleHash;
        public HdCacheEntry[] entries;
    }

    sealed class HdCacheInput
    {
        public ApprovedAsset asset;
        public AtlasSpec? atlas;
        public string hash;
    }

    sealed class HdCachePlan
    {
        public readonly List<HdCacheInput> changed = new List<HdCacheInput>();
        public readonly HashSet<string> resetImporter = new HashSet<string>( StringComparer.Ordinal );
        public readonly List<string> removed = new List<string>();
    }

    [Serializable]
    sealed class HdCacheInputDescription
    {
        public string recipe;
        public string unityVersion;
        public string target;
        public string path;
        public string approvedPath;
        public string imageHash;
        public string sourcePath;
        public string sourceHash;
        public string sourceMetaHash;
        public string sourceDependencyHash;
        public int maxTextureSize;
        public float scale;
        public int spriteCount;
        public string allowedSprites;
        public bool standaloneSlice;
        public HdCacheSpriteDescription[] sprites;
    }

    [Serializable]
    sealed class HdCacheSpriteDescription
    {
        public string name;
        public string path;
        public string dependencyHash;
        public Rect rect;
        public Vector2 pivot;
        public Vector4 border;
        public float ppu;
    }

    static string BuildAndValidateCachedBundle()
    {
        var timer = System.Diagnostics.Stopwatch.StartNew();
        ValidateHdCacheDirectory( GetAbsoluteProjectPath( HdCacheAssets ) );
        if ( Directory.Exists( GetAbsoluteProjectPath( StagingFolder ) ) )
            throw new InvalidOperationException( "Legacy Resources staging remains; refusing a Player build." );
        string recipe = GetHdCacheRecipeHash();
        List<HdCacheInput> inputs = CreateHdCacheInputs( recipe );
        string fingerprint = GetHdCacheInputsHash( inputs );
        HdCacheState previous = ReadHdCacheState();
        HdCachePlan plan = PlanHdCacheChanges( inputs, previous );
        string bundlePath = GetAbsoluteProjectPath( BundleOutputRelativePath + "/" + BundleName );
        bool reusable = CanReuseHdCacheBundle( previous, recipe, fingerprint, bundlePath );
        double inputSeconds = timer.Elapsed.TotalSeconds;

        if ( reusable && plan.changed.Count == 0 && plan.removed.Count == 0 )
        {
            ValidateBuiltBundle( bundlePath );
            LogHdCacheBuild( "reused", inputs.Count, 0, 0, inputSeconds, timer.Elapsed.TotalSeconds );
            return bundlePath;
        }

        Debug.LogFormat( "SWORDSMAN_HD_CACHE_PLAN assets={0} changed={1} removed={2} reusable={3}",
            inputs.Count, plan.changed.Count, plan.removed.Count, reusable );
        {
            PrepareHdCacheAssets( inputs, plan );
            if ( !reusable )
            {
                Directory.CreateDirectory( Path.GetDirectoryName( bundlePath ) );
                var names = new List<string>();
                var addresses = new List<string>();
                foreach ( HdCacheInput input in inputs )
                {
                    names.Add( GetHdCacheAssetPath( input.asset.stagingPath ) );
                    addresses.Add( input.asset.stagingPath.ToLowerInvariant() );
                }
                BuildAssetBundleOptions options = BuildAssetBundleOptions.ChunkBasedCompression;
                // An unproven/invalid output must not inherit Unity's old bundle build cache.
                if ( previous == null || previous.version != HdCacheVersion ||
                    previous.unityVersion != Application.unityVersion || previous.recipeHash != recipe ||
                    ( previous.inputsHash == fingerprint && File.Exists( bundlePath ) ) )
                    options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
                AssetBundleManifest built = BuildPipeline.BuildAssetBundles(
                    Path.GetDirectoryName( bundlePath ),
                    new[] { new AssetBundleBuild { assetBundleName = BundleName, assetNames = names.ToArray(), addressableNames = addresses.ToArray() } },
                    options, BuildTarget.StandaloneOSX );
                if ( built == null ) throw new InvalidOperationException( "HD AssetBundle build failed." );
            }
            ValidateBuiltBundle( bundlePath );
            var records = new List<HdCacheEntry>();
            foreach ( HdCacheInput input in inputs )
                records.Add( new HdCacheEntry {
                    path = input.asset.stagingPath, inputHash = input.hash,
                    imageHash = GetSha256( GetHdCacheFilePath( input.asset.stagingPath ) ),
                    metaHash = GetSha256( GetHdCacheFilePath( input.asset.stagingPath ) + ".meta" ),
                } );
            WriteHdCacheState( new HdCacheState {
                version = HdCacheVersion, unityVersion = Application.unityVersion, recipeHash = recipe,
                inputsHash = fingerprint, bundleHash = GetSha256( bundlePath ), entries = records.ToArray(),
            } );
        }
        LogHdCacheBuild( reusable ? "repaired-reused" : "rebuilt", inputs.Count,
            plan.changed.Count, plan.removed.Count, inputSeconds, timer.Elapsed.TotalSeconds );
        return bundlePath;
    }

    static string GetHdCacheRecipeHash()
    {
        string source = File.ReadAllText( GetAbsoluteProjectPath( "Assets/Editor/SwordsmanHd4xBuild.cs" ) );
        int first = source.IndexOf( "    static void ConfigureSingleImporter(", StringComparison.Ordinal );
        if ( first < 0 ) throw new InvalidOperationException( "Cannot identify the HD importer recipe." );
        int last = source.IndexOf( "    static void ValidateStaticBundleAssets(", first, StringComparison.Ordinal );
        if ( first < 0 || last < 0 ) throw new InvalidOperationException( "Cannot identify the HD importer recipe." );
        // Per-asset declarations are deliberately excluded so one new image does not invalidate every importer.
        return HashHdCacheText( source.Substring( first, last - first ) +
            File.ReadAllText( GetAbsoluteProjectPath( "Assets/Editor/SwordsmanHd4xBuildCache.cs" ) ) +
            File.ReadAllText( GetAbsoluteProjectPath( "Assets/Editor/SwordsmanHd4xSourceGeometry.cs" ) ) );
    }

    static List<HdCacheInput> CreateHdCacheInputs( string recipe )
    {
        var result = new List<HdCacheInput>();
        var fileHashes = new Dictionary<string, string>( StringComparer.Ordinal );
        foreach ( AtlasSpec spec in SpriteAtlases ) result.Add( CreateHdCacheInput( spec.asset, spec, recipe, fileHashes ) );
        foreach ( ApprovedAsset asset in SingleAssets ) result.Add( CreateHdCacheInput( asset, null, recipe, fileHashes ) );
        result.Sort( ( a, b ) => string.CompareOrdinal( a.asset.stagingPath, b.asset.stagingPath ) );
        var paths = new HashSet<string>( StringComparer.OrdinalIgnoreCase );
        foreach ( HdCacheInput input in result )
        {
            HdCacheRelativePath( input.asset.stagingPath );
            if ( !paths.Add( input.asset.stagingPath ) ) throw new InvalidOperationException( "Duplicate HD cache path." );
        }
        return result;
    }

    static bool CanReuseHdCacheBundle( HdCacheState state, string recipe, string inputsHash, string bundlePath )
    {
        return state != null && state.version == HdCacheVersion &&
            state.unityVersion == Application.unityVersion && state.recipeHash == recipe &&
            state.inputsHash == inputsHash && File.Exists( bundlePath ) &&
            GetSha256( bundlePath ) == state.bundleHash;
    }

    static HdCacheInput CreateHdCacheInput( ApprovedAsset asset, AtlasSpec? atlas, string recipe,
        Dictionary<string, string> fileHashes )
    {
        Func<string, string> fileHash = path => {
            string value;
            if ( !fileHashes.TryGetValue( path, out value ) )
            {
                value = GetSha256( GetAbsoluteProjectPath( path ) ); fileHashes.Add( path, value );
            }
            return value;
        };
        string imageHash = fileHash( asset.approvedRelativePath );
        SourceTexture frozen;
        bool retired = TryFrozenSource(asset.sourceAssetPath, out frozen);
        string sourceHash = retired ? frozen.hash : fileHash( asset.sourceAssetPath );
        if ( imageHash != asset.approvedSha256 || sourceHash != asset.sourceSha256 )
            throw new InvalidOperationException( "Approved/source HD input hash mismatch: " + asset.label );
        List<SourceSprite> sprites = atlas.HasValue ? FindLegacySpritesForTexture( asset.sourceAssetPath, atlas.Value.allowedSpriteName )
            : new List<SourceSprite> { GetSourceSpriteGeometry( asset.sourceAssetPath ) };
        var geometry = new List<HdCacheSpriteDescription>();
        foreach ( SourceSprite sprite in sprites )
        {
            if ( sprite == null ) throw new InvalidOperationException( "Missing source Sprite: " + asset.label );
            string path = sprite.path;
            geometry.Add( new HdCacheSpriteDescription {
                name = sprite.name, path = path, dependencyHash = sprite.dependencyHash,
                rect = sprite.rect, pivot = sprite.pivot, border = sprite.border, ppu = sprite.pixelsPerUnit,
            } );
        }
        var description = new HdCacheInputDescription {
            recipe = recipe, unityVersion = Application.unityVersion, target = BuildTarget.StandaloneOSX.ToString(),
            path = asset.stagingPath, approvedPath = asset.approvedRelativePath, imageHash = imageHash,
            sourcePath = asset.sourceAssetPath, sourceHash = sourceHash,
            sourceMetaHash = retired ? frozen.metaHash : fileHash( asset.sourceAssetPath + ".meta" ),
            sourceDependencyHash = retired ? frozen.dependencyHash : AssetDatabase.GetAssetDependencyHash( asset.sourceAssetPath ).ToString(),
            maxTextureSize = asset.maxTextureSize, scale = atlas.HasValue ? atlas.Value.linearScale : SingleScale,
            spriteCount = atlas.HasValue ? atlas.Value.expectedSpriteCount : 1,
            allowedSprites = atlas.HasValue ? atlas.Value.allowedSpriteName : null,
            standaloneSlice = atlas.HasValue && atlas.Value.standaloneSlice, sprites = geometry.ToArray(),
        };
        return new HdCacheInput { asset = asset, atlas = atlas, hash = HashHdCacheText( JsonUtility.ToJson( description ) ) };
    }

    static string GetHdCacheInputsHash( List<HdCacheInput> inputs )
    {
        var text = new StringBuilder();
        foreach ( HdCacheInput input in inputs ) text.Append( input.asset.stagingPath ).Append( ':' ).Append( input.hash ).Append( '\n' );
        return HashHdCacheText( text.ToString() );
    }

    static HdCachePlan PlanHdCacheChanges( List<HdCacheInput> inputs, HdCacheState previous )
    {
        var known = new Dictionary<string, HdCacheEntry>( StringComparer.Ordinal );
        if ( previous != null ) foreach ( HdCacheEntry entry in previous.entries ) known.Add( entry.path, entry );
        var plan = new HdCachePlan();
        foreach ( HdCacheInput input in inputs )
        {
            string path = input.asset.stagingPath;
            HdCacheEntry old;
            known.TryGetValue( path, out old );
            string cached = GetHdCacheFilePath( path );
            bool metaValid = old != null && File.Exists( cached + ".meta" ) && GetSha256( cached + ".meta" ) == old.metaHash;
            bool imageValid = File.Exists( cached ) && GetSha256( cached ) == input.asset.approvedSha256;
            if ( old == null || old.inputHash != input.hash || !metaValid || !imageValid ) plan.changed.Add( input );
            if ( !metaValid ) plan.resetImporter.Add( path );
            known.Remove( path );
        }
        foreach ( string path in known.Keys ) { HdCacheRelativePath( path ); plan.removed.Add( path ); }
        return plan;
    }

    static void PrepareHdCacheAssets( List<HdCacheInput> inputs, HdCachePlan plan )
    {
        var expected = new HashSet<string>( StringComparer.Ordinal );
        foreach ( HdCacheInput input in inputs ) expected.Add( GetHdCacheFilePath( input.asset.stagingPath ) );
        string root = GetAbsoluteProjectPath( HdCacheAssets );
        if ( Directory.Exists( root ) )
            foreach ( string file in Directory.GetFiles( root, "*.png", SearchOption.AllDirectories ) )
                if ( !expected.Contains( Path.GetFullPath( file ) ) )
                {
                    File.Delete( file );
                    if ( File.Exists( file + ".meta" ) ) File.Delete( file + ".meta" );
                }
        foreach ( HdCacheInput input in plan.changed )
        {
            string path = input.asset.stagingPath;
            string absolute = GetHdCacheFilePath( path );
            // An altered/missing cached importer is recreated by Unity, never by copying a source GUID.
            if ( plan.resetImporter.Contains( path ) )
            {
                if ( File.Exists( absolute ) ) File.Delete( absolute );
                if ( File.Exists( absolute + ".meta" ) ) File.Delete( absolute + ".meta" );
            }
            Directory.CreateDirectory( Path.GetDirectoryName( absolute ) );
            if ( !File.Exists( absolute ) || GetSha256( absolute ) != input.asset.approvedSha256 )
                File.Copy( GetAbsoluteProjectPath( input.asset.approvedRelativePath ), absolute, true );
        }
        AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach ( HdCacheInput input in plan.changed )
            {
                if ( input.atlas.HasValue ) ConfigureAtlasImporter( input.atlas.Value );
                else ConfigureSingleImporter( input.asset );
            }
        }
        finally { AssetDatabase.StopAssetEditing(); }
        AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
    }

    static string HdCacheRelativePath( string path )
    {
        if ( !path.StartsWith( StagingFolder + "/", StringComparison.Ordinal ) ||
            path.Contains( ".." ) || !path.EndsWith( ".png", StringComparison.OrdinalIgnoreCase ) )
            throw new InvalidOperationException( "Unexpected HD cache asset path: " + path );
        return path.Substring( StagingFolder.Length + 1 );
    }

    static string GetHdCacheAssetPath( string stagingPath )
    {
        string relative = HdCacheRelativePath( stagingPath );
        if ( relative.StartsWith( "Resources/", StringComparison.Ordinal ) )
            relative = "ImportedResources/" + relative.Substring( "Resources/".Length );
        foreach ( string part in relative.Split( '/' ) )
            if ( string.Equals( part, "Resources", StringComparison.OrdinalIgnoreCase ) )
                throw new InvalidOperationException( "HD import cache cannot contain a Resources folder." );
        return HdCacheAssets + "/" + relative;
    }

    static string GetHdCacheFilePath( string stagingPath )
    {
        return GetAbsoluteProjectPath( GetHdCacheAssetPath( stagingPath ) );
    }

    static void ValidateHdCacheDirectory( string root )
    {
        if ( !Directory.Exists( root ) ) return;
        if ( ( File.GetAttributes( root ) & FileAttributes.ReparsePoint ) != 0 )
            throw new InvalidOperationException( "HD cache cannot be a symbolic link." );
        foreach ( string path in Directory.GetFileSystemEntries( root ) )
        {
            if ( ( File.GetAttributes( path ) & FileAttributes.ReparsePoint ) != 0 )
                throw new InvalidOperationException( "HD cache contains a symbolic link." );
            if ( Directory.Exists( path ) )
            {
                if ( string.Equals( Path.GetFileName( path ), "Resources", StringComparison.OrdinalIgnoreCase ) )
                    throw new InvalidOperationException( "HD import cache contains a Resources folder." );
                ValidateHdCacheDirectory( path );
            }
            else if ( !path.EndsWith( ".png", StringComparison.OrdinalIgnoreCase ) &&
                      !path.EndsWith( ".meta", StringComparison.OrdinalIgnoreCase ) )
                throw new InvalidOperationException( "Unexpected file in reserved HD cache: " + path );
        }
    }

    static HdCacheState ReadHdCacheState()
    {
        string path = GetAbsoluteProjectPath( HdCacheManifest );
        if ( !File.Exists( path ) ) return null;
        try
        {
            HdCacheState state = JsonUtility.FromJson<HdCacheState>( File.ReadAllText( path ) );
            if ( state == null || state.entries == null || state.bundleHash == null || state.bundleHash.Length != 64 ||
                state.inputsHash == null || state.inputsHash.Length != 64 || state.recipeHash == null || state.recipeHash.Length != 64 ) return null;
            var paths = new HashSet<string>( StringComparer.Ordinal );
            foreach ( HdCacheEntry entry in state.entries )
            {
                if ( entry == null || entry.path == null || entry.inputHash == null || entry.inputHash.Length != 64 ||
                    entry.metaHash == null || entry.metaHash.Length != 64 ) return null;
                HdCacheRelativePath( entry.path );
                if ( !paths.Add( entry.path ) ) return null;
            }
            return state;
        }
        catch ( Exception exception ) when ( exception is ArgumentException || exception is IOException || exception is InvalidOperationException )
        {
            Debug.Log( "SWORDSMAN_HD_CACHE invalid-manifest; rebuilding verified inputs." );
            return null;
        }
    }

    static void WriteHdCacheState( HdCacheState state )
    {
        string path = GetAbsoluteProjectPath( HdCacheManifest );
        Directory.CreateDirectory( Path.GetDirectoryName( path ) );
        string temporary = path + ".tmp";
        File.WriteAllText( temporary, JsonUtility.ToJson( state, true ) );
        if ( File.Exists( path ) ) File.Replace( temporary, path, null );
        else File.Move( temporary, path );
    }

    static string HashHdCacheText( string text )
    {
        using ( var sha = System.Security.Cryptography.SHA256.Create() )
            return BitConverter.ToString( sha.ComputeHash( Encoding.UTF8.GetBytes( text ) ) ).Replace( "-", "" ).ToLowerInvariant();
    }

    static void LogHdCacheBuild( string mode, int assets, int imported, int removed, double inputSeconds, double totalSeconds )
    {
        Debug.LogFormat( "SWORDSMAN_HD_CACHE mode={0} assets={1} imported={2} unchanged={3} removed={4} inputSeconds={5:F3} totalSeconds={6:F3}",
            mode, assets, imported, assets - imported, removed, inputSeconds, totalSeconds );
    }
}
