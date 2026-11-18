using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if SWORDSMAN_CHAPTER_TESTS && (DEVELOPMENT_BUILD || UNITY_EDITOR)
[DefaultExecutionOrder(-10000)]
public sealed class SwordsmanChapterTestDesk : MonoBehaviour
{
    public static SwordsmanChapterTestDesk Instance { get; private set; }
    public static bool InputBlocked { get { return Instance != null && ( Instance.menuOpen || Instance.busy || Instance.seekBattle ||
        Instance.waitForInputRelease || Time.frameCount <= Instance.blockedThroughFrame ); } }
    public SwordsmanChapterTestCatalog Catalog { get; private set; }
    public string LaunchError { get; private set; }
    public bool Busy { get { return busy; } }
    public bool CheckpointCaptured { get; private set; }
    public string Mode { get; private set; }

    bool menuOpen;
    bool waitForInputRelease;
    int blockedThroughFrame = -1;
    float menuTimeScale = 1;
    bool titleWasPaused;
    bool busy;
    bool seekBattle;
    bool captureRequested;
    bool cacheAvailable;
    int selected = 1;
    int selectedLevel = 1;
    int selectedRpg;
    int cameraX;
    int cameraY;
    float nextPointerAction;
    bool pointerQueued;
    Vector2 queuedPointer;
    float nextClick;
    float seekStarted;
    float nextCameraMove;
    string message = "选择章节后即可直达，无需通关前面的章节。";
    string fatalError;
    SwordsmanChapterTestPreset activePreset;
    int activeLevel;
    GUIStyle titleStyle;
    GUIStyle labelStyle;
    GUIStyle smallStyle;
    GUIStyle buttonStyle;
    GUIStyle selectedStyle;
    GUIStyle chapterStyle;
    GUIStyle summaryStyle;
    GUIStyle presetStyle;
    GUIStyle pageStyle;
    GUIStyle toggleStyle;
    GUIStyle mockStatusStyle;

    struct RpgEntry
    {
        public int town;
        public int position;
        public int nextStage;
    }

    sealed class Interlude
    {
        public readonly List<RpgEntry> rpg = new List<RpgEntry>();
        public int innNextStage = -1;
    }

    readonly Interlude[] interludes = new Interlude[GameDefine.MAX_STAGE];

#if SWORDSMAN_CHAPTER_TESTS_LARGE_WINDOW && UNITY_STANDALONE_OSX
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void ApplyLargeWindow()
    {
        if ( Application.isBatchMode ) return;
        string[] args = Environment.GetCommandLineArgs();
        if ( Array.Exists( args, arg => arg == "-screen-width" || arg == "-screen-height" ||
            arg == "-screen-fullscreen" || arg == "-popupwindow" ) ) return;

        // Unity can restore fullscreen settings from a previous build with the same bundle ID.
        Screen.SetResolution( 2560, 1600, FullScreenMode.Windowed );
    }
#endif

