using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class SwordsmanMacBuild
{
    private static readonly string[] Scenes =
    {
        "Assets/Scenes/level0.unity",
        "Assets/Scenes/level1.unity",
        "Assets/Scenes/level2.unity",
        "Assets/Scenes/level3.unity",
    };

    public const string PlayerRelativePath = "Builds/macOS/Swordsman-ChapterTests-AllChapters.app";

    public static void BuildDevelopmentPlayerChapterTestsAllChapters()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-cache-benchmark" ) >= 0 )
            throw new InvalidOperationException( "Separate benchmark players are retired. Use the canonical AllChapters build without the old benchmark argument." );
        var timer = System.Diagnostics.Stopwatch.StartNew();
        BuildChapterTests();
        Debug.LogFormat( "SWORDSMAN_MAC_BUILD_TOTAL seconds={0:F3}", timer.Elapsed.TotalSeconds );
    }

    static void BuildChapterTests()
    {
        const string fixtureFolder = "Assets/StreamingAssets/SwordsmanChapterTests";
        if ( Directory.Exists( fixtureFolder ) )
            throw new InvalidOperationException( "Reserved chapter test staging folder already exists." );
        string[] buildArgs = Environment.GetCommandLineArgs();
        int reuseIndex = Array.IndexOf( buildArgs, "-swordsman-reuse-hd-bundle-sha256" );
        if ( reuseIndex >= 0 && reuseIndex + 1 >= buildArgs.Length ) throw new InvalidOperationException( "Missing locked bundle SHA256." );
        string bundlePath = reuseIndex < 0 ? SwordsmanHd4xBuild.BuildAndValidateApprovedBundle()
            : SwordsmanHd4xBuild.ValidateExistingApprovedBundle( buildArgs[reuseIndex + 1] );
        SwordsmanHd4xBuild.StageBundleForPlayer( bundlePath );
        try
        {
            Directory.CreateDirectory( fixtureFolder );
            File.Copy( "Assets/ChapterTests/presets.json", fixtureFolder + "/presets.json", false );
            if ( Directory.Exists( "Assets/ChapterTests/Checkpoints" ) )
            {
                foreach ( string file in Directory.GetFiles( "Assets/ChapterTests/Checkpoints", "*", SearchOption.AllDirectories ) )
                {
                    string name = Path.GetFileName( file );
                    if ( name != "BattleStart.dat" && name != "BattleStart.json" ) continue;
                    string target = fixtureFolder + "/Checkpoints" + file.Substring( "Assets/ChapterTests/Checkpoints".Length );
                    Directory.CreateDirectory( Path.GetDirectoryName( target ) ); File.Copy( file, target, false );
                }
            }
            AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
            string[] defines = { "SWORDSMAN_CHAPTER_TESTS", "SWORDSMAN_CHAPTER_TESTS_LARGE_WINDOW" };
            BuildDevelopmentPlayerInternal( defines, 2560, 1600 );
        }
        finally
        {
            AssetDatabase.DeleteAsset( fixtureFolder );
            SwordsmanHd4xBuild.RemoveStagedPlayerBundle();
        }
    }

    static void BuildDevelopmentPlayerInternal( string[] extraDefines, int width, int height )
    {
        string outputPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "../" + PlayerRelativePath));
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.CompressWithLz4HC,
        };

        if ( extraDefines != null )
        {
            options.extraScriptingDefines = extraDefines;
        }

        int oldWidth = PlayerSettings.defaultScreenWidth;
        int oldHeight = PlayerSettings.defaultScreenHeight;
        FullScreenMode oldMode = PlayerSettings.fullScreenMode;
        string settingsPath = Path.GetFullPath( Path.Combine( Application.dataPath, "../ProjectSettings/ProjectSettings.asset" ) );
        byte[] originalSettings = File.ReadAllBytes( settingsPath );

        BuildReport report;
        try
        {
            PlayerSettings.defaultScreenWidth = width;
            PlayerSettings.defaultScreenHeight = height;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            report = BuildPipeline.BuildPlayer(options);
        }
        finally
        {
            PlayerSettings.defaultScreenWidth = oldWidth;
            PlayerSettings.defaultScreenHeight = oldHeight;
            PlayerSettings.fullScreenMode = oldMode;
            AssetDatabase.SaveAssets();
            File.WriteAllBytes( settingsPath, originalSettings );
        }

        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                "macOS Development Player build failed: " + summary.result);
        }

        RunMacBuildPostprocess( outputPath, "share_texture_storage.py", "Logs/TextureStorage/builds", false );
        RunMacBuildPostprocess( outputPath, "macos_splash_postprocess.py", "Logs/UnitySplash/builds", true );
        // Sharing/splash rewrite only affected blocks with system LZ4. Normalize
        // both final containers through Unity's built-in HC codec, then re-sign.
        RecompressPlayerArchives( outputPath );

        Debug.LogFormat(
            "SWORDSMAN_MAC_BUILD result={0} errors={1} warnings={2} size={3} output={4}",
            summary.result,
            summary.totalErrors,
            summary.totalWarnings,
            summary.totalSize,
            summary.outputPath);

    }

    public static void ValidateLz4HcRecompression()
    {
        string[] args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf( args, "-swordsman-lz4hc-probe-report" );
        if ( index >= 0 && index + 1 >= args.Length ) throw new InvalidOperationException( "Missing HC probe report path." );
        RecompressArchive( Path.GetFullPath( PlayerRelativePath + "/Contents/Resources/Data/data.unity3d" ),
            Path.GetFullPath( index < 0 ? "Logs/Lz4HC/probe.json" : args[index + 1] ), false );
    }

    static void RecompressPlayerArchives( string outputPath )
    {
        string evidence = Path.GetFullPath( "Logs/Lz4HC/builds/" + DateTime.UtcNow.ToString( "yyyyMMddTHHmmssfffffffZ" ) );
        string data = Path.Combine( outputPath, "Contents/Resources/Data" );
        RecompressArchive( Path.Combine( data, "data.unity3d" ), Path.Combine( evidence, "data.unity3d.json" ), true );
        RecompressArchive( Path.Combine( data, "StreamingAssets/SwordsmanHD4X/swordsman-hd4x" ),
            Path.Combine( evidence, "swordsman-hd4x.json" ), true );
        string hdBundle = Path.Combine( data, "StreamingAssets/SwordsmanHD4X/swordsman-hd4x" );
        string lzfse = hdBundle + ".swlzfse";
        string lzfseReport = Path.GetFullPath( "Logs/LZFSE/builds/" + DateTime.UtcNow.ToString( "yyyyMMddTHHmmssfffffffZ" ) + "/container.json" );
        RunProcess( "/usr/bin/python3", "-B " + Quote( Path.GetFullPath( "scripts/validate_lzfse_bundle.py" ) )
            + " --input " + Quote( hdBundle ) + " --output " + Quote( lzfse )
            + " --report " + Quote( lzfseReport ) + " --production" );
        File.Delete( hdBundle );
        File.Move( lzfse, hdBundle );
        RunProcess( "/usr/bin/codesign", "--force --sign - " + Quote( outputPath ) );
        RunProcess( "/usr/bin/codesign", "--verify --deep --strict " + Quote( outputPath ) );
    }

    static void RecompressArchive( string input, string report, bool replace )
    {
        string output = input + ".lz4hc-tmp";
        if ( File.Exists( output ) ) throw new InvalidOperationException( "Unfinished HC output exists: " + output );
        var timer = System.Diagnostics.Stopwatch.StartNew();
        try
        {
            // Unity 2021's RecompressAssetBundleAsync rejects LZ4HC. Use the
            // matching Editor's own exported HC codec through a bounded stdio worker.
            var start = new System.Diagnostics.ProcessStartInfo( "/usr/bin/python3",
                "-B " + Quote( Path.GetFullPath( "scripts/recompress_unityfs.py" ) )
                + " --input " + Quote( input ) + " --output " + Quote( output ) + " --report " + Quote( report )
                + " --unity-native-hc" + ( replace ? " --replace" : "" ) )
            {
                UseShellExecute = false, RedirectStandardInput = true,
                RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true,
            };
            using ( var process = System.Diagnostics.Process.Start( start ) )
            {
                var errors = process.StandardError.ReadToEndAsync();
                var worker = System.Threading.Tasks.Task.Run( () =>
                {
                    using ( var reader = new BinaryReader( process.StandardOutput.BaseStream ) )
                    using ( var writer = new BinaryWriter( process.StandardInput.BaseStream ) )
                    {
                        while ( true )
                        {
                            int length = reader.ReadInt32();
                            if ( length == 0 ) break;
                            if ( length < 0 || length > 16 * 1024 * 1024 ) throw new InvalidOperationException( "Invalid HC request length." );
                            byte[] raw = reader.ReadBytes( length );
                            if ( raw.Length != length ) throw new EndOfStreamException( "Truncated HC request." );
                            byte[] packed = new byte[length + length / 255 + 32];
                            int size = CompressHc( raw, packed, raw.Length, packed.Length, 12 );
                            if ( size <= 0 ) throw new InvalidOperationException( "Unity native HC encoding failed." );
                            writer.Write( size ); writer.Write( packed, 0, size ); writer.Flush();
                        }
                    }
                } );
                try
                {
                    if ( !worker.Wait( 720000 ) || !process.WaitForExit( 720000 ) )
                        throw new TimeoutException( "HC verification worker timed out." );
                    Debug.Log( errors.GetAwaiter().GetResult() );
                    if ( process.ExitCode != 0 ) throw new InvalidOperationException( "HC verification failed; archive was not approved." );
                }
                finally { if ( !process.HasExited ) process.Kill(); }
            }
            Debug.LogFormat( "SWORDSMAN_LZ4HC archive={0} seconds={1:F3} report={2}", input, timer.Elapsed.TotalSeconds, report );
        }
        finally { if ( File.Exists( output ) ) File.Delete( output ); }
    }

    [System.Runtime.InteropServices.DllImport( "__Internal", EntryPoint = "LZ4_compress_HC" )]
    static extern int CompressHc( byte[] source, byte[] destination, int sourceSize, int capacity, int level );

    static string Quote( string value )
    {
        return "\"" + value.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ) + "\"";
    }

    static void RunProcess( string executable, string arguments )
    {
        var start = new System.Diagnostics.ProcessStartInfo( executable, arguments )
        {
            UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true, CreateNoWindow = true,
        };
        using ( var process = System.Diagnostics.Process.Start( start ) )
        {
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            if ( !process.WaitForExit( 720000 ) ) { process.Kill(); throw new TimeoutException( executable ); }
            Debug.Log( stdout.GetAwaiter().GetResult() );
            if ( process.ExitCode != 0 ) throw new InvalidOperationException( stderr.GetAwaiter().GetResult() );
        }
    }

    static void RunMacBuildPostprocess( string outputPath, string script, string evidence, bool inspectSplash )
    {
        string root = Path.GetFullPath( Path.Combine( Application.dataPath, ".." ) );
        Func<string, string> quote = value => "\"" + value.Replace( "\\", "\\\\" ).Replace( "\"", "\\\"" ) + "\"";
        var start = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "/usr/bin/python3",
            Arguments = quote( Path.Combine( root, "scripts/" + script ) )
                + " --app " + quote( outputPath )
                + ( inspectSplash ? " --binary2text " + quote( Path.Combine( EditorApplication.applicationContentsPath, "Tools/binary2text" ) ) : "" )
                + " --evidence " + quote( Path.Combine( root, evidence ) ),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        using ( var process = System.Diagnostics.Process.Start( start ) )
        {
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            if ( !process.WaitForExit( 720000 ) )
            {
                process.Kill();
                throw new InvalidOperationException( "macOS build postprocessing timed out (" + script + "); do not distribute this build." );
            }
            Debug.Log( stdout.GetAwaiter().GetResult() );
            if ( process.ExitCode != 0 )
                throw new InvalidOperationException( "macOS build postprocessing failed (" + script + "); do not distribute this build. "
                    + stderr.GetAwaiter().GetResult() );
        }
    }
}
