using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

// Default macOS company startup cover; no scene or splash settings change.
public sealed class SwordsmanStartupPrototype : MonoBehaviour
{
    static SwordsmanStartupPrototype instance;
    static readonly DateTime processStart = ReadProcessStart();
    static bool firstLogoSubmitted;
    static readonly List<string> phases = new List<string>();
    // Approved 2560x1600 company lockup, including its intentional transparent margins.
    public static Rect LogoRect { get { return new Rect( 0, 0, 1280, 800 ); } }
    public static Rect LogoUV { get { return new Rect( 0, 0, 1, 1 ); } }
    Texture2D logo;
    bool visible = true;

    public static bool Enabled { get { return Supported && !Application.isBatchMode; } }
    public static bool IsVisible { get { return instance != null && instance.visible; } }
    public static bool Tracing { get { return Enabled || ( Supported && HasArgument( "-swordsman-startup-timing" ) ); } }
    public static void CapturePreview( string name )
    {
        if ( !HasArgument( "-swordsman-startup-capture" ) ) return;
        if ( !Enabled ) name = "baseline-" + name;
        string[] args = Environment.GetCommandLineArgs();
        int directoryIndex = Array.IndexOf( args, "-swordsman-startup-capture-directory" );
        string directory;
        if ( directoryIndex >= 0 && directoryIndex + 1 < args.Length ) directory = System.IO.Path.GetFullPath( args[directoryIndex + 1] );
#if UNITY_EDITOR
        else directory = System.IO.Path.GetFullPath( System.IO.Path.Combine( Application.dataPath, "../Logs/startup-company-logo-20261113/captures" ) );
#else
        else return;
#endif
        System.IO.Directory.CreateDirectory( directory );
        ScreenCapture.CaptureScreenshot( System.IO.Path.Combine( directory, name + ".png" ) );
    }
    static bool Supported
    {
        get
        {
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            return true;
#else
            return false;
#endif
        }
    }
    static bool HasArgument( string argument ) { return Array.IndexOf( Environment.GetCommandLineArgs(), argument ) >= 0; }
    static DateTime ReadProcessStart()
    {
        try { using ( var process = System.Diagnostics.Process.GetCurrentProcess() ) return process.StartTime.ToUniversalTime(); }
        catch { return DateTime.UtcNow; }
    }
    public static void Mark( string phase )
    {
        if ( !Tracing ) return;
        phases.Add( phase );
        Debug.Log( "SWORDSMAN_STARTUP " + JsonUtility.ToJson( new Sample {
            phase = phase, processMilliseconds = ( DateTime.UtcNow - processStart ).TotalMilliseconds,
            unitySeconds = Time.realtimeSinceStartup, frame = Time.frameCount } ) );
    }
    [Serializable] sealed class Sample { public string phase; public double processMilliseconds; public float unitySeconds; public int frame; }

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSplashScreen )]
    static void BeforeSplash() { Mark( "before_splash_callback" ); }

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
    static void Create()
    {
        if ( !Tracing ) return;
        Mark( "before_first_scene" );
        SceneManager.sceneLoaded += SceneLoaded;
        if ( Enabled && !Application.isBatchMode )
        {
            var go = new GameObject( "SwordsmanStartupPrototype" );
            DontDestroyOnLoad( go ); instance = go.AddComponent<SwordsmanStartupPrototype>();
        }
    }
    static void SceneLoaded( Scene scene, LoadSceneMode mode ) { Mark( "scene_loaded_" + scene.buildIndex ); }
    public static void Finish( string reason )
    {
        Mark( reason );
        if ( instance != null ) instance.visible = false;
        if ( reason == "opening_finished_or_skipped" && instance != null && HasArgument( "-swordsman-startup-report" ) )
            instance.StartCoroutine( instance.VerifyOpening() );
    }
    public static void BeginVideo()
    {
        if ( instance == null || instance.visible ) return;
        instance.visible = true;
    }
    void Awake()
    {
        // Load before the first GUI repaint, avoiding a placeholder-to-video transition.
        logo = Resources.Load<Texture2D>( "StartupPrototype/PortraitStill" );
        Mark( logo != null ? "startup_static_logo_loaded" : "startup_static_logo_missing" );
    }
    [Serializable] sealed class PlaybackCheck
    {
        public bool passed, staticLogoLoaded, noPortraitVideoPlayer, logoRenderedBeforeReveal, originalContentDecoded, originalFinished;
        public int logoTextureWidth, logoTextureHeight;
        public Rect logoLogicalRect;
        public string[] phases;
    }
    IEnumerator VerifyOpening()
    {
        yield return null;
        CapturePreview( "after-opening" );
        yield return new WaitForEndOfFrame();
        yield return null;
        int repaint = phases.IndexOf( "logo_first_repaint_submitted" );
        int content = phases.IndexOf( "video_first_content_frame_ready" );
        int reveal = phases.IndexOf( "logo_hidden_after_video_render" );
        var check = new PlaybackCheck { staticLogoLoaded = logo != null && phases.Contains( "startup_static_logo_loaded" ),
            noPortraitVideoPlayer = GetComponents<UnityEngine.Video.VideoPlayer>().Length == 0,
            logoTextureWidth = logo == null ? 0 : logo.width, logoTextureHeight = logo == null ? 0 : logo.height,
            logoLogicalRect = LogoRect,
            logoRenderedBeforeReveal = repaint >= 0 && repaint < content && content < reveal,
            originalContentDecoded = content >= 0, originalFinished = phases.Contains( "opening_finished_or_skipped" ), phases = phases.ToArray() };
        // Company PNG is the only startup content; retain the existing no-hold video handoff.
        check.passed = check.staticLogoLoaded && check.noPortraitVideoPlayer && check.logoTextureWidth == 2560 && check.logoTextureHeight == 1600 &&
            check.logoRenderedBeforeReveal && check.originalContentDecoded && check.originalFinished && !phases.Contains( "video_error" );
        string[] args = Environment.GetCommandLineArgs(); int index = Array.IndexOf( args, "-swordsman-startup-report" );
        if ( index + 1 < args.Length )
        {
            string path = System.IO.Path.GetFullPath( args[index + 1] );
            System.IO.Directory.CreateDirectory( System.IO.Path.GetDirectoryName( path ) );
            System.IO.File.WriteAllText( path, JsonUtility.ToJson( check, true ) );
        }
        Application.Quit( check.passed ? 0 : 1 );
    }
    void OnGUI()
    {
        if ( !visible ) return;
        Matrix4x4 oldMatrix = GUI.matrix; Color oldColor = GUI.color; int oldDepth = GUI.depth;
        GUI.depth = -2000; GUI.matrix = Matrix4x4.identity;
        GUI.color = new Color32( 14, 17, 21, 255 );
        GUI.DrawTexture( new Rect( 0, 0, Screen.width, Screen.height ), Texture2D.whiteTexture ); GUI.color = Color.white;
        // One uniform scale keeps the logo proportional during window initialization/resizing.
        float scale = Mathf.Min( Screen.width / 1280f, Screen.height / 800f );
        GUI.matrix = Matrix4x4.TRS( new Vector3( ( Screen.width - 1280 * scale ) * 0.5f, ( Screen.height - 800 * scale ) * 0.5f, 0 ),
            Quaternion.identity, new Vector3( scale, scale, 1 ) );
        if ( logo != null ) GUI.DrawTextureWithTexCoords( LogoRect, logo, LogoUV, true );
        if ( Event.current.type == EventType.Repaint && !firstLogoSubmitted )
        {
            firstLogoSubmitted = true; Mark( "logo_first_repaint_submitted" ); CapturePreview( "logo" );
        }
        GUI.matrix = oldMatrix; GUI.color = oldColor; GUI.depth = oldDepth;
    }
}
