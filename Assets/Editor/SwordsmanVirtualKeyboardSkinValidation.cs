using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Explicit editor-only validation. Does not build a Player, save a scene or write preferences.
public static class SwordsmanVirtualKeyboardSkinValidation
{
    static string DirectoryPath
    {
        get
        {
            var args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf( args, "-swordsman-keyboard-skin-output" );
            return index >= 0 && index + 1 < args.Length ? args[index + 1] : "Logs/virtual-keyboard-bronze-20261114";
        }
    }
    const string Running = "Swordsman.VirtualKeyboardSkin.Validation";
    static readonly string[] Names = { "DPad", "A", "B" };
    static IEnumerator routine;
    static double deadline;
    static bool oldEnabled;
    static float oldScale;
    static bool capturedSettings;
    [Serializable] sealed class Result
    {
        public string environment = "Unity Editor Play Mode; no Player build or native mouse automation";
        public List<string> passed = new List<string>();
        public string error = "";
    }
    static Result result;
    static readonly Dictionary<string, Rect> alphaBounds = new Dictionary<string, Rect>();

    static Rect VisibleBounds( Image image )
    {
        string path = "Assets/Resources/VirtualKeyboard/Bronze/" + image.sprite.name + ".png";
        Rect normalized;
        if ( !alphaBounds.TryGetValue( path, out normalized ) )
        {
            var texture = new Texture2D( 2, 2 ); texture.LoadImage( File.ReadAllBytes( path ) );
            var pixels = texture.GetPixels32();
            int minX = texture.width, minY = texture.height, maxX = -1, maxY = -1;
            for ( int y = 0; y < texture.height; y++ )
                for ( int x = 0; x < texture.width; x++ )
                    if ( pixels[y * texture.width + x].a > 0 )
                    {
                        minX = Math.Min( minX, x ); minY = Math.Min( minY, y );
                        maxX = Math.Max( maxX, x ); maxY = Math.Max( maxY, y );
                    }
            normalized = Rect.MinMaxRect( ( minX + 0.5f ) / texture.width, ( minY + 0.5f ) / texture.height,
                ( maxX + 0.5f ) / texture.width, ( maxY + 0.5f ) / texture.height );
            alphaBounds[path] = normalized; UnityEngine.Object.DestroyImmediate( texture );
        }
        var rect = image.rectTransform;
        var canvas = image.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
        Vector3 low = canvas.InverseTransformPoint( rect.TransformPoint( new Vector3(
            rect.rect.xMin + normalized.xMin * rect.rect.width, rect.rect.yMin + normalized.yMin * rect.rect.height ) ) );
        Vector3 high = canvas.InverseTransformPoint( rect.TransformPoint( new Vector3(
            rect.rect.xMin + normalized.xMax * rect.rect.width, rect.rect.yMin + normalized.yMax * rect.rect.height ) ) );
        return Rect.MinMaxRect( low.x, low.y, high.x, high.y );
    }

    static void CheckPadding( ETCDPad pad, ETCButton a, ETCButton b, string scale )
    {
        Rect p = VisibleBounds( pad.GetComponent<Image>() );
        Rect ab = VisibleBounds( a.GetComponent<Image>() ), bb = VisibleBounds( b.GetComponent<Image>() );
        Rect canvas = pad.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>().rect;
        Check( Mathf.Abs( ( p.xMin - canvas.xMin ) - ( canvas.xMax - bb.xMax ) ) < 0.05f,
            "DPad left padding matches B right at " + scale );
        Check( Mathf.Abs( p.yMin - ab.yMin ) < 0.05f, "DPad bottom padding matches A bottom at " + scale );
        Check( canvas.Contains( p.min ) && canvas.Contains( p.max ), "DPad visible artwork within canvas at " + scale );
    }

