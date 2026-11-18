using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Player;
using System;
using System.Collections.Generic;
using System.IO;

public static class SwordsmanStartupPrototypeValidation
{
    const string Running = "Swordsman.StartupPrototype.ValidationRunning";
    static double deadline;
    static int step, callbacks;
    static bool prepared, firstFrame, contentFrame, revealed, finished;
    static bool staticLogoLoaded;
    static List<string> phases = new List<string>();
    static string output;
    static double captureDeadline;
    [Serializable] sealed class Result
    {
        public string environment = "Unity Editor Play Mode; not standalone cold-start measurement";
        public bool prototype, prepared, firstFrame, revealed;
        public bool skipCallbackOnce, missingVideoCallbackOnce;
        public bool staticLogoLoaded, noPortraitVideoPlayer;
        public int logoTextureWidth, logoTextureHeight;
        public Rect logoLogicalRect;
        public string error = "";
        public string[] phases;
    }
    static Result result;
    const string LogoPath = "Assets/Resources/StartupPrototype/PortraitStill.png";
    const string LogoGuid = "981a079c580e946d9a4e6b5bb21f132d";

    // User-approved single-image import change; no other importers are touched.
    public static void ConfigureAndValidate()
    {
        if ( AssetDatabase.AssetPathToGUID( LogoPath ) != LogoGuid ) throw new Exception( "Logo GUID changed." );
        AssetDatabase.ImportAsset( LogoPath, ImportAssetOptions.ForceSynchronousImport );
        var importer = (TextureImporter)AssetImporter.GetAtPath( LogoPath );
        importer.textureType = TextureImporterType.Default;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 4096;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaSource = TextureImporterAlphaSource.FromInput;
        importer.alphaIsTransparency = true;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        var standalone = importer.GetPlatformTextureSettings( "Standalone" );
        standalone.overridden = true;
        standalone.maxTextureSize = 4096;
        standalone.format = TextureImporterFormat.RGBA32;
        standalone.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SetPlatformTextureSettings( standalone );
        importer.SaveAndReimport();
        Validate();
        SwordsmanSceneValidation.ValidateScenes();
    }
    public static void Validate()
    {
        VideoClip clip = Resources.Load<VideoClip>( "Movie/Title" );
        if ( clip == null || clip.frameCount == 0 ) throw new Exception( "Original opening video is unavailable." );
        var logo = AssetDatabase.LoadAssetAtPath<Texture2D>( LogoPath );
        var importer = (TextureImporter)AssetImporter.GetAtPath( LogoPath );
        if ( logo == null || logo.width != 2560 || logo.height != 1600 || logo.format != TextureFormat.RGBA32 ||
            AssetDatabase.AssetPathToGUID( LogoPath ) != LogoGuid || importer.npotScale != TextureImporterNPOTScale.None ||
            importer.mipmapEnabled || !importer.alphaIsTransparency ) throw new Exception( "Static logo texture/import settings failed." );
        Rect rect = SwordsmanStartupPrototype.LogoRect, uv = SwordsmanStartupPrototype.LogoUV;
        if ( rect != new Rect( 0, 0, 1280, 800 ) || uv != new Rect( 0, 0, 1, 1 ) )
            throw new Exception( "Company logo geometry failed." );
        string source = "Builds/design/company-logo-20261113/ideabox-company-startup-v1.png";
        using ( var sha = System.Security.Cryptography.SHA256.Create() )
        {
            if ( !Convert.ToBase64String( sha.ComputeHash( File.ReadAllBytes( LogoPath ) ) ).Equals(
                Convert.ToBase64String( sha.ComputeHash( File.ReadAllBytes( source ) ) ) ) )
                throw new Exception( "Runtime PNG differs from the approved company logo." );
        }
        string coverSource = File.ReadAllText( "Assets/Scripts/Title/SwordsmanStartupPrototype.cs" );
        if ( coverSource.Contains( "GUI.Label" ) || coverSource.Contains( "GUIStyle" ) || coverSource.Contains( "loading_dots_" ) )
            throw new Exception( "Startup cover must contain only the company PNG, without text or loading animation." );
        Debug.LogFormat( "SWORDSMAN_STARTUP_VALIDATION staticLogo={0}x{1} format={2} top={3} ratio={4} clip={5} frames={6}",
            logo.width, logo.height, logo.format, rect.y, rect.width / rect.height, clip.name, clip.frameCount );
    }

    // Compile actual macOS release scripts only: no Player, staging, build postprocess or settings changes.
    public static void ValidateReleaseScripts()
    {
        Validate();
        SwordsmanSceneValidation.ValidateScenes();
        const string directory = "Logs/startup-company-logo-default-20261113/release-scripts";
        Directory.CreateDirectory( directory );
        var settings = new ScriptCompilationSettings {
            target = BuildTarget.StandaloneOSX, group = BuildTargetGroup.Standalone,
            options = ScriptCompilationOptions.None, extraScriptingDefines = new string[0] };
        var result = PlayerBuildInterface.CompilePlayerScripts( settings, directory );
        string assemblyPath = Path.Combine( directory, "Assembly-CSharp.dll" );
        if ( result.assemblies == null || result.assemblies.Count == 0 || !File.Exists( assemblyPath ) )
            throw new Exception( "Release script compilation failed." );
        // Load the freshly compiled release bytes, not the Editor's Assembly-CSharp.
        var assembly = System.Reflection.Assembly.Load( File.ReadAllBytes( assemblyPath ) );
        var type = assembly.GetType( "SwordsmanStartupPrototype", true );
        var flags = System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic;
        bool supported = (bool)type.GetProperty( "Supported", flags ).GetValue( null, null );
        bool enabled = (bool)type.GetProperty( "Enabled" ).GetValue( null, null );
        if ( !supported || enabled != !Application.isBatchMode )
            throw new Exception( "Release compilation does not enable the default macOS logo correctly." );
        Debug.Log( "SWORDSMAN_STARTUP_RELEASE_SCRIPTS passed supported=" + supported +
            " enabled=" + enabled + " batchMode=" + Application.isBatchMode + " assemblies=" + result.assemblies.Count );
    }

