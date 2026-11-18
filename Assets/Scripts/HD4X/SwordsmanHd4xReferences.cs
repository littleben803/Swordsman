using System;
using System.Collections.Generic;
using UnityEngine;

// Bundle references contain geometry and IDs only; no legacy Texture2D/Sprite dependency.
public static partial class SwordsmanHd4xSpriteResolver
{
    [Serializable]
    public sealed class ReferenceEntry
    {
        public string id;
        public string[] aliases;
        public string source;
        public string resourcePath;
        public string bundlePath;
        public string spriteName;
        public Vector2 worldSize;
        public Vector2 worldPivot;
        public Vector4 worldBorder;
        public float pixelsPerUnit;
    }
    [Serializable]
    public sealed class ReferenceManifest { public ReferenceEntry[] entries; }
    static Dictionary<string, ReferenceEntry> referenceEntries;
    static Dictionary<string, string> resourceReferences;
    static readonly Dictionary<string, Sprite> referenceSprites = new Dictionary<string, Sprite>(StringComparer.Ordinal);
    static readonly HashSet<Sprite> resolvedReferences = new HashSet<Sprite>();

    static void EnsureReferencesLoaded()
    {
        if (referenceEntries != null) return;
        TextAsset json = Resources.Load<TextAsset>("SwordsmanHD4X/References");
        if (json == null) throw RequiredResourceFailure("Reference manifest is missing.");
        ReferenceManifest manifest = JsonUtility.FromJson<ReferenceManifest>(json.text);
        var entries = new Dictionary<string, ReferenceEntry>(StringComparer.Ordinal);
        var resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (ReferenceEntry entry in manifest.entries)
        {
            foreach (string alias in entry.aliases) entries.Add(alias, entry);
            if (!string.IsNullOrEmpty(entry.resourcePath)) resources.Add(entry.resourcePath, entry.id);
        }
        referenceEntries = entries;
        resourceReferences = resources;
    }

    public static Sprite ResolveReference(string id)
    {
        EnsureReferencesLoaded();
        ReferenceEntry entry;
        if (!referenceEntries.TryGetValue(id, out entry)) throw RequiredResourceFailure("Unknown reference ID: " + id);
        Sprite result;
        if (referenceSprites.TryGetValue(entry.id, out result)) return result;
        EnsureBundleLoaded();
        Sprite[] sprites = bundle.LoadAssetWithSubAssets<Sprite>(entry.bundlePath);
        result = Array.Find(sprites, sprite => sprite != null && string.Equals(sprite.name, entry.spriteName, StringComparison.OrdinalIgnoreCase));
        if (result == null || !Mathf.Approximately(result.pixelsPerUnit, entry.pixelsPerUnit) ||
            Vector2.Distance(result.rect.size / result.pixelsPerUnit, entry.worldSize) > 0.001f ||
            Vector2.Distance(result.pivot / result.pixelsPerUnit, entry.worldPivot) > 0.001f ||
            Vector4.Distance(result.border / result.pixelsPerUnit, entry.worldBorder) > 0.001f)
            throw RequiredResourceFailure("Reference Sprite is missing or its geometry differs: " + id);
        result.name = entry.spriteName;
        referenceSprites.Add(entry.id, result);
        resolvedReferences.Add(result);
        loadedUISprites.Add(result);
        return result;
    }

    public static bool IsMigratedSprite(Sprite sprite)
    {
        return sprite != null && resolvedReferences.Contains(sprite);
    }

    // Validation callers may now receive an HD Sprite directly from a migrated prefab.
    public static bool MatchesReplacement(Sprite actual, Sprite original, float scale)
    {
        if (actual == null || original == null) return false;
        if (IsMigratedSprite(original)) return actual == original;
        return actual != original && actual.texture.width == original.texture.width * scale &&
            actual.texture.height == original.texture.height * scale &&
            Mathf.Approximately(actual.pixelsPerUnit, original.pixelsPerUnit * scale);
    }

    public static Sprite LoadResource(string path)
    {
        EnsureReferencesLoaded();
        string id;
        if (resourceReferences.TryGetValue(path, out id)) return ResolveReference(id);
        // Non-migrated resources keep their established Resources path and behavior.
        return Resources.Load<Sprite>(path);
    }
}