    Interlude GetInterlude( int stage )
    {
        if ( interludes[stage] != null ) return interludes[stage];
        var options = new Interlude();
        GameCampScript camp = GameCampData.instance.getData( stage );
        var seen = new HashSet<int>();
        void AddTown( int town, int position, int nextStage )
        {
            var target = GameRPGData.instance.getData( town );
            if ( target?.Info.Pos == null || position < 0 || position >= target.Info.Pos.Length ||
                target.Info.Pos[position] == null || !seen.Add( town ) ) return;
            options.rpg.Add( new RpgEntry { town = town, position = position, nextStage = nextStage } );
        }

        foreach ( var group in GameBattleData.instance.getStage( stage ).Event )
        {
            foreach ( var command in group.Event )
            {
                if ( command.Type != 0x02 || command.Data.Length < 6 ) continue;
                int town = BitConverter.ToInt16( command.Data, 0 );
                int campParameter = BitConverter.ToInt16( command.Data, 2 );
                int nextStage = BitConverter.ToInt16( command.Data, 4 );
                if ( town >= 0 ) AddTown( town, 0, nextStage );
                else if ( campParameter >= 0 && camp != null && camp.Town >= 0 )
                {
                    if ( options.innNextStage < 0 ) options.innNextStage = nextStage;
                    AddTown( camp.Town, 1, nextStage );
                }
            }
        }
        // Follow the authored room connections, including conditional script branches.
        for ( int index = 0; index < options.rpg.Count; index++ )
        {
            RpgEntry entry = options.rpg[index];
            foreach ( var group in GameRPGData.instance.getData( entry.town ).Event )
            {
                foreach ( var command in group.Event )
                {
                    if ( command.Type == 0x26 && command.Data.Length >= 4 )
                        AddTown( BitConverter.ToInt16( command.Data, 0 ), BitConverter.ToInt16( command.Data, 2 ), entry.nextStage );
                    else if ( command.Type == 0x29 && camp != null && camp.Town >= 0 && options.innNextStage < 0 )
                        options.innNextStage = entry.nextStage;
                }
            }
        }
        interludes[stage] = options;
        return options;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Create()
    {
        GameObject go = new GameObject( "SwordsmanChapterTestDesk" );
        DontDestroyOnLoad( go );
        Instance = go.AddComponent<SwordsmanChapterTestDesk>();
    }

    void Awake()
    {
        try
        {
            Catalog = SwordsmanChapterTestSession.LoadCatalog(); selectedLevel = Catalog.entries[selected].suggestedLevel;
            // Each headless process initializes only its own assigned profile.
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-check" ) < 0 ) RefreshCache();
        }
        catch ( Exception e ) { fatalError = "测试配置不可用：" + e.Message; }
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        string[] args = Environment.GetCommandLineArgs();
        if ( Array.IndexOf( args, "-swordsman-chapter-tests-check" ) >= 0 ||
             Array.IndexOf( args, "-swordsman-static-mvp-check" ) >= 0 ||
             Array.IndexOf( args, "-swordsman-chapter01-check" ) >= 0 ||
             Array.IndexOf( args, "-swordsman-rpg-map-check" ) >= 0 ) yield break;
        int stageArg = Array.IndexOf( args, "-swordsman-test-stage" );
        if ( stageArg >= 0 && stageArg + 1 < args.Length )
        {
            int stage;
            if ( Catalog == null || !int.TryParse( args[stageArg + 1], out stage ) || stage < 0 || stage >= Catalog.entries.Length )
            { fatalError = "章节参数无效。"; SetMenu( true ); yield break; }
            selected = stage; selectedLevel = Catalog.entries[selected].suggestedLevel;
            int modeArg = Array.IndexOf( args, "-swordsman-test-mode" );
            string mode = modeArg >= 0 && modeArg + 1 < args.Length ? args[modeArg + 1] : "opening";
            yield return Enter( Catalog.entries[selected], selectedLevel, mode );
        }
    }

    void RefreshCache()
    {
        cacheAvailable = Catalog != null && SwordsmanChapterTestSession.HasCheckpoint( Catalog.entries[selected], selectedLevel );
    }

    void SetMenu( bool open )
    {
        if ( menuOpen == open ) return;
        if ( open )
        {
            menuTimeScale = Time.timeScale;
            titleWasPaused = GameTitleUI.instance != null && GameTitleUI.instance.Pause;
        }
        menuOpen = open;
        Time.timeScale = open ? 0.0f : menuTimeScale;
        if ( GameTitleUI.instance != null ) GameTitleUI.instance.Pause = open || titleWasPaused;
        pointerQueued = false;
        nextPointerAction = Time.unscaledTime + 0.25f;
        if ( !open )
        {
            waitForInputRelease = true;
            blockedThroughFrame = Time.frameCount + 1;
        }
        Debug.Log( "SWORDSMAN_TEST_MENU open=" + open + " timeScale=" + Time.timeScale );
    }

    bool TitleMenuReady()
    {
        return SceneManager.GetActiveScene().buildIndex == (int)GameSceneType.Title &&
            GameSceneManager.instance != null && !GameSceneManager.instance.IsLoading &&
            GameMovieManager.instance != null && !GameMovieManager.instance.IsPlaying &&
            GameTitleUI.instance != null && GameTitleUI.instance.IsShow && !GameTitleUI.instance.IsFadeIn &&
            ( GameBlackUI.instance == null || !GameBlackUI.instance.IsShow ) &&
            ( GameSLUI.instance == null || !GameSLUI.instance.IsShow ) &&
            ( GameHelpUI.instance == null || !GameHelpUI.instance.IsShow );
    }

    static Rect LauncherRect() { return new Rect( 1070, 60, 194, 36 ); }
    static Rect CloseRect() { return new Rect( 44, 40, 110, 36 ); }

    void Update()
    {
        if ( waitForInputRelease && !Input.anyKey && !pointerQueued ) waitForInputRelease = false;
        if ( Input.GetKeyDown( KeyCode.F9 ) && !busy )
        {
            if ( menuOpen ) SetMenu( false );
            else if ( TitleMenuReady() ) SetMenu( true );
            else if ( activePreset != null && SceneManager.GetActiveScene().buildIndex != (int)GameSceneType.Title ) StartCoroutine( ReturnToDesk() );
            return;
        }
        if ( Input.GetMouseButtonDown( 0 ) || Input.GetMouseButtonUp( 0 ) )
            Debug.Log( "SWORDSMAN_TEST_POINTER raw=" + MousePosition() + " down=" + Input.GetMouseButtonDown( 0 ) );
        if ( menuOpen ) { MenuInput(); return; }
        if ( !busy && TitleMenuReady() && ( Input.GetMouseButtonDown( 0 ) || pointerQueued ) &&
            LauncherRect().Contains( pointerQueued ? queuedPointer : MousePosition() ) )
        { SetMenu( true ); return; }
        if ( !busy && activePreset != null && SceneManager.GetActiveScene().buildIndex != (int)GameSceneType.Title &&
            ( Input.GetMouseButtonDown( 0 ) || pointerQueued ) && new Rect( 16, 16, 290, 36 ).Contains( pointerQueued ? queuedPointer : MousePosition() ) )
        { pointerQueued = false; StartCoroutine( ReturnToDesk() ); return; }
        pointerQueued = false;
        if ( busy || activePreset == null ) return;
        if ( Mode == "inspect" )
        {
            if ( Time.unscaledTime >= nextCameraMove )
            {
                int dx = Input.GetKey( KeyCode.RightArrow ) ? 1 : Input.GetKey( KeyCode.LeftArrow ) ? -1 : 0;
                int dy = Input.GetKey( KeyCode.DownArrow ) ? 1 : Input.GetKey( KeyCode.UpArrow ) ? -1 : 0;
                if ( dx != 0 || dy != 0 )
                {
                    cameraX = Mathf.Clamp( cameraX + dx, 0, 100 ); cameraY = Mathf.Clamp( cameraY + dy, 0, 100 );
                    GameBattleSceneMovement.instance.moveToEvent( cameraX, cameraY ); nextCameraMove = Time.unscaledTime + 0.12f;
                }
            }
            return;
        }
        if ( captureRequested && BattleReady() )
        {
            if ( Run( () => SwordsmanChapterTestSession.CaptureCheckpoint( activePreset, activeLevel ) ) )
            {
                captureRequested = false; seekBattle = false; Time.timeScale = 1;
                CheckpointCaptured = true; RefreshCache(); message = "战斗起点已保存，下次可直接进入。";
                Debug.Log( "SWORDSMAN_CHAPTER_TEST_CHECKPOINT captured stage=" + activePreset.stage );
            }
            else { captureRequested = false; seekBattle = false; Time.timeScale = 1; }
        }
        if ( seekBattle )
        {
            bool choosing = GameMsgBoxChooseUI.instance != null && GameMsgBoxChooseUI.instance.IsShow;
            if ( choosing ) { seekBattle = false; Time.timeScale = 1; message = "出现剧情选择，请手动选择；到达战斗后仍会保存起点。"; }
            else if ( Time.unscaledTime - seekStarted > 90 )
            { seekBattle = false; Time.timeScale = 1; message = "尚未到达战斗，请继续剧情；不会把当前状态误存为起点。"; }
            else if ( GameMsgBoxUI.instance != null && GameMsgBoxUI.instance.IsShow && Time.unscaledTime >= nextClick )
            { GameMsgBoxUI.instance.onClick(); nextClick = Time.unscaledTime + 0.06f; }
        }
    }

    static Vector2 MousePosition()
    {
        return new Vector2( Input.mousePosition.x * 1280.0f / Screen.width, 800.0f - Input.mousePosition.y * 800.0f / Screen.height );
    }

    void SelectRow( int index )
    {
        selected = Mathf.Clamp( index, 0, Catalog.entries.Length - 1 );
        selectedRpg = 0;
        selectedLevel = Catalog.entries[selected].suggestedLevel; RefreshCache();

    }

    void MenuInput()
    {
        Vector2 mouse = pointerQueued ? queuedPointer : MousePosition();
        bool clicked = ( pointerQueued || Input.GetMouseButtonDown( 0 ) || Input.GetMouseButtonUp( 0 ) ) && Time.unscaledTime >= nextPointerAction;
        pointerQueued = false;
        if ( Input.GetKeyDown( KeyCode.Escape ) || ( clicked && CloseRect().Contains( mouse ) ) )
        { SetMenu( false ); return; }
        if ( Catalog == null ) return;
        if ( Input.GetKeyDown( KeyCode.UpArrow ) ) SelectRow( selected - 3 );
        if ( Input.GetKeyDown( KeyCode.DownArrow ) ) SelectRow( selected + 3 );
        if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) SelectRow( selected - 1 );
        if ( Input.GetKeyDown( KeyCode.RightArrow ) ) SelectRow( selected + 1 );
        Interlude options = GetInterlude( selected );
        string mode = Input.GetKeyDown( KeyCode.Return ) ? "opening" : Input.GetKeyDown( KeyCode.B ) ? "battle" : Input.GetKeyDown( KeyCode.V ) ? "inspect" :
            Input.GetKeyDown( KeyCode.R ) && options.rpg.Count > 0 ? "rpg" :
            Input.GetKeyDown( KeyCode.I ) && options.innNextStage >= 0 ? "inn" : null;
        if ( clicked ) nextPointerAction = Time.unscaledTime + 0.25f;
        if ( clicked )
        {
            if ( new Rect( 40, 95, 858, 620 ).Contains( mouse ) )
            {
                float x = mouse.x - 40, y = mouse.y - 95;
                int column = Mathf.FloorToInt( x / 286 ), row = Mathf.FloorToInt( y / 48 );
                int index = row * 3 + column;
                if ( column < 3 && index < Catalog.entries.Length && x - column * 286 < 278 && y - row * 48 < 42 ) SelectRow( index );
            }
            for ( int i = 0; i < 7; i++ )
                if ( MockRect( i ).Contains( mouse ) ) SwordsmanChapterTestMock.Pending ^= (SwordsmanChapterMockOption)( 1 << i );
            if ( new Rect( 1140, 648, 90, 32 ).Contains( mouse ) ) SwordsmanChapterTestMock.Pending = SwordsmanChapterMockOption.None;
            if ( ActionRect( 0 ).Contains( mouse ) ) mode = "opening";
            if ( ActionRect( 1 ).Contains( mouse ) ) mode = "battle";
            if ( ActionRect( 2 ).Contains( mouse ) ) mode = "inspect";
            options = GetInterlude( selected );
            if ( options.rpg.Count > 0 && ActionRect( 3, true ).Contains( mouse ) ) mode = "rpg";
            if ( options.rpg.Count > 1 && new Rect( 1178, 312, 52, 36 ).Contains( mouse ) )
                selectedRpg = ( selectedRpg + 1 ) % options.rpg.Count;
            if ( options.innNextStage >= 0 && ActionRect( 4 ).Contains( mouse ) ) mode = "inn";
        }
        if ( mode != null ) StartCoroutine( Enter( Catalog.entries[selected], selectedLevel, mode, selectedRpg ) );
    }

