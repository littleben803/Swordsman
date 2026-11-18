using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

#if SWORDSMAN_CHAPTER_TESTS && DEVELOPMENT_BUILD
public sealed class SwordsmanChapterTestChecks : MonoBehaviour
{
    readonly List<int> inspected = new List<int>();
    SwordsmanChapterTestDesk desk;
    string reportPath;

    [Serializable]
    public sealed class Result
    {
        public int[] enteredStages;
        public bool firstChapterCheckpointCaptured;
        public bool battleCheckpointReloaded;
        public bool ordinarySaveRoundTrip;
        public bool corruptCheckpointRejected;
        public bool wrongStageRejected;
        public bool changedPresetRejected;
        public bool realSaveCopyLoaded;
        public bool realSaveSampleAvailable;
        public bool allPathsIsolated;
        public string[] limitations = { "合成测试起点，非真实通关进度", "36关资源入口检查不等于全剧情通关", "剧情选择不自动决策" };
        public string error;
    }
    readonly Result result = new Result();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-check" ) < 0 ) return;
        GameObject go = new GameObject( "SwordsmanChapterTestChecks" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanChapterTestChecks>();
    }

    bool Check( Action action )
    {
        try { action(); return true; }
        catch ( Exception e )
        {
            result.error = e.ToString(); Finish( false ); return false;
        }
    }
    static void Require( bool value, string message ) { if ( !value ) throw new InvalidOperationException( message ); }

