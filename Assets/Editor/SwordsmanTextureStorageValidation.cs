using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

// Explicit Editor-only verification. Does not save assets, scenes or preferences.
public static partial class SwordsmanHd4xBuild
{
    [Serializable]
    sealed class TextureStorageSnapshot
    {
        public int assets;
        public int sprites;
        public List<string> descriptors = new List<string>();
        public List<string> gpuPixels = new List<string>();
    }

    [Serializable]
    sealed class TextureStorageReport
    {
        public bool passed;
        public TextureStorageSnapshot before;
        public TextureStorageSnapshot after;
    }

    public static void ValidateTextureStorageCandidate()
    {
        string[] args = Environment.GetCommandLineArgs();
        Func<string, string> argument = name => {
            int index = Array.IndexOf( args, name );
            if ( index < 0 || index + 1 >= args.Length ) throw new InvalidOperationException( "Missing " + name );
            return Path.GetFullPath( args[index + 1] );
        };
        string original = argument( "-swordsman-storage-original" );
        string candidate = argument( "-swordsman-storage-candidate" );
        string output = argument( "-swordsman-storage-report" );
        if ( original == candidate ) throw new InvalidOperationException( "Validation requires distinct inputs." );
        ValidateBuiltBundle( original );
        var report = new TextureStorageReport { before = SnapshotTextureStorage( original ) };
        ValidateBuiltBundle( candidate );
        report.after = SnapshotTextureStorage( candidate );
        report.passed = JsonUtility.ToJson( report.before ) == JsonUtility.ToJson( report.after );
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        if ( !report.passed ) throw new InvalidOperationException( "Texture storage descriptors/GPU pixels differ." );
        SwordsmanSceneValidation.ValidateScenes();
        Debug.LogFormat( "SWORDSMAN_TEXTURE_STORAGE_UNITY passed assets={0} sprites={1} gpuTextures={2}",
            report.after.assets, report.after.sprites, report.after.gpuPixels.Count );
    }

    static TextureStorageSnapshot SnapshotTextureStorage( string path )
    {
        var samples = new HashSet<string>( StringComparer.OrdinalIgnoreCase ) {
            "map20", "map22-1", "map23-1", "map31-1", "map34-3",
            "camp08", "camp12", "camp14", "camp15", "camp16",
            "cursor_t122", "cursor_t123", "gameover_t137", "gameover_t138",
        };
        var result = new TextureStorageSnapshot();
        AssetBundle bundle = AssetBundle.LoadFromFile( path );
        if ( bundle == null ) throw new InvalidOperationException( "Candidate bundle cannot be loaded." );
        Func<float, string> number = value => value.ToString( "R", CultureInfo.InvariantCulture );
        try
        {
            string[] names = bundle.GetAllAssetNames(); Array.Sort( names, StringComparer.Ordinal );
            result.assets = names.Length;
            foreach ( string name in names )
            {
                Texture2D texture = bundle.LoadAsset<Texture2D>( name );
                if ( texture == null ) throw new InvalidOperationException( "Texture missing: " + name );
                result.descriptors.Add( name + "|texture|" + texture.name + "|" + texture.width + "|" + texture.height +
                    "|" + texture.format + "|" + texture.mipmapCount + "|" + texture.filterMode + "|" + texture.wrapModeU +
                    "|" + texture.wrapModeV + "|" + texture.wrapModeW + "|" + texture.anisoLevel + "|" + number( texture.mipMapBias ) );
                Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>( name );
                Array.Sort( sprites, ( a, b ) => string.CompareOrdinal( a.name, b.name ) );
                foreach ( Sprite sprite in sprites )
                {
                    Rect rect = sprite.rect; Vector2 pivot = sprite.pivot; Vector4 border = sprite.border;
                    result.descriptors.Add( name + "|sprite|" + sprite.name + "|" + sprite.texture.name + "|" +
                        number( rect.x ) + "|" + number( rect.y ) + "|" + number( rect.width ) + "|" + number( rect.height ) + "|" +
                        number( pivot.x ) + "|" + number( pivot.y ) + "|" + number( sprite.pixelsPerUnit ) + "|" +
                        number( border.x ) + "|" + number( border.y ) + "|" + number( border.z ) + "|" + number( border.w ) );
                    result.sprites++;
                }
                if ( samples.Contains( Path.GetFileNameWithoutExtension( name ) ) )
                    result.gpuPixels.Add( name + "|" + TextureStorageGpuHash( texture ) );
            }
            if ( result.assets != 1819 || result.sprites != 2329 || result.gpuPixels.Count != samples.Count )
                throw new InvalidOperationException( "Storage verification coverage differs from the current approved bundle." );
            return result;
        }
        finally { bundle.Unload( true ); }
    }

    static string TextureStorageGpuHash( Texture2D texture )
    {
        RenderTexture previous = RenderTexture.active;
        RenderTexture target = RenderTexture.GetTemporary( texture.width, texture.height, 0,
            RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear );
        Texture2D pixels = null;
        try
        {
            Graphics.Blit( texture, target ); RenderTexture.active = target;
            pixels = new Texture2D( texture.width, texture.height, TextureFormat.RGBA32, false, true );
            pixels.ReadPixels( new Rect( 0, 0, texture.width, texture.height ), 0, 0, false );
            using ( SHA256 hash = SHA256.Create() )
                return BitConverter.ToString( hash.ComputeHash( pixels.GetRawTextureData() ) ).Replace( "-", "" ).ToLowerInvariant();
        }
        finally
        {
            RenderTexture.active = previous;
            if ( pixels != null ) UnityEngine.Object.DestroyImmediate( pixels );
            RenderTexture.ReleaseTemporary( target );
        }
    }
}