    public static bool BattleReady()
    {
        return SceneManager.GetActiveScene().buildIndex == (int)GameSceneType.Battle &&
            GameBattleTurn.instance != null && GameBattleTurn.instance.Turn == 1 && GameBattleTurn.instance.UserTurn &&
            !GameBattleTurn.instance.Pause && GameBattleInput.instance != null && !GameBattleInput.instance.Pause &&
            GameBattleEventManager.instance != null && !GameBattleEventManager.instance.IsShow &&
            GameMsgBoxUI.instance != null && !GameMsgBoxUI.instance.IsShow &&
            GameBattleCursor.instance != null && GameBattleCursor.instance.IsShow && !GameBattleUnitManager.instance.IsNpcAI;
    }

    bool Run( Action action )
    {
        try { action(); return true; }
        catch ( Exception e ) { LaunchError = e.Message; message = "测试入口失败：" + e.Message; Debug.LogError( "SWORDSMAN_CHAPTER_TEST failed: " + e ); return false; }
    }

    public IEnumerator Enter( SwordsmanChapterTestPreset preset, int level, string mode, int rpgIndex = 0 )
    {
        if ( busy ) yield break;
        SwordsmanChapterTestMock.BeginEntry();
        if ( mode == "rpg" || mode == "inn" )
        {
            yield return EnterInterlude( preset, level, mode, rpgIndex );
            yield break;
        }
        if ( mode != "opening" && mode != "battle" && mode != "inspect" )
        { LaunchError = "未知测试入口。"; SetMenu( true ); yield break; }
        SetMenu( false ); Time.timeScale = 1; busy = true; seekBattle = false; captureRequested = false; CheckpointCaptured = false;
        LaunchError = null; Mode = mode; activePreset = preset; activeLevel = level; selected = preset.stage; selectedLevel = level;
        bool restored = false;
        if ( !Run( () => {
            if ( mode == "battle" ) restored = SwordsmanChapterTestSession.RestoreCheckpoint( preset, level );
            if ( restored ) GameSceneManager.instance.loadScene( GameSceneType.Battle, GameSceneLoadMode.LoadBattle );
            else
            {
                SwordsmanChapterTestSession.Apply( preset, level );
                if ( mode == "inspect" )
                {
                    GameSceneManager.instance.SceneType = GameSceneType.Battle;
                    SceneManager.LoadScene( (int)GameSceneType.Battle );
                }
                else GameSceneManager.instance.loadScene( GameSceneType.Battle, GameSceneLoadMode.StartBattle );
            }
        } ) ) { busy = false; SetMenu( true ); yield break; }
        float deadline = Time.realtimeSinceStartup + 20;
        do { yield return null; } while ( ( SceneManager.GetActiveScene().buildIndex != 3 || GameSceneManager.instance.IsLoading ) && Time.realtimeSinceStartup < deadline );
        yield return null;
        yield return null;
        if ( SceneManager.GetActiveScene().buildIndex != 3 || GameSceneManager.instance.IsLoading )
        { LaunchError = "场景加载超时。"; busy = false; message = LaunchError; yield break; }
        if ( mode == "inspect" && !Run( () => {
            GameBattleManager.instance.active(); GameBattleManager.instance.showLayer( 1, false );
            GameBattleManager.instance.initTreasures(); GameBattleManager.instance.updateTreasures();
            if ( preset.stage == 1 )
            {
                GameObject prop = GameObject.Find( "BattleScene/Background/Eanm_14" );
                if ( prop != null ) prop.GetComponent<GameAnimation>().showFrame( 0 );
            }
            GameBattleUnitManager.instance.initUnits(); GameBattleUnitManager.instance.showUnits( 0 );
            GameBattleInput.instance.Pause = true; GameBattleTurn.instance.Pause = true;
            GameBattleUnit unit = GameBattleUnitManager.instance.Units[0]; cameraX = Mathf.Max( 0, unit.PosX - 5 ); cameraY = Mathf.Max( 0, unit.PosY - 4 );
            if ( preset.stage == 1 ) { cameraX = 8; cameraY = 20; }
            GameBattleSceneMovement.instance.moveToEvent( cameraX, cameraY ); GameBattleCursor.instance.unShow();
            GameBlackUI.instance.unShowBlack( 0.1f, null );
        } ) ) { busy = false; yield break; }
        if ( !Run( SwordsmanChapterTestMock.AfterBattleEntry ) ) { busy = false; SetMenu( true ); yield break; }
        if ( mode == "inspect" ) yield return Resources.UnloadUnusedAssets();
        busy = false;
        captureRequested = mode != "inspect" && !restored;
        seekBattle = mode == "battle" && !restored;
        if ( seekBattle ) { Time.timeScale = 4; seekStarted = Time.unscaledTime; message = "首次进入：正在快进原版开场，到达战斗后建立起点。"; }
        else message = mode == "inspect" ? "资源查看：方向键移动镜头；F9返回选幕。" : restored ? "已从独立快照进入战斗起点。" : "按原版流程播放本幕开场，前置剧情状态仍需核验。";
        CheckpointCaptured = restored;
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_ENTER stage=" + preset.stage + " mode=" + mode + " restored=" + restored + " units=" + GameBattleUnitManager.instance.Units.Length );
    }