    IEnumerator Start()
    {
        yield return null; yield return null; yield return null;
        string[] args = Environment.GetCommandLineArgs(); int index = Array.IndexOf( args, "-swordsman-chapter-tests-report" );
        if ( index < 0 || index + 1 >= args.Length ) { Debug.LogError( "Chapter tests require a report path." ); Application.Quit( 1 ); yield break; }
        reportPath = args[index + 1]; desk = SwordsmanChapterTestDesk.Instance;
        if ( Array.IndexOf( args, "-swordsman-chapter-tests-mock-check" ) >= 0 )
        { yield return SwordsmanChapterMockChecks.Run( desk, reportPath ); yield break; }
        if ( Array.IndexOf( args, "-swordsman-chapter-tests-interlude-check" ) >= 0 )
        { yield return CheckInterludes(); yield break; }
        if ( Array.IndexOf( args, "-swordsman-chapter-tests-shop-check" ) >= 0 )
        { yield return CheckShops(); yield break; }
        int stageIndex = Array.IndexOf( args, "-swordsman-chapter-tests-stage-check" );
        if ( stageIndex >= 0 ) { yield return CheckStage( int.Parse( args[stageIndex + 1] ) ); yield break; }
        if ( !Check( () => {
            Require( desk != null && desk.Catalog != null && desk.Catalog.entries.Length == 36, "Missing test catalog." );
            Require( Path.GetFileName( SwordsmanChapterTestSession.RootDirectory ) == "Automation", "Not in the automation sandbox." );
            if ( Directory.Exists( SwordsmanChapterTestSession.RootDirectory ) ) Directory.Delete( SwordsmanChapterTestSession.RootDirectory, true );
        } ) ) yield break;
        for ( int i = 0; i < desk.Catalog.entries.Length; i++ )
        {
            int stage = i; SwordsmanChapterTestPreset preset = desk.Catalog.entries[i];
            yield return desk.Enter( preset, preset.suggestedLevel, "inspect" );
            if ( !Check( () => {
                Require( desk.LaunchError == null, "Entry failure stage=" + stage + ": " + desk.LaunchError );
                Require( SceneManager.GetActiveScene().buildIndex == 3 && GameUserData.instance.Stage == stage, "Stage mismatch." );
                Require( GameBattleUnitManager.instance.Units.Length == GameBattleData.instance.getStage( stage ).Man.Length, "Unit count mismatch." );
                Require( GameSetting.PersistentDataPath == SwordsmanChapterTestSession.SaveDirectory && GameSetting.PersistentDataPath.StartsWith( SwordsmanChapterTestSession.RootDirectory + Path.DirectorySeparatorChar ), "Save path escaped sandbox." );
                Require( GameSetting.PersistentDataPath != Application.persistentDataPath, "Real save directory selected." );
                inspected.Add( stage ); Debug.Log( "SWORDSMAN_CHAPTER_TEST_SCAN passed stage=" + stage );
            } ) ) yield break;
        }
        SwordsmanChapterTestPreset first = desk.Catalog.entries[1]; int level = first.suggestedLevel;
        yield return desk.Enter( first, level, "battle" );
        float deadline = Time.realtimeSinceStartup + 120;
        while ( !desk.CheckpointCaptured && desk.LaunchError == null && Time.realtimeSinceStartup < deadline ) yield return null;
        if ( !Check( () => {
            Require( desk.CheckpointCaptured && SwordsmanChapterTestDesk.BattleReady(), "First chapter did not reach a player turn." );
            Require( SwordsmanChapterTestSession.HasCheckpoint( first, level ), "Battle checkpoint failed validation." );
            result.firstChapterCheckpointCaptured = true;
            GameUserData.instance.save( 0 );
            Require( new FileInfo( Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "Save0.dat" ) ).Length == 3000, "Ordinary save format changed." );
        } ) ) yield break;
        int expectedLevel = GameUserData.instance.getUnitBase( 0 ).LV;
        GameUserData.instance.load( 0 );
        deadline = Time.realtimeSinceStartup + 20;
        while ( ( SceneManager.GetActiveScene().buildIndex != 2 || GameSceneManager.instance.IsLoading ) && Time.realtimeSinceStartup < deadline ) yield return null;
        if ( !Check( () => {
            Require( SceneManager.GetActiveScene().buildIndex == 2 && GameUserData.instance.Stage == 1 && GameUserData.instance.getUnitBase( 0 ).LV == expectedLevel, "Ordinary save round trip failed." );
            result.ordinarySaveRoundTrip = true;
        } ) ) yield break;
        yield return desk.Enter( first, level, "battle" );
        if ( !Check( () => {
            Require( desk.CheckpointCaptured && desk.LaunchError == null && GameUserData.instance.Stage == 1, "Battle checkpoint reload failed." );
            result.battleCheckpointReloaded = true;
            string path = Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "BattleStart.dat" ); byte[] bytes = File.ReadAllBytes( path );
            string metadataPath = Path.Combine( SwordsmanChapterTestSession.SaveDirectory, "BattleStart.json" );
            string originalMetadata = File.ReadAllText( metadataPath );
            try
            {
                byte[] invalid = (byte[])bytes.Clone(); invalid[20] ^= 0x7f; File.WriteAllBytes( path, invalid );
                result.corruptCheckpointRejected = !SwordsmanChapterTestSession.HasCheckpoint( first, level );
                invalid = (byte[])bytes.Clone(); Array.Copy( BitConverter.GetBytes( 2 ), 0, invalid, 0, sizeof( int ) ); File.WriteAllBytes( path, invalid );
                SwordsmanChapterTestCheckpoint wrong = JsonUtility.FromJson<SwordsmanChapterTestCheckpoint>( originalMetadata );
                wrong.binaryHash = SwordsmanChapterTestSession.Hash( invalid );
                File.WriteAllText( metadataPath, JsonUtility.ToJson( wrong, true ) );
                result.wrongStageRejected = !SwordsmanChapterTestSession.HasCheckpoint( first, level );
            }
            finally { File.WriteAllBytes( path, bytes ); File.WriteAllText( metadataPath, originalMetadata ); }
            first.revision++;
            try { result.changedPresetRejected = !SwordsmanChapterTestSession.HasCheckpoint( first, level ); }
            finally { first.revision--; }
            Require( result.corruptCheckpointRejected && result.wrongStageRejected && result.changedPresetRejected && SwordsmanChapterTestSession.HasCheckpoint( first, level ), "Checkpoint validation did not reject invalid data." );
            result.allPathsIsolated = true;
        } ) ) yield break;
        // Read-only compatibility sample: copy into the automation sandbox before using the unchanged reader.
        string legacy = Path.Combine( Application.persistentDataPath, "SaveBattle.dat" );
        if ( File.Exists( legacy ) )
        {
            byte[] bytes = File.ReadAllBytes( legacy ); result.realSaveSampleAvailable = true;
            if ( !Check( () => {
                Require( bytes.Length == 102400 && BitConverter.ToInt32( bytes, 0 ) >= 0 && BitConverter.ToInt32( bytes, 0 ) < 36, "Legacy sample is not a valid battle snapshot." );
                string path = Path.Combine( SwordsmanChapterTestSession.RootDirectory, "LegacyReadOnlyCopy" ); Directory.CreateDirectory( path );
                File.WriteAllBytes( Path.Combine( path, "SaveBattle.dat" ), bytes ); GameSetting.PersistentDataPath = path;
                GameSceneManager.instance.loadScene( GameSceneType.Battle, GameSceneLoadMode.LoadBattle );
            } ) ) yield break;
            deadline = Time.realtimeSinceStartup + 20;
            while ( GameSceneManager.instance.IsLoading && Time.realtimeSinceStartup < deadline ) yield return null;
            if ( !Check( () => {
                Require( SceneManager.GetActiveScene().buildIndex == 3 && GameUserData.instance.Stage == BitConverter.ToInt32( bytes, 0 ), "Legacy copy did not load." );
                Require( SwordsmanChapterTestSession.Hash( File.ReadAllBytes( legacy ) ) == SwordsmanChapterTestSession.Hash( bytes ), "Real save changed." );
                result.realSaveCopyLoaded = true;
            } ) ) yield break;
        }
        Finish( true );
    }

    [Serializable]
    public sealed class StageResult
    {
        public int stage;
        public string title;
        public int level;
        public string checkpointSource;
        public bool openingReachedPlayerTurn;
        public bool checkpointValidated;
        public bool battleReloadReady;
        public bool hasControllableUnits;
        public bool stateRoundTrip;
        public bool ordinarySaveRoundTrip;
        public string ordinarySaveBlocker;
        public bool pathIsolated;
        public string beforeState;
        public string afterState;
        public string diagnostic;
        public string error;
        public string[] runtimeErrors;
        public float seconds;
    }

    static string State()
    {
        System.Text.StringBuilder state = new System.Text.StringBuilder();
        state.Append( "stage=" ).Append( GameUserData.instance.Stage ).Append( ";turn=" ).Append( GameBattleTurn.instance.Turn );
        foreach ( GameBattleUnit unit in GameBattleUnitManager.instance.Units )
        {
            GameBattleUnitData d = unit.getUnitData();
            state.Append( "|" ).Append( unit.BattleManID ).Append( "," ).Append( unit.UnitID ).Append( "," ).Append( unit.PosX ).Append( "," ).Append( unit.PosY )
                .Append( "," ).Append( unit.IsShow ).Append( "," ).Append( unit.IsKilled ).Append( "," ).Append( unit.IsActed ).Append( "," ).Append( unit.LV )
                .Append( "," ).Append( d.HP ).Append( "," ).Append( d.MP );
        }
        for ( int i = 0; i < GameDefine.MAX_GAMEDATA; i++ ) state.Append( ":" ).Append( GameUserData.instance.getGameData( i ) );
        for ( int i = 0; i < GameDefine.MAX_USER; i++ ) state.Append( "#" ).Append( JsonUtility.ToJson( GameUserData.instance.getUnitBase( i ) ) );
        state.Append( "|item216=" ).Append( GameUserData.instance.haveItem( 216 ) );
        return state.ToString();
    }

    static string Diagnostic()
    {
        var e = GameBattleEventManager.instance;
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        return "scene=" + SceneManager.GetActiveScene().buildIndex + " stage=" + GameUserData.instance.Stage + " turn=" + GameBattleTurn.instance.Turn +
            " user=" + GameBattleTurn.instance.UserTurn + " event=" + e.IsShow + " id=" + e.GetType().GetField( "activeID", flags ).GetValue( e ) +
            " step=" + e.GetType().GetField( "activeStep", flags ).GetValue( e ) + " message=" + GameMsgBoxUI.instance.IsShow +
            " choice=" + GameMsgBoxChooseUI.instance.IsShow + " npc=" + GameBattleUnitManager.instance.IsNpcAI;
    }

    static void CheckTurnBanner()
    {
        var turn = GameBattleTurn.instance;
        var animation = turn.transform.Find( "Cha_tur" ).GetComponent<GameAnimation>();
        var numbers = turn.transform.Find( "Numbers" );
        var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
        var show = typeof( GameBattleTurn ).GetMethod( "showAnimation", flags );
        var side = typeof( GameBattleTurn ).GetField( "type", flags );
        var round = typeof( GameBattleTurn ).GetField( "turn", flags );
        var seen = new HashSet<string>();
        void CheckImage( UnityEngine.UI.Image image )
        {
            Sprite actual = image.sprite;
            Sprite legacy = actual == null ? null : Array.Find( animation.sprites, s => s.name == actual.name );
            Require( legacy != null && ( actual != legacy || SwordsmanHd4xSpriteResolver.IsMigratedSprite( legacy ) ) && actual.texture.width == 2048 && actual.texture.height == 4096 &&
                Vector2.Distance( image.rectTransform.sizeDelta, legacy.rect.size / legacy.pixelsPerUnit ) < .001f,
                "Turn banner HD sprite or display geometry mismatch." );
            seen.Add( actual.name );
        }

        // Exercise the actual turn banner and manually sample both existing playback ranges.
        foreach ( GameBattleTurnType type in new[] { GameBattleTurnType.User, GameBattleTurnType.Enemy } )
        {
            side.SetValue( turn, type );
            round.SetValue( turn, 23 );
            show.Invoke( turn, null );
            int start = type == GameBattleTurnType.User ? 12 : 28;
            for ( int frame = start; frame < start + 15; frame++ )
            {
                animation.showFrame( frame );
                var images = animation.GetComponentsInChildren<UnityEngine.UI.Image>();
                Require( images.Length == animation.saf1[ frame ].count, "Turn banner frame layer count mismatch." );
                foreach ( var image in images ) CheckImage( image );
            }
            var digits = numbers.GetComponentsInChildren<UnityEngine.UI.Image>();
            Require( digits.Length == 4, "Turn 23 did not assemble four glyphs." );
            foreach ( var image in digits ) CheckImage( image );
        }
        for ( int n = 1; n <= 9; n++ )
        {
            round.SetValue( turn, n );
            show.Invoke( turn, null );
            var digits = numbers.GetComponentsInChildren<UnityEngine.UI.Image>();
            Require( digits.Length == 2, "Single digit turn glyph count mismatch." );
            foreach ( var image in digits ) CheckImage( image );
        }
        Require( seen.Count == 54, "Turn banner did not display all 54 HD slices." );
        Debug.Log( "SWORDSMAN_HD_TURN_BANNER_CHECK passed frames=30 distinctSlices=54" );
    }

    static void CheckDialogueArrow()
    {
        GameMsgBoxUI dialogue = GameMsgBoxUI.instance;
        var text = dialogue.transform.Find( "left" ).GetComponentInChildren<GameImageText>();
        var animation = text.GetComponentInChildren<GameAnimation>();
        Require( text.IsOver && !dialogue.IsFadeIn && !animation.pause,
            "Dialogue did not reach its continue-arrow animation." );
        for ( int frame = 27; frame <= 40; frame++ )
        {
            animation.showFrame( frame );
            Sprite legacy = animation.sprites[ animation.saf1[ frame ].saf11[ 0 ].textureID ];
            var images = animation.GetComponentsInChildren<UnityEngine.UI.Image>();
            Require( images.Length == 1, "Dialogue continue-arrow layer count mismatch." );
            var image = images[0];
            Require( image.sprite != null && ( image.sprite != legacy || SwordsmanHd4xSpriteResolver.IsMigratedSprite( legacy ) ) && image.sprite.name == "MSG_BOX_t" + ( frame - 23 ) &&
                image.sprite.texture.width == 2048 && image.sprite.texture.height == 512 &&
                Vector2.Distance( image.rectTransform.sizeDelta, legacy.rect.size / legacy.pixelsPerUnit ) < .001f,
                "Dialogue continue-arrow sprite or display geometry mismatch." );
        }
        Debug.Log( "SWORDSMAN_HD_DIALOGUE_ARROW_CHECK passed frames=14" );
    }

    IEnumerator CheckInterludes()
    {
        var stage0 = desk.Catalog.entries[0];
        var stage1 = desk.Catalog.entries[1];
        var stage3 = desk.Catalog.entries[3];
        var stage7 = desk.Catalog.entries[7];
        yield return desk.Enter( stage1, stage1.suggestedLevel, "rpg" );
        if ( !Check( () => Require( desk.LaunchError != null && SceneManager.GetActiveScene().buildIndex == 0,
            "Chapter with no RPG interlude accepted the RPG shortcut." ) ) ) yield break;
        yield return desk.Enter( stage0, stage0.suggestedLevel, "rpg" );
        if ( !Check( () => CheckRpg( 0, 0, 1 ) ) ) yield break;
        yield return desk.Enter( stage3, stage3.suggestedLevel, "rpg", 1 );
        if ( !Check( () => CheckRpg( 3, 2, 4 ) ) ) yield break;
        yield return desk.Enter( stage7, stage7.suggestedLevel, "rpg", 3 );
        if ( !Check( () => CheckRpg( 7, 26, 8 ) ) ) yield break;
        yield return desk.Enter( stage0, stage0.suggestedLevel, "inn" );
        if ( !Check( () => {
            Require( desk.LaunchError == null && SceneManager.GetActiveScene().buildIndex == (int)GameSceneType.Camp &&
                GameCampUI.instance.IsShow && GameUserData.instance.Stage == 0 && GameUserData.instance.NextStage == 1,
                "Chapter zero inn did not open its original Camp00 UI." );
            CheckCamp00Background();
            CheckCampBackdropReplacements();
            CheckInnAlchemy();
            GameCampUI.instance.leave();
        } ) ) yield break;
        float deadline = Time.realtimeSinceStartup + 15;
        while ( ( SceneManager.GetActiveScene().buildIndex != (int)GameSceneType.Rpg || GameSceneManager.instance.IsLoading ) &&
                Time.realtimeSinceStartup < deadline ) yield return null;
        if ( !Check( () => CheckRpg( 0, 0, 1 ) ) ) yield break;
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_INTERLUDES passed rpg=3 inn=1 return=1 noRoute=1" );
        Finish( true );

        void CheckRpg( int stage, int town, int nextStage )
        {
            Require( desk.LaunchError == null && SceneManager.GetActiveScene().buildIndex == (int)GameSceneType.Rpg &&
                GameRPGManager.instance.ActiveID == town && GameUserData.instance.Stage == stage &&
                GameUserData.instance.NextStage == nextStage, "Unexpected RPG scene, route or continuation." );
            Require( GameSetting.PersistentDataPath == SwordsmanChapterTestSession.SaveDirectory &&
                GameSetting.PersistentDataPath != Application.persistentDataPath, "Interlude escaped the test profile." );
        }
    }

    static void CheckCamp00Background()
    {
        var animation = GameCampUI.instance.transform.Find( "Background" ).GetComponentInChildren<GameAnimation>();
        Require( animation.animationName == "Camp/Image/Camp00" && animation.saf1.Length == 8,
            "Camp00 foreground animation must keep its original eight frames." );
        foreach ( var frame in animation.saf1 )
        {
            var layer = frame.saf11[0];
            Require( frame.count == 4 && layer.textureX == 3 && layer.textureY == 0 &&
                animation.sprites[layer.textureID].name == "Camp10_t0",
                "Each Camp00 frame must start with the restored room at its original position." );
        }
        var image = animation.transform.GetChild( 0 ).GetComponent<UnityEngine.UI.Image>();
        Require( image.sprite.name == "Camp10_t0" && image.sprite.rect.size == new Vector2( 1284, 804 ) &&
            image.rectTransform.sizeDelta == new Vector2( 321, 201 ) &&
            image.rectTransform.anchoredPosition == new Vector2( animation.offsetX + 3, animation.offsetY ),
            "Camp00 room sprite was not drawn behind the foreground at the expected size and position." );
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_CAMP00_BACKGROUND passed frames=8 position=3,0" );
    }

    static void CheckCampBackdropReplacements()
    {
        for ( int camp = 1; camp <= 33; camp++ )
        {
            var prefab = SwordsmanHd4xResources.LoadPrefab( "Prefab/Camp/Image/camp" + GameDefine.getString2( camp ) );
            var animation = prefab.GetComponent<GameAnimation>();
            int replaced = 0;
            foreach ( Sprite original in animation.sprites )
            {
                Sprite resolved = SwordsmanHd4xSpriteResolver.Resolve( original );
                if ( SwordsmanHd4xSpriteResolver.IsMigratedSprite( original ) ) { replaced++; continue; }
                if ( resolved == original ) continue;
                Require( resolved.name == original.name && resolved.rect.size == original.rect.size * 2 &&
                    Mathf.Approximately( resolved.pixelsPerUnit, original.pixelsPerUnit * 2 ),
                    "Camp backdrop did not preserve its original world size: " + original.name );
                replaced++;
            }
            Require( replaced == 1, "Camp " + camp + " must replace only its fixed background." );
        }
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_CAMP_BACKDROPS passed backgrounds=33" );
    }

    static void CheckInnAlchemy()
    {
        var alchemy = GameAlchemyUI.instance;
        var bag = alchemy.transform.Find( "bag0" ).GetComponent<GameAlchemyUIBag>();
        alchemy.show( null );
        CheckEmptyBag();
        alchemy.unShow();

        GameUserData.instance.addItem( 0 );
        alchemy.show( null );
        Require( bag.getItem()?.ID == 0 && bag.getItemDes() == bag.getItem().Description,
                 "Alchemy no longer selects and describes a normal item." );
        alchemy.unShow();

        GameUserData.instance.removeItem( 0 );
        alchemy.show( null );
        CheckEmptyBag();
        alchemy.unShow();
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_INN_ALCHEMY passed empty=2 populated=1" );

        void CheckEmptyBag()
        {
            alchemy.select( 1 );
            alchemy.onClick();
            bag.showAskUI( true );
            Require( alchemy.IsShow && bag.Selection == GameDefine.INVALID_ID && bag.getItem() == null &&
                string.IsNullOrEmpty( bag.getItemDes() ) && !alchemy.isEnabledAlchemy() && !alchemy.isShowAskUI(),
                "Empty alchemy inventory must have no item, description, or confirmation prompt." );
        }
    }

    IEnumerator CheckShops()
    {
        var preset = desk.Catalog.entries[0];
        yield return desk.Enter( preset, preset.suggestedLevel, "rpg" );
        if ( !Check( () => Require( desk.LaunchError == null && GameRPGManager.instance.ActiveID == 0,
                                    "Chapter zero RPG entrance failed." ) ) ) yield break;

        var shop = GameShopUI.instance;
        var list = shop.transform.Find( "shop" );
        string[] backdrops = { "Shop06_t16", "Shop08_t0", "Shop07_t0" };
        for ( int i = 0; i < backdrops.Length; i++ )
        {
            string backdrop = backdrops[i];
            GameRPGEventManager.instance.showEvent( i + 2, 0, null );
            float deadline = Time.realtimeSinceStartup + 15;
            while ( ( !list.gameObject.activeInHierarchy || shop.IsFadeIn ) && Time.realtimeSinceStartup < deadline )
                yield return null;
            if ( !Check( () => {
                Require( shop.IsShow && list.gameObject.activeInHierarchy && !shop.IsFadeIn,
                         "Shop event did not finish opening the buy/sell UI." );
                Require( HasVisibleSprite( name => name == backdrop, 1284 ), "HD backdrop missing: " + backdrop );
                Require( HasVisibleSprite( name => name == "Item_win_t10", 1128 ), "HD shop panel missing." );
                Require( HasVisibleSprite( name => name == "Item_win_t11", 1128 ), "HD inventory panel missing." );
                Require( HasVisibleSprite( name => name == "Item_win_t0", 120 ), "HD price icon missing." );
                var slot = list.Find( "slot0" ).GetComponent<GameItemUISlot>();
                var price = slot.transform.Find( "moneyImg" ).GetComponent<UnityEngine.UI.Image>();
                var salePrice = shop.transform.Find( "bag/slot0/moneyImg" ).GetComponent<UnityEngine.UI.Image>();
                Require( price.color == new Color( 1, 1, 1, slot.Enabled ? 1 : 0.2f ) &&
                    salePrice.color == new Color( 1, 1, 1, 0.2f ),
                    "Shop currency artwork must keep its original RGB on both active and dimmed rows." );
                Require( HasVisibleSprite( name => name == "selection", 920 ), "HD selection highlight missing." );
            } ) ) yield break;

            // A synthetic character may start with all personal slots filled.
            // Use the original bag switch before testing the purchase prompt.
            var bag = shop.transform.Find( "bag" ).GetComponent<GameItemBagUI>();
            if ( i == 0 && !Check( () => CheckShopEquipmentIcons( shop, list, bag ) ) ) yield break;
            if ( bag.itemFull() )
            {
                shop.moveRight();
                shop.moveRight();
                shop.moveLeft();
            }
            shop.select( 0 );
            foreach ( bool confirm in new[] { true, false } )
            {
                shop.showAskUI( confirm );
                yield return null;
                if ( !Check( () => Require( HasVisibleSprite( name => {
                    const string prefix = "Sqw_int_t";
                    return name.StartsWith( prefix, StringComparison.Ordinal ) &&
                        int.TryParse( name.Substring( prefix.Length ), out int frame ) && ( frame < 11 ) == confirm;
                }, 344 ), "HD buy/sell prompt state missing: " + confirm +
                           ", opened=" + shop.isShowAskUI() + ", full=" + bag.itemFull() +
                           ", gold=" + GameUserData.instance.Gold ) ) ) yield break;
            }
            shop.unShow();
            yield return null;
        }
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_SHOPS passed rpg=0 shops=3 panels=2 promptStates=2 selection=1" );
        Finish( true );

        bool HasVisibleSprite( Func<string, bool> matches, float width )
        {
            foreach ( var image in shop.GetComponentsInChildren<UnityEngine.UI.Image>() )
                if ( image.sprite != null && matches( image.sprite.name ) && Mathf.Approximately( image.sprite.rect.width, width ) )
                    return true;
            return false;
        }
    }

    static void CheckShopEquipmentIcons( GameShopUI shop, Transform list, GameItemBagUI bag )
    {
        const short spear = 45;
        const int spearUser = 2;
        var state = GameUserData.instance;
        var companion = state.getUnitBase( spearUser );
        short wasInTeam = companion.InTeam;
        Transform weapon = list.Find( "slot2" );
        int itemSlot = Array.IndexOf( state.ItemBag, (short)GameDefine.INVALID_ID );
        Require( itemSlot >= 0 && weapon.GetComponent<GameItemUISlot>().Item.ID == spear,
            "Chapter zero shop equipment fixture is missing." );

        companion.InTeam = 1;
        state.addItem( spear );
        try
        {
            shop.select( 2 );
            Require( shop.getSelection() == 2 && weapon.Find( "icon11" ).gameObject.activeSelf,
                "An incompatible spear should still be selectable." );

            shop.moveRight();
            shop.moveRight();
            Require( bag.UserID == spearUser && weapon.Find( "icon1" ).gameObject.activeSelf &&
                !weapon.Find( "icon11" ).gameObject.activeSelf,
                "Switching to the spear user must refresh the shop icon immediately." );

            shop.moveRight();
            Transform sharedSpear = bag.transform.Find( "slot" + itemSlot );
            Require( bag.Type == GameItemBagUI.GameItemBagUIType.Bag &&
                sharedSpear.Find( "icon1" ).gameObject.activeSelf,
                "Shared inventory must use the shop's last selected character." );

            shop.moveRight();
            Require( bag.UserID == 0 && weapon.Find( "icon11" ).gameObject.activeSelf,
                "Returning to the lead character must restore the red cross." );
            shop.moveLeft();
        }
        finally
        {
            state.removeItem( itemSlot );
            companion.InTeam = wasInTeam;
        }
        Debug.Log( "SWORDSMAN_CHAPTER_TEST_SHOP_EQUIPMENT_ICONS passed users=2 sharedBag=1" );
    }

    static void CheckAttackInfo()
    {
        var attack = GameBattleAttackUI.instance;
        attack.gameObject.SetActive( true );
        try
        {
            CheckBackground( "leftInfo", "Fig_misc_t0", 29 );
            CheckBackground( "rightInfo", "Fig_misc_t3", 27 );
        }
        finally
        {
            attack.gameObject.SetActive( false );
        }
        Debug.Log( "SWORDSMAN_HD_ATTACK_INFO_CHECK passed sprites=Fig_misc_t0,Fig_misc_t3" );

        void CheckBackground( string panel, string spriteName, int height )
        {
            var image = attack.transform.Find( panel + "/background" ).GetComponent<UnityEngine.UI.Image>();
            Sprite sprite = image.sprite;
            Require( image.isActiveAndEnabled && sprite != null && sprite.name == spriteName &&
                sprite.texture.width == 2048 && sprite.texture.height == 2048 &&
                Mathf.Approximately( sprite.pixelsPerUnit, 16f ) &&
                image.rectTransform.sizeDelta == new Vector2( 105, height ),
                "Battle attack info HD replacement or layout mismatch: " + panel );
        }
    }

    IEnumerator CheckStage( int stage )
    {
        StageResult r = new StageResult { stage = stage };
        List<string> errors = new List<string>();
        Application.LogCallback capture = ( message, trace, type ) => { if ( type == LogType.Error || type == LogType.Exception || type == LogType.Assert ) errors.Add( message ); };
        Application.logMessageReceived += capture;
        float started = Time.realtimeSinceStartup;
        SwordsmanChapterTestPreset preset = desk.Catalog.entries[stage]; r.title = preset.title; r.level = preset.suggestedLevel;
        bool reuse = Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-reuse" ) >= 0;
        string directory = SwordsmanChapterTestSession.DirectoryFor( stage, r.level );
        if ( !reuse && Directory.Exists( directory ) ) Directory.Delete( directory, true );
        r.checkpointSource = reuse ? ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-package-check" ) >= 0 ? "packaged" : "existing" ) : "original-opening";
        if ( reuse && !SwordsmanChapterTestSession.HasCheckpoint( preset, r.level ) )
        {
            r.error = "Required persisted/packaged checkpoint was missing or invalid; opening fallback is prohibited in reuse checks.";
            Application.logMessageReceived -= capture; r.runtimeErrors = errors.ToArray();
            Directory.CreateDirectory( Path.GetDirectoryName( reportPath ) ); File.WriteAllText( reportPath, JsonUtility.ToJson( r, true ) );
            Application.Quit( 1 ); yield break;
        }
        yield return desk.Enter( preset, r.level, "battle" );
        float deadline = Time.realtimeSinceStartup + 210;
        while ( !desk.CheckpointCaptured && desk.LaunchError == null && Time.realtimeSinceStartup < deadline &&
            GameUserData.instance.Stage == stage && SceneManager.GetActiveScene().buildIndex == 3 && !GameMsgBoxChooseUI.instance.IsShow )
        {
            // Batch validation may wait longer than the interactive 90-second fast-forward limit.
            Time.timeScale = 4;
            if ( GameMsgBoxUI.instance.IsShow ) GameMsgBoxUI.instance.onClick();
            yield return null;
        }
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-hd-turn-banner" ) >= 0 )
        {
            bool passed = false;
            try
            {
                Require( desk.CheckpointCaptured && SwordsmanChapterTestDesk.BattleReady(), "Turn banner checkpoint did not reach battle." );
                CheckTurnBanner();
                GameMsgBoxUI.instance.showText( -1, 0, 0, "验", null );
                passed = true;
            }
            catch ( Exception e ) { Debug.LogError( "SWORDSMAN_HD_TURN_BANNER_CHECK failed: " + e ); }
            if ( passed )
            {
                GameMsgBoxUI dialogue = GameMsgBoxUI.instance;
                var text = dialogue.transform.Find( "left" ).GetComponentInChildren<GameImageText>();
                float fadeDeadline = Time.realtimeSinceStartup + 3;
                while ( ( dialogue.IsFadeIn || !text.IsOver ) && Time.realtimeSinceStartup < fadeDeadline )
                    yield return null;
                try
                {
                    CheckDialogueArrow();
                    Require( errors.Count == 0, "Runtime errors during HD UI check." );
                }
                catch ( Exception e )
                {
                    passed = false;
                    Debug.LogError( "SWORDSMAN_HD_DIALOGUE_ARROW_CHECK failed: " + e );
                }
            }
            Application.logMessageReceived -= capture;
            Application.Quit( passed ? 0 : 1 );
            yield break;
        }
        try
        {
            r.diagnostic = Diagnostic();
            Require( desk.CheckpointCaptured && SwordsmanChapterTestDesk.BattleReady(), "Opening did not reach a controllable player turn: " + r.diagnostic );
            r.openingReachedPlayerTurn = !reuse;
            r.hasControllableUnits = GameBattleUnitManager.instance.getValidUserCount() > 0;
            Require( r.hasControllableUnits, "No controllable player unit at checkpoint." );
            r.checkpointValidated = SwordsmanChapterTestSession.HasCheckpoint( preset, r.level );
            r.pathIsolated = GameSetting.PersistentDataPath == directory && directory.StartsWith( SwordsmanChapterTestSession.RootDirectory + Path.DirectorySeparatorChar );
            Require( r.checkpointValidated && r.pathIsolated, "Invalid checkpoint/path." );
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-hd-attack-info" ) >= 0 )
                CheckAttackInfo();
            if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter-tests-hd-minimap" ) >= 0 )
            {
                GameBattleMapUI map = GameBattleMapUI.instance;
                Sprite original = map.transform.Find( "map" ).GetComponentInChildren<GameAnimation>( true ).sprites[ stage + 1 ];
                var image = map.transform.Find( "map/image" ).GetComponent<UnityEngine.UI.Image>();
                map.updateData();
                Require( SwordsmanHd4xSpriteResolver.MatchesReplacement( image.sprite, original, 2f ) && image.sprite.name == original.name &&
                    Vector2.Distance( image.rectTransform.sizeDelta, original.rect.size / original.pixelsPerUnit ) < 0.001f,
                    "Battle minimap HD replacement or layout mismatch." );
                map.clear();
                Debug.Log( "SWORDSMAN_HD_MINIMAP_CHECK passed sprite=" + image.sprite.name );
            }
            r.beforeState = State();
            GameUserData.instance.save( 0 );
            Require( new FileInfo( Path.Combine( directory, "Save0.dat" ) ).Length == 3000, "Ordinary save length mismatch." );
            GameCampScript camp = GameCampData.instance.getData( stage );
            string campPath = camp == null ? "" : "Prefab/Camp/Image/camp" + GameDefine.getString2( camp.Camp );
            if ( SwordsmanHd4xResources.LoadPrefab( campPath ) == null )
            {
                Require( stage == 34 && camp != null && camp.Camp == 34, "Unexpected missing camp prefab: " + campPath );
                r.ordinarySaveBlocker = "missing-original-camp-prefab:" + campPath;
            }
            else GameUserData.instance.load( 0 );
        }
        catch ( Exception e ) { r.error = e.Message; }
        if ( r.error == null )
        {
            deadline = Time.realtimeSinceStartup + 20;
            while ( GameSceneManager.instance.IsLoading && Time.realtimeSinceStartup < deadline ) yield return null;
            r.ordinarySaveRoundTrip = string.IsNullOrEmpty( r.ordinarySaveBlocker ) && SceneManager.GetActiveScene().buildIndex == 2 && GameUserData.instance.Stage == stage && GameUserData.instance.getUnitBase( 0 ).LV == r.level;
            yield return desk.Enter( preset, r.level, "battle" );
            deadline = Time.realtimeSinceStartup + 10;
            while ( !SwordsmanChapterTestDesk.BattleReady() && Time.realtimeSinceStartup < deadline ) yield return null;
            try
            {
                r.battleReloadReady = desk.CheckpointCaptured && SwordsmanChapterTestDesk.BattleReady() && GameUserData.instance.Stage == stage;
                r.afterState = State(); r.stateRoundTrip = r.beforeState == r.afterState;
                Require( r.battleReloadReady && GameBattleUnitManager.instance.getValidUserCount() > 0 && r.stateRoundTrip && ( r.ordinarySaveRoundTrip || !string.IsNullOrEmpty( r.ordinarySaveBlocker ) ), "Snapshot/ordinary save round trip state mismatch." );
            }
            catch ( Exception e ) { r.error = e.Message; }
        }
        Application.logMessageReceived -= capture;
        r.runtimeErrors = errors.ToArray(); r.seconds = Time.realtimeSinceStartup - started; Time.timeScale = 1;
        Directory.CreateDirectory( Path.GetDirectoryName( reportPath ) ); File.WriteAllText( reportPath, JsonUtility.ToJson( r, true ) );
        bool success = r.error == null && errors.Count == 0;
        Debug.Log( "SWORDSMAN_STAGE_CHECK " + ( success ? "passed" : "failed" ) + " stage=" + stage + " seconds=" + r.seconds + " error=" + r.error + " campBlocker=" + r.ordinarySaveBlocker );
        Application.Quit( success ? 0 : 1 );
    }

    void Finish( bool success )
    {
        result.enteredStages = inspected.ToArray(); Time.timeScale = 1;
        if ( reportPath != null ) { Directory.CreateDirectory( Path.GetDirectoryName( reportPath ) ); File.WriteAllText( reportPath, JsonUtility.ToJson( result, true ) ); }
        Debug.Log( "SWORDSMAN_CHAPTER_TESTS " + ( success ? "passed" : "failed" ) + " entered=" + inspected.Count + " checkpoint=" + result.battleCheckpointReloaded + " ordinarySave=" + result.ordinarySaveRoundTrip + " legacyCopy=" + result.realSaveCopyLoaded );
        if ( !success ) Debug.LogError( result.error );
        Application.Quit( success ? 0 : 1 );
    }
}
#endif
