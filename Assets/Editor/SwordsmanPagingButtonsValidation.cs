using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Explicit Editor Play Mode check; no Player build, scene save or preference/save-file writes.
public static class SwordsmanPagingButtonsValidation
{
    const string Key = "Swordsman.PagingButtons.Check";
    static string Output
    {
        get
        {
            var args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf( args, "-swordsman-paging-output" );
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : "Logs/virtual-keyboard-xy-20261115";
        }
    }
    static IEnumerator routine;
    static double deadline;
    static bool oldEnabled;
    static float oldScale;
    static bool captured;
    [Serializable] sealed class Report { public List<string> passed = new List<string>(); public string error = ""; }
    static Report report;
    static void Configure( bool value )
    {
        typeof(SwordsmanVirtualKeyboardSkinValidation).GetMethod( "ConfigureGameView", BindingFlags.Static | BindingFlags.NonPublic ).Invoke( null, new object[] { value } );
    }
    public static void Run()
    {
        Directory.CreateDirectory( Output );
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        Configure( true ); SessionState.SetBool( Key, true ); EditorApplication.EnterPlaymode();
    }
    [InitializeOnLoadMethod] static void Resume()
    {
        if ( !SessionState.GetBool( Key, false ) ) return;
        report = new Report(); routine = Verify(); deadline = EditorApplication.timeSinceStartup + 150;
        EditorApplication.update += Tick;
    }
    static void Tick()
    {
        try
        {
            if ( EditorApplication.timeSinceStartup > deadline ) throw new Exception( "Paging check timeout" );
            if ( EditorApplication.isPlaying && !routine.MoveNext() ) Finish( "" );
        }
        catch ( Exception e ) { Finish( e.ToString() ); }
    }
    static void Check( bool value, string name )
    {
        if ( !value ) throw new Exception( name ); report.passed.Add( name );
    }
    static void Flag( object target, bool value )
    {
        if ( target == null ) throw new Exception( "Missing fixture panel" );
        for ( Type type = target.GetType(); type != null; type = type.BaseType )
        {
            var field = type.GetField( "isShow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.DeclaredOnly );
            if ( field != null ) { field.SetValue( target, value ); return; }
        }
        throw new Exception( "Missing isShow fixture field" );
    }
    static void Visible( bool value, string name )
    {
        var ui = GameTouchRightUI.instance; ui.RefreshPageButtons();
        Check( ui.transform.Find( "ButtonC" ).gameObject.activeSelf == value &&
            ui.transform.Find( "ButtonD" ).gameObject.activeSelf == value, name );
    }
    static void PressPageDown()
    {
        var control = GameTouchRightUI.instance.transform.Find( "ButtonD" ).gameObject;
        var pointer = new PointerEventData( EventSystem.current ) { pointerId = -1 };
        ExecuteEvents.Execute( control, pointer, ExecuteEvents.pointerDownHandler );
        ExecuteEvents.Execute( control, pointer, ExecuteEvents.pointerUpHandler );
    }
    static void VerifyTuning()
    {
        var oldMode = GameSetting.instance.mode;
        var objects = new List<GameObject>();
        try
        {
            var units = new List<GameBattleUnit>();
            for ( int i = 0; i < 2; i++ )
            {
                var go = new GameObject( "IsolatedDifficultyFixture" ); objects.Add( go );
                var unit = go.AddComponent<GameBattleUnit>(); unit.enabled = false;
                var data = new GameUnit { UnitID = (short)( i == 0 ? 0 : 10 ), UnitCampType = i == 0 ? GameUnitCampType.User : GameUnitCampType.Enemy };
                var man = new GameBattleMan { BattleAIType = GameBattleAIType.None, UnitBase = new GameUnitBase { UnitID = data.UnitID, LV = 1 } };
                typeof(GameBattleUnit).GetField( "gameUnit", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( unit, data );
                typeof(GameBattleUnit).GetField( "gameBattleMan", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( unit, man );
                units.Add( unit );
            }
            foreach ( var mode in new[] { GameSetting.GameMode.Normal, GameSetting.GameMode.Hard, GameSetting.GameMode.Easy } )
            {
                GameSetting.instance.mode = mode;
                Check( GameEasyRules.Damage( units[1], units[0], 100 ) == ( mode == GameSetting.GameMode.Easy ? 90 : 100 ), "current incoming damage " + mode );
                Check( GameEasyRules.Damage( units[0], units[1], 100 ) == ( mode == GameSetting.GameMode.Easy ? 110 : 100 ), "unchanged outgoing damage " + mode );
                typeof(GameBattleUnit).GetField( "tempExp", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( units[0], (short)0 );
                units[0].getUnitBase().Exp = 0;
                units[0].addExpSkill( units[0] );
                Check( units[0].TempExp == ( mode == GameSetting.GameMode.Easy ? 55 : 50 ), "current experience 110 percent only in Easy " + mode );
                units[0].addExp();
                Check( units[0].getUnitBase().Exp == ( mode == GameSetting.GameMode.Easy ? 55 : 50 ), "experience payout matches UI " + mode );
            }
        }
        finally
        {
            GameSetting.instance.mode = oldMode;
            foreach ( var go in objects ) UnityEngine.Object.DestroyImmediate( go );
        }
    }
    static IEnumerator Verify()
    {
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            yield return null;
        }
        oldEnabled = GameSetting.instance.virtualKeyboardEnabled; oldScale = GameSetting.instance.touchScale; captured = true;
        VerifyTuning();
        GameSetting.instance.virtualKeyboardEnabled = true; GameSetting.instance.touchScale = 1;
        Visible( false, "title hides paging" );
        GameUserData.instance.init(); GameUserData.instance.setStage( 1 );
        for ( short i = 0; i < 30; i++ ) GameUserData.instance.addItem( i );
        EditorSceneManager.LoadSceneInPlayMode( "Assets/Scenes/level2.unity", new LoadSceneParameters( LoadSceneMode.Single ) );
        yield return null; yield return null;
        GameSceneManager.instance.SceneType = GameSceneType.Camp;
        GameCampInput.instance.enabled = false;
        GameBlackUI.instance.unShow();
        GameTouchCenterUI.instance.showUI();
        GameCampUI.instance.show( GameCampData.instance.getData( 1 ) );
        Visible( false, "camp main menu hides paging" );
        GameCampSelectUI.instance.unShow();
        GameItemUI.instance.show( null );
        Visible( true, "camp inventory shows paging" );
        double settle = EditorApplication.timeSinceStartup + 0.6;
        while ( EditorApplication.timeSinceStartup < settle ) yield return null;
        var root = GameTouchRightUI.instance.transform;
        foreach ( string name in new[] { "ButtonA", "ButtonB" } )
        {
            var control = root.Find( name ).GetComponent<ETCButton>();
            var label = control.GetComponentInChildren<Text>();
            Check( label != null && label.text == ( name == "ButtonA" ? "Ent" : "Esc" ) && label.preferredWidth < 60 &&
                !control.GetComponent<Image>().enabled && control.GetComponentInChildren<GameVirtualKeyboardPageFace>() != null,
                name + " keyboard label fits and baked letter hidden" );
            Canvas.ForceUpdateCanvases();
            var pointer = new PointerEventData( EventSystem.current ) { pointerId = -1, position = RectTransformUtility.WorldToScreenPoint( null, control.transform.position ) };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll( pointer, hits );
            Check( hits.Count > 0 && ExecuteEvents.GetEventHandler<IPointerDownHandler>( hits[0].gameObject ) == control.gameObject, name + " labeled face receives pointer" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerDownHandler );
            Check( GameInputManager.getKeyDown( name == "ButtonA" ? GameInputCode.Confirm : GameInputCode.Cancel ), name + " original confirm/cancel callback" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerUpHandler );
        }
        foreach ( string name in new[] { "ButtonA", "ButtonB", "ButtonC", "ButtonD" } )
            Check( ((RectTransform)root.Find( name )).sizeDelta == new Vector2( 80, 80 ), name + " identical 80x80 target" );
        for ( int i = 0; i < 4; i++ ) for ( int j = i + 1; j < 4; j++ )
        {
            var a = (RectTransform)root.Find( "Button" + "ABCD"[i] ); var b = (RectTransform)root.Find( "Button" + "ABCD"[j] );
            Check( !new Rect( a.anchoredPosition - a.sizeDelta / 2, a.sizeDelta ).Overlaps( new Rect( b.anchoredPosition - b.sizeDelta / 2, b.sizeDelta ) ),
                "paging targets do not overlap " + i + j );
        }
        foreach ( string name in new[] { "ButtonC", "ButtonD" } )
        {
            var control = root.Find( name ).GetComponent<ETCButton>();
            var face = control.GetComponentInChildren<GameVirtualKeyboardPageFace>();
            Check( face != null && !face.Source.enabled && face.mainTexture == root.Find( "ButtonA" ).GetComponent<Image>().sprite.texture && face.raycastTarget,
                name + " uses original bronze RGBA material and pointer graphic" );
            Check( face.PageDirection == ( name == "ButtonC" ? 1 : -1 ) && control.GetComponentInChildren<Text>() == null,
                name + " selected page arrow without legacy letter" );
            Canvas.ForceUpdateCanvases();
            var pointer = new PointerEventData( EventSystem.current ) { pointerId = -1,
                position = RectTransformUtility.WorldToScreenPoint( null, control.transform.position ) };
            var hits = new List<RaycastResult>(); EventSystem.current.RaycastAll( pointer, hits );
            Check( hits.Count > 0 && ExecuteEvents.GetEventHandler<IPointerDownHandler>( hits[0].gameObject ) == control.gameObject,
                name + " receives actual topmost UI raycast: " + string.Join( ",", hits.ConvertAll( hit => hit.gameObject.name ).ToArray() ) + " at " + pointer.position );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerDownHandler );
            Check( GameInputManager.getKeyDown( name == "ButtonC" ? GameInputCode.Up1 : GameInputCode.Down1 ), name + " retains paging callback" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerUpHandler );
        }
        var bag = GameItemUI.instance.transform.Find( "bag0" ).GetComponent<GameItemBagUI>();
        GameItemUI.instance.select( 0 ); PressPageDown();
        typeof(GameCampInput).GetField( "time", BindingFlags.Instance | BindingFlags.NonPublic ).SetValue( GameCampInput.instance, 1f );
        typeof(GameCampInput).GetMethod( "moveItemUI", BindingFlags.Instance | BindingFlags.NonPublic ).Invoke( GameCampInput.instance, null );
        Check( bag.Selection == GameItemBagUI.MAX_SLOT - 1, "actual inventory advances original page amount" );
        GameItemUI.instance.showAskUI( true ); Visible( false, "inventory confirmation hides paging" );
        GameItemUI.instance.unShowAskUI(); Visible( true, "closing confirmation restores paging" );
        PressPageDown(); GameTouchRightUI.instance.transform.Find( "ButtonC" ).GetComponent<ETCButton>().onPressed.Invoke();
        GameItemUI.instance.unShow(); Visible( false, "closing inventory hides paging" );
        Check( !GameInputManager.getKeyDown( GameInputCode.Down1 ) && !GameInputManager.getKey( GameInputCode.Up1 ), "hiding releases only pending page input" );
        GameAlchemyUI.instance.show( null ); Visible( true, "alchemy list shows paging" );
        Flag( GameBlackUI.instance, true ); Visible( false, "transition overlay hides paging" ); Flag( GameBlackUI.instance, false );
        GameSetting.instance.virtualKeyboardEnabled = false; Visible( false, "disabled virtual keyboard hides paging" );
        GameSetting.instance.virtualKeyboardEnabled = true;
        GameSceneManager.instance.SceneType = GameSceneType.Battle; Visible( false, "battle hides paging" );
        GameSceneManager.instance.SceneType = GameSceneType.Camp; GameAlchemyUI.instance.unShow();
        GameItemUI.instance.show( null ); GameTouchRightUI.instance.RefreshPageButtons();
        foreach ( float scale in new[] { 1f, 1.5f } )
        {
            root.localScale = Vector3.one * scale; Canvas.ForceUpdateCanvases();
            foreach ( string name in new[] { "ButtonA", "ButtonB", "ButtonC", "ButtonD" } )
            {
                Vector3[] corners = new Vector3[4]; ((RectTransform)root.Find( name )).GetWorldCorners( corners );
                Vector2 low = RectTransformUtility.WorldToScreenPoint( null, corners[0] );
                Vector2 high = RectTransformUtility.WorldToScreenPoint( null, corners[2] );
                Check( low.x >= 0 && low.y >= 0 && high.x <= Screen.width && high.y <= Screen.height,
                    name + " within screen at " + scale );
            }
        }
        root.localScale = Vector3.one;
        // Do not save the scene or any game data. Capture the actual Editor-rendered four-button cluster.
        double settled = EditorApplication.timeSinceStartup + 0.5;
        while ( EditorApplication.timeSinceStartup < settled ) yield return null;
        ScreenCapture.CaptureScreenshot( Path.GetFullPath( Output + "/four-buttons.png" ) );
        double until = EditorApplication.timeSinceStartup + 0.5;
        while ( EditorApplication.timeSinceStartup < until ) yield return null;
        GameItemUI.instance.unShow(); Visible( false, "two-button layout restored" );
        Check( ((RectTransform)root.Find( "ButtonA" )).anchoredPosition == new Vector2( -111, 45 ) &&
            ((RectTransform)root.Find( "ButtonB" )).anchoredPosition == new Vector2( -45, 111 ), "original A/B positions restored outside lists" );
        EditorSceneManager.LoadSceneInPlayMode( "Assets/Scenes/level1.unity", new LoadSceneParameters( LoadSceneMode.Single ) );
        while ( SceneManager.GetActiveScene().path != "Assets/Scenes/level1.unity" || GameRPGInput.instance == null ) yield return null;
        yield return null;
        GameSceneManager.instance.SceneType = GameSceneType.Rpg;
        GameRPGInput.instance.enabled = false; GameBlackUI.instance.unShow();
        Visible( false, "RPG exploration hides paging" );
        GameItemUI.instance.show( null ); Visible( true, "RPG inventory shows paging" );
        GameItemUI.instance.unShow();
        Flag( GameShopUI.instance, true ); Visible( false, "shop intro without lists hides paging" );
        GameShopUI.instance.enabled = false; GameShopUI.instance.gameObject.SetActive( true );
        var readyBag = (GameItemBagUI)typeof(GameShopUI).GetField( "bagUI", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( GameShopUI.instance );
        readyBag.enabled = false; readyBag.gameObject.SetActive( true );
        Visible( true, "RPG shop list state shows paging" );
        var selling = (GameItemBagUI)typeof(GameShopUI).GetField( "shopUI", BindingFlags.Instance | BindingFlags.NonPublic ).GetValue( GameShopUI.instance );
        var shopBag = selling.Enabled ? selling : readyBag;
        var askField = typeof(GameItemBagUI).GetField( "askUI", BindingFlags.Instance | BindingFlags.NonPublic );
        var ask = askField.GetValue( shopBag );
        if ( ask == null )
        {
            // This state-only shop fixture skips its opening animation; bind its inactive authored ask panel explicitly.
            ask = shopBag.GetComponentInChildren<GameAskUI>( true ); askField.SetValue( shopBag, ask );
        }
        Flag( ask, true ); Visible( false, "shop confirmation hides paging" );
        Flag( ask, false ); Flag( GameShopUI.instance, false );
        Visible( false, "leaving shop hides paging" );
        EditorSceneManager.LoadSceneInPlayMode( "Assets/Scenes/level3.unity", new LoadSceneParameters( LoadSceneMode.Single ) );
        while ( SceneManager.GetActiveScene().path != "Assets/Scenes/level3.unity" ||
                GameBattleJudgment.instance == null || GameBattleUnitManager.instance == null ) yield return null;
        // Isolate reward settlement from stage objectives and unit inventory; no battle events or save writes.
        typeof(GameBattleUnitManager).GetField( "units", BindingFlags.Instance | BindingFlags.NonPublic )
            .SetValue( GameBattleUnitManager.instance, new GameBattleUnit[0] );
        var previousMode = GameSetting.instance.mode;
        try
        {
            foreach ( var mode in new[] { GameSetting.GameMode.Easy, GameSetting.GameMode.Normal, GameSetting.GameMode.Hard } )
                foreach ( int retry in new[] { 0, 1, 3 } )
                {
                    GameSetting.instance.mode = mode; GameUserData.instance.setStage( 34 );
                    GameUserData.instance.Proficiency = 24; GameUserData.instance.ProficiencyStage = 1;
                    GameUserData.instance.ReloadBattleCount = retry;
                    GameBattleJudgment.instance.UpdateProficiency();
                    Check( GameUserData.instance.Proficiency == ( retry == 0 ? 25 : 24 ), "actual reward settlement " + mode + " retries=" + retry );
                    Check( GameUserData.instance.ProficiencyStage == 0 && GameUserData.instance.ReloadBattleCount == 0,
                        "settlement clears stage counters " + mode + " retries=" + retry );
                }
        }
        finally { GameSetting.instance.mode = previousMode; }
    }
    static void Finish( string error )
    {
        SessionState.SetBool( Key, false ); EditorApplication.update -= Tick;
        if ( captured ) { GameSetting.instance.virtualKeyboardEnabled = oldEnabled; GameSetting.instance.touchScale = oldScale; }
        report.error = error;
        try { Configure( false ); } catch ( Exception e ) { report.error += e; }
        File.WriteAllText( Output + "/editor-check.json", JsonUtility.ToJson( report, true ) );
        EditorApplication.Exit( report.error.Length == 0 ? 0 : 1 );
    }
}
