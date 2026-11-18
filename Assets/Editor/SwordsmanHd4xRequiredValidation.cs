using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

// Explicit source-stage verification; never builds a Player or saves project assets.
public static class SwordsmanHd4xRequiredValidation
{
    static readonly List<string> passed = new List<string>();
    static Type Resolver { get { return typeof(SwordsmanHd4xSpriteResolver); } }
    static FieldInfo Field(string name) { return Resolver.GetField(name, BindingFlags.NonPublic | BindingFlags.Static); }
    static void Require(bool value, string name)
    {
        if (!value) throw new InvalidOperationException(name);
        passed.Add(name);
    }
    static void Reject(Action action, string name)
    {
        try { action(); }
        catch (InvalidOperationException exception)
        {
            if (!exception.Message.StartsWith("HD_REQUIRED:", StringComparison.Ordinal)) throw;
            passed.Add(name);
            return;
        }
        throw new InvalidOperationException("Required HD accepted invalid input: " + name);
    }
    static Sprite Source(string path)
    {
        Sprite result = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (result == null) result = SwordsmanHd4xBuild.CreateRetiredGeometryProbe(path);
        return result;
    }
    static void CheckHD(Sprite original, Sprite hd, string label)
    {
        Require(hd != null && hd != original && hd.pixelsPerUnit > original.pixelsPerUnit &&
            Vector2.Distance(hd.rect.size / hd.pixelsPerUnit, original.rect.size / original.pixelsPerUnit) < 0.001f &&
            Vector2.Distance(hd.pivot / hd.pixelsPerUnit, original.pivot / original.pixelsPerUnit) < 0.001f &&
            Vector4.Distance(hd.border / hd.pixelsPerUnit, original.border / original.pixelsPerUnit) < 0.001f, label);
    }
    public static void Validate()
    {
        passed.Clear();
        Sprite map = Source("Assets/Resources/texture/map/stage01/map01.png");
        CheckHD(map, SwordsmanHd4xSpriteResolver.Resolve(map, "Texture/Map/Stage01/map01"), "approved single geometry");
        Sprite ui = Source("Assets/Sprite/HelpWindow_0.asset");
        CheckHD(ui, SwordsmanHd4xSpriteResolver.Resolve(ui), "approved UI geometry and border");
        Sprite face = Source("Assets/Sprite/FACE01_t0.asset");
        CheckHD(face, SwordsmanHd4xSpriteResolver.Resolve(face), "approved portrait geometry");
        Sprite native = Source("Assets/Sprite/camp01_t0.asset");
        Require(SwordsmanHd4xSpriteResolver.Resolve(native) == native, "unselected mixed-atlas slice stays native");
        Require(SwordsmanHd4xSpriteResolver.Resolve((Sprite)null) == null, "optional native null preserved");
        Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(null, "Texture/Map/Stage01/map01"); }, "required source missing");
        Require(Resolver.Assembly.GetType("SwordsmanHd4xDebugOverlay") == null, "retired overlay absent");

        var texture = new Texture2D(16, 16);
        var fake = Sprite.Create(texture, new Rect(0, 0, 16, 16), Vector2.zero, 2);
        try
        {
            texture.name = "man1804"; fake.name = "man1804_t0";
            Require(SwordsmanHd4xSpriteResolver.Resolve(fake) == fake &&
                Mathf.Approximately(SwordsmanHd4xSpriteResolver.GetUIScale(fake, 0.73f), 0.73f), "actor and scale stay native");
            texture.name = "map01"; fake.name = "map01";
            Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(fake, "Texture/Map/Stage01/map01"); }, "cached single bad geometry rejected");
            texture.name = "map02"; fake.name = "map02";
            for (int i = 0; i < 2; i++) Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(fake, "Texture/Map/Stage02/map02"); }, "uncached single bad geometry rejected " + i);
            texture.name = "FACE01"; fake.name = "FACE01_t0";
            Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(fake); }, "portrait bad geometry rejected");
            texture.name = "HelpWindow"; fake.name = "HelpWindow_0";
            Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(fake); }, "UI bad geometry rejected");
        }
        finally { UnityEngine.Object.DestroyImmediate(fake); UnityEngine.Object.DestroyImmediate(texture); }

        var uiCache = (Dictionary<string, Sprite>)Field("uiSprites").GetValue(null);
        Sprite cachedUI = uiCache["HelpWindow/HelpWindow_0"];
        uiCache.Remove("HelpWindow/HelpWindow_0");
        try { Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(ui); }, "missing UI does not return OLD"); }
        finally { uiCache.Add("HelpWindow/HelpWindow_0", cachedUI); }
        var faceCache = (Dictionary<string, Sprite>)Field("portraitSprites").GetValue(null);
        Sprite cachedFace = faceCache["FACE01/FACE01_t0"];
        faceCache.Remove("FACE01/FACE01_t0");
        try { Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(face); }, "missing portrait does not return OLD"); }
        finally { faceCache.Add("FACE01/FACE01_t0", cachedFace); }
        Field("portraitSprites").SetValue(null, null);
        try { Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(face); }, "incomplete portrait initialization rejected"); }
        finally { Field("portraitSprites").SetValue(null, faceCache); }

        object bundle = Field("bundle").GetValue(null);
        object attempted = Field("bundleLoadAttempted").GetValue(null);
        object failure = Field("bundleFailure").GetValue(null);
        Sprite map03 = Source("Assets/Resources/texture/map/stage03/map03.png");
        try
        {
            Field("bundle").SetValue(null, null);
            Field("bundleLoadAttempted").SetValue(null, true);
            Field("bundleFailure").SetValue(null, "Injected unavailable bundle for validation");
            for (int i = 0; i < 2; i++) Reject(() => { SwordsmanHd4xSpriteResolver.Resolve(map03, "Texture/Map/Stage03/map03"); }, "unavailable bundle rejected " + i);
        }
        finally
        {
            Field("bundle").SetValue(null, bundle);
            Field("bundleLoadAttempted").SetValue(null, attempted);
            Field("bundleFailure").SetValue(null, failure);
        }
        SwordsmanSceneValidation.ValidateScenes();
        var settings = new ScriptCompilationSettings {
            group = BuildTargetGroup.Standalone,
            target = BuildTarget.StandaloneOSX,
            options = ScriptCompilationOptions.DevelopmentBuild,
            extraScriptingDefines = new[] { "SWORDSMAN_CHAPTER_TESTS", "SWORDSMAN_CHAPTER_TESTS_LARGE_WINDOW" }
        };
        string output = Path.GetFullPath("Logs/hd-required-stage1-20261116/player-scripts");
        Directory.CreateDirectory(output);
        var result = PlayerBuildInterface.CompilePlayerScripts(settings, output);
        Require(result.assemblies != null && result.assemblies.Count > 0, "Development Player scripts compiled without HD macro");
        File.WriteAllLines("Logs/hd-required-stage1-20261116/passed.txt", passed);
        Debug.Log("SWORDSMAN_HD_REQUIRED_VALIDATION passed checks=" + passed.Count + " scenes=4 playerScripts=true playerBuild=false");
    }
}
