using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR || (SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD)
// Shared Editor/Player checks. No game-save writes; every touched preference is restored by the caller.
public static class SwordsmanDialogueVerification
{
    public sealed class Preferences
    {
        readonly Dictionary<string, object> values = new Dictionary<string, object>();
        public Preferences()
        {
            foreach ( string key in new[] { "fastDialogue", "virtualKeyboardEnabled", "touchScale", "mode", "location", "enableMusic", "enableSound", "quality" } )
                values[key] = !PlayerPrefs.HasKey( key ) ? null : key == "touchScale" ? (object)PlayerPrefs.GetFloat( key ) : PlayerPrefs.GetInt( key );
        }
        public void Restore()
        {
            foreach ( var pair in values )
                if ( pair.Value == null ) PlayerPrefs.DeleteKey( pair.Key );
                else if ( pair.Value is float ) PlayerPrefs.SetFloat( pair.Key, (float)pair.Value );
                else PlayerPrefs.SetInt( pair.Key, (int)pair.Value );
            PlayerPrefs.Save(); GameSetting.instance.load();
        }
    }
    static T Field<T>( object target, string name )
    {
        return (T)target.GetType().GetField( name, BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( target );
    }
    static PointerEventData Pointer( Transform node )
    {
        return new PointerEventData( EventSystem.current ) { pointerId = -1,
            position = RectTransformUtility.WorldToScreenPoint( null, node.position ) };
    }
    static GameImageText ActiveText()
    {
        return Field<GameMsgBoxTextUI>( GameMsgBoxUI.instance, "active" ).GetComponentInChildren<GameImageText>();
    }
    static IEnumerator Typing( bool story, bool fast, Action<float> measured )
    {
        GameSetting.instance.fastDialogue = fast;
        if ( story ) GameMsgBoxUI.instance.showDialogue( -1, 0, 0, "人物对话速度检查甲乙丙丁戊己庚辛壬癸", null );
        else GameMsgBoxUI.instance.showText( -1, 0, 0, "人物对话速度检查甲乙丙丁戊己庚辛壬癸", null );
        var text = ActiveText(); float start = Time.realtimeSinceStartup;
        while ( text.text.Length < 12 ) yield return null;
        measured( Time.realtimeSinceStartup - start );
    }
    public static IEnumerator Run( Action<bool, string> check, Action<string> capture, Func<int, IEnumerator> load )
    {
        float deadline = Time.realtimeSinceStartup + 30;
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            if ( Time.realtimeSinceStartup > deadline ) throw new Exception( "Title timeout" );
            yield return null;
        }
        PlayerPrefs.DeleteKey( "fastDialogue" ); GameSetting.instance.load();
        check( !GameSetting.instance.fastDialogue && GameSetting.instance.DialogueSpeedMultiplier == 1f, "old preferences default to normal dialogue" );
        var help = GameHelpUI.instance;
        help.show(); GameSetting.instance.virtualKeyboardEnabled = true; GameTouchCenterUI.instance.showUI();
        yield return new WaitForSecondsRealtime( 0.7f );
        check( help.transform.Find( "dialogue" ) != null && help.transform.Find( "dialogueValue" ).GetComponent<Text>().text == "正常", "single dialogue row defaults to normal" );
        float previous = float.MaxValue;
        foreach ( string name in new[] { "setting0", "dialogue", "setting1", "keyboard", "setting2" } )
        {
            float y = ((RectTransform)help.transform.Find( name )).anchoredPosition.y;
            check( previous - y >= 16, "settings row order and spacing " + name ); previous = y;
        }
        foreach ( bool traditional in new[] { false, true } )
        {
            GameSetting.instance.location = traditional ? GameSetting.GameLocation.TraditionalChinese : GameSetting.GameLocation.SimplifiedChinese;
            help.updateSettingText(); Canvas.ForceUpdateCanvases();
            var label = help.transform.Find( "dialogue/text" ).GetComponent<Text>();
            check( label.text == ( traditional ? "對話速度" : "对话速度" ) && label.preferredWidth <= label.rectTransform.rect.width,
                "dialogue label fits " + traditional );
            capture( traditional ? "settings-traditional" : "settings-simplified" );
            yield return new WaitForSecondsRealtime( 0.2f );
        }
        var value = help.transform.Find( "dialogueValue" );
        var pointer = Pointer( value ); var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll( pointer, hits );
        check( hits.Count > 0 && ExecuteEvents.GetEventHandler<IPointerClickHandler>( hits[0].gameObject ) == value.gameObject, "dialogue value receives mouse clicks" );
        ExecuteEvents.Execute( value.gameObject, pointer, ExecuteEvents.pointerClickHandler );
        check( GameSetting.instance.fastDialogue && value.GetComponent<Text>().text == "快速", "mouse click switches to fast" );
        GameSetting.instance.load(); check( GameSetting.instance.fastDialogue, "fast dialogue preference reloads" );
        help.select( GameHelpUI.DialogueSelection );
        var confirm = GameTouchRightUI.instance.transform.Find( "ButtonA" );
        var confirmPointer = Pointer( confirm );
        ExecuteEvents.Execute( confirm.gameObject, confirmPointer, ExecuteEvents.pointerDownHandler );
        for ( int i = 0; i < 5; i++ ) yield return null;
        ExecuteEvents.Execute( confirm.gameObject, confirmPointer, ExecuteEvents.pointerUpHandler );
        check( !GameSetting.instance.fastDialogue && help.Selection == GameHelpUI.DialogueSelection, "virtual Ent uses selected dialogue toggle" );
        help.activateSelection(); check( GameSetting.instance.fastDialogue, "shared confirm route toggles dialogue" );
        GameSetting.instance.virtualKeyboardEnabled = false; help.updateSettingText();
        help.select( GameHelpUI.ShrinkSelection ); check( help.Selection == GameHelpUI.CloseSelection, "navigation skips disabled size controls after new row" );
        help.unShow(); GameTouchCenterUI.instance.showUI();

        GameUserData.instance.init(); GameUserData.instance.setStage( 1 );
        yield return load( 1 );
        GameSceneManager.instance.SceneType = GameSceneType.Rpg;
        GameRPGInput.instance.enabled = false; GameBlackUI.instance.unShow();
        float normal = 0, fast = 0, ordinary = 0;
        yield return Typing( true, false, t => normal = t );
        check( Field<float>( GameMsgBoxUI.instance, "playbackRate" ) == 1f && Field<float>( ActiveText(), "playbackRate" ) == 1f, "normal story uses original timing" );
        yield return Typing( true, true, t => fast = t );
        check( Field<float>( GameMsgBoxUI.instance, "playbackRate" ) == 1.5f && Field<float>( ActiveText(), "playbackRate" ) == 1.5f, "fast story explicitly uses 1.5 rate" );
        check( normal / fast > 1.25f && normal / fast < 1.9f, "measured story typing ratio " + ( normal / fast ).ToString( "F2" ) );
        yield return Typing( false, true, t => ordinary = t );
        check( Field<float>( GameMsgBoxUI.instance, "playbackRate" ) == 1f && Field<float>( ActiveText(), "playbackRate" ) == 1f,
            "ordinary and experience message path stays at normal rate" );
        check( ordinary / normal > 0.8f && ordinary / normal < 1.2f, "ordinary typing unchanged with fast setting" );
        foreach ( bool fastMode in new[] { false, true } )
        {
            GameSetting.instance.fastDialogue = fastMode;
            bool done = false;
            GameMsgBoxUI.instance.showDialogue( -1, 0, 0, "甲", () => done = true );
            ActiveText().onClick();
            while ( !ActiveText().IsOver ) yield return null;
            float start = Time.realtimeSinceStartup;
            while ( !done ) yield return null;
            float duration = Time.realtimeSinceStartup - start;
            check( Mathf.Abs( duration - ( fastMode ? 5f / 1.5f : 5f ) ) < 0.2f, "measured auto advance " + fastMode + " duration=" + duration.ToString( "F2" ) );
        }
        GameSetting.instance.fastDialogue = true;
        GameMsgBoxUI.instance.showDialogue( -1, 0, 0, "一\n二\n三\n四\n五", null );
        while ( !ActiveText().IsStopLine ) yield return null;
        GameMsgBoxUI.instance.onClick();
        check( !ActiveText().IsStopLine && Field<float>( ActiveText(), "playbackRate" ) == 1.5f, "manual page continuation retains fast dialogue rate" );
        GameMsgBoxUI.instance.showText( -1, 0, 0, "普通提示", null );
        check( !ActiveText().IsStopLine && Field<float>( ActiveText(), "playbackRate" ) == 1f, "new ordinary message clears stale page state and rate" );
        GameMsgBoxUI.instance.unShow();

        yield return load( 3 );
        GameSceneManager.instance.SceneType = GameSceneType.Battle;
        GameBattleInput.instance.enabled = false; GameBlackUI.instance.unShow();
        var toast = GameBattleGetItemUI.instance;
        foreach ( bool fastMode in new[] { false, true } )
        {
            GameSetting.instance.fastDialogue = fastMode;
            int callbacks = 0; float start = Time.realtimeSinceStartup;
            toast.show( -1, 100, () => callbacks++ );
            while ( !toast.IsFadeOut ) yield return null;
            float visible = Time.realtimeSinceStartup - start;
            check( Mathf.Abs( visible - 2f ) < 0.15f, "reward toast holds two seconds independent of dialogue " + fastMode );
            var fade = toast.GetComponent<GameUIFade>();
            check( Mathf.Approximately( fade.FadeOutTime, 0.3f ), "reward toast retains authored 0.3 second fade" );
            while ( toast.gameObject.activeSelf ) yield return null;
            check( callbacks == 1 && Mathf.Abs( Time.realtimeSinceStartup - start - 2.3f ) < 0.2f, "toast fade finishes once at 2.3 seconds " + fastMode );
        }
        int manualCallbacks = 0;
        toast.show( -1, 100, () => manualCallbacks++ );
        toast.unShowFade();
        while ( toast.gameObject.activeSelf ) yield return null;
        check( manualCallbacks == 1, "manual toast dismissal still completes once" );
    }
}
#endif

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
public sealed class SwordsmanDialogueChecks : MonoBehaviour
{
    [Serializable] sealed class Report { public List<string> passed = new List<string>(); public string error = ""; }
    readonly Report report = new Report();
    string output;
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Install()
    {
        var args = Environment.GetCommandLineArgs(); int i = Array.IndexOf( args, "-swordsman-dialogue-report" );
        if ( i < 0 || i + 1 >= args.Length ) return;
        var go = new GameObject( "DialogueSpeedChecks" ); DontDestroyOnLoad( go );
        go.AddComponent<SwordsmanDialogueChecks>().output = args[i + 1];
    }
    IEnumerator Load( int scene )
    {
        GameSceneManager.instance.loadScene( (GameSceneType)scene, GameSceneLoadMode.Count );
        while ( GameSceneManager.instance.IsLoading || SceneManager.GetActiveScene().buildIndex != scene ) yield return null;
        yield return null;
    }
    IEnumerator Start()
    {
        var prefs = new SwordsmanDialogueVerification.Preferences();
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        var stack = new Stack<IEnumerator>();
        stack.Push( SwordsmanDialogueVerification.Run( (ok, name) => { if ( !ok ) throw new Exception( name ); report.passed.Add( name ); },
            name => ScreenCapture.CaptureScreenshot( Path.Combine( Path.GetDirectoryName( output ), name + ".png" ) ), Load ) );
        float deadline = Time.realtimeSinceStartup + 120;
        while ( stack.Count > 0 )
        {
            object current = null;
            try
            {
                if ( Time.realtimeSinceStartup > deadline ) throw new Exception( "Dialogue test timeout" );
                if ( !stack.Peek().MoveNext() ) { stack.Pop(); continue; }
                current = stack.Peek().Current;
                if ( current is IEnumerator ) { stack.Push( (IEnumerator)current ); continue; }
            }
            catch ( Exception e ) { report.error = e.ToString(); break; }
            yield return current;
        }
        prefs.Restore(); File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        Application.Quit( report.error.Length == 0 ? 0 : 1 );
    }
}
#endif