    public static void ConfigureAndValidate()
    {
        AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );
        foreach ( string name in Names )
        {
            string path = "Assets/Resources/VirtualKeyboard/Bronze/" + name + ".png";
            var importer = (TextureImporter)AssetImporter.GetAtPath( path );
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings( settings );
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings( settings );
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.filterMode = FilterMode.Bilinear;
            var platform = importer.GetPlatformTextureSettings( "Standalone" );
            platform.overridden = true; platform.maxTextureSize = 2048;
            platform.format = TextureImporterFormat.RGBA32;
            platform.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SetPlatformTextureSettings( platform );
            importer.SaveAndReimport();
            var sprite = Resources.Load<Sprite>( "VirtualKeyboard/Bronze/" + name );
            if ( sprite == null || sprite.texture.format != TextureFormat.RGBA32 || sprite.rect.width != sprite.rect.height )
                throw new Exception( "Invalid imported skin: " + name );
            var texture = new Texture2D( 2, 2 );
            texture.LoadImage( File.ReadAllBytes( path ) );
            int clear = 0, translucent = 0, metal = 0;
            foreach ( Color32 pixel in texture.GetPixels32() )
            {
                if ( pixel.a == 0 ) clear++;
                else if ( pixel.a <= 160 ) translucent++;
                else if ( pixel.a >= 220 ) metal++;
            }
            UnityEngine.Object.DestroyImmediate( texture );
            if ( clear == 0 || translucent == 0 || metal == 0 ) throw new Exception( "Alpha separation failed: " + name );
            Debug.Log( "SWORDSMAN_KEYBOARD_SKIN_ASSET " + name + " clear=" + clear + " translucent=" + translucent + " metal=" + metal );
        }
        SwordsmanSceneValidation.ValidateScenes();
        Debug.Log( "SWORDSMAN_KEYBOARD_SKIN_IMPORT_COMPLETE" );
    }

    public static void RunPlayMode()
    {
        ConfigureAndValidate();
        EditorSceneManager.OpenScene( "Assets/Scenes/level0.unity", OpenSceneMode.Single );
        ConfigureGameView( true );
        SessionState.SetBool( Running, true );
        EditorApplication.EnterPlaymode();
    }

    [InitializeOnLoadMethod]
    static void Resume()
    {
        if ( !SessionState.GetBool( Running, false ) ) return;
        result = new Result(); deadline = EditorApplication.timeSinceStartup + 100;
        routine = CheckPlayMode();
        EditorApplication.update += Tick;
    }

    static void Tick()
    {
        try
        {
            if ( EditorApplication.timeSinceStartup > deadline ) throw new Exception( "Editor skin check timed out" );
            if ( !EditorApplication.isPlaying ) return;
            if ( !routine.MoveNext() ) Finish( null );
        }
        catch ( Exception exception ) { Finish( exception.ToString() ); }
    }

    static void Check( bool condition, string name )
    {
        if ( !condition ) throw new Exception( name );
        result.passed.Add( name );
    }

    // A temporary Game View preset, removed again before exit; no ProjectSettings changes.
    static void ConfigureGameView( bool configure )
    {
        var assembly = typeof(EditorWindow).Assembly;
        var sizesType = assembly.GetType( "UnityEditor.GameViewSizes" );
        var sizes = sizesType.GetProperty( "instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy ).GetValue( null, null );
        var groupType = assembly.GetType( "UnityEditor.GameViewSizeGroupType" );
        var group = sizesType.GetMethod( "GetGroup" ).Invoke( sizes, new[] { Enum.Parse( groupType, "Standalone" ) } );
        var groupClass = group.GetType();
        var view = EditorWindow.GetWindow( assembly.GetType( "UnityEditor.GameView" ) );
        var selected = view.GetType().GetProperty( "selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
        if ( configure )
        {
            SessionState.SetInt( Running + ".previousSize", (int)selected.GetValue( view, null ) );
            int index = (int)groupClass.GetMethod( "GetTotalCount" ).Invoke( group, null );
            var size = Activator.CreateInstance( assembly.GetType( "UnityEditor.GameViewSize" ),
                new object[] { Enum.Parse( assembly.GetType( "UnityEditor.GameViewSizeType" ), "FixedResolution" ), 1280, 800, "Swordsman Skin Validation" } );
            groupClass.GetMethod( "AddCustomSize" ).Invoke( group, new[] { size } );
            SessionState.SetInt( Running + ".temporarySize", index );
            selected.SetValue( view, index, null ); view.Show();
        }
        else
        {
            selected.SetValue( view, SessionState.GetInt( Running + ".previousSize", 0 ), null );
            groupClass.GetMethod( "RemoveCustomSize" ).Invoke( group, new object[] { SessionState.GetInt( Running + ".temporarySize", -1 ) } );
        }
    }

    static PointerEventData Pointer( RectTransform rect, Vector2 direction )
    {
        Vector3[] corners = new Vector3[4]; rect.GetWorldCorners( corners );
        Vector2 a = RectTransformUtility.WorldToScreenPoint( null, corners[0] );
        Vector2 b = RectTransformUtility.WorldToScreenPoint( null, corners[2] );
        return new PointerEventData( EventSystem.current ) { pointerId = -1, position = ( a + b ) / 2 + Vector2.Scale( b - a, direction ) * 0.35f };
    }

    static IEnumerator CheckPlayMode()
    {
        while ( GameTitleUI.instance == null || !GameTitleUI.instance.IsShow || GameTitleUI.instance.IsFadeIn )
        {
            if ( GameMovieManager.instance != null && GameMovieManager.instance.IsPlaying ) GameMovieManager.instance.stopMovie();
            yield return null;
        }
        Application.runInBackground = true;
        oldEnabled = GameSetting.instance.virtualKeyboardEnabled;
        oldScale = GameSetting.instance.touchScale; capturedSettings = true;
        GameSetting.instance.virtualKeyboardEnabled = true; GameSetting.instance.touchScale = 1;
        GameTouchCenterUI.instance.showUI(); GameHelpUI.instance.show();
        double ready = EditorApplication.timeSinceStartup + 1;
        while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        GameTitleInput.instance.enabled = false;
        var pad = GameTouchLeftUI.instance.GetComponentInChildren<ETCDPad>();
        Check( Handles.GetMainGameViewSize() == new Vector2( 1280, 800 ), "native 1280x800 Game View" );
        var a = GameTouchRightUI.instance.transform.Find( "ButtonA" ).GetComponent<ETCButton>();
        var b = GameTouchRightUI.instance.transform.Find( "ButtonB" ).GetComponent<ETCButton>();
        Check( pad.GetComponent<RectTransform>().sizeDelta == new Vector2( 230, 230 ) && pad.buttonSizeCoef == 3 &&
            pad.dPadAxisCount == ETCBase.DPadAxis.Two_Axis, "original 230x230 DPad and dead zone preserved" );
        Check( Vector2.Distance( pad.GetComponent<RectTransform>().anchoredPosition, new Vector2( 101.44f, 99.39f ) ) < 0.01f,
            "DPad moved to matched corner padding" );
        Check( a.GetComponent<RectTransform>().sizeDelta == new Vector2( 80, 80 ) &&
            b.GetComponent<RectTransform>().sizeDelta == new Vector2( 80, 80 ), "user-approved A/B 80x80 hit sizes" );
        Check( a.GetComponent<RectTransform>().anchoredPosition == new Vector2( -111, 45 ) &&
            b.GetComponent<RectTransform>().anchoredPosition == new Vector2( -45, 111 ), "compact A lower-left B upper-right and bottom/right inset" );
        Check( pad.GetComponent<Image>().sprite.name == "DPad" && a.normalSprite.name == "A" && b.normalSprite.name == "B",
            "all approved skins loaded" );
        CheckPadding( pad, a, b, "100%" );
        foreach ( var control in new[] { a, b } )
            Check( control.GetComponentInChildren<Text>().text == ( control == a ? "Ent" : "Esc" ) && !control.GetComponent<Image>().enabled && control.normalSprite == control.pressedSprite,
                control.name + " correct keyboard label without baked legacy letter or red pressed sprite" );
        var updatePad = typeof(ETCDPad).GetMethod( "UpdateDPad", BindingFlags.Instance | BindingFlags.NonPublic );
        var directions = new[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right, Vector2.zero };
        var codes = new[] { GameInputCode.Up, GameInputCode.Down, GameInputCode.Left, GameInputCode.Right };
        foreach ( Vector2 direction in directions )
        {
            var pointer = Pointer( pad.GetComponent<RectTransform>(), direction );
            ExecuteEvents.Execute( pad.gameObject, pointer, ExecuteEvents.pointerDownHandler );
            updatePad.Invoke( pad, null );
            for ( int i = 0; i < codes.Length; i++ )
                Check( GameInputManager.getKeyDown( codes[i] ) == ( direction != Vector2.zero && direction == directions[i] ),
                    "pointer " + direction + " routes " + codes[i] );
            ExecuteEvents.Execute( pad.gameObject, pointer, ExecuteEvents.pointerUpHandler );
            Check( pad.axisX.axisValue == 0 && pad.axisY.axisValue == 0, "direction release " + direction );
        }
        foreach ( var control in new[] { a, b } )
        {
            var pointer = Pointer( control.GetComponent<RectTransform>(), Vector2.zero );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerDownHandler );
            Check( GameInputManager.getKeyDown( control == a ? GameInputCode.Confirm : GameInputCode.Cancel ), control.name + " original input callback" );
            yield return null;
            Check( control.GetComponent<Image>().sprite == control.normalSprite, control.name + " pressed skin retained" );
            ExecuteEvents.Execute( control.gameObject, pointer, ExecuteEvents.pointerUpHandler );
        }
        ScreenCapture.CaptureScreenshot( Path.GetFullPath( DirectoryPath + "/editor-settings.png" ) );
        ready = EditorApplication.timeSinceStartup + 0.7;
        while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        GameSetting.instance.touchScale = 1.5f;
        GameTouchLeftUI.instance.showUI(); GameTouchRightUI.instance.showUI();
        ready = EditorApplication.timeSinceStartup + 0.7;
        while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        Check( Mathf.Approximately( GameTouchRightUI.instance.transform.localScale.x, 1.5f ), "original scaling to 150 percent" );
        Canvas.ForceUpdateCanvases();
        CheckPadding( pad, a, b, "150%" );
        foreach ( var control in new[] { a, b } )
        {
            var corners = new Vector3[4]; control.GetComponent<RectTransform>().GetWorldCorners( corners );
            var canvas = control.GetComponentInParent<Canvas>().rootCanvas.GetComponent<RectTransform>();
            foreach ( var corner in corners )
                Check( canvas.rect.Contains( canvas.InverseTransformPoint( corner ) ), control.name + " scaled corner within canvas" );
        }
        ScreenCapture.CaptureScreenshot( Path.GetFullPath( DirectoryPath + "/editor-settings-150.png" ) );
        ready = EditorApplication.timeSinceStartup + 0.7;
        while ( EditorApplication.timeSinceStartup < ready ) yield return null;
        GameSetting.instance.virtualKeyboardEnabled = false; GameTouchCenterUI.instance.showUI(); GameHelpUI.instance.select( GameHelpUI.KeyboardSelection );
        Check( !GameTouchLeftUI.instance.IsShow && !GameTouchRightUI.instance.IsShow &&
            !GameHelpUI.instance.transform.Find( "setting20" ).GetComponent<Button>().interactable &&
            !GameHelpUI.instance.transform.Find( "setting21" ).GetComponent<Button>().interactable,
            "off hides skin and disables size controls" );
        GameSetting.instance.virtualKeyboardEnabled = true; GameTouchCenterUI.instance.showUI(); GameHelpUI.instance.select( GameHelpUI.KeyboardSelection );
        Check( GameHelpUI.instance.transform.Find( "setting20" ).GetComponent<Button>().interactable &&
            GameHelpUI.instance.transform.Find( "setting21" ).GetComponent<Button>().interactable, "on restores size controls" );
    }

    static void Finish( string error )
    {
        EditorApplication.update -= Tick; SessionState.SetBool( Running, false );
        if ( capturedSettings && GameSetting.instance != null )
        {
            GameSetting.instance.virtualKeyboardEnabled = oldEnabled; GameSetting.instance.touchScale = oldScale;
        }
        result.error = error ?? "";
        try { ConfigureGameView( false ); }
        catch ( Exception exception ) { result.error += " Game View restore: " + exception.Message; }
        Directory.CreateDirectory( DirectoryPath );
        File.WriteAllText( DirectoryPath + "/editor-playmode.json", JsonUtility.ToJson( result, true ) );
        Debug.Log( "SWORDSMAN_KEYBOARD_SKIN_PLAYMODE " + ( error ?? "passed" ) );
        EditorApplication.Exit( result.error.Length == 0 ? 0 : 1 );
    }
}
