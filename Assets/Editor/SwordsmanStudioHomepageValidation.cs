using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class SwordsmanStudioHomepageValidation
{
    static string Output
    {
        get
        {
            var args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf( args, "-swordsman-studio-output" );
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : "Logs/studio-homepage-20261115";
        }
    }
    const string Running = "Swordsman.StudioHomepage.Validation";
    static IEnumerator routine;
    static double deadline;
    static GameSetting.GameLocation previousLanguage;
    static bool languageCaptured;
    [Serializable] sealed class Result
    {
        public List<string> passed = new List<string>();
        public string error = "";
        public bool requestedSystemBrowser;
        public string homepage = GameHelpUI.StudioHomepageUrl;
    }
    static Result result;

    static GameHelpUI FindHelp()
    {
        foreach ( var root in EditorSceneManager.GetActiveScene().GetRootGameObjects() )
        {
            var help = root.GetComponentInChildren<GameHelpUI>( true );
            if ( help != null ) return help;
        }
        throw new Exception( "Settings panel missing" );
    }

    // Only the studio text, its two rects and the existing studio localization entry are edited.
    public static void Apply()
    {
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        var help = FindHelp();
        var button = help.transform.Find( "buttonB" ).GetComponent<RectTransform>();
        button.sizeDelta = new Vector2( 142, 10 );
        button.anchoredPosition = new Vector2( 0, -31 );
        var info = button.Find( "text" ).GetComponent<Text>();
        info.text = GameHelpUI.StudioInformation( false );
        info.rectTransform.sizeDelta = new Vector2( 284, 56 );
        info.rectTransform.anchoredPosition = new Vector2( 0, -9 );
        // Three lines render below this first-line-only button. The text itself does not receive clicks.
        info.raycastTarget = false;
        foreach ( var root in EditorSceneManager.GetActiveScene().GetRootGameObjects() )
        {
            var strings = root.GetComponentInChildren<GameStringData>( true );
            if ( strings == null ) continue;
            var serialized = new SerializedObject( strings );
            var entry = serialized.FindProperty( "data" ).GetArrayElementAtIndex( (int)GameStringType.ActiveGame0 );
            string existing = entry.FindPropertyRelative( "stringS" ).stringValue;
            if ( !existing.Contains( "xStudios" ) && !existing.Contains( "Biyou Village Studio" ) )
                throw new Exception( "Unexpected legacy studio localization entry" );
            entry.FindPropertyRelative( "stringS" ).stringValue = GameHelpUI.StudioInformation( false );
            entry.FindPropertyRelative( "stringT" ).stringValue = GameHelpUI.StudioInformation( true );
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }
        EditorSceneManager.MarkSceneDirty( EditorSceneManager.GetActiveScene() );
        EditorSceneManager.SaveScene( EditorSceneManager.GetActiveScene() );
        Debug.Log( "SWORDSMAN_STUDIO_MIGRATION_COMPLETE" );
    }

    static void ConfigureView( bool configure )
    {
        typeof(SwordsmanVirtualKeyboardSkinValidation).GetMethod( "ConfigureGameView", BindingFlags.Static | BindingFlags.NonPublic )
            .Invoke( null, new object[] { configure } );
    }

    public static void RunPlayMode()
    {
        Directory.CreateDirectory( Output );
        SwordsmanSceneValidation.ValidateScenes();
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        ConfigureView( true );
        SessionState.SetBool( Running, true );
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    static void Resume()
    {
        if ( !SessionState.GetBool( Running, false ) ) return;
        result = new Result(); deadline = EditorApplication.timeSinceStartup + 100;
        routine = Verify(); EditorApplication.update += Tick;
    }
    static void Tick()
    {
        try
        {
            if ( EditorApplication.timeSinceStartup > deadline ) throw new Exception( "Studio panel check timed out" );
            if ( EditorApplication.isPlaying && !routine.MoveNext() ) Finish( null );
        }
        catch ( Exception exception ) { Finish( exception.ToString() ); }
    }
    static void Check( bool condition, string name )
    {
        if ( !condition ) throw new Exception( name );
        result.passed.Add( name );
    }
    static IEnumerator Verify()
    {
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            yield return null;
        }
        previousLanguage = GameSetting.instance.location; languageCaptured = true;
        GameHelpUI.instance.show();
        double ready = EditorApplication.timeSinceStartup + 0.6;
        while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        var help = GameHelpUI.instance;
        var button = help.transform.Find( "buttonB" ).GetComponent<Button>();
        var info = button.transform.Find( "text" ).GetComponent<Text>();
        Check( button.onClick.GetPersistentEventCount() == 1 && button.onClick.GetPersistentMethodName( 0 ) == "onClickB" &&
            button.onClick.GetPersistentTarget( 0 ) == help && typeof(GameHelpUI).GetMethod( "onClickB" ) != null,
            "existing studio click binding resolves" );
        var uri = new Uri( GameHelpUI.StudioHomepageUrl );
        Check( uri.Scheme == "https" && uri.Host == "www.ideaboxapps.com" && uri.AbsolutePath == "/", "fixed official HTTPS homepage" );
        foreach ( bool traditional in new[] { false, true } )
        {
            GameSetting.instance.location = traditional ? GameSetting.GameLocation.TraditionalChinese : GameSetting.GameLocation.SimplifiedChinese;
            help.updateSettingText();
            for ( int i = 0; i < 4; i++ ) yield return null;
            Canvas.ForceUpdateCanvases();
            string expected = GameHelpUI.StudioInformation( traditional );
            Check( info.text == expected && info.text.Split( '\n' ).Length == 3, "exact three studio lines " + traditional );
            Check( !info.text.Contains( "（" ) && !info.text.Contains( "fox526594" ) && !info.text.Contains( "B站" ) &&
                !info.text.Split( '\n' )[0].EndsWith( traditional ? "主頁" : "主页" ), "no instruction parentheses, legacy promotion or homepage suffix " + traditional );
            Check( info.preferredWidth <= info.rectTransform.rect.width && info.preferredHeight <= info.rectTransform.rect.height &&
                info.cachedTextGenerator.lineCount == 3, "studio text fits without extra wrapping " + traditional );
            var rect = button.GetComponent<RectTransform>();
            for ( int line = 0; line < 3; line++ )
            {
                Vector3 world = info.rectTransform.TransformPoint( new Vector3( 0, 16 - line * 16, 0 ) );
                Vector2 screen = RectTransformUtility.WorldToScreenPoint( null, world );
                Check( RectTransformUtility.RectangleContainsScreenPoint( rect, screen ) == ( line == 0 ),
                    "only homepage line receives clicks: line " + ( line + 1 ) + " traditional " + traditional );
            }
            ScreenCapture.CaptureScreenshot( Path.GetFullPath( Output + ( traditional ? "/traditional.png" : "/simplified.png" ) ) );
            ready = EditorApplication.timeSinceStartup + 0.3;
            while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        }
        GameSetting.instance.location = previousLanguage; help.updateSettingText();
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-open-studio-homepage" ) >= 0 )
        {
            button.onClick.Invoke(); result.requestedSystemBrowser = true;
            ready = EditorApplication.timeSinceStartup + 0.5;
            while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        }
    }
    static void Finish( string error )
    {
        SessionState.SetBool( Running, false ); EditorApplication.update -= Tick;
        if ( languageCaptured && GameSetting.instance != null ) GameSetting.instance.location = previousLanguage;
        result.error = error ?? "";
        try { ConfigureView( false ); }
        catch ( Exception exception ) { result.error += " View restore: " + exception.Message; }
        Directory.CreateDirectory( Output ); File.WriteAllText( Output + "/editor-check.json", JsonUtility.ToJson( result, true ) );
        Debug.Log( "SWORDSMAN_STUDIO_CHECK " + ( result.error.Length == 0 ? "passed" : result.error ) );
        EditorApplication.Exit( result.error.Length == 0 ? 0 : 1 );
    }
}
