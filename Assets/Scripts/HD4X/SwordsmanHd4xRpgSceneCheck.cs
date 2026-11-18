using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if DEVELOPMENT_BUILD
// Opt-in check of the real RPG scene and its showLayer loading path. No save writes.
public sealed class SwordsmanHd4xRpgSceneCheck : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-rpg-map-check" ) < 0 ) return;
        GameObject check = new GameObject( "SwordsmanHd4xRpgSceneCheck" );
        DontDestroyOnLoad( check );
        check.AddComponent<SwordsmanHd4xRpgSceneCheck>();
    }

    IEnumerator Start()
    {
        // Allow the title scene's normal initialization to finish before switching scenes.
        yield return null;
        yield return null;
        GameUserData.instance.setTown( 1 );
        GameUserData.instance.TownPosition = 1;
        GameSceneManager.instance.SceneType = GameSceneType.Rpg;
        SceneManager.LoadScene( (int)GameSceneType.Rpg );
        yield return null;
        yield return null;

        bool review = Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-rpg-map-review" ) >= 0;
        try
        {
            GameObject map = GameObject.Find( "RPGScene/Background/map00" );
            Sprite actual = map == null ? null : map.GetComponent<SpriteRenderer>().sprite;
            Sprite original = SwordsmanHd4xSpriteResolver.LoadResource( "Texture/RPG/map00" );
            if ( actual == null || original == null || GameRPGManager.instance.ActiveID != 1 )
                throw new InvalidOperationException( "The actual RPG scene did not load map00." );
            Vector2 originalSize = original.rect.size / original.pixelsPerUnit;
            Vector2 actualSize = actual.rect.size / actual.pixelsPerUnit;
            if ( Vector2.Distance( originalSize, actualSize ) > 0.001f ||
                Vector2.Distance( original.pivot / original.pixelsPerUnit, actual.pivot / actual.pixelsPerUnit ) > 0.001f )
                throw new InvalidOperationException( "RPG world size or pivot changed." );
            if ( ( actual == original && !SwordsmanHd4xSpriteResolver.IsMigratedSprite( original ) ) || actual.texture.width != 4096 || actual.texture.height != 8192 ||
                !Mathf.Approximately( actual.pixelsPerUnit, 4.0f ))
                throw new InvalidOperationException( "The real RPG renderer did not receive the expected map." );
            Debug.LogFormat( "SWORDSMAN_RPG_MAP_CHECK passed mode={0} scene={1} texture={2}x{3} ppu={4} world={5}x{6}",
                "static", SceneManager.GetActiveScene().buildIndex,
                actual.texture.width, actual.texture.height, actual.pixelsPerUnit, actualSize.x, actualSize.y );
            if ( review ) GameRPGSceneMovement.instance.followPos( 250.0f, -150.0f );
            else Application.Quit( 0 );
        }
        catch ( Exception exception )
        {
            Debug.LogError( "SWORDSMAN_RPG_MAP_CHECK failed: " + exception );
            Application.Quit( 1 );
        }
    }
}

// Opt-in first-chapter scene fixture. Uses actual map/animation/dialogue loading paths;
// it does not execute the story, advance turns, or call save APIs.
public sealed class SwordsmanHd4xChapterOneCheck : MonoBehaviour
{

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter01-check" ) < 0 ) return;
        GameObject check = new GameObject( "SwordsmanHd4xChapterOneCheck" );
        DontDestroyOnLoad( check );
        check.AddComponent<SwordsmanHd4xChapterOneCheck>();
    }

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        if ( !Step( () => {
            GameUserData.instance.setStage( 1 );
            GameSceneManager.instance.SceneType = GameSceneType.Battle;
            SceneManager.LoadScene( (int)GameSceneType.Battle );
        } ) ) yield break;
        yield return null;
        yield return null;
        if ( !Step( () => {
            GameBattleManager.instance.active();
            GameBattleManager.instance.showLayer( 1, false );
            GameBattleUnitManager.instance.initUnits();
            GameBattleSceneMovement.instance.moveToEvent( 8, 20 );
            GameBattleCursor.instance.unShow();
            GameBlackUI.instance.unShowBlack( 0.1f, null );
            Transform background = GameObject.Find( "BattleScene/Background" ).transform;
            SpriteRenderer map = FindRenderer( background, "map01" );
            CheckSprite( map == null ? null : map.sprite, "map01", 4096, 4096, 4.0f, new Vector2( 1024, 1024 ) );
            GameAnimation prop = background.Find( "Eanm_14" ).GetComponent<GameAnimation>();
            prop.showFrame( 0 );
            SpriteRenderer renderer = FindRenderer( prop.transform, "Eanm_14_t0" );
            CheckSprite( renderer == null ? null : renderer.sprite, "Eanm_14_t0", 256, 256, 4.0f, new Vector2( 45.5f, 30.5f ) );
            GameBattleManager.instance.unShowEffect( 3, 1 );
        } ) ) yield break;
        yield return null;
        if ( !Step( () => {
            Transform prop = GameObject.Find( "BattleScene/Background/Eanm_14" ).transform;
            if ( prop.GetComponentsInChildren<SpriteRenderer>().Length != 0 )
                throw new InvalidOperationException( "Tabletop hide did not clear its actual renderer." );
            GameBattleManager.instance.showEffect( 3, 1 );
            SpriteRenderer renderer = FindRenderer( prop, "Eanm_14_t0" );
            CheckSprite( renderer == null ? null : renderer.sprite, "Eanm_14_t0", 256, 256, 4.0f, new Vector2( 45.5f, 30.5f ) );
        } ) ) yield break;
        int[] faces = { 2, 2, 2, 2, 40, 41, 49, 49 };
        int[] expressions = { 0, 1, 2, 9, 0, 0, 0, 5 };
        string[] names = { "FACE02_t2", "FACE02_t7", "FACE02_t6", "FACE02_t11", "Face40_t1", "Face41_t0", "Face49_t0", "Face49_t1" };
        Vector2[] sizes = { new Vector2( 115.461935f, 95 ), new Vector2( 109.461935f, 96 ), new Vector2( 114, 96 ), new Vector2( 109.461935f, 98 ), new Vector2( 123, 95 ), new Vector2( 123, 96 ), new Vector2( 123, 100 ), new Vector2( 123, 100 ) };
        for ( int i = 0; i < faces.Length; i++ )
        {
            int index = i;
            if ( !Step( () => GameMsgBoxUI.instance.showText( faces[ index ], expressions[ index ], 0, "第一幕资源检查", null ) ) ) yield break;
            yield return null;
            if ( !Step( () => {
                Sprite actual = null;
                UnityEngine.UI.Image[] images = GameMsgBoxUI.instance.GetComponentsInChildren<UnityEngine.UI.Image>();
                for ( int j = 0; j < images.Length; j++ )
                    if ( images[ j ].sprite != null && images[ j ].sprite.name == names[ index ] ) actual = images[ j ].sprite;
                CheckSprite( actual, names[ index ], faces[ index ] == 2 ? 4096 : 1024,
                    faces[ index ] == 2 ? 4096 : 2048, 8.0f, sizes[ index ] );
            } ) ) yield break;
        }
        Debug.Log( "SWORDSMAN_CHAPTER01_CHECK passed mode=" + ( "static") +
            " scene=3 map=1 portraits=8 prop=1 hideShow=passed originalUnits=" + GameBattleUnitManager.instance.Units.Length + " saveWrites=0 fixture=true" );
        string[] args = Environment.GetCommandLineArgs();
        bool review = Array.IndexOf( args, "-swordsman-chapter01-review" ) >= 0;
        if ( !review ) { Application.Quit( 0 ); yield break; }
        GameMsgBoxUI.instance.showText( 2, 0, 0, "第一幕资源检查", null );
        yield return new WaitForSeconds( 0.5f );
        int screenshot = Array.IndexOf( args, "-swordsman-chapter01-screenshot" );
        if ( screenshot >= 0 && screenshot + 1 < args.Length ) ScreenCapture.CaptureScreenshot( args[ screenshot + 1 ] );
    }

    bool Step( Action action )
    {
        try { action(); return true; }
        catch ( Exception exception )
        {
            Debug.LogError( "SWORDSMAN_CHAPTER01_CHECK failed: " + exception );
            Application.Quit( 1 );
            return false;
        }
    }

    static SpriteRenderer FindRenderer( Transform root, string name )
    {
        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();
        for ( int i = 0; i < renderers.Length; i++ )
            if ( renderers[ i ].sprite != null && string.Equals( renderers[ i ].sprite.name, name, StringComparison.OrdinalIgnoreCase ) ) return renderers[ i ];
        return null;
    }

    void CheckSprite( Sprite actual, string name, int width, int height, float ppu, Vector2 worldSize )
    {
        if ( actual == null || !string.Equals( actual.name, name, StringComparison.OrdinalIgnoreCase ) ||
            Vector2.Distance( actual.rect.size / actual.pixelsPerUnit, worldSize ) > 0.001f ||
            !Mathf.Approximately( actual.pixelsPerUnit, ppu) ||
            ( ( actual.texture.width != width || actual.texture.height != height ) ) )
            throw new InvalidOperationException( "Actual scene Sprite mismatch: " + name );
    }
}

#if SWORDSMAN_CHAPTER_TESTS
// Explicit resource fixture through the existing chapter test desk; no story/save APIs.
public sealed class SwordsmanHd4xChapterTwoCheck : MonoBehaviour
{
    readonly System.Collections.Generic.List<string> errors = new System.Collections.Generic.List<string>();

    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter02-check" ) < 0 ) return;
        GameObject go = new GameObject( "SwordsmanHd4xChapterTwoCheck" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanHd4xChapterTwoCheck>();
    }

    void Capture( string message, string stack, LogType type )
    {
        if ( type == LogType.Error || type == LogType.Exception || type == LogType.Assert ) errors.Add( message );
    }