    // CLI entry: omit -quit so actual Play Mode frames and AVFoundation decoding can run.
    public static void RunPlayMode()
    {
        Validate();
        if ( !SwordsmanStartupPrototype.Enabled ) throw new Exception( "Default macOS startup cover must be enabled in graphical Play Mode." );
        SessionState.SetBool( Running, true );
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-startup-capture" ) >= 0 )
            EditorApplication.ExecuteMenuItem( "Window/General/Game" );
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    static void Resume()
    {
        if ( !SessionState.GetBool( Running, false ) ) return;
        output = Path.GetFullPath( "Logs/startup-company-logo-default-20261113/editor-playmode.json" );
        result = new Result { prototype = SwordsmanStartupPrototype.Enabled };
        deadline = EditorApplication.timeSinceStartup + 90;
        Application.logMessageReceived += Log;
        EditorApplication.update += Tick;
    }
    static void Log( string message, string stack, LogType type )
    {
        if ( !message.StartsWith( "SWORDSMAN_STARTUP {" ) ) return;
        phases.Add( message.Substring( "SWORDSMAN_STARTUP ".Length ) );
        if ( message.Contains( "\"video_prepared\"" ) ) prepared = true;
        if ( message.Contains( "\"video_first_frame_ready\"" ) ) firstFrame = true;
        if ( message.Contains( "\"video_first_content_frame_ready\"" ) ) contentFrame = true;
        if ( message.Contains( "\"logo_hidden_after_video_render\"" ) ) revealed = true;
        if ( message.Contains( "\"startup_static_logo_loaded\"" ) ) staticLogoLoaded = true;
    }
    static void Tick()
    {
        if ( finished ) return;
        try
        {
            if ( EditorApplication.timeSinceStartup > deadline ) throw new Exception( "Play Mode validation timed out at step " + step );
            if ( !EditorApplication.isPlaying || GameMovieManager.instance == null ) return;
            var movie = GameMovieManager.instance;
            if ( step == 0 && firstFrame && contentFrame && ( !result.prototype || revealed ) )
            {
                SwordsmanStartupPrototype.CapturePreview( "video-after-reveal" );
                captureDeadline = EditorApplication.timeSinceStartup + 1;
                step = 3;
            }
            else if ( step == 3 && EditorApplication.timeSinceStartup >= captureDeadline )
            {
                result.prepared = prepared; result.firstFrame = true; result.revealed = revealed;
                result.staticLogoLoaded = staticLogoLoaded;
                var cover = UnityEngine.Object.FindObjectOfType<SwordsmanStartupPrototype>();
                result.noPortraitVideoPlayer = cover != null && cover.GetComponents<VideoPlayer>().Length == 0;
                var logo = Resources.Load<Texture2D>( "StartupPrototype/PortraitStill" );
                result.logoTextureWidth = logo.width; result.logoTextureHeight = logo.height;
                result.logoLogicalRect = SwordsmanStartupPrototype.LogoRect;
                if ( result.prototype && ( !staticLogoLoaded || !result.noPortraitVideoPlayer ) )
                    throw new Exception( "Static logo / no-portrait-player check failed." );
                if ( result.prototype )
                {
                    callbacks = 0;
                    movie.playStartupPrototype( "Movie/Title", () => callbacks++ );
                    movie.stopMovie();
                    result.skipCallbackOnce = callbacks == 1 && !movie.IsPlaying;
                    if ( !result.skipCallbackOnce ) throw new Exception( "Skip callback or player state failed." );
                    callbacks = 0;
                    movie.playStartupPrototype( "__StartupPrototype_MissingVideo__", () => callbacks++ );
                    step = 1;
                }
                else Complete( null );
            }
            else if ( step == 1 && callbacks > 0 )
            {
                result.missingVideoCallbackOnce = callbacks == 1 && !movie.IsPlaying;
                if ( !result.missingVideoCallbackOnce ) throw new Exception( "Missing-video fallback failed." );
                Complete( null );
            }
        }
        catch ( Exception e ) { Complete( e.Message ); }
    }
    static void Complete( string error )
    {
        finished = true;
        result.error = error ?? ""; result.phases = phases.ToArray();
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        File.WriteAllText( output, JsonUtility.ToJson( result, true ) );
        SessionState.SetBool( Running, false );
        EditorApplication.update -= Tick; Application.logMessageReceived -= Log;
        Debug.Log( "SWORDSMAN_STARTUP_PLAYMODE " + ( error == null ? "passed" : error ) );
        EditorApplication.Exit( error == null ? 0 : 1 );
    }
}
