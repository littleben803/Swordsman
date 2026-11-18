using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
// Explicit opt-in only. Exercises real UI pointer handlers; never writes a game save.
public sealed class SwordsmanVirtualKeyboardChecks : MonoBehaviour
{
    [Serializable] public sealed class Report
    {
        public List<string> passed = new List<string>();
        public List<string> errors = new List<string>();
    }
    readonly Report report = new Report();
    string output;
    string captureDirectory;
    bool openHomepage;
    readonly Dictionary<string, object> preferences = new Dictionary<string, object>();
    readonly string[] keys = { "virtualKeyboardEnabled", "touchScale", "fastDialogue", "mode", "location", "enableMusic", "enableSound", "quality" };

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Install()
    {
        var args = Environment.GetCommandLineArgs();
        int index = Array.IndexOf( args, "-swordsman-virtual-keyboard-report" );
        if ( index < 0 || index + 1 >= args.Length ) return;
        var go = new GameObject( "VirtualKeyboardChecks" ); DontDestroyOnLoad( go );
        var checks = go.AddComponent<SwordsmanVirtualKeyboardChecks>();
        checks.output = args[index + 1];
        int capture = Array.IndexOf( args, "-swordsman-virtual-keyboard-capture" );
        if ( capture >= 0 && capture + 1 < args.Length ) checks.captureDirectory = args[capture + 1];
        checks.openHomepage = Array.IndexOf( args, "-swordsman-virtual-keyboard-open-homepage" ) >= 0;
    }
    void Check( bool value, string name )
    {
        ( value ? report.passed : report.errors ).Add( name );
    }
    void Capture( string name )
    {
        if ( string.IsNullOrEmpty( captureDirectory ) ) return;
        Directory.CreateDirectory( captureDirectory );
        ScreenCapture.CaptureScreenshot( Path.Combine( captureDirectory, name + ".png" ) );
    }
    static PointerEventData Pointer( RectTransform rect, Vector2 direction )
    {
        Vector3[] corners = new Vector3[4]; rect.GetWorldCorners( corners );
        Vector2 a = RectTransformUtility.WorldToScreenPoint( null, corners[0] );
        Vector2 b = RectTransformUtility.WorldToScreenPoint( null, corners[2] );
        return new PointerEventData( EventSystem.current ) { pointerId = -1, position = ( a + b ) * 0.5f + Vector2.Scale( b - a, direction ) * 0.35f };
    }
    IEnumerator Direction( ETCDPad pad, Vector2 direction, int delta )
    {
        GameHelpUI.instance.select( GameHelpUI.LanguageSelection ); GameTouchManager.instance.ResetControls();
        var pointer = Pointer( pad.GetComponent<RectTransform>(), direction );
        ExecuteEvents.Execute( pad.gameObject, pointer, ExecuteEvents.pointerDownHandler );
        for ( int i = 0; i < 8; i++ ) yield return null;
        int expected = GameHelpUI.LanguageSelection + delta;
        Check( GameHelpUI.instance.Selection == expected, "pointer direction " + direction );
        ExecuteEvents.Execute( pad.gameObject, pointer, ExecuteEvents.pointerUpHandler );
        for ( int i = 0; i < 4; i++ ) yield return null;
        Check( GameHelpUI.instance.Selection == expected && !GameTouchManager.instance.getKey( GameInputCode.Right ) &&
            !GameTouchManager.instance.getKey( GameInputCode.Left ), "release " + direction );
    }
    IEnumerator Start()
    {
        float deadline = Time.realtimeSinceStartup + 35;
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            if ( Time.realtimeSinceStartup > deadline ) { Check( false, "title timeout" ); Finish(); yield break; }
            yield return null;
        }
        foreach ( string key in keys )
            preferences[key] = !PlayerPrefs.HasKey( key ) ? null : key == "touchScale" ? (object)PlayerPrefs.GetFloat( key ) : PlayerPrefs.GetInt( key );
        Check( !GameSetting.VirtualKeyboardDefault, "Mac defaults off" );
        Check( EventSystem.current.currentInputModule is StandaloneInputModule, "legacy pointer module active" );
        var mode = GameSetting.instance.mode;
        GameHelpUI.instance.show();
        if ( !GameSetting.instance.virtualKeyboardEnabled ) GameHelpUI.instance.onKeyboardToggle();
        yield return new WaitForSecondsRealtime( 0.6f );
        Check( GameTouchLeftUI.instance.IsShow && GameTouchRightUI.instance.IsShow && GameTouchManager.instance.IsShow, "enabled controls visible" );
        var studioButton = GameHelpUI.instance.transform.Find( "buttonB" ).GetComponent<Button>();
        var studioText = studioButton.transform.Find( "text" ).GetComponent<Text>();
        Check( studioButton.onClick.GetPersistentEventCount() == 1 && studioButton.onClick.GetPersistentMethodName( 0 ) == "onClickB" &&
            studioButton.onClick.GetPersistentTarget( 0 ) == GameHelpUI.instance, "packaged studio callback binding" );
        Check( GameHelpUI.StudioHomepageUrl == "https://www.ideaboxapps.com/", "packaged official homepage" );
        var language = GameSetting.instance.location;
        foreach ( bool traditional in new[] { false, true } )
        {
            GameSetting.instance.location = traditional ? GameSetting.GameLocation.TraditionalChinese : GameSetting.GameLocation.SimplifiedChinese;
            GameHelpUI.instance.updateSettingText();
            for ( int i = 0; i < 4; i++ ) yield return null;
            Canvas.ForceUpdateCanvases();
            Check( studioText.text == GameHelpUI.StudioInformation( traditional ) && studioText.text.Split( '\n' ).Length == 3,
                "packaged exact studio lines " + traditional );
            Check( !studioText.text.Contains( "（" ) && !studioText.text.Contains( "fox526594" ) && !studioText.text.Contains( "B站" ),
                "studio excludes instruction and legacy text " + traditional );
            Check( studioText.preferredWidth <= studioText.rectTransform.rect.width && studioText.preferredHeight <= studioText.rectTransform.rect.height &&
                studioText.cachedTextGenerator.lineCount == 3, "packaged studio text fits " + traditional );
            for ( int line = 0; line < 3; line++ )
            {
                Vector3 world = studioText.rectTransform.TransformPoint( new Vector3( 0, 16 - line * 16, 0 ) );
                Check( RectTransformUtility.RectangleContainsScreenPoint( studioButton.GetComponent<RectTransform>(),
                    RectTransformUtility.WorldToScreenPoint( null, world ) ) == ( line == 0 ),
                    "homepage line hit region " + line + " traditional " + traditional );
            }
            Capture( traditional ? "studio-traditional" : "studio-simplified" );
            yield return new WaitForSecondsRealtime( 0.2f );
        }
        GameSetting.instance.location = language; GameHelpUI.instance.updateSettingText();
        var pad = GameTouchLeftUI.instance.GetComponentInChildren<ETCDPad>();
        foreach ( string name in new[] { "DPad", "A", "B" } )
        {
            var skin = Resources.Load<Sprite>( "VirtualKeyboard/Bronze/" + name );
            Check( skin != null && skin.texture.format == TextureFormat.RGBA32, "packaged RGBA skin " + name );
        }
        var skinA = GameTouchRightUI.instance.transform.Find( "ButtonA" ).GetComponent<ETCButton>();
        var skinB = GameTouchRightUI.instance.transform.Find( "ButtonB" ).GetComponent<ETCButton>();
        Check( skinA.GetComponentInChildren<Text>().text == "Ent" && skinB.GetComponentInChildren<Text>().text == "Esc" &&
            !skinA.GetComponent<Image>().enabled && !skinB.GetComponent<Image>().enabled, "packaged Ent/Esc labels without baked controller letters" );
        Check( GameTouchRightUI.instance.transform.Find( "ButtonC" ).GetComponentInChildren<GameVirtualKeyboardPageFace>( true ).PageDirection == 1 &&
            GameTouchRightUI.instance.transform.Find( "ButtonD" ).GetComponentInChildren<GameVirtualKeyboardPageFace>( true ).PageDirection == -1,
            "packaged selected bar-and-arrow paging symbols" );
        Check( pad != null && pad.GetComponent<Image>().sprite == Resources.Load<Sprite>( "VirtualKeyboard/Bronze/DPad" ) &&
            pad.GetComponent<RectTransform>().sizeDelta == new Vector2( 230, 230 ) && pad.buttonSizeCoef == 3,
            "bronze DPad applied with original hit geometry" );
        Check( pad != null && Vector2.Distance( pad.GetComponent<RectTransform>().anchoredPosition,
            new Vector2( 101.44f, 99.39f ) ) < 0.01f, "DPad position matches mirrored corner padding" );
        Check( skinA.normalSprite == Resources.Load<Sprite>( "VirtualKeyboard/Bronze/A" ) &&
            skinB.normalSprite == Resources.Load<Sprite>( "VirtualKeyboard/Bronze/B" ) &&
            skinA.pressedSprite == skinA.normalSprite && skinB.pressedSprite == skinB.normalSprite,
            "A/B normal and pressed skins applied" );
        Check( skinA.GetComponent<RectTransform>().anchoredPosition == new Vector2( -111, 45 ) &&
            skinB.GetComponent<RectTransform>().anchoredPosition == new Vector2( -45, 111 ) &&
            skinA.GetComponent<RectTransform>().sizeDelta == new Vector2( 80, 80 ) &&
            skinB.GetComponent<RectTransform>().sizeDelta == new Vector2( 80, 80 ),
            "compact A lower-left B upper-right with approved 80x80 targets" );
        Capture( "keyboard-on" );
        if ( pad == null ) Check( false, "direction pad missing" );
        else
        {
            yield return Direction( pad, Vector2.right, 1 );
            yield return Direction( pad, Vector2.left, -1 );
            yield return Direction( pad, Vector2.up, -1 );
            yield return Direction( pad, Vector2.down, 1 );
        }
        for ( int i = 0; i < 12; i++ ) GameHelpUI.instance.onSetting21();
        Check( Mathf.Approximately( GameSetting.instance.touchScale, 1.5f ) &&
            Mathf.Approximately( GameTouchLeftUI.instance.transform.localScale.x, 1.5f ), "maximum scale 150%" );
        yield return new WaitForSecondsRealtime( 0.2f );
        Capture( "keyboard-150" );
        yield return new WaitForSecondsRealtime( 0.2f );
        for ( int i = 0; i < 12; i++ ) GameHelpUI.instance.onSetting20();
        Check( Mathf.Approximately( GameSetting.instance.touchScale, 1 ) &&
            Mathf.Approximately( GameTouchRightUI.instance.transform.localScale.x, 1 ), "minimum scale 100%" );
        GameSetting.instance.load(); Check( GameSetting.instance.virtualKeyboardEnabled, "enabled preference reload" );
        GameHelpUI.instance.select( GameHelpUI.KeyboardSelection );
        Check( GameHelpUI.instance.transform.Find( "keyboardOff" ) == null &&
            GameHelpUI.instance.transform.Find( "keyboardOn" ) == null, "single keyboard value only" );
        var a = GameTouchRightUI.instance.transform.Find( "ButtonA" ).gameObject;
        var pointerA = Pointer( a.GetComponent<RectTransform>(), Vector2.zero );
        ExecuteEvents.Execute( a, pointerA, ExecuteEvents.pointerDownHandler );
        for ( int i = 0; i < 5; i++ ) yield return null;
        ExecuteEvents.Execute( a, pointerA, ExecuteEvents.pointerUpHandler );
        Check( !GameSetting.instance.virtualKeyboardEnabled && !GameTouchManager.instance.IsShow &&
            GameHelpUI.instance.Selection == GameHelpUI.KeyboardSelection, "virtual A toggles selected keyboard row off" );
        var value = GameHelpUI.instance.transform.Find( "keyboardValue" ).GetComponent<Text>();
        Check( value.text == "关闭" || value.text == "關閉", "single value displays off" );
        var shrink = GameHelpUI.instance.transform.Find( "setting20" ).GetComponent<Button>();
        var grow = GameHelpUI.instance.transform.Find( "setting21" ).GetComponent<Button>();
        float savedScale = GameSetting.instance.touchScale;
        Check( !shrink.interactable && !grow.interactable, "resize buttons disabled when keyboard off" );
        yield return new WaitForSecondsRealtime( 0.6f );
        Capture( "keyboard-off" );
        yield return new WaitForSecondsRealtime( 0.2f );
        GameHelpUI.instance.onSetting20(); GameHelpUI.instance.onSetting21();
        Check( Mathf.Approximately( GameSetting.instance.touchScale, savedScale ), "disabled resize callbacks preserve scale" );
        GameHelpUI.instance.select( GameHelpUI.ShrinkSelection );
        Check( GameHelpUI.instance.Selection == GameHelpUI.CloseSelection, "forward navigation skips disabled size row" );
        GameHelpUI.instance.select( GameHelpUI.GrowSelection );
        Check( GameHelpUI.instance.Selection == GameHelpUI.KeyboardSelection, "backward navigation skips disabled size row" );
        GameHelpUI.instance.activateSelection();
        yield return new WaitForSecondsRealtime( 0.6f );
        Check( GameSetting.instance.virtualKeyboardEnabled && GameTouchManager.instance.IsShow &&
            ( value.text == "打开" || value.text == "打開" ), "confirm action toggles selected keyboard row on" );
        Check( shrink.interactable && grow.interactable && Mathf.Approximately( GameSetting.instance.touchScale, savedScale ),
            "reopening restores resize buttons and preserves scale" );
        GameHelpUI.instance.select( GameHelpUI.CloseSelection );
        ExecuteEvents.Execute( a, pointerA, ExecuteEvents.pointerDownHandler );
        for ( int i = 0; i < 5; i++ ) yield return null;
        ExecuteEvents.Execute( a, pointerA, ExecuteEvents.pointerUpHandler );
        Check( !GameHelpUI.instance.IsShow, "A confirms selected close action" );
        GameHelpUI.instance.show(); GameHelpUI.instance.select( 0 );
        var b = GameTouchRightUI.instance.transform.Find( "ButtonB" ).gameObject;
        var pointerB = Pointer( b.GetComponent<RectTransform>(), Vector2.zero );
        ExecuteEvents.Execute( b, pointerB, ExecuteEvents.pointerDownHandler );
        for ( int i = 0; i < 5; i++ ) yield return null;
        ExecuteEvents.Execute( b, pointerB, ExecuteEvents.pointerUpHandler );
        Check( !GameHelpUI.instance.IsShow, "B cancels help" );
        GameHelpUI.instance.show(); GameTouchManager.instance.OnPressRight();
        if ( GameSetting.instance.virtualKeyboardEnabled ) GameHelpUI.instance.onKeyboardToggle();
        GameSetting.instance.load();
        Check( !GameSetting.instance.virtualKeyboardEnabled && !GameTouchManager.instance.IsShow &&
            !GameTouchLeftUI.instance.IsShow && !GameTouchRightUI.instance.IsShow &&
            !GameTouchManager.instance.getKey( GameInputCode.Right ), "disabled controls hidden, released and persisted" );
        Check( GameSetting.instance.mode == mode, "keyboard operations preserve difficulty" );
        // Restore every touched preference, including originally absent keys.
        foreach ( var entry in preferences )
        {
            if ( entry.Value == null ) PlayerPrefs.DeleteKey( entry.Key );
            else if ( entry.Value is float ) PlayerPrefs.SetFloat( entry.Key, (float)entry.Value );
            else PlayerPrefs.SetInt( entry.Key, (int)entry.Value );
        }
        PlayerPrefs.Save(); GameSetting.instance.load();
        GameHelpUI.instance.unShow(); GameTouchManager.instance.ResetControls(); GameTouchCenterUI.instance.showUI();
        if ( openHomepage )
        {
            // Explicit test-only flag; dispatch the final Player's actual Button pointer-click handler once.
            Application.runInBackground = true;
            GameHelpUI.instance.show();
            Check( ExecuteEvents.Execute( studioButton.gameObject, Pointer( studioButton.GetComponent<RectTransform>(), Vector2.zero ),
                ExecuteEvents.pointerClickHandler ), "homepage pointer callback dispatched to system" );
            yield return new WaitForSecondsRealtime( 0.4f );
        }
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-paging-check" ) >= 0 )
            yield return VerifyPaging();
        Finish();
    }

    IEnumerator VerifyPaging()
    {
        bool enabled = GameSetting.instance.virtualKeyboardEnabled;
        float scale = GameSetting.instance.touchScale;
        GameSetting.instance.virtualKeyboardEnabled = true; GameSetting.instance.touchScale = 1;
        // In-memory fixture only: no desk checkpoint creation or game-save writes.
        GameUserData.instance.init(); GameUserData.instance.setStage( 1 );
        for ( short i = 0; i < 30; i++ ) GameUserData.instance.addItem( i );
        GameSceneManager.instance.loadScene( GameSceneType.Camp, GameSceneLoadMode.Count );
        while ( GameSceneManager.instance.IsLoading || GameCampInput.instance == null ) yield return null;
        GameCampInput.instance.enabled = false; GameBlackUI.instance.unShow();
        GameCampUI.instance.show( GameCampData.instance.getData( 1 ) ); GameTouchCenterUI.instance.showUI();
        var root = GameTouchRightUI.instance.transform;
        Check( !root.Find( "ButtonC" ).gameObject.activeSelf && !root.Find( "ButtonD" ).gameObject.activeSelf,
            "packaged camp menu hides X/Y" );
        GameCampSelectUI.instance.unShow(); GameItemUI.instance.show( null );
        yield return new WaitForSecondsRealtime( 0.7f );
        foreach ( string name in new[] { "ButtonC", "ButtonD" } )
        {
            var control = root.Find( name ).GetComponent<ETCButton>();
            var rect = control.GetComponent<RectTransform>();
            var face = control.GetComponentInChildren<GameVirtualKeyboardPageFace>();
            Check( control.gameObject.activeSelf && rect.sizeDelta == new Vector2( 80, 80 ) && face != null && !face.Source.enabled,
                "packaged " + name + " matching bronze target without baked letter" );
            var pointer = Pointer( rect, Vector2.zero ); var hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll( pointer, hits );
            Check( hits.Count > 0 && ExecuteEvents.GetEventHandler<IPointerDownHandler>( hits[0].gameObject ) == control.gameObject,
                "packaged " + name + " topmost pointer target" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerDownHandler );
            Check( GameInputManager.getKeyDown( name == "ButtonC" ? GameInputCode.Up1 : GameInputCode.Down1 ),
                "packaged " + name + " original page action" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerUpHandler );
        }
        Capture( "paging-inventory" ); yield return new WaitForSecondsRealtime( 0.3f );
        GameItemUI.instance.showAskUI( true ); GameTouchRightUI.instance.RefreshPageButtons();
        Check( !root.Find( "ButtonC" ).gameObject.activeSelf, "packaged inventory confirmation hides paging" );
        GameItemUI.instance.unShowAskUI(); GameTouchRightUI.instance.RefreshPageButtons();
        Check( root.Find( "ButtonC" ).gameObject.activeSelf, "packaged closing confirmation restores paging" );
        GameTouchManager.instance.OnDownDown1(); GameItemUI.instance.unShow(); GameTouchRightUI.instance.RefreshPageButtons();
        Check( !root.Find( "ButtonC" ).gameObject.activeSelf && !root.Find( "ButtonD" ).gameObject.activeSelf &&
            !GameTouchManager.instance.getKeyDown( GameInputCode.Down1 ), "packaged closing inventory releases and hides paging" );
        Check( ((RectTransform)root.Find( "ButtonA" )).anchoredPosition == new Vector2( -111, 45 ) &&
            ((RectTransform)root.Find( "ButtonB" )).anchoredPosition == new Vector2( -45, 111 ), "packaged non-list A/B layout restored" );
        GameSetting.instance.virtualKeyboardEnabled = enabled; GameSetting.instance.touchScale = scale;
    }
    void Finish()
    {
        Directory.CreateDirectory( Path.GetDirectoryName( output ) );
        File.WriteAllText( output, JsonUtility.ToJson( report, true ) );
        Debug.Log( "SWORDSMAN_VIRTUAL_KEYBOARD passed=" + report.passed.Count + " errors=" + report.errors.Count );
        Application.Quit( report.errors.Count == 0 ? 0 : 1 );
    }
}
#endif