    IEnumerator Start()
    {
        Application.logMessageReceived += Capture;
        yield return null; yield return null; yield return null;
        SwordsmanChapterTestDesk desk = SwordsmanChapterTestDesk.Instance;
        if ( !Step( () => {
            if ( desk == null || desk.Catalog == null || desk.Catalog.entries.Length != 36 )
                throw new InvalidOperationException( "Missing 36-chapter test desk." );
        } ) ) yield break;
        SwordsmanChapterTestPreset preset = desk.Catalog.entries[2];
        yield return desk.Enter( preset, preset.suggestedLevel, "inspect" );
        if ( !Step( () => {
            if ( desk.LaunchError != null || SceneManager.GetActiveScene().buildIndex != 3 || GameUserData.instance.Stage != 2 )
                throw new InvalidOperationException( "Chapter two inspect entry failed: " + desk.LaunchError );
            SpriteRenderer[] renderers = GameObject.Find( "BattleScene/Background" ).GetComponentsInChildren<SpriteRenderer>();
            Sprite actual = null;
            for ( int i = 0; i < renderers.Length; i++ )
                if ( renderers[i].sprite != null && string.Equals( renderers[i].sprite.name, "map02", StringComparison.OrdinalIgnoreCase ) ) actual = renderers[i].sprite;
            CheckSprite( actual, SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Map/Stage02/map02" ), 4096, 2048, 4 );
            GameBattleTitleUI.instance.showTitle( 2, null );
        } ) ) yield break;
        yield return null;
        if ( !Step( () => {
            Sprite actual = GameBattleTitleUI.instance.transform.Find( "Image" ).GetComponent<UnityEngine.UI.Image>().sprite;
            CheckSprite( actual, SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Map/Stage02/Tanm_02_t0" ), 2048, 1024, 4 );
            GameBattleTitleUI.instance.unShow();
        } ) ) yield break;
        int[] faces = { 1, 1, 1, 1, 2, 2, 2, 38, 38 };
        int[] expressions = { 0, 2, 3, 10, 0, 3, 9, 1, 2 };
        for ( int i = 0; i < faces.Length; i++ )
        {
            int index = i;
            if ( !Step( () => GameMsgBoxUI.instance.showText( faces[index], expressions[index], 0, "第二幕静态资源检查", null ) ) ) yield break;
            yield return null;
            if ( !Step( () => {
                GameAnimation original = SwordsmanHd4xResources.LoadPrefab( "Prefab/Face/Face" + faces[index].ToString( "D2" ) ).GetComponent<GameAnimation>();
                Sprite legacy = original.sprites[original.saf1[expressions[index]].saf11[0].textureID];
                Sprite actual = null;
                UnityEngine.UI.Image[] images = GameMsgBoxUI.instance.GetComponentsInChildren<UnityEngine.UI.Image>();
                for ( int j = 0; j < images.Length; j++ )
                    if ( images[j].sprite != null && images[j].sprite.name == legacy.name ) actual = images[j].sprite;
                CheckSprite( actual, legacy, faces[index] == 38 ? 1024 : faces[index] == 1 ? 2048 : 4096, 4096, 8 );
            } ) ) yield break;
        }
        if ( !Step( () => {
            if ( errors.Count != 0 ) throw new InvalidOperationException( "Runtime errors: " + string.Join( "; ", errors.ToArray() ) );
            if ( GameBattleUnitManager.instance.Units.Length != 14 || !GameBattleTurn.instance.Pause )
                throw new InvalidOperationException( "Original unit count or paused inspect mode changed." );
        } ) ) yield break;
        Application.logMessageReceived -= Capture;
        Debug.Log( "SWORDSMAN_CHAPTER02_CHECK passed mode=" + ( "static") +
            " scene=3 map=1 title=1 portraits=9 newPortraits=3 reusedPortraits=6 originalUnits=14 saveWrites=0 fixture=true testDesk=true" );
        string[] args = Environment.GetCommandLineArgs();
        if ( Array.IndexOf( args, "-swordsman-chapter02-review" ) < 0 ) { Application.Quit( 0 ); yield break; }
        GameMsgBoxUI.instance.showText( 38, 1, 0, "第二幕静态资源检查", null );
        GameBattleSceneMovement.instance.moveToEvent( 9, 9 );
        yield return new WaitForSeconds( 0.5f );
        int screenshot = Array.IndexOf( args, "-swordsman-chapter02-screenshot" );
        if ( screenshot >= 0 && screenshot + 1 < args.Length ) ScreenCapture.CaptureScreenshot( args[screenshot + 1] );
    }

    bool Step( Action action )
    {
        try { action(); return true; }
        catch ( Exception e ) { Application.logMessageReceived -= Capture; Debug.LogError( "SWORDSMAN_CHAPTER02_CHECK failed: " + e ); Application.Quit( 1 ); return false; }
    }

    void CheckSprite( Sprite actual, Sprite original, int width, int height, float ppu )
    {
        if ( actual == null || original == null || actual.name != original.name ||
            Vector2.Distance( actual.rect.size / actual.pixelsPerUnit, original.rect.size / original.pixelsPerUnit ) > 0.001f ||
            Vector2.Distance( actual.pivot / actual.pixelsPerUnit, original.pivot / original.pixelsPerUnit ) > 0.001f ||
            ( ( actual == original && !SwordsmanHd4xSpriteResolver.IsMigratedSprite( original ) ) || actual.texture.width != width || actual.texture.height != height || !Mathf.Approximately( actual.pixelsPerUnit, ppu )) )
            throw new InvalidOperationException( "Actual chapter two sprite/geometry mismatch: " + ( original == null ? "missing original" : original.name ) );
    }
}

// Opt-in checks of chapters three/four through the existing inspect entry. No story/turn/save writes.
public sealed class SwordsmanHd4xChapterThreeFourCheck : MonoBehaviour
{
    readonly System.Collections.Generic.List<string> errors = new System.Collections.Generic.List<string>();
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-chapter0304-check" ) < 0 ) return;
        GameObject go = new GameObject( "SwordsmanHd4xChapterThreeFourCheck" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanHd4xChapterThreeFourCheck>();
    }
    void Capture( string message, string stack, LogType type )
    {
        if ( type == LogType.Error || type == LogType.Exception || type == LogType.Assert ) errors.Add( message );
    }
    IEnumerator Start()
    {
        Application.logMessageReceived += Capture;
        yield return null; yield return null; yield return null;
        string[] args = Environment.GetCommandLineArgs();
        int arg = Array.IndexOf( args, "-swordsman-chapter0304-stage" );
        int stage = arg >= 0 && arg + 1 < args.Length && args[arg + 1] == "4" ? 4 : 3;
        SwordsmanChapterTestDesk desk = SwordsmanChapterTestDesk.Instance;
        if ( !Step( () => {
            if ( desk == null || desk.Catalog == null || desk.Catalog.entries.Length != 36 )
                throw new InvalidOperationException( "Missing 36-chapter test desk." );
        } ) ) yield break;
        SwordsmanChapterTestPreset preset = desk.Catalog.entries[stage];
        yield return desk.Enter( preset, preset.suggestedLevel, "inspect" );
        if ( !Step( () => {
            if ( desk.LaunchError != null || SceneManager.GetActiveScene().buildIndex != 3 || GameUserData.instance.Stage != stage )
                throw new InvalidOperationException( "Inspect entry failed: " + desk.LaunchError );
            Transform background = GameObject.Find( "BattleScene/Background" ).transform;
            CheckSprite( FindRenderer( background, "map0" + stage ), SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Map/Stage0" + stage + "/map0" + stage ), 2 );
            if ( stage == 4 ) CheckSprite( FindRenderer( background, "map04-1" ), SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Map/Stage04/map04-1" ), 2 );
            GameBattleTitleUI.instance.showTitle( stage, null );
        } ) ) yield break;
        yield return null;
        if ( !Step( () => {
            Sprite actual = GameBattleTitleUI.instance.transform.Find( "Image" ).GetComponent<UnityEngine.UI.Image>().sprite;
            CheckSprite( actual, SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Map/Stage0" + stage + "/Tanm_0" + stage + "_t0" ), 2 );
            GameBattleTitleUI.instance.unShow();
        } ) ) yield break;
        int[] chapter3Faces = { 1, 2, 2, 4, 1, 4, 36, 4, 4, 1, 36, 1, 36, 2, 1, 2, 4 };
        int[] chapter3Expressions = { 0, 1, 0, 0, 9, 3, 7, 5, 1, 1, 3, 10, 0, 5, 4, 3, 10 };
        string[] chapter3Names = { "FACE01_t7", "FACE02_t7", "FACE02_t2", "FACE04_t4", "FACE01_t0", "FACE04_t0", "Face36_t1", "FACE04_t6", "FACE04_t1", "FACE01_t1", "Face36_t3", "FACE01_t8", "Face36_t2", "FACE02_t0", "FACE01_t2", "FACE02_t9", "FACE04_t7" };
        int[] chapter4Faces = { 25, 34, 34, 2, 1, 4, 34, 25, 2, 4, 1, 25, 25, 2, 2, 1, 2, 2, 34, 15, 45 };
        int[] chapter4Expressions = { 1, 3, 0, 0, 2, 5, 1, 2, 1, 0, 0, 0, 3, 3, 2, 4, 7, 6, 2, 0, 0 };
        string[] chapter4Names = { "Face25_t2", "Face34_t1", "Face34_t0", "FACE02_t2", "FACE01_t4", "FACE04_t6", "Face34_t3", "Face25_t0", "FACE02_t7", "FACE04_t4", "FACE01_t7", "Face25_t1", "Face25_t3", "FACE02_t9", "FACE02_t6", "FACE01_t2", "FACE02_t3", "FACE02_t5", "Face34_t2", "Face15_t0", "Face45_t0" };
        int[] faces = stage == 3 ? chapter3Faces : chapter4Faces;
        int[] expressions = stage == 3 ? chapter3Expressions : chapter4Expressions;
        string[] names = stage == 3 ? chapter3Names : chapter4Names;
        for ( int i = 0; i < faces.Length; i++ )
        {
            int index = i;
            if ( !Step( () => GameMsgBoxUI.instance.showText( faces[index], expressions[index], 0, "静态资源检查", null ) ) ) yield break;
            yield return null;
            if ( !Step( () => {
                GameAnimation original = SwordsmanHd4xResources.LoadPrefab( "Prefab/Face/Face" + faces[index].ToString( "D2" ) ).GetComponent<GameAnimation>();
                Sprite legacy = original.sprites[original.saf1[expressions[index]].saf11[0].textureID];
                if ( legacy.name != names[index] ) throw new InvalidOperationException( "Portrait expression mapping changed: " + names[index] );
                Sprite actual = null;
                UnityEngine.UI.Image[] images = GameMsgBoxUI.instance.GetComponentsInChildren<UnityEngine.UI.Image>();
                for ( int j = 0; j < images.Length; j++ )
                    if ( images[j].sprite != null && images[j].sprite.name == legacy.name ) actual = images[j].sprite;
                CheckSprite( actual, legacy, 4 );
            } ) ) yield break;
        }
        if ( !Step( () => {
            if ( errors.Count != 0 ) throw new InvalidOperationException( "Runtime errors: " + string.Join( "; ", errors.ToArray() ) );
            if ( GameBattleUnitManager.instance.Units.Length != ( stage == 3 ? 26 : 22 ) || !GameBattleTurn.instance.Pause )
                throw new InvalidOperationException( "Original unit count or paused inspect mode changed." );
        } ) ) yield break;
        Application.logMessageReceived -= Capture;
        Debug.Log( "SWORDSMAN_CHAPTER0304_CHECK passed mode=" + ( "static") +
            " stage=" + stage + " scene=3 maps=" + ( stage == 3 ? 1 : 2 ) + " title=1 portraits=" + faces.Length +
            " originalUnits=" + GameBattleUnitManager.instance.Units.Length + " saveWrites=0 fixture=true testDesk=true" );
        if ( Array.IndexOf( args, "-swordsman-chapter0304-review" ) < 0 ) { Application.Quit( 0 ); yield break; }
        GameMsgBoxUI.instance.showText( stage == 3 ? 36 : 34, 0, 0, "静态资源检查", null );
        GameBattleSceneMovement.instance.moveToEvent( stage == 3 ? 8 : 14, stage == 3 ? 12 : 12 );
        yield return new WaitForSeconds( 0.7f );
        int screenshot = Array.IndexOf( args, "-swordsman-chapter0304-screenshot" );
        if ( screenshot >= 0 && screenshot + 1 < args.Length ) ScreenCapture.CaptureScreenshot( args[screenshot + 1] );
    }
    static Sprite FindRenderer( Transform root, string name )
    {
        SpriteRenderer[] renderers = root.GetComponentsInChildren<SpriteRenderer>();
        for ( int i = 0; i < renderers.Length; i++ )
            if ( renderers[i].sprite != null && string.Equals( renderers[i].sprite.name, name, StringComparison.OrdinalIgnoreCase ) ) return renderers[i].sprite;
        return null;
    }
    bool Step( Action action )
    {
        try { action(); return true; }
        catch ( Exception e ) { Application.logMessageReceived -= Capture; Debug.LogError( "SWORDSMAN_CHAPTER0304_CHECK failed: " + e ); Application.Quit( 1 ); return false; }
    }
    void CheckSprite( Sprite actual, Sprite original, float scale )
    {
        if ( actual == null || original == null || actual.name != original.name ||
            Vector2.Distance( actual.rect.size / actual.pixelsPerUnit, original.rect.size / original.pixelsPerUnit ) > 0.001f ||
            Vector2.Distance( actual.pivot / actual.pixelsPerUnit, original.pivot / original.pixelsPerUnit ) > 0.001f ||
            Vector4.Distance( actual.border / actual.pixelsPerUnit, original.border / original.pixelsPerUnit ) > 0.001f ||
            ( !SwordsmanHd4xSpriteResolver.MatchesReplacement( actual, original, scale )) )
            throw new InvalidOperationException( "Actual chapter sprite/geometry mismatch: " + ( original == null ? "missing original" : original.name ) );
    }
}

public sealed class SwordsmanHd4xRpgTwoSceneCheck : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-rpg02-map-check" ) < 0 ) return;
        GameObject check = new GameObject( "SwordsmanHd4xRpgTwoSceneCheck" );
        DontDestroyOnLoad( check );
        check.AddComponent<SwordsmanHd4xRpgTwoSceneCheck>();
    }

    IEnumerator Start()
    {
        // Allow the title scene's normal initialization to finish before switching scenes.
        yield return null;
        yield return null;
        GameUserData.instance.setTown( 2 );
        GameUserData.instance.TownPosition = 1;
        GameSceneManager.instance.SceneType = GameSceneType.Rpg;
        SceneManager.LoadScene( (int)GameSceneType.Rpg );
        yield return null;
        yield return null;

        bool review = Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-rpg02-map-review" ) >= 0;
        try
        {
            GameObject map = GameObject.Find( "RPGScene/Background/map02" );
            Sprite actual = map == null ? null : map.GetComponent<SpriteRenderer>().sprite;
            Sprite original = SwordsmanHd4xSpriteResolver.LoadResource( "Texture/RPG/map02" );
            if ( actual == null || original == null || GameRPGManager.instance.ActiveID != 2 )
                throw new InvalidOperationException( "The actual RPG scene did not load map02." );
            Vector2 originalSize = original.rect.size / original.pixelsPerUnit;
            Vector2 actualSize = actual.rect.size / actual.pixelsPerUnit;
            if ( Vector2.Distance( originalSize, actualSize ) > 0.001f ||
                Vector2.Distance( original.pivot / original.pixelsPerUnit, actual.pivot / actual.pixelsPerUnit ) > 0.001f )
                throw new InvalidOperationException( "RPG world size or pivot changed." );
            if ( ( actual == original && !SwordsmanHd4xSpriteResolver.IsMigratedSprite( original ) ) || actual.texture.width != 4096 || actual.texture.height != 2048 ||
                !Mathf.Approximately( actual.pixelsPerUnit, 4.0f ))
                throw new InvalidOperationException( "The real RPG renderer did not receive the expected map." );
            Debug.LogFormat( "SWORDSMAN_RPG02_MAP_CHECK passed mode={0} scene={1} texture={2}x{3} ppu={4} world={5}x{6}",
                "static", SceneManager.GetActiveScene().buildIndex,
                actual.texture.width, actual.texture.height, actual.pixelsPerUnit, actualSize.x, actualSize.y );
            if ( review ) GameRPGSceneMovement.instance.followPos( 250.0f, -150.0f );
            else Application.Quit( 0 );
        }
        catch ( Exception exception )
        {
            Debug.LogError( "SWORDSMAN_RPG02_MAP_CHECK failed: " + exception );
            Application.Quit( 1 );
        }
    }
}



// Explicit Development-only resource fixture. Freezes combat UI before animation/hit callbacks,
// checks real show/showSkill/showSkillMap construction and replays original entrance frames.
public sealed class SwordsmanHd4xFixedTerrainCheck : MonoBehaviour
{
    readonly System.Collections.Generic.List<string> errors = new System.Collections.Generic.List<string>();
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-fixed-terrain-check" ) < 0 ) return;
        GameObject go = new GameObject( "SwordsmanHd4xFixedTerrainCheck" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanHd4xFixedTerrainCheck>();
    }
    void Capture( string message, string stack, LogType type )
    {
        if ( type == LogType.Error || type == LogType.Exception || type == LogType.Assert ) errors.Add( message );
    }
    IEnumerator Start()
    {
        Application.logMessageReceived += Capture;
        yield return null; yield return null; yield return null;
        string[] args = Environment.GetCommandLineArgs();
        int arg = Array.IndexOf( args, "-swordsman-fixed-terrain-stage" );
        int stage = 3;
        if ( arg >= 0 && ( arg + 1 >= args.Length || !int.TryParse( args[arg + 1], out stage ) || stage < 0 || stage > 35 ) )
        { Debug.LogError( "SWORDSMAN_FIXED_TERRAIN_CHECK invalid stage; allowed 0..35" ); Application.Quit( 1 ); yield break; }
        int screenshotArg = Array.IndexOf( args, "-swordsman-fixed-terrain-screenshots" );
        string screenshots = screenshotArg >= 0 && screenshotArg + 1 < args.Length ? args[screenshotArg + 1] : null;
        SwordsmanChapterTestDesk desk = SwordsmanChapterTestDesk.Instance;
        if ( !Step( () => {
            if ( desk == null || desk.Catalog == null || desk.Catalog.entries.Length != 36 ) throw new InvalidOperationException( "Missing test desk." );
        } ) ) yield break;
        SwordsmanChapterTestPreset preset = desk.Catalog.entries[stage];
        yield return desk.Enter( preset, preset.suggestedLevel, "inspect" );
        GameBattleUnit[] units = GameBattleUnitManager.instance.Units;
        string before = UnitState( units );
        GameBattleUnit attacker = null;
        GameSkill skill = null, mapSkill = null;
        if ( !Step( () => {
            if ( desk.LaunchError != null || GameUserData.instance.Stage != stage || !GameBattleTurn.instance.Pause ) throw new InvalidOperationException( "Inspect entry failed." );
            for ( int i = 0; i < units.Length; i++ )
            {
                if ( !units[i].IsUser ) continue;
                GameSkill a = FindSkill( units[i], GameSkillBattleType.UI );
                GameSkill b = FindSkill( units[i], GameSkillBattleType.UIMap );
                if ( a != null && b != null ) { attacker = units[i]; skill = a; mapSkill = b; break; }
            }
            if ( attacker == null ) throw new InvalidOperationException( "No original actor with both UI skill animation resources." );
        } ) ) yield break;
        GameBattleUnit target = units[0];
        for ( int i = 0; i < units.Length; i++ ) if ( units[i].IsEnemy ) { target = units[i]; break; }
        GameBattleAttackUI ui = GameBattleAttackUI.instance;
        ui.enabled = false; // Fixture-only: no fade/attack/hit callbacks or health changes.
        Time.timeScale = 0;
        string[] modes = { "attack", "skill", "map-skill" };
        for ( int mode = 0; mode < 3; mode++ )
        {
            for ( byte f = 0; f < 6; f++ )
            {
                int kind = mode; byte floor = f;
                if ( !Step( () => {
                    GameBattleAttackResultPhysical result = new GameBattleAttackResultPhysical { type = GameBattleAttackResultPhysical.Type.Normal, AddType = GameAttackAddType.None };
                    if ( kind == 0 ) ui.show( floor, attacker, target, GameBattleAttackResultSide.Left, result, false, null );
                    else if ( kind == 1 ) ui.showSkill( floor, skill, attacker, target, GameBattleAttackMapDirection.North, GameBattleAttackResultSide.Left, result, null );
                    else ui.showSkillMap( floor, mapSkill, attacker, target, GameBattleAttackMapDirection.North, GameBattleAttackResultSide.Left, new System.Collections.Generic.List<GameBattleAttackResultSkill>(), null );
                    Field( ui, "scale", 1.0f ); Field( ui, "alpha", 1.0f ); Field( ui, "startFade", false ); Field( ui, "isShow", false );
                    typeof( GameBattleAttackUI ).GetMethod( "updateStage", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).Invoke( ui, null );
                    foreach ( GameAnimation animation in ui.GetComponentsInChildren<GameAnimation>() ) animation.pause = true;
                } ) ) yield break;
                yield return null;
                if ( !Step( () => {
                    string name = ( ( stage == 0 || stage == 9 || stage == 10 || stage == 11 ) ? "FLOOR_" : "Floor_" ) + stage.ToString( "D2" ) + "_t" + floor;
                    UnityEngine.UI.Image[] images = ui.transform.Find( "floor" ).GetComponentsInChildren<UnityEngine.UI.Image>();
                    if ( images.Length != 1 || images[0].sprite == null ) throw new InvalidOperationException( "Floor image construction mismatch: " + name );
                    UnityEngine.UI.Image image = images[0];
                    Sprite original = SwordsmanHd4xSpriteResolver.LoadResource( "Texture/Stage/Stage" + stage.ToString( "D2" ) + "/Floor_" + stage.ToString( "D2" ) + "/" + name );
                    if ( image.sprite.name != name || original == null ||
                        Vector2.Distance( image.rectTransform.sizeDelta, original.rect.size / original.pixelsPerUnit ) > 0.001f ||
                        !Mathf.Approximately( SwordsmanHd4xSpriteResolver.GetUIScale( image.sprite, 0.5f ), 1.0f / image.sprite.pixelsPerUnit ) ||
                        ( !SwordsmanHd4xSpriteResolver.MatchesReplacement( image.sprite, original, 2 )) )
                        throw new InvalidOperationException( "Actual fixed terrain Sprite/UI size mismatch: " + name );
                    Debug.Log( "SWORDSMAN_FIXED_TERRAIN_FRAME mode=" + modes[kind] + " stage=" + stage + " frame=" + floor + " ppu=" + image.sprite.pixelsPerUnit + " size=" + image.rectTransform.sizeDelta + " originalActor=" + attacker.Sprite + " skill=" + ( kind == 2 ? mapSkill.ID : skill.ID ) );
                } ) ) yield break;
                if ( screenshots != null && ( mode == 0 && f == ( stage == 3 ? 2 : 0 ) || mode == 1 && f == ( stage == 3 ? 1 : 2 ) || mode == 2 && f == 5 ) )
                {
                    yield return new WaitForEndOfFrame();
                    ScreenCapture.CaptureScreenshot( System.IO.Path.Combine( screenshots, "stage" + stage + "-" + modes[mode] + "-floor" + f + ".png" ) );
                    yield return null; yield return null;
                }
            }
        }
        ui.clear(); ui.gameObject.SetActive( false ); Time.timeScale = 1;
        int entranceFrames = 0;
        if ( stage == 3 )
        {
            GameObject entrance = GameObject.Find( "BattleScene/Background/Eanm_08" );
            GameAnimation animation = entrance == null ? null : entrance.GetComponent<GameAnimation>();
            GameAnimation original = SwordsmanHd4xResources.LoadPrefab( "Prefab/Stage/Stage03/Eanm_08" ).GetComponent<GameAnimation>();
            if ( !Step( () => {
                if ( animation == null || original.saf1.Length != 70 ) throw new InvalidOperationException( "Original entrance animation missing." );
                animation.playSound = false; animation.pause = true;
                GameBattleSceneMovement.instance.moveToEvent( 7, 0 );
            } ) ) yield break;
            for ( int i = 0; i < 70; i++ )
            {
                int frame = i;
                if ( !Step( () => { animation.pause = false; animation.showFrame( frame ); animation.pause = true; } ) ) yield break;
                yield return null;
                if ( !Step( () => {
                    SpriteRenderer[] actual = animation.GetComponentsInChildren<SpriteRenderer>();
                    if ( actual.Length != original.saf1[frame].saf11.Length ) throw new InvalidOperationException( "Entrance part count mismatch at " + frame );
                    foreach ( GameAnimation.SAF11 part in original.saf1[frame].saf11 )
                    {
                        Sprite expected = original.sprites[part.textureID]; bool found = false;
                        foreach ( SpriteRenderer renderer in actual ) if ( renderer.sprite == expected ) found = true;
                        if ( !found ) throw new InvalidOperationException( "Original entrance Sprite replaced at " + frame );
                    }
                } ) ) yield break;
                entranceFrames++;
                if ( screenshots != null && ( i == 10 || i == 15 || i == 18 ) )
                {
                    yield return new WaitForEndOfFrame();
                    ScreenCapture.CaptureScreenshot( System.IO.Path.Combine( screenshots, "entrance-frame" + i + ".png" ) );
                    yield return null; yield return null;
                }
            }
        }
        if ( !Step( () => {
            if ( errors.Count != 0 ) throw new InvalidOperationException( string.Join( "; ", errors.ToArray() ) );
            if ( UnitState( units ) != before ) throw new InvalidOperationException( "Fixture changed unit HP/MP/position." );
        } ) ) yield break;
        Application.logMessageReceived -= Capture;
        Debug.Log( "SWORDSMAN_FIXED_TERRAIN_CHECK passed mode=" + ( "static") + " stage=" + stage + " entrypoints=3 variants=6 uiChecks=18 entranceFrames=" + entranceFrames + " originalActorSprites=true unitStateUnchanged=true saveWrites=0 fixture=true" );
        Application.Quit( 0 );
    }
    static void Field( GameBattleAttackUI ui, string name, object value )
    {
        typeof( GameBattleAttackUI ).GetField( name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( ui, value );
    }
    static GameSkill FindSkill( GameBattleUnit unit, GameSkillBattleType type )
    {
        for ( int i = 0; i < 1024; i++ )
        {
            GameSkill skill = GameSkillData.instance.getData( i );
            if ( skill == null || skill.BattleType != type ) continue;
            string path = "Prefab/Sprite/man" + GameDefine.getString3( unit.Sprite ) + "/" + GameDefine.getString3( unit.Sprite ) + ( skill.BattleSprite > 0 ? "-" + skill.BattleSprite : "" ) + "s";
            if ( SwordsmanHd4xResources.LoadPrefab( path ) != null ) return skill;
        }
        return null;
    }
    static string UnitState( GameBattleUnit[] units )
    {
        string value = "";
        foreach ( GameBattleUnit unit in units ) value += unit.HP + ":" + unit.MP + ":" + unit.PosX + ":" + unit.PosY + ";";
        return value;
    }
    bool Step( Action action )
    {
        try { action(); return true; }
        catch ( Exception e ) { Time.timeScale = 1; Application.logMessageReceived -= Capture; Debug.LogError( "SWORDSMAN_FIXED_TERRAIN_CHECK failed: " + e ); Application.Quit( 1 ); return false; }
    }
}

// Explicit Development fixtures for chapters five through fifteen. No normal-startup path.
public sealed class SwordsmanHd4xBatchSceneCheck : MonoBehaviour
{
    readonly System.Collections.Generic.List<string> errors = new System.Collections.Generic.List<string>();
    [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.AfterSceneLoad )]
    static void Create()
    {
        if ( Array.IndexOf( Environment.GetCommandLineArgs(), "-swordsman-batch-scene-check" ) < 0 ) return;
        GameObject go = new GameObject( "SwordsmanHd4xBatchSceneCheck" );
        DontDestroyOnLoad( go ); go.AddComponent<SwordsmanHd4xBatchSceneCheck>();
    }
    void Capture( string message, string stack, LogType type )
    { if ( type == LogType.Error || type == LogType.Exception || type == LogType.Assert ) errors.Add( message ); }
    IEnumerator Start()
    {
        Application.logMessageReceived += Capture;
        yield return null; yield return null; yield return null;
        string[] args = Environment.GetCommandLineArgs();
        int ri = Array.IndexOf( args, "-swordsman-batch-rpg-id" );
        if ( ri >= 0 )
        {
            int id = 0; int mi = Array.IndexOf( args, "-swordsman-batch-rpg-map" );
            if ( !Step( () => { if ( ri+1 >= args.Length || !int.TryParse(args[ri+1],out id) || mi < 0 || mi+1 >= args.Length ) throw new InvalidOperationException("Invalid RPG check arguments"); } ) ) yield break;
            GameUserData.instance.setTown( id ); GameUserData.instance.TownPosition = 1;
            GameSceneManager.instance.SceneType = GameSceneType.Rpg; SceneManager.LoadScene( (int)GameSceneType.Rpg );
            yield return null; yield return null; yield return null;
            if ( !Step( () => {
                string path = "texture/rpg/" + args[mi+1];
                Transform root = GameObject.Find("RPGScene/Background").transform;
                CheckSprite( FindRenderer(root,args[mi+1]), SwordsmanHd4xSpriteResolver.LoadResource(path),2 );
                if ( GameRPGManager.instance.ActiveID != id ) throw new InvalidOperationException("Wrong RPG ID");
                typeof(SwordsmanChapterTestDesk).GetMethod("SetMenu",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).Invoke(SwordsmanChapterTestDesk.Instance,new object[]{false});
                GameRPGSceneMovement.instance.followPos(250.0f,-150.0f); GameBlackUI.instance.gameObject.SetActive(false); Time.timeScale=0;
            } ) ) yield break;
            yield return CaptureScreenshot(args);
            Finish("rpg="+id+" map="+args[mi+1]); yield break;
        }
        int si = Array.IndexOf( args, "-swordsman-batch-stage" ); int stage = 5;
        if ( !Step( () => { if ( si < 0 || si+1 >= args.Length || !int.TryParse(args[si+1],out stage) || stage < 5 || stage > 35 ) throw new InvalidOperationException("Invalid stage; allowed5..35"); } ) ) yield break;
        SwordsmanChapterTestDesk desk = SwordsmanChapterTestDesk.Instance;
        if ( !Step( () => { if ( desk == null || desk.Catalog == null || desk.Catalog.entries.Length != 36 ) throw new InvalidOperationException("Missing test desk"); } ) ) yield break;
        SwordsmanChapterTestPreset preset = desk.Catalog.entries[stage];
        yield return desk.Enter(preset,preset.suggestedLevel,"inspect");
        string[] maps = null, names = null; int[] faces = null, expressions = null;
        switch(stage)
        {
            case 5:
                maps = new string[] { "texture/map/stage05/map05" };
                faces = new int[] { 2, 4, 1, 35, 35, 1, 4, 11, 11, 2, 35, 35, 45 };
                expressions = new int[] { 0, 0, 0, 8, 0, 1, 1, 0, 1, 5, 1, 2, 0 };
                names = new string[] { "FACE02_t2", "FACE04_t4", "FACE01_t7", "Face35_t4", "Face35_t3", "FACE01_t1", "FACE04_t1", "Face11_t1", "Face11_t0", "FACE02_t0", "Face35_t1", "Face35_t2", "Face45_t0" }; break;
            case 6:
                maps = new string[] { "texture/map/stage06/map06", "texture/map/stage06/Map06-1" };
                faces = new int[] { 4, 1, 4, 2, 33, 4, 2, 5, 5 };
                expressions = new int[] { 1, 0, 0, 0, 0, 2, 5, 0, 10 };
                names = new string[] { "FACE04_t1", "FACE01_t7", "FACE04_t4", "FACE02_t2", "Face33_t1", "FACE04_t5", "FACE02_t0", "Face05_t0", "Face05_t2" }; break;
            case 7:
                maps = new string[] { "texture/map/stage07/map07" };
                faces = new int[] { 1, 5, 4, 3, 3, 3, 2, 3, 4, 2, 32, 5, 3, 1, 3, 1, 2, 4, 1, 4, 2, 1, 2, 3, 1, 2, 49, 45 };
                expressions = new int[] { 0, 0, 0, 7, 0, 3, 0, 2, 5, 5, 0, 1, 1, 2, 9, 1, 2, 1, 4, 3, 8, 9, 3, 8, 3, 1, 0, 0 };
                names = new string[] { "FACE01_t7", "Face05_t0", "FACE04_t4", "FACE03_t6", "FACE03_t0", "FACE03_t1", "FACE02_t2", "FACE03_t3", "FACE04_t6", "FACE02_t0", "Face32_t1", "Face05_t1", "FACE03_t2", "FACE01_t4", "FACE03_t5", "FACE01_t1", "FACE02_t6", "FACE04_t1", "FACE01_t2", "FACE04_t0", "FACE02_t1", "FACE01_t0", "FACE02_t9", "FACE03_t4", "FACE01_t3", "FACE02_t7", "Face49_t0", "Face45_t0" }; break;
            case 8:
                maps = new string[] { "texture/map/stage08/Map08" };
                faces = new int[] { 4, 5, 3, 2, 1, 35, 3, 4, 1, 35, 2, 10, 2, 2, 10, 35, 10, 1, 2, 1, 2, 3, 4, 45 };
                expressions = new int[] { 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 3, 0, 5, 2, 1, 2, 3, 3, 11, 2, 6, 3, 5, 0 };
                names = new string[] { "FACE04_t4", "Face05_t0", "FACE03_t0", "FACE02_t2", "FACE01_t7", "Face35_t3", "FACE03_t2", "FACE04_t1", "FACE01_t1", "Face35_t1", "FACE02_t9", "Face10_t1", "FACE02_t0", "FACE02_t6", "Face10_t2", "Face35_t2", "Face10_t3", "FACE01_t3", "FACE02_t10", "FACE01_t4", "FACE02_t5", "FACE03_t1", "FACE04_t6", "Face45_t0" }; break;
            case 9:
                maps = new string[] { "texture/map/stage09/MAP09" };
                faces = new int[] { 6, 2, 33, 3, 4, 4, 2, 5, 1, 6, 3, 1, 6, 5, 2, 1, 1, 3, 2, 6, 4 };
                expressions = new int[] { 10, 2, 0, 1, 2, 1, 5, 1, 1, 2, 2, 0, 0, 0, 0, 2, 4, 0, 3, 1, 0 };
                names = new string[] { "Face06_t4", "FACE02_t6", "Face33_t1", "FACE03_t2", "FACE04_t5", "FACE04_t1", "FACE02_t0", "Face05_t1", "FACE01_t1", "Face06_t1", "FACE03_t3", "FACE01_t7", "Face06_t3", "Face05_t0", "FACE02_t2", "FACE01_t4", "FACE01_t2", "FACE03_t0", "FACE02_t9", "Face06_t2", "FACE04_t4" }; break;
            case 10:
                maps = new string[] { "texture/map/stage10/MAP10", "texture/map/stage10/Map10-1" };
                faces = new int[] { 2, 5, 4, 1, 6, 8, 6, 3, 9, 9, 1, 2, 2, 2, 2, 45 };
                expressions = new int[] { 0, 0, 0, 0, 0, 8, 1, 0, 0, 1, 2, 2, 1, 7, 3, 0 };
                names = new string[] { "FACE02_t2", "Face05_t0", "FACE04_t4", "FACE01_t7", "Face06_t3", "Face08_t5", "Face06_t2", "FACE03_t0", "Face09_t1", "Face09_t2", "FACE01_t4", "FACE02_t6", "FACE02_t7", "FACE02_t3", "FACE02_t9", "Face45_t0" }; break;
            case 11:
                maps = new string[] { "texture/map/stage11/Map11", "texture/map/stage11/Map11-1" };
                faces = new int[] { 17, 41, 17, 24, 24, 2, 1, 24, 1, 2, 1, 1, 4, 16, 45 };
                expressions = new int[] { 0, 0, 1, 2, 0, 0, 2, 3, 4, 2, 0, 1, 2, 0, 0 };
                names = new string[] { "Face17_t1", "Face41_t0", "Face17_t2", "Face24_t1", "Face24_t2", "FACE02_t2", "FACE01_t4", "Face24_t4", "FACE01_t2", "FACE02_t6", "FACE01_t7", "FACE01_t1", "FACE04_t5", "Face16_t1", "Face45_t0" }; break;
            case 12:
                maps = new string[] { "texture/map/stage12/Map12" };
                faces = new int[] { 1, 2, 5, 7, 6, 2, 7, 4, 3, 41, 7, 2, 3, 1, 2, 6, 2, 45 };
                expressions = new int[] { 0, 0, 0, 0, 0, 5, 1, 0, 0, 0, 7, 1, 2, 1, 7, 3, 4, 0 };
                names = new string[] { "FACE01_t7", "FACE02_t2", "Face05_t0", "Face07_t0", "Face06_t3", "FACE02_t0", "Face07_t2", "FACE04_t4", "FACE03_t0", "Face41_t0", "Face07_t3", "FACE02_t7", "FACE03_t3", "FACE01_t1", "FACE02_t3", "Face06_t0", "FACE02_t8", "Face45_t0" }; break;
            case 13:
                maps = new string[] { "texture/map/stage13/Map13", "texture/map/stage13/Map13-1", "texture/map/stage13/Map13-2" };
                faces = new int[] { 1, 2, 38, 37, 3, 2, 7, 2, 39, 6, 1, 4, 39, 2, 1, 34, 37, 2, 34, 2, 1, 22, 23, 2, 2, 17, 23, 22, 1, 5, 24, 17, 23, 22, 24, 1, 32, 30, 34, 4, 1, 31, 4, 3, 4, 2, 1, 3, 11, 11, 3, 2 };
                expressions = new int[] { 10, 6, 8, 8, 1, 2, 0, 5, 0, 1, 1, 1, 3, 1, 2, 0, 9, 3, 3, 4, 4, 2, 2, 0, 7, 0, 3, 3, 0, 1, 0, 1, 0, 0, 2, 8, 0, 2, 8, 2, 7, 2, 3, 0, 0, 8, 9, 3, 1, 0, 2, 9 };
                names = new string[] { "FACE01_t8", "FACE02_t5", "Face38_t0", "Face37_t0", "FACE03_t2", "FACE02_t6", "Face07_t0", "FACE02_t0", "Face39_t1", "Face06_t2", "FACE01_t1", "FACE04_t1", "Face39_t0", "FACE02_t7", "FACE01_t4", "Face34_t0", "Face37_t1", "FACE02_t9", "Face34_t1", "FACE02_t8", "FACE01_t2", "Face22_t0", "Face23_t0", "FACE02_t2", "FACE02_t3", "Face17_t1", "Face23_t1", "Face22_t2", "FACE01_t7", "Face05_t1", "Face24_t2", "Face17_t2", "Face23_t2", "Face22_t1", "Face24_t1", "FACE01_t5", "Face32_t1", "Face30_t1", "Face34_t4", "FACE04_t5", "FACE01_t6", "Face31_t0", "FACE04_t0", "FACE03_t0", "FACE04_t4", "FACE02_t1", "FACE01_t0", "FACE03_t1", "Face11_t0", "Face11_t1", "FACE03_t3", "FACE02_t11" }; break;
            case 14:
                maps = new string[] { "texture/map/stage14/Map14" };
                faces = new int[] { 1, 1, 3, 1, 1, 1, 2, 2, 1, 2, 2, 2, 8, 2, 1, 36, 1, 6, 8, 7, 5, 36, 3, 5, 36, 2, 36, 45 };
                expressions = new int[] { 10, 3, 3, 4, 7, 9, 6, 1, 1, 0, 7, 8, 8, 5, 0, 3, 2, 0, 0, 0, 0, 2, 1, 1, 0, 9, 1, 0 };
                names = new string[] { "FACE01_t8", "FACE01_t3", "FACE03_t1", "FACE01_t2", "FACE01_t6", "FACE01_t0", "FACE02_t5", "FACE02_t7", "FACE01_t1", "FACE02_t2", "FACE02_t3", "FACE02_t1", "Face08_t5", "FACE02_t0", "FACE01_t7", "Face36_t3", "FACE01_t4", "Face06_t3", "Face08_t3", "Face07_t0", "Face05_t0", "Face36_t4", "FACE03_t2", "Face05_t1", "Face36_t2", "FACE02_t11", "Face36_t0", "Face45_t0" }; break;
            case 15:
                maps = new string[] { "texture/map/stage15/Map15" };
                faces = new int[] { 8, 5, 7, 3, 1, 9, 9, 2, 2, 1, 3, 2, 6, 1, 9, 33, 9, 1, 2, 3, 1, 2, 2, 33, 33, 1, 6, 3 };
                expressions = new int[] { 9, 0, 0, 0, 0, 0, 1, 5, 0, 4, 3, 2, 3, 3, 9, 0, 8, 1, 1, 1, 2, 7, 6, 3, 1, 8, 0, 9 };
                names = new string[] { "Face08_t4", "Face05_t0", "Face07_t0", "FACE03_t0", "FACE01_t7", "Face09_t1", "Face09_t2", "FACE02_t0", "FACE02_t2", "FACE01_t2", "FACE03_t1", "FACE02_t6", "Face06_t0", "FACE01_t3", "Face09_t3", "Face33_t1", "Face09_t0", "FACE01_t1", "FACE02_t7", "FACE03_t2", "FACE01_t4", "FACE02_t3", "FACE02_t5", "Face33_t0", "Face33_t2", "FACE01_t5", "Face06_t3", "FACE03_t5" }; break;
            case 16:
                maps = new string[] { "texture/map/stage16/Map16" };
                faces = new int[] { 34, 20, 19, 19, 4, 20, 1, 1, 1, 2, 34, 34, 9, 34, 1, 2, 3, 8, 7, 5, 2, 45 };
                expressions = new int[] { 0, 1, 1, 0, 0, 0, 0, 3, 1, 1, 1, 2, 1, 3, 2, 0, 0, 0, 0, 0, 3, 0 };
                names = new string[] { "Face34_t0", "Face20_t1", "Face19_t1", "Face19_t0", "FACE04_t4", "Face20_t0", "FACE01_t7", "FACE01_t3", "FACE01_t1", "FACE02_t7", "Face34_t3", "Face34_t2", "Face09_t2", "Face34_t1", "FACE01_t4", "FACE02_t2", "FACE03_t0", "Face08_t3", "Face07_t0", "Face05_t0", "FACE02_t9", "Face45_t0" }; break;
            case 17:
                maps = new string[] { "texture/map/stage17/Map17" };
                faces = new int[] { 4, 26, 1, 4, 2, 8, 8, 7, 5, 4, 6, 4, 2, 3, 4, 1, 4, 45 };
                expressions = new int[] { 1, 8, 4, 9, 3, 0, 9, 0, 0, 3, 0, 0, 0, 2, 10, 0, 5, 0 };
                names = new string[] { "FACE04_t1", "Face26_t0", "FACE01_t2", "FACE04_t3", "FACE02_t9", "Face08_t3", "Face08_t4", "Face07_t0", "Face05_t0", "FACE04_t0", "Face06_t3", "FACE04_t4", "FACE02_t2", "FACE03_t3", "FACE04_t7", "FACE01_t7", "FACE04_t6", "Face45_t0" }; break;
            case 18:
                maps = new string[] { "texture/map/stage18/Map18", "texture/map/stage18/Map18-1" };
                faces = new int[] { 27, 4, 27, 5, 3, 1, 27, 9, 6, 3, 4, 1, 7, 8, 3, 6 };
                expressions = new int[] { 0, 1, 3, 1, 1, 1, 2, 1, 2, 2, 2, 0, 0, 9, 0, 0 };
                names = new string[] { "Face27_t0", "FACE04_t1", "Face27_t2", "Face05_t1", "FACE03_t2", "FACE01_t1", "Face27_t1", "Face09_t2", "Face06_t1", "FACE03_t3", "FACE04_t5", "FACE01_t7", "Face07_t0", "Face08_t4", "FACE03_t0", "Face06_t3" }; break;
            case 19:
                maps = new string[] { "texture/map/stage19/Map19" };
                faces = new int[] { 4, 3, 8, 5, 6, 27, 4, 27, 7, 3, 27, 1, 9, 3, 2, 1, 1, 2, 2, 1, 1, 3 };
                expressions = new int[] { 2, 3, 9, 0, 1, 2, 1, 0, 1, 1, 3, 1, 0, 0, 1, 4, 2, 0, 3, 3, 0, 9 };
                names = new string[] { "FACE04_t5", "FACE03_t1", "Face08_t4", "Face05_t0", "Face06_t2", "Face27_t1", "FACE04_t1", "Face27_t0", "Face07_t2", "FACE03_t2", "Face27_t2", "FACE01_t1", "Face09_t1", "FACE03_t0", "FACE02_t7", "FACE01_t2", "FACE01_t4", "FACE02_t2", "FACE02_t9", "FACE01_t3", "FACE01_t7", "FACE03_t5" }; break;
            case 20:
                maps = new string[] { "texture/map/stage20/Map20", "texture/map/stage20/map20-1", "texture/map/stage20/map20-2" };
                faces = new int[] { 25, 1, 2, 1, 25, 2, 1, 2, 1, 2, 3, 6, 3, 4, 2, 5, 4, 7, 41, 2, 1, 41, 2, 34, 1, 1, 3, 7, 34, 1, 3, 5, 4, 6, 34, 45 };
                expressions = new int[] { 3, 4, 3, 9, 0, 6, 7, 0, 0, 2, 0, 0, 3, 0, 8, 0, 3, 3, 0, 7, 3, 9, 1, 3, 8, 1, 1, 1, 1, 2, 2, 1, 1, 2, 2, 0 };
                names = new string[] { "Face25_t3", "FACE01_t2", "FACE02_t9", "FACE01_t0", "Face25_t1", "FACE02_t5", "FACE01_t6", "FACE02_t2", "FACE01_t7", "FACE02_t6", "FACE03_t0", "Face06_t3", "FACE03_t1", "FACE04_t4", "FACE02_t1", "Face05_t0", "FACE04_t0", "Face07_t1", "Face41_t0", "FACE02_t3", "FACE01_t3", "Face41_t1", "FACE02_t7", "Face34_t1", "FACE01_t5", "FACE01_t1", "FACE03_t2", "Face07_t2", "Face34_t3", "FACE01_t4", "FACE03_t3", "Face05_t1", "FACE04_t1", "Face06_t1", "Face34_t2", "Face45_t0" }; break;
            case 21:
                maps = new string[] { "texture/map/stage21/Map21" };
                faces = new int[] { 6, 2, 5, 1, 8, 2, 2, 3, 7, 3, 8, 4, 3, 1, 2, 34 };
                expressions = new int[] { 0, 0, 0, 0, 9, 2, 6, 2, 1, 1, 0, 0, 0, 4, 3, 1 };
                names = new string[] { "Face06_t3", "FACE02_t2", "Face05_t0", "FACE01_t7", "Face08_t4", "FACE02_t6", "FACE02_t5", "FACE03_t3", "Face07_t2", "FACE03_t2", "Face08_t3", "FACE04_t4", "FACE03_t0", "FACE01_t2", "FACE02_t9", "Face34_t3" }; break;
            case 22:
                maps = new string[] { "texture/map/stage22/Map22", "texture/map/stage22/Map22-1" };
                faces = new int[] { 7, 2, 34, 1, 3, 34, 4, 34, 2, 1, 34, 3, 4, 3, 1, 2, 1, 2, 3, 2, 5, 6, 8, 9, 1, 1, 34, 6 };
                expressions = new int[] { 0, 0, 0, 2, 2, 1, 1, 3, 1, 8, 8, 0, 0, 3, 0, 7, 4, 6, 1, 2, 0, 0, 0, 0, 3, 1, 2, 1 };
                names = new string[] { "Face07_t0", "FACE02_t2", "Face34_t0", "FACE01_t4", "FACE03_t3", "Face34_t3", "FACE04_t1", "Face34_t1", "FACE02_t7", "FACE01_t5", "Face34_t4", "FACE03_t0", "FACE04_t4", "FACE03_t1", "FACE01_t7", "FACE02_t3", "FACE01_t2", "FACE02_t5", "FACE03_t2", "FACE02_t6", "Face05_t0", "Face06_t3", "Face08_t3", "Face09_t1", "FACE01_t3", "FACE01_t1", "Face34_t2", "Face06_t2" }; break;
            case 23:
                maps = new string[] { "texture/map/stage23/Map23", "texture/map/stage23/map23-1", "texture/map/stage23/map23-2" };
                faces = new int[] { 1, 2, 3, 8, 4, 8, 4, 3, 1, 3 };
                expressions = new int[] { 0, 0, 0, 0, 0, 1, 3, 3, 1, 1 };
                names = new string[] { "FACE01_t7", "FACE02_t2", "FACE03_t0", "Face08_t3", "FACE04_t4", "Face08_t1", "FACE04_t0", "FACE03_t1", "FACE01_t1", "FACE03_t2" }; break;
            case 24:
                maps = new string[] { "texture/map/stage24/Map24", "texture/map/stage24/map24-1" };
                faces = new int[] { 1, 3, 2, 8, 4, 3, 6, 5, 7, 2, 1, 4, 3, 5, 1 };
                expressions = new int[] { 0, 0, 0, 9, 2, 1, 2, 0, 1, 1, 1, 1, 2, 1, 2 };
                names = new string[] { "FACE01_t7", "FACE03_t0", "FACE02_t2", "Face08_t4", "FACE04_t5", "FACE03_t2", "Face06_t1", "Face05_t0", "Face07_t2", "FACE02_t7", "FACE01_t1", "FACE04_t1", "FACE03_t3", "Face05_t1", "FACE01_t4" }; break;
            case 25:
                maps = new string[] { "texture/map/stage25/Map25", "texture/map/stage25/Map25-1" };
                faces = new int[] { 7, 5, 6, 3, 4, 1, 2 };
                expressions = new int[] { 0, 0, 0, 0, 0, 0, 2 };
                names = new string[] { "Face07_t0", "Face05_t0", "Face06_t3", "FACE03_t0", "FACE04_t4", "FACE01_t7", "FACE02_t6" }; break;
            case 26:
                maps = new string[] { "texture/map/stage26/Map26", "texture/map/stage26/Map26-1" };
                faces = new int[] { 1, 8, 2, 3, 5, 1, 2, 2, 1, 3, 1, 2, 7, 4, 6, 37, 35, 1, 5, 32, 1, 3, 4, 4 };
                expressions = new int[] { 10, 9, 0, 0, 0, 0, 2, 11, 4, 2, 2, 6, 1, 2, 1, 0, 0, 8, 1, 0, 1, 1, 1, 0 };
                names = new string[] { "FACE01_t8", "Face08_t4", "FACE02_t2", "FACE03_t0", "Face05_t0", "FACE01_t7", "FACE02_t6", "FACE02_t10", "FACE01_t2", "FACE03_t3", "FACE01_t4", "FACE02_t5", "Face07_t2", "FACE04_t5", "Face06_t2", "Face37_t2", "Face35_t3", "FACE01_t5", "Face05_t1", "Face32_t1", "FACE01_t1", "FACE03_t2", "FACE04_t1", "FACE04_t4" }; break;
            case 27:
                maps = new string[] { "texture/map/stage27/Map27" };
                faces = new int[] { 8, 3, 4, 11, 1, 1, 3, 1, 3 };
                expressions = new int[] { 9, 0, 0, 0, 0, 4, 3, 3, 2 };
                names = new string[] { "Face08_t4", "FACE03_t0", "FACE04_t4", "Face11_t1", "FACE01_t7", "FACE01_t2", "FACE03_t1", "FACE01_t3", "FACE03_t3" }; break;
            case 28:
                maps = new string[] { "texture/map/stage28/Map28", "texture/map/stage28/Map28-1", "texture/map/stage28/map28-2" };
                faces = new int[] { 3, 1, 3, 1, 1, 4, 6, 1, 6, 5, 1, 4, 7, 3, 1, 6, 6, 7, 32, 2, 4, 1, 3 };
                expressions = new int[] { 2, 0, 1, 4, 2, 0, 2, 1, 0, 0, 7, 5, 0, 3, 3, 3, 1, 1, 0, 10, 2, 8, 0 };
                names = new string[] { "FACE03_t3", "FACE01_t7", "FACE03_t2", "FACE01_t2", "FACE01_t4", "FACE04_t4", "Face06_t1", "FACE01_t1", "Face06_t3", "Face05_t0", "FACE01_t6", "FACE04_t6", "Face07_t0", "FACE03_t1", "FACE01_t3", "Face06_t0", "Face06_t2", "Face07_t2", "Face32_t1", "FACE02_t4", "FACE04_t5", "FACE01_t5", "FACE03_t0" }; break;
            case 29:
                maps = new string[] { "texture/map/stage29/Map29", "texture/map/stage29/map29-1" };
                faces = new int[] { 3, 1, 3, 1, 1, 2, 1, 11, 3, 11, 3, 2, 2, 1, 1, 2, 6, 6, 5, 4, 2, 6, 33, 33, 10, 2, 10, 1, 28, 4, 10, 10 };
                expressions = new int[] { 0, 0, 3, 7, 4, 6, 3, 0, 2, 1, 1, 2, 7, 1, 9, 8, 2, 0, 0, 0, 0, 1, 0, 1, 0, 3, 3, 2, 0, 2, 1, 8 };
                names = new string[] { "FACE03_t0", "FACE01_t7", "FACE03_t1", "FACE01_t6", "FACE01_t2", "FACE02_t5", "FACE01_t3", "Face11_t1", "FACE03_t3", "Face11_t0", "FACE03_t2", "FACE02_t6", "FACE02_t3", "FACE01_t1", "FACE01_t0", "FACE02_t1", "Face06_t1", "Face06_t3", "Face05_t0", "FACE04_t4", "FACE02_t2", "Face06_t2", "Face33_t1", "Face33_t2", "Face10_t1", "FACE02_t9", "Face10_t3", "FACE01_t4", "Face28_t0", "FACE04_t5", "Face10_t2", "Face10_t0" }; break;
            case 30:
                maps = new string[] { "texture/map/stage30/Map30" };
                faces = new int[] { 8, 6, 3, 1, 36, 4, 35, 5, 1, 36, 3, 1, 2, 2, 7, 5, 1, 2, 1, 32, 1, 2 };
                expressions = new int[] { 9, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 7, 6, 0, 0, 0, 3, 2, 4, 0, 2, 1 };
                names = new string[] { "Face08_t4", "Face06_t3", "FACE03_t0", "FACE01_t7", "Face36_t2", "FACE04_t4", "Face35_t1", "Face05_t1", "FACE01_t1", "Face36_t0", "FACE03_t2", "FACE01_t6", "FACE02_t5", "FACE02_t2", "Face07_t0", "Face05_t0", "FACE01_t3", "FACE02_t6", "FACE01_t2", "Face32_t1", "FACE01_t4", "FACE02_t7" }; break;
            case 31:
                maps = new string[] { "texture/map/stage31/Map31", "texture/map/stage31/Map31-1", "texture/map/stage31/map31-2" };
                faces = new int[] { 32, 3, 1, 1, 1, 2, 32, 2, 4, 3, 32, 2, 3, 1, 4, 5, 7, 1, 7, 1, 11, 24, 24, 1, 3, 2, 2, 2, 5 };
                expressions = new int[] { 0, 1, 4, 2, 1, 4, 8, 6, 0, 0, 1, 1, 4, 8, 1, 0, 1, 7, 0, 6, 0, 10, 11, 0, 3, 0, 2, 7, 1 };
                names = new string[] { "Face32_t1", "FACE03_t2", "FACE01_t2", "FACE01_t4", "FACE01_t1", "FACE02_t8", "Face32_t0", "FACE02_t5", "FACE04_t4", "FACE03_t0", "Face32_t2", "FACE02_t7", "FACE03_t7", "FACE01_t5", "FACE04_t1", "Face05_t0", "Face07_t2", "FACE01_t6", "Face07_t0", "FACE01_t9", "Face11_t1", "Face24_t6", "Face24_t7", "FACE01_t7", "FACE03_t1", "FACE02_t2", "FACE02_t6", "FACE02_t3", "Face05_t1" }; break;
            case 32:
                maps = new string[] { "texture/map/stage32/Map32" };
                faces = new int[] { 8, 3, 4, 3, 7, 6, 1 };
                expressions = new int[] { 9, 0, 2, 3, 0, 1, 0 };
                names = new string[] { "Face08_t4", "FACE03_t0", "FACE04_t5", "FACE03_t1", "Face07_t0", "Face06_t2", "FACE01_t7" }; break;
            case 33:
                maps = new string[] { "texture/map/stage33/Map33" };
                faces = new int[] { 2, 4, 3, 1, 6, 5, 4, 1, 2, 1 };
                expressions = new int[] { 2, 0, 3, 3, 1, 0, 3, 0, 1, 1 };
                names = new string[] { "FACE02_t6", "FACE04_t4", "FACE03_t1", "FACE01_t3", "Face06_t2", "Face05_t0", "FACE04_t0", "FACE01_t7", "FACE02_t7", "FACE01_t1" }; break;
            case 34:
                maps = new string[] { "texture/map/stage34/Map34", "texture/map/stage34/map34-1", "texture/map/stage34/map34-2", "texture/map/stage34/Map34-3", "texture/map/stage34/map34-4" };
                faces = new int[] { 1, 4, 2, 8, 3, 1, 5, 7, 4, 4, 6, 2, 1, 7, 5, 1, 3, 1, 1, 6, 3, 6, 3, 7, 4, 2, 11, 11, 2, 1, 2, 24, 24, 1, 2 };
                expressions = new int[] { 2, 2, 0, 9, 1, 1, 1, 1, 1, 0, 1, 2, 8, 3, 0, 3, 3, 4, 0, 2, 2, 0, 0, 0, 3, 3, 2, 0, 6, 7, 8, 10, 11, 9, 1 };
                names = new string[] { "FACE01_t4", "FACE04_t5", "FACE02_t2", "Face08_t4", "FACE03_t2", "FACE01_t1", "Face05_t1", "Face07_t2", "FACE04_t1", "FACE04_t4", "Face06_t2", "FACE02_t6", "FACE01_t5", "Face07_t1", "Face05_t0", "FACE01_t3", "FACE03_t1", "FACE01_t2", "FACE01_t7", "Face06_t1", "FACE03_t3", "Face06_t3", "FACE03_t0", "Face07_t0", "FACE04_t0", "FACE02_t9", "Face11_t2", "Face11_t1", "FACE02_t5", "FACE01_t6", "FACE02_t1", "Face24_t6", "Face24_t7", "FACE01_t0", "FACE02_t7" }; break;
            case 35:
                maps = new string[] { "texture/map/stage35/Map35" };
                faces = new int[] { 1, 51, 4, 1, 2, 4, 5, 1, 44, 43, 6, 3, 10, 3, 9, 4, 8, 7, 1, 2 };
                expressions = new int[] { 10, 0, 2, 0, 2, 1, 0, 1, 0, 0, 2, 1, 1, 3, 0, 3, 9, 7, 2, 1 };
                names = new string[] { "FACE01_t8", "Face51_t0", "FACE04_t5", "FACE01_t7", "FACE02_t6", "FACE04_t1", "Face05_t0", "FACE01_t1", "Face44_t0", "Face43_t0", "Face06_t1", "FACE03_t2", "Face10_t2", "FACE03_t1", "Face09_t1", "FACE04_t0", "Face08_t4", "Face07_t3", "FACE01_t4", "FACE02_t7" }; break;
        }
        GameBattleUnit[] units = GameBattleUnitManager.instance.Units; string before = UnitState(units);
        if ( !Step( () => {
            if ( desk.LaunchError != null || GameUserData.instance.Stage != stage || !GameBattleTurn.instance.Pause ) throw new InvalidOperationException("Inspect entry failed");
            Transform root = GameObject.Find("BattleScene/Background").transform;
            // Chapter13's forest and foreground are serialized on the later story layer.
            if ( stage == 13 ) GameBattleManager.instance.showLayer( 2, false );
            if ( stage == 20 ) { GameBattleManager.instance.showLayer( 2, false ); GameBattleManager.instance.showLayer( 3, false ); }
            if ( stage >= 22 && stage <= 24 ) GameBattleManager.instance.showLayer( 2, false );
            if ( stage == 28 ) GameBattleManager.instance.showLayer( 3, false );
            if ( stage == 29 ) GameBattleManager.instance.showLayer( 2, false );
            if ( stage == 31 ) GameBattleManager.instance.showLayer( 2, false );
            if ( stage == 34 ) { GameBattleManager.instance.showLayer( 2, false ); GameBattleManager.instance.showLayer( 3, false ); GameBattleManager.instance.showLayer( 4, false ); }
            foreach ( string path in maps ) CheckSprite(FindRenderer(root,System.IO.Path.GetFileName(path)),SwordsmanHd4xSpriteResolver.LoadResource(path),2);
            if ( stage == 13 ) GameBattleManager.instance.unShowLayer( 2 );
            if ( stage == 20 ) { GameBattleManager.instance.unShowLayer( 2 ); GameBattleManager.instance.unShowLayer( 3 ); }
            if ( stage >= 22 && stage <= 24 ) GameBattleManager.instance.unShowLayer( 2 );
            if ( stage == 28 ) GameBattleManager.instance.unShowLayer( 3 );
            if ( stage == 29 ) GameBattleManager.instance.unShowLayer( 2 );
            if ( stage == 31 ) GameBattleManager.instance.unShowLayer( 2 );
            if ( stage == 34 ) { GameBattleManager.instance.unShowLayer( 4 ); GameBattleManager.instance.unShowLayer( 3 ); GameBattleManager.instance.unShowLayer( 2 ); }
            GameBattleTitleUI.instance.showTitle(stage,null);
        } ) ) yield break;
        yield return null;
        if ( !Step( () => {
            CheckSprite(GameBattleTitleUI.instance.transform.Find("Image").GetComponent<UnityEngine.UI.Image>().sprite,SwordsmanHd4xSpriteResolver.LoadResource("texture/map/stage"+stage.ToString("D2")+"/TANM_"+stage.ToString("D2")+"_t0"),2);
            GameBattleTitleUI.instance.unShow();
        } ) ) yield break;
        for ( int i=0;i<faces.Length;i++ )
        {
            int k=i;
            if ( !Step( () => GameMsgBoxUI.instance.showText(faces[k],expressions[k],0,"静态资源检查",null) ) ) yield break;
            yield return null;
            if ( !Step( () => {
                GameAnimation original = SwordsmanHd4xResources.LoadPrefab("Prefab/Face/Face"+faces[k].ToString("D2")).GetComponent<GameAnimation>();
                Sprite legacy = original.sprites[original.saf1[expressions[k]].saf11[0].textureID];
                if ( legacy.name != names[k] ) throw new InvalidOperationException("Expression mapping changed: "+names[k]);
                Sprite actual=null;
                foreach ( UnityEngine.UI.Image im in GameMsgBoxUI.instance.GetComponentsInChildren<UnityEngine.UI.Image>() ) if ( im.sprite != null && im.sprite.name == legacy.name ) actual=im.sprite;
                CheckSprite(actual,legacy,4);
            } ) ) yield break;
        }
        string[] props = stage==5 ? new[]{"Eanm_12"} : stage==9 ? new[]{"Eanm_01","Eanm_02"} : stage==20 ? new[]{"Eanm_04"} : stage==21 ? new[]{"Eanm_03"} : stage==27 ? new[]{"Eanm_01"} : stage==30 ? new[]{"Eanm_02"} : stage==34 ? new[]{"Eanm_07"} : new string[0];
        foreach ( string prop in props )
        {
            GameObject obj=null; GameAnimation animation=null; GameAnimation original=null;
            if ( !Step( () => {
                GameObject prefab=SwordsmanHd4xResources.LoadPrefab("Prefab/Stage/Stage"+stage.ToString("D2")+"/"+prop);
                original=prefab.GetComponent<GameAnimation>();obj=Instantiate(prefab); animation=obj.GetComponent<GameAnimation>();animation.pause=true;
            } ) ) yield break;
            yield return null;
            for(int f=0;f<original.saf1.Length;f++)
            {
                int frame=f;
                if(!Step(()=>{animation.pause=false;animation.showFrame(frame);animation.pause=true;}))yield break;
                yield return null;
                if(!Step(()=>{Sprite legacy=original.sprites[original.saf1[frame].saf11[0].textureID];CheckSprite(FindRenderer(obj.transform,legacy.name),legacy,2);}))yield break;
            }
            Destroy(obj);
        }
        if(!Step(()=>{if(before!=UnitState(units)||!GameBattleTurn.instance.Pause)throw new InvalidOperationException("Original units changed");}))yield break;
        yield return CaptureMapTour(args,stage);
        yield return CaptureScreenshot(args);
        Finish("stage="+stage+" maps="+maps.Length+" title=1 portraits="+faces.Length+" props="+props.Length+" originalUnits="+units.Length);
    }
    // Opt-in Development capture only; source-pixel coordinates keep the normal camera viewport.
    IEnumerator CaptureMapTour(string[] args,int stage)
    {
        int arg=Array.IndexOf(args,"-swordsman-stage09-map-tour");
        if(arg<0)yield break;
        if(!Step(()=>{if(stage!=9||arg+1>=args.Length)throw new InvalidOperationException("Map tour requires stage9 and an output directory");}))yield break;
        string folder=args[arg+1];System.IO.Directory.CreateDirectory(folder);
        SpriteRenderer map=null;
        if(!Step(()=>{
            foreach(SpriteRenderer sr in GameObject.Find("BattleScene/Background").GetComponentsInChildren<SpriteRenderer>(true))
                if(sr.sprite!=null&&sr.sprite.name=="MAP09")map=sr;
            if(map==null)throw new InvalidOperationException("Missing actual MAP09 renderer");
            GameMsgBoxUI.instance.unShow();GameBlackUI.instance.gameObject.SetActive(false);Time.timeScale=0;
        }))yield break;
        Vector3 saved=GameCameraManager.instance.mainCamera.transform.position;
        int[] xs={320,882};int[] ys={200,545,890,1235,1580};
        foreach(int y in ys)foreach(int x in xs)
        {
            Vector3 point=map.transform.TransformPoint(new Vector3(x/2.0f,-y/2.0f,0));
            GameCameraManager.instance.mainCamera.transform.position=new Vector3(point.x,point.y,saved.z);
            yield return null;yield return new WaitForEndOfFrame();
            string file=System.IO.Path.Combine(folder,"map09-x"+x+"-y"+y+".png");ScreenCapture.CaptureScreenshot(file);
            yield return null;yield return null;
            Debug.Log("SWORDSMAN_MAP09_TOUR sourceCenter="+x+","+y+" file="+file+" viewport="+Screen.width+"x"+Screen.height);
        }
        GameCameraManager.instance.mainCamera.transform.position=saved;
    }
    IEnumerator CaptureScreenshot(string[] args)
    {
        int arg=Array.IndexOf(args,"-swordsman-batch-screenshot");
        if(arg<0||arg+1>=args.Length)yield break;
        yield return new WaitForEndOfFrame();ScreenCapture.CaptureScreenshot(args[arg+1]);yield return null;yield return null;
    }
    void Finish(string scope)
    {
        if(!Step(()=>{if(errors.Count!=0)throw new InvalidOperationException(string.Join(";",errors.ToArray()));}))return;
        Application.logMessageReceived-=Capture;Debug.Log("SWORDSMAN_BATCH_SCENE_CHECK passed mode="+("static")+" "+scope+" saveWrites=0 fixture=true");Application.Quit(0);
    }
    static Sprite FindRenderer(Transform root,string name)
    {
        foreach(SpriteRenderer sr in root.GetComponentsInChildren<SpriteRenderer>(true))if(sr.sprite!=null&&string.Equals(sr.sprite.name,name,StringComparison.OrdinalIgnoreCase))return sr.sprite;
        return null;
    }
    void CheckSprite(Sprite actual,Sprite original,float scale)
    {
        if(actual==null||original==null||actual.name!=original.name||Vector2.Distance(actual.rect.size/actual.pixelsPerUnit,original.rect.size/original.pixelsPerUnit)>.001f||Vector2.Distance(actual.pivot/actual.pixelsPerUnit,original.pivot/original.pixelsPerUnit)>.001f||Vector4.Distance(actual.border/actual.pixelsPerUnit,original.border/original.pixelsPerUnit)>.001f||(!SwordsmanHd4xSpriteResolver.MatchesReplacement(actual,original,scale)))throw new InvalidOperationException("Actual batch Sprite geometry/replacement mismatch: "+(original==null?"missing":original.name));
    }
    static string UnitState(GameBattleUnit[] units)
    {string s="";foreach(GameBattleUnit u in units)s+=u.Sprite+":"+u.HP+":"+u.MP+":"+u.PosX+":"+u.PosY+";";return s;}
    bool Step(Action action)
    {try{action();return true;}catch(Exception e){Application.logMessageReceived-=Capture;Debug.LogError("SWORDSMAN_BATCH_SCENE_CHECK failed: "+e);Application.Quit(1);return false;}}
}

#endif

#endif
