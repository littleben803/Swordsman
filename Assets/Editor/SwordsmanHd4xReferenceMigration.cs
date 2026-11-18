using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Entry = SwordsmanHd4xSpriteResolver.ReferenceEntry;
using Manifest = SwordsmanHd4xSpriteResolver.ReferenceManifest;

// Explicit, staged migration only. Retains original images, Sprites and animation data.
public static class SwordsmanHd4xReferenceMigration
{
    const string Output = "Logs/hd-reference-stage2-20261116";
    const string ManifestPath = "Assets/Resources/SwordsmanHD4X/References.json";
    static readonly Dictionary<string, Entry> byGuid = new Dictionary<string, Entry>();
    static readonly List<string> migrated = new List<string>();
    static int properties;

    public static void Apply()
    {
        if (File.Exists("docs/hd-retired-source-geometry.json")) throw new InvalidOperationException("Retired OLD sources cannot be migrated again; use Validate.");
        Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(Output + "/migration-input.json"));
        foreach (Entry entry in manifest.entries)
        {
            Sprite original = LoadSource(entry);
            if (original == null) throw new InvalidOperationException("Missing source " + entry.source);
            Sprite hd = string.IsNullOrEmpty(entry.resourcePath)
                ? SwordsmanHd4xSpriteResolver.Resolve(original)
                : SwordsmanHd4xSpriteResolver.Resolve(original, entry.resourcePath);
            // GAMEOVER uses standalone omission atlases rather than SingleSpecs.
            if (hd == original && !string.IsNullOrEmpty(entry.resourcePath)) hd = SwordsmanHd4xSpriteResolver.Resolve(original);
            if (hd == null || hd == original) throw new InvalidOperationException("Not covered " + entry.source);
            entry.spriteName = original.name;
            entry.worldSize = original.rect.size / original.pixelsPerUnit;
            entry.worldPivot = original.pivot / original.pixelsPerUnit;
            entry.worldBorder = original.border / original.pixelsPerUnit;
            entry.pixelsPerUnit = hd.pixelsPerUnit;
            foreach (string alias in entry.aliases) byGuid.Add(alias, entry);
        }
        Directory.CreateDirectory(Path.GetDirectoryName(ManifestPath));
        File.WriteAllText(ManifestPath, JsonUtility.ToJson(manifest, true));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        // Validate the descriptor against the actual Bundle before editing any hard reference.
        foreach (Entry entry in manifest.entries)
        {
            Sprite result = SwordsmanHd4xSpriteResolver.ResolveReference(entry.id);
            if (result == null) throw new InvalidOperationException(entry.id);
        }
        string[] prefabs = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
        foreach (string path in prefabs)
        {
            // Avoid importing/reserializing unrelated prefabs.
            string text = File.ReadAllText(path);
            if (!byGuid.Keys.Any(guid => text.Contains("guid: " + guid + ","))) continue;
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (Migrate(root)) { PrefabUtility.SaveAsPrefabAsset(root, path); migrated.Add(path); }
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
        }
        foreach (string path in new[] { "Assets/Scenes/level0.unity", "Assets/Scenes/level1.unity", "Assets/Scenes/level2.unity", "Assets/Scenes/level3.unity" })
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool changed = false;
            foreach (GameObject root in scene.GetRootGameObjects()) changed |= Migrate(root);
            if (changed) { EditorSceneManager.SaveScene(scene); migrated.Add(path); }
        }
        AssetDatabase.SaveAssets();
        File.WriteAllLines(Output + "/migrated-assets.txt", migrated);
        File.WriteAllText(Output + "/migration-summary.json", "{\"entries\":" + manifest.entries.Length + ",\"properties\":" + properties + ",\"assets\":" + migrated.Count + "}");
        Debug.Log("SWORDSMAN_HD_REFERENCE_APPLY entries=" + manifest.entries.Length + " properties=" + properties + " assets=" + migrated.Count);
    }

    static bool Migrate(GameObject root)
    {
        var bindings = new List<SwordsmanHd4xReferenceBindings.Binding>();
        foreach (Component component in root.GetComponentsInChildren<Component>(true))
        {
            if (component == null || component is SwordsmanHd4xReferenceBindings) continue;
            var serialized = new SerializedObject(component);
            var iterator = serialized.GetIterator();
            var paths = new List<string>();
            while (iterator.Next(true))
            {
                if (iterator.propertyType != SerializedPropertyType.ObjectReference) continue;
                Sprite original = iterator.objectReferenceValue as Sprite;
                if (original == null) continue;
                Entry entry;
                if (!byGuid.TryGetValue(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(original)), out entry)) continue;
                // Verify runtime setter supports this field before clearing its serialized value.
                SwordsmanHd4xReferenceBindings.SetSprite(component, iterator.propertyPath, original);
                bindings.Add(new SwordsmanHd4xReferenceBindings.Binding {
                    target = component, propertyPath = iterator.propertyPath, referenceId = entry.id
                });
                paths.Add(iterator.propertyPath);
            }
            foreach (string path in paths) serialized.FindProperty(path).objectReferenceValue = null;
            if (paths.Count > 0) serialized.ApplyModifiedPropertiesWithoutUndo();
        }
        if (bindings.Count == 0) return false;
        var bridge = root.GetComponent<SwordsmanHd4xReferenceBindings>();
        if (bridge != null) throw new InvalidOperationException("Unexpected preexisting bridge: " + root.name);
        bridge = root.AddComponent<SwordsmanHd4xReferenceBindings>();
        bridge.bindings = bindings.ToArray();
        EditorUtility.SetDirty(bridge);
        properties += bindings.Count;
        return true;
    }

    public static void Validate()
    {
        Manifest manifest = JsonUtility.FromJson<Manifest>(File.ReadAllText(ManifestPath));
        SwordsmanHd4xBuild.ValidateRetiredReferenceGeometry();
        int descriptors = 0, bindings = 0;
        var retiredPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Entry entry in manifest.entries)
        {
            Sprite hd = SwordsmanHd4xSpriteResolver.ResolveReference(entry.id);
            retiredPaths.Add(entry.source);
            if (hd.name != entry.spriteName || Vector2.Distance(hd.rect.size / hd.pixelsPerUnit, entry.worldSize) > .001f ||
                Vector2.Distance(hd.pivot / hd.pixelsPerUnit, entry.worldPivot) > .001f ||
                Vector4.Distance(hd.border / hd.pixelsPerUnit, entry.worldBorder) > .001f)
                throw new InvalidOperationException("Descriptor geometry " + entry.source);
            if (!string.IsNullOrEmpty(entry.resourcePath) && SwordsmanHd4xSpriteResolver.LoadResource(entry.resourcePath) != hd)
                throw new InvalidOperationException("Resource route " + entry.resourcePath);
            descriptors++;
        }
        foreach (string path in File.ReadAllLines(Output + "/migrated-assets.txt"))
        {
            foreach (string dependency in AssetDatabase.GetDependencies(path, true))
                if (retiredPaths.Contains(dependency)) throw new InvalidOperationException("Legacy dependency remains: " + path + " -> " + dependency);
            if (path.EndsWith(".prefab"))
            {
                GameObject root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                bindings += ValidateRoot(root);
                string resourcePath = path.Substring("Assets/Resources/".Length, path.Length - "Assets/Resources/".Length - ".prefab".Length);
                GameObject loaded = SwordsmanHd4xResources.LoadPrefab(resourcePath);
                foreach (var bridge in loaded.GetComponentsInChildren<SwordsmanHd4xReferenceBindings>(true))
                    foreach (var binding in bridge.bindings)
                    {
                        var property = new SerializedObject(binding.target).FindProperty(binding.propertyPath);
                        if (property.objectReferenceValue != SwordsmanHd4xSpriteResolver.ResolveReference(binding.referenceId))
                            throw new InvalidOperationException("Prefab loader failed: " + path);
                        SwordsmanHd4xReferenceBindings.SetSprite(binding.target, binding.propertyPath, null);
                    }
            }
            else
            {
                Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
                foreach (GameObject root in scene.GetRootGameObjects()) bindings += ValidateRoot(root);
            }
        }
        Sprite native = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprite/camp01_t0.asset");
        if (SwordsmanHd4xSpriteResolver.Resolve(native) != native) throw new InvalidOperationException("Mixed atlas changed.");
        if (SwordsmanHd4xSpriteResolver.LoadResource("White") != Resources.Load<Sprite>("White")) throw new InvalidOperationException("Native path changed.");
        SwordsmanSceneValidation.ValidateScenes();
        var settings = new UnityEditor.Build.Player.ScriptCompilationSettings {
            group = BuildTargetGroup.Standalone, target = BuildTarget.StandaloneOSX,
            options = UnityEditor.Build.Player.ScriptCompilationOptions.DevelopmentBuild,
            extraScriptingDefines = new[] { "SWORDSMAN_CHAPTER_TESTS", "SWORDSMAN_CHAPTER_TESTS_LARGE_WINDOW" }
        };
        string output = Path.GetFullPath(Output + "/player-scripts"); Directory.CreateDirectory(output);
        var result = UnityEditor.Build.Player.PlayerBuildInterface.CompilePlayerScripts(settings, output);
        if (result.assemblies == null || result.assemblies.Count == 0) throw new InvalidOperationException("Player script compilation failed.");
        File.WriteAllText(Output + "/validation.json", "{\"descriptors\":" + descriptors + ",\"bindings\":" + bindings + ",\"scenes\":4,\"missingScripts\":0,\"playerScripts\":true,\"playerAppBuilt\":false}");
        Debug.Log("SWORDSMAN_HD_REFERENCE_VALIDATION passed descriptors=" + descriptors + " bindings=" + bindings + " scenes=4 playerScripts=true playerAppBuilt=false");
    }

    static Sprite LoadSource(Entry entry)
    {
        // Resources historically resolves these standalone serialized Sprites, whose
        // names can differ in case from the PNG filename (for example map21).
        if (!string.IsNullOrEmpty(entry.resourcePath))
        {
            Sprite serialized = AssetDatabase.LoadAssetAtPath<Sprite>(Path.ChangeExtension(entry.source, ".asset"));
            if (serialized != null) return serialized;
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(entry.source);
    }

    static int ValidateRoot(GameObject root)
    {
        int count = 0;
        foreach (var bridge in root.GetComponentsInChildren<SwordsmanHd4xReferenceBindings>(true))
        {
            foreach (var binding in bridge.bindings)
            {
                SerializedProperty value = new SerializedObject(binding.target).FindProperty(binding.propertyPath);
                if (value == null || value.objectReferenceValue != null) throw new InvalidOperationException("Serialized OLD binding remains: " + binding.propertyPath);
            }
            bridge.Apply();
            foreach (var binding in bridge.bindings)
            {
                SerializedProperty value = new SerializedObject(binding.target).FindProperty(binding.propertyPath);
                if (value.objectReferenceValue != SwordsmanHd4xSpriteResolver.ResolveReference(binding.referenceId)) throw new InvalidOperationException("Binding not restored.");
                count++;
            }
            // Leave project assets as serialized: no dirty asset is saved by validation.
            foreach (var binding in bridge.bindings) SwordsmanHd4xReferenceBindings.SetSprite(binding.target, binding.propertyPath, null);
        }
        return count;
    }
}
