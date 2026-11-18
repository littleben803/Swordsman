using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
// Opt-in regression of the real title/load callbacks; does not read or write save files.
public sealed class SwordsmanTitleTransitionChecks : MonoBehaviour
{
    [Serializable]
    public sealed class Result
    {
        public int completedCycles;
        public int sampledFrames;
        public int hiddenLoadFrames;
        public int movingMenuFrames;
        public int independentTextShadows;
        public bool fadeEndpointCallbacks;
        public bool earlyCancelBlocked;
        public List<string> errors = new List<string>();
    }
    Result result = new Result();
    string output;
    bool sampling;
    bool exiting;
    bool finished;
    Image menu;
    Text[] labels;
    float[] labelAlpha;
    Text loadTitle;
    Vector2 menuPosition;
    int cycle;
    int hiddenInCycle;
    HashSet<string> captures = new HashSet<string>();

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Install()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-title-transition-check" ) < 0 ) return;
        var go = new GameObject( "TitleTransitionChecks" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanTitleTransitionChecks>();
    }
    static void Invoke( string method )
    {
        typeof( GameTitleInput ).GetMethod( method, BindingFlags.Instance | BindingFlags.NonPublic ).Invoke( GameTitleInput.instance, null );
    }
    void Error( string message )
    {
        if ( !result.errors.Contains( message ) ) result.errors.Add( message );
    }
    IEnumerator Start()
    {
        var args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf( args, "-swordsman-title-transition-report" );
        if ( index < 0 || index + 1 >= args.Length ) { Application.Quit( 2 ); yield break; }
        output = args[index + 1];
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        float deadline = Time.realtimeSinceStartup + 20;
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            if ( Time.realtimeSinceStartup > deadline ) { Error( "Title initialization timed out." ); Finish(); yield break; }
            yield return null;
        }
        GameTitleUI.instance.select( 1 );
        menu = GameTitleUI.instance.transform.Find( "title" ).GetComponent<Image>();
        labels = menu.GetComponentsInChildren<Text>();
        labelAlpha = new float[labels.Length];
        for ( int i = 0; i < labels.Length; i++ ) labelAlpha[i] = labels[i].color.a;
        menuPosition = menu.rectTransform.anchoredPosition;
        loadTitle = GameSLUI.instance.transform.Find( "text" ).GetComponent<Text>();
        foreach ( var label in labels )
        {
            var shadow = label.GetComponent<Shadow>();
            if ( shadow != null && !shadow.useGraphicAlpha ) result.independentTextShadows++;
        }
        var loadShadow = loadTitle.GetComponent<Shadow>();
        if ( loadShadow != null && !loadShadow.useGraphicAlpha ) result.independentTextShadows++;
        sampling = true;
        for ( cycle = 0; cycle < 3; cycle++ )
        {
            GameBlackUI.instance.showBlack( 1.0f, () => Invoke( "onShowBlackOverLoadGame" ) );
            deadline = Time.realtimeSinceStartup + 10;
            while ( !GameSLUI.instance.IsShow || GameSLUI.instance.IsFadeIn || GameBlackUI.instance.IsShow )
            {
                if ( Time.realtimeSinceStartup > deadline ) { Error( "Load entry timed out." ); Finish(); yield break; }
                yield return null;
            }
            if ( loadTitle.text != "前历再续" ) Error( "Local load title mismatch." );
            exiting = true;
            hiddenInCycle = 0;
            typeof( GameTitleInput ).GetField( "cancel", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( GameTitleInput.instance, true );
            Invoke( "updateSLUI" );
            deadline = Time.realtimeSinceStartup + 10;
            while ( !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn || GameBlackUI.instance.IsShow )
            {
                if ( Time.realtimeSinceStartup > deadline ) { Error( "Title return timed out." ); Finish(); yield break; }
                yield return null;
            }
            if ( Vector2.Distance( menu.rectTransform.anchoredPosition, menuPosition ) > 0.001f ) Error( "Menu position drifted." );
            result.completedCycles++;
            exiting = false;
            yield return null;
        }
        sampling = false;
        // A cancel during entry must not restart or reverse a partially initialized fade.
        GameSLUI.instance.show( GameSLType.Load ); GameSLUI.instance.showFade();
        typeof( GameTitleInput ).GetField( "cancel", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( GameTitleInput.instance, true );
        Invoke( "updateSLUI" );
        result.earlyCancelBlocked = GameSLUI.instance.IsFadeIn && !GameSLUI.instance.IsFadeOut;
        GameSLUI.instance.unShow();
        if ( !result.earlyCancelBlocked ) Error( "Early cancel interrupted entry." );
        yield return CheckEndpoint();
        if ( result.hiddenLoadFrames == 0 || result.movingMenuFrames == 0 ) Error( "Transition coverage missing." );
        yield return null;
        Finish();
    }
    IEnumerator CheckEndpoint()
    {
        var root = new GameObject( "FadeEndpointProbe", typeof( RectTransform ), typeof( CanvasRenderer ), typeof( Image ), typeof( GameUIFade ) );
        var child = new GameObject( "Label", typeof( RectTransform ), typeof( CanvasRenderer ), typeof( Text ) );
        child.transform.SetParent( root.transform, false );
        var image = root.GetComponent<Image>(); var text = child.GetComponent<Text>();
        text.canvasRenderer.SetAlpha( 0.65f );
        var fade = root.GetComponent<GameUIFade>();
        fade.FadeOutTime = 0; fade.FadeOutDistance = 17; fade.FadeOutFadeType = GameUIFadeType.West;
        bool outOK = false;
        fade.startUIFade( false, () => { outOK = image.color.a == 0 && text.canvasRenderer.GetAlpha() == 0 && root.GetComponent<RectTransform>().anchoredPosition.x == -17; } );
        fade.updateFade(); fade.updateUIStageFadeOver();
        fade.FadeInTime = 0; fade.FadeInDistance = 17; fade.FadeInFadeType = GameUIFadeType.West;
        bool inOK = false;
        fade.startUIFade( true, () => { inOK = image.color.a == 1 && text.color.a == 1 && Mathf.Approximately( text.canvasRenderer.GetAlpha(), 0.65f ) && root.GetComponent<RectTransform>().anchoredPosition.x == 0; } );
        fade.updateFade();
        result.fadeEndpointCallbacks = outOK && inOK;
        if ( !result.fadeEndpointCallbacks ) Error( "Fade callback observed non-final graphics." );
        Destroy( root ); yield return null;
    }
    void LateUpdate()
    {
        if ( !sampling || finished ) return;
        result.sampledFrames++;
        if ( GameTitleUI.instance.gameObject.activeInHierarchy )
        {
            float alpha = menu.color.a;
            for ( int i = 0; i < labels.Length; i++ )
                if ( Mathf.Abs( labels[i].color.a * labels[i].canvasRenderer.GetAlpha() / labelAlpha[i] - alpha ) > 0.002f ) Error( "Menu text/background alpha diverged: " + labels[i].name );
            if ( GameTitleUI.instance.IsFadeIn && alpha > 0 && alpha < 1 )
            {
                result.movingMenuFrames++;
                Capture( "return", alpha );
            }
        }
        var load = GameSLUI.instance;
        if ( exiting && load.gameObject.activeInHierarchy && !load.IsFadeIn && !load.IsFadeOut && GameBlackUI.instance.IsShow )
        {
            result.hiddenLoadFrames++;
            if ( loadTitle.color.a * loadTitle.canvasRenderer.GetAlpha() != 0 ) Error( "Load title remained visible after fade-out." );
            // Let the canvas and GPU consume the terminal state before taking visual evidence.
            if ( ++hiddenInCycle >= 3 ) Capture( "exit", 0 );
        }
    }
    void Capture( string phase, float progress )
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-title-transition-capture" ) < 0 ) return;
        string key = "cycle" + cycle + "-" + phase + "-" + Mathf.FloorToInt( progress * 4 );
        if ( captures.Add( key ) ) ScreenCapture.CaptureScreenshot( Path.Combine( Path.GetDirectoryName( output ), key + ".png" ) );
    }
    void Finish()
    {
        if ( finished ) return; finished = true;
        File.WriteAllText( output, JsonUtility.ToJson( result, true ) );
        Debug.Log( "SWORDSMAN_TITLE_TRANSITION passed=" + ( result.errors.Count == 0 ) + " cycles=" + result.completedCycles + " frames=" + result.sampledFrames );
        Application.Quit( result.errors.Count == 0 ? 0 : 1 );
    }
}
#endif