    IEnumerator EnterInterlude( SwordsmanChapterTestPreset preset, int level, string mode, int index )
    {
        var options = GetInterlude( preset.stage );
        bool inn = mode == "inn";
        if ( inn ? options.innNextStage < 0 : index < 0 || index >= options.rpg.Count )
        {
            LaunchError = "本幕没有该场景入口。";
            message = LaunchError;
            SetMenu( true );
            yield break;
        }
        SetMenu( false ); Time.timeScale = 1; busy = true; seekBattle = false; captureRequested = false; CheckpointCaptured = false;
        LaunchError = null; Mode = mode; activePreset = preset; activeLevel = level; selected = preset.stage; selectedLevel = level;
        GameSceneType scene = inn ? GameSceneType.Camp : GameSceneType.Rpg;
        if ( !Run( () => {
            SwordsmanChapterTestSession.Apply( preset, level );
            if ( inn )
            {
                GameCampScript camp = GameCampData.instance.getData( preset.stage );
                GameUserData.instance.setNextStage( options.innNextStage );
                GameUserData.instance.setTown( camp.Town );
                GameSceneManager.instance.loadScene( scene, GameSceneLoadMode.Count );
            }
            else
            {
                RpgEntry entry = options.rpg[index];
                GameUserData.instance.setNextStage( entry.nextStage );
                GameUserData.instance.setTown( entry.town );
                GameSceneManager.instance.loadScene( scene, GameSceneLoadMode.BattleOver );
                // The loader sets the default before the RPG scene starts. Room links can use other positions.
                GameUserData.instance.TownPosition = entry.position;
            }
        } ) ) { busy = false; SetMenu( true ); yield break; }
        float deadline = Time.realtimeSinceStartup + 20;
        do { yield return null; }
        while ( ( SceneManager.GetActiveScene().buildIndex != (int)scene || GameSceneManager.instance.IsLoading ) &&
                Time.realtimeSinceStartup < deadline );
        busy = false;
        if ( SceneManager.GetActiveScene().buildIndex != (int)scene || GameSceneManager.instance.IsLoading )
        { LaunchError = "场景加载超时。"; message = LaunchError; yield break; }
        message = inn ? "已进入客栈休息场景，F9 返回选幕。" : "已进入 RPG 场景，可以按原流程逛店。F9 返回选幕。";
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_INTERLUDE stage=" + preset.stage + " mode=" + mode +
            " scene=" + scene + " town=" + GameUserData.instance.Town );
    }

    IEnumerator ReturnToDesk()
    {
        seekBattle = false; captureRequested = false; busy = true; Time.timeScale = 1;
        GameSceneManager.instance.loadScene( GameSceneType.Title, GameSceneLoadMode.New );
        float deadline = Time.realtimeSinceStartup + 20;
        while ( ( SceneManager.GetActiveScene().buildIndex != 0 || GameSceneManager.instance.IsLoading ) && Time.realtimeSinceStartup < deadline ) yield return null;
        yield return null;
        // Returning from a test should reveal the title behind the desk, not pause its movie.
        if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.onClick();
        while ( !TitleMenuReady() && Time.realtimeSinceStartup < deadline ) yield return null;
        busy = false; activePreset = null; RefreshCache(); SetMenu( true );
    }

    static Rect ActionRect( int row, bool rpg = false )
    { return new Rect( 930, 180 + row * 44, rpg ? 242 : 300, 36 ); }

    static Rect MockRect( int index )
    {
        if ( index < 2 ) return new Rect( 930 + index * 156, 472, 144, 32 );
        if ( index < 6 ) return new Rect( 930 + ( index - 2 ) % 2 * 156, 540 + ( index - 2 ) / 2 * 40, 144, 32 );
        return new Rect( 930, 648, 198, 32 );
    }

    void Styles()
    {
        if ( titleStyle != null ) return;
        titleStyle = new GUIStyle( GUI.skin.label ) { fontSize = 30, fontStyle = FontStyle.Bold };
        labelStyle = new GUIStyle( GUI.skin.label ) { fontSize = 21, wordWrap = true };
        smallStyle = new GUIStyle( GUI.skin.label ) { fontSize = 17, wordWrap = true };
        buttonStyle = new GUIStyle { fontSize = 17, alignment = TextAnchor.MiddleLeft, wordWrap = true };
        buttonStyle.border = new RectOffset();
        buttonStyle.padding = new RectOffset( 12, 12, 2, 2 );
        foreach ( GUIStyleState state in new[] { buttonStyle.normal, buttonStyle.hover, buttonStyle.active, buttonStyle.focused,
            buttonStyle.onNormal, buttonStyle.onHover, buttonStyle.onActive, buttonStyle.onFocused } )
            state.textColor = Color.white;
        pageStyle = new GUIStyle( buttonStyle ) { alignment = TextAnchor.MiddleCenter, padding = new RectOffset(), wordWrap = false };
        toggleStyle = new GUIStyle( buttonStyle ) { fontSize = 14 };
        mockStatusStyle = new GUIStyle( buttonStyle ) { fontSize = 17 };
        chapterStyle = new GUIStyle( buttonStyle );
        selectedStyle = new GUIStyle( buttonStyle ) { fontStyle = FontStyle.Bold };
        summaryStyle = new GUIStyle( labelStyle ) { fontSize = 19, wordWrap = false };
        presetStyle = new GUIStyle( smallStyle ) { fontSize = 13, wordWrap = true };
    }

    static void DrawButton( Rect rect, string text, GUIStyle style, Color background )
    {
        // Draw every state as a solid fill; never inherit stretched skin button artwork.
        bool highlighted = GUI.enabled && rect.Contains( Event.current.mousePosition );
        if ( highlighted ) background = Input.GetMouseButton( 0 )
            ? Color.Lerp( background, Color.black, 0.18f ) : Color.Lerp( background, Color.white, 0.18f );
        Color previous = GUI.color;
        GUI.color = previous * background;
        GUI.DrawTexture( rect, Texture2D.whiteTexture );
        GUI.color = previous;
        GUIStyle textStyle = style;
        if ( style.CalcHeight( new GUIContent( text ), rect.width ) > rect.height )
        {
            textStyle = new GUIStyle( style );
            while ( textStyle.fontSize > 15 && textStyle.CalcHeight( new GUIContent( text ), rect.width ) > rect.height ) textStyle.fontSize--;
        }
        GUI.Label( rect, text, textStyle );
    }

    void OnGUI()
    {
        Styles(); GUI.depth = -1000;
        Event pointer = Event.current;
        if ( pointer.rawType == EventType.MouseDown && pointer.button == 0 )
        {
            queuedPointer = new Vector2( pointer.mousePosition.x * 1280.0f / Screen.width, pointer.mousePosition.y * 800.0f / Screen.height );
            pointerQueued = true; Debug.Log( "SWORDSMAN_TEST_POINTER gui=" + queuedPointer );
        }
        Matrix4x4 old = GUI.matrix; GUI.matrix = Matrix4x4.Scale( new Vector3( Screen.width / 1280.0f, Screen.height / 800.0f, 1 ) );
        if ( !menuOpen )
        {
            if ( !busy && TitleMenuReady() )
                DrawButton( LauncherRect(), "章节测试台", pageStyle, new Color( 0.12f, 0.32f, 0.50f, 0.92f ) );
            if ( activePreset == null || SceneManager.GetActiveScene().buildIndex == (int)GameSceneType.Title )
            { GUI.matrix = old; return; }
            GUI.enabled = !busy;
            DrawButton( new Rect( 16, 16, 290, 36 ), "F9 返回选幕（结束当前测试）", buttonStyle, new Color( 0, 0.478f, 1 ) );
            GUI.enabled = true;
            Rect mockStatusRect = new Rect( 16, 60, 290, 36 );
            string mockStatus = SwordsmanChapterTestMock.Summary;
            mockStatusStyle.fontSize = 17;
            while ( mockStatusStyle.fontSize > 11 && mockStatusStyle.CalcHeight( new GUIContent( mockStatus ), mockStatusRect.width ) > mockStatusRect.height ) mockStatusStyle.fontSize--;
            Color statusColor = GUI.color;
            GUI.color = statusColor * new Color( 0.12f, 0.43f, 0.36f );
            GUI.DrawTexture( mockStatusRect, Texture2D.whiteTexture );
            GUI.color = statusColor;
            GUI.Label( mockStatusRect, mockStatus, mockStatusStyle );
            GUI.matrix = old; return;
        }
        Color oldColor = GUI.color;
        GUI.color = new Color( 0, 0, 0, 0.15f ); GUI.DrawTexture( new Rect( 0, 0, 1280, 800 ), Texture2D.whiteTexture );
        GUI.color = new Color( 0.08f, 0.09f, 0.12f, 0.78f ); GUI.DrawTexture( new Rect( 20, 20, 1240, 760 ), Texture2D.whiteTexture ); GUI.color = oldColor;
        DrawButton( CloseRect(), "关闭 [Esc]", pageStyle, new Color( 0.20f, 0.30f, 0.41f ) );
        GUI.Label( new Rect( 174, 38, 700, 45 ), "章节测试台", titleStyle );
        if ( fatalError != null ) { GUI.Label( new Rect( 50, 160, 1130, 150 ), fatalError, labelStyle ); GUI.matrix = old; return; }
        GUI.BeginGroup( new Rect( 40, 95, 858, 620 ) );
        for ( int i = 0; i < Catalog.entries.Length; i++ )
        {
            int col = i % 3, row = i / 3;
            DrawButton( new Rect( col * 286, row * 48, 278, 42 ), Catalog.entries[i].title, i == selected ? selectedStyle : chapterStyle,
                i == selected ? new Color( 0.12f, 0.40f, 0.72f ) : new Color( 0.20f, 0.30f, 0.41f ) );
        }
        GUI.EndGroup();
        SwordsmanChapterTestPreset preset = Catalog.entries[selected];
        int separator = preset.title.IndexOf( ' ' );
        string chapterTitle = separator < 0 ? preset.title : preset.title.Substring( 0, separator ) + "：" + preset.title.Substring( separator ).Trim();
        string summary = chapterTitle + "  角色等级 " + selectedLevel;
        summaryStyle.fontSize = 17;
        while ( summaryStyle.fontSize > 13 && summaryStyle.CalcSize( new GUIContent( summary ) ).x > 300 ) summaryStyle.fontSize--;
        GUI.Label( new Rect( 930, 100, 300, 28 ), summary, summaryStyle );
        GUI.Label( new Rect( 930, 130, 300, 20 ), "预设初始装备+确定成长，非真实通关存档", presetStyle );
        GUI.Label( new Rect( 930, 152, 300, 20 ), "RPG 和客栈休息入口使用预设进度", presetStyle );
        Color actionBlue = new Color( 0, 0.478f, 1 );
        DrawButton( ActionRect( 0 ), "剧情开场 [Enter]", buttonStyle, actionBlue );
        DrawButton( ActionRect( 1 ), "战斗起点 [B]", buttonStyle, actionBlue );
        DrawButton( ActionRect( 2 ), "资源查看 [V]", buttonStyle, actionBlue );
        Interlude options = GetInterlude( selected );
        Color disabledButton = new Color( 0.24f, 0.27f, 0.32f );
        GUI.enabled = options.rpg.Count > 0;
        string rpgLabel = GUI.enabled ? "RPG 场景 " + options.rpg[selectedRpg].town.ToString( "D2" ) + " [R]" : "RPG 场景 [R]";
        DrawButton( ActionRect( 3, true ), rpgLabel, buttonStyle, GUI.enabled ? actionBlue : disabledButton );
        GUI.enabled = options.rpg.Count > 1;
        string rpgPage = options.rpg.Count > 0 ? ( selectedRpg + 1 ) + "/" + options.rpg.Count : "0/0";
        DrawButton( new Rect( 1178, 312, 52, 36 ), rpgPage, pageStyle, GUI.enabled ? new Color( 1, 0.231f, 0.188f ) : disabledButton );
        GUI.enabled = options.innNextStage >= 0;
        DrawButton( ActionRect( 4 ), "客栈休息 [I]", buttonStyle, GUI.enabled ? actionBlue : disabledButton );
        GUI.enabled = true;
        if ( preset.stage == 34 ) GUI.Label( new Rect( 930, 400, 300, 38 ), "本幕原版 camp34 资源缺失\n请用战斗起点；营地读取未通过", presetStyle );
        GUI.Label( new Rect( 930, 444, 300, 24 ), "生存辅助 · 我方可控角色", smallStyle );
        GUI.Label( new Rect( 930, 512, 300, 24 ), "角色能力与装备", smallStyle );
        GUI.Label( new Rect( 930, 620, 300, 24 ), "地图辅助", smallStyle );
        string[] mockLabels = { "无限血量", "无限气量", "全技能术法", "顶配武器", "顶配防具", "顶配饰品", "显示隐藏宝物" };
        for ( int i = 0; i < mockLabels.Length; i++ )
        {
            bool on = ( SwordsmanChapterTestMock.Pending & (SwordsmanChapterMockOption)( 1 << i ) ) != 0;
            DrawButton( MockRect( i ), ( on ? "[开] " : "[关] " ) + mockLabels[i], toggleStyle,
                on ? new Color( 0.08f, 0.39f, 0.64f ) : new Color( 0.20f, 0.25f, 0.32f ) );
        }
        DrawButton( new Rect( 1140, 648, 90, 32 ), "全部关闭", toggleStyle, new Color( 1, 0.231f, 0.188f ) );
        GUI.Label( new Rect( 930, 690, 300, 30 ), "选择开关后，重新进入剧情生效", presetStyle );
        GUI.Label( new Rect( 46, 733, 1110, 30 ), message, smallStyle );
        GUI.matrix = old;
    }
}
#endif
