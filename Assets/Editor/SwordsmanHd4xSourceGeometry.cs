using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Pixel-free provenance and geometry for the explicitly retired source textures.
public static partial class SwordsmanHd4xBuild
{
    const string SourceGeometryPath = "docs/hd-retired-source-geometry.json";
    const string SourceGeometrySha256 = "34609af73c2c6c3bec55775fd0c70268b782cd83d4d4c2389b6882aaa815dd52";
    [Serializable]
    sealed class SourceSprite
    {
        public string name, path, dependencyHash;
        public Rect rect;
        public Vector2 pivot;
        public Vector4 border;
        public float pixelsPerUnit;
    }
    [Serializable]
    sealed class SourceTexture
    {
        public string path, hash, metaHash, dependencyHash;
        public SourceSprite direct;
        public SourceSprite[] sprites;
    }
    [Serializable] sealed class SourceGeometry { public SourceTexture[] textures; }
    static Dictionary<string, SourceTexture> frozenSources;

    static SourceSprite DescribeSourceSprite(Sprite sprite)
    {
        if (sprite == null) return null;
        string path = AssetDatabase.GetAssetPath(sprite);
        return new SourceSprite { name = sprite.name, path = path,
            dependencyHash = AssetDatabase.GetAssetDependencyHash(path).ToString(),
            rect = sprite.rect, pivot = sprite.pivot, border = sprite.border, pixelsPerUnit = sprite.pixelsPerUnit };
    }
    static bool TryFrozenSource(string path, out SourceTexture texture)
    {
        if (frozenSources == null)
        {
            ValidateHash(GetAbsoluteProjectPath(SourceGeometryPath), SourceGeometrySha256, "Retired source geometry");
            SourceGeometry geometry = JsonUtility.FromJson<SourceGeometry>(File.ReadAllText(SourceGeometryPath));
            if (geometry.textures.Length != 611) throw new InvalidOperationException("Retired geometry scope mismatch.");
            // JsonUtility writes null nested classes as empty records.
            foreach (var source in geometry.textures)
                if (source.direct != null && string.IsNullOrEmpty(source.direct.path)) source.direct = null;
            frozenSources = geometry.textures.ToDictionary(x => x.path, StringComparer.OrdinalIgnoreCase);
        }
        return frozenSources.TryGetValue(path, out texture);
    }
    static SourceSprite GetSourceSpriteGeometry(string path)
    {
        SourceTexture texture;
        return TryFrozenSource(path, out texture) ? texture.direct : DescribeSourceSprite(AssetDatabase.LoadAssetAtPath<Sprite>(path));
    }

    static List<SourceSprite> FindLegacySpritesForTexture(string path, string allowed)
    {
        SourceTexture frozen;
        if (!TryFrozenSource(path, out frozen)) return FindLiveSourceSprites(path, allowed).ConvertAll(DescribeSourceSprite);
        if (frozen.direct != null && allowed == frozen.direct.name) return new List<SourceSprite> { frozen.direct };
        return frozen.sprites.Where(s => allowed == null || Array.IndexOf(allowed.Split('|'), s.name) >= 0).ToList();
    }

    public static void RebaseRetiredGeometryCache()
    {
        HdCacheState state = ReadHdCacheState();
        if (state == null) throw new InvalidOperationException("A verified existing HD cache is required.");
        var prior = state.entries.ToDictionary(x => x.path);
        List<HdCacheInput> equivalent = CreateHdCacheInputs(state.recipeHash);
        int changedDependencyFingerprints = 0;
        foreach (var input in equivalent)
        {
            HdCacheEntry cached;
            if (!prior.TryGetValue(input.asset.stagingPath, out cached)) throw new InvalidOperationException("Missing HD cache entry.");
            string cachedPath = GetHdCacheAssetPath(input.asset.stagingPath);
            ValidateHash(cachedPath, input.asset.approvedSha256, "Cached approved pixels");
            ValidateHash(cachedPath + ".meta", cached.metaHash, "Cached importer");
            if (cached.imageHash != input.asset.approvedSha256) throw new InvalidOperationException("Cached art provenance differs.");
            if (cached.inputHash != input.hash) changedDependencyFingerprints++;
        }
        // Artifact dependency hashes can drift across Editor imports. Require live
        // source geometry, PNG/meta provenance and every cached output to agree.
        foreach (var frozen in frozenSources.Values)
        {
            ValidateHash(frozen.path, frozen.hash, "Live retirement source");
            ValidateHash(frozen.path + ".meta", frozen.metaHash, "Live retirement source meta");
            CompareSourceShape(frozen.direct, DescribeSourceSprite(AssetDatabase.LoadAssetAtPath<Sprite>(frozen.path)));
            foreach (var sprite in frozen.sprites)
                CompareSourceShape(sprite, DescribeSourceSprite(AssetDatabase.LoadAssetAtPath<Sprite>(sprite.path)));
        }
        string bundle = GetAbsoluteProjectPath(BundleOutputRelativePath + "/" + BundleName);
        ValidateHash(bundle, state.bundleHash, "Cached HD Bundle");
        ValidateBuiltBundle(bundle);
        string recipe = GetHdCacheRecipeHash();
        List<HdCacheInput> current = CreateHdCacheInputs(recipe);
        foreach (var input in current) prior[input.asset.stagingPath].inputHash = input.hash;
        state.recipeHash = recipe;
        state.inputsHash = GetHdCacheInputsHash(current);
        WriteHdCacheState(state);
        File.WriteAllText("Logs/hd-cleanup-20261116/cache-rebase.json", "{\"verifiedInputs\":" + equivalent.Count + ",\"changedDependencyFingerprints\":" + changedDependencyFingerprints + ",\"reimported\":0,\"bundleChanged\":false}");
        Debug.Log("SWORDSMAN_RETIRED_CACHE_REBASE passed inputs=" + equivalent.Count + " reimported=0");
    }

    static void CompareSourceShape(SourceSprite frozen, SourceSprite live)
    {
        if (frozen == null && live == null) return;
        if (frozen == null || live == null || frozen.name != live.name ||
            !string.Equals(frozen.path, live.path, StringComparison.OrdinalIgnoreCase) ||
            frozen.rect != live.rect || frozen.pivot != live.pivot || frozen.border != live.border ||
            frozen.pixelsPerUnit != live.pixelsPerUnit)
            throw new InvalidOperationException("Frozen geometry differs: " + (frozen == null ? "null" : JsonUtility.ToJson(frozen)) + " live=" + (live == null ? "null" : JsonUtility.ToJson(live)));
    }

    public static void ValidateRetiredReferenceGeometry()
    {
        SourceTexture unused;
        TryFrozenSource("", out unused);
        var sprites = new Dictionary<string, SourceSprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var source in frozenSources.Values)
        {
            if (source.direct != null) sprites[source.direct.path] = source.direct;
            foreach (var sprite in source.sprites) sprites[sprite.path] = sprite;
        }
        var manifest = JsonUtility.FromJson<SwordsmanHd4xSpriteResolver.ReferenceManifest>(File.ReadAllText("Assets/Resources/SwordsmanHD4X/References.json"));
        foreach (var entry in manifest.entries)
        {
            SourceSprite original = sprites[entry.source];
            if (!string.Equals(entry.spriteName, original.name, StringComparison.OrdinalIgnoreCase) ||
                Vector2.Distance(entry.worldSize, original.rect.size / original.pixelsPerUnit) > .001f ||
                Vector2.Distance(entry.worldPivot, original.pivot / original.pixelsPerUnit) > .001f ||
                Vector4.Distance(entry.worldBorder, original.border / original.pixelsPerUnit) > .001f)
                throw new InvalidOperationException("Frozen reference geometry differs: " + entry.source);
        }
    }

    public static Sprite CreateRetiredGeometryProbe(string path)
    {
        SourceTexture unused; TryFrozenSource("", out unused);
        foreach (var source in frozenSources.Values)
        {
            SourceSprite shape = source.direct != null && string.Equals(source.direct.path, path, StringComparison.OrdinalIgnoreCase)
                ? source.direct : Array.Find(source.sprites, s => string.Equals(s.path, path, StringComparison.OrdinalIgnoreCase));
            if (shape == null) continue;
            var texture = new Texture2D(Mathf.CeilToInt(shape.rect.xMax), Mathf.CeilToInt(shape.rect.yMax));
            texture.name = Path.GetFileNameWithoutExtension(source.path);
            var sprite = Sprite.Create(texture, shape.rect, new Vector2(shape.pivot.x / shape.rect.width, shape.pivot.y / shape.rect.height),
                shape.pixelsPerUnit, 0, SpriteMeshType.FullRect, shape.border);
            sprite.name = shape.name;
            return sprite;
        }
        throw new InvalidOperationException("No frozen geometry probe for " + path);
    }

    public static void ExportRetiredSourceGeometry()
    {
        const string output = "Logs/hd-cleanup-20261116";
        string[] files = JsonUtility.FromJson<CleanupFiles>(File.ReadAllText(output + "/delete-input.json")).files;
        var retired = new HashSet<string>(files.Where(p => p.EndsWith(".png")), StringComparer.OrdinalIgnoreCase);
        var assets = new Dictionary<string, ApprovedAsset>(StringComparer.OrdinalIgnoreCase);
        foreach (AtlasSpec spec in SpriteAtlases) assets[spec.asset.sourceAssetPath] = spec.asset;
        foreach (ApprovedAsset asset in SingleAssets) assets[asset.sourceAssetPath] = asset;
        // Some original atlas filenames differ from their Sprite prefixes. Group
        // the exact audited Sprite files by texture, rather than searching names.
        var byTexture = new Dictionary<string, List<Sprite>>(StringComparer.OrdinalIgnoreCase);
        foreach (string path in files.Where(p => p.EndsWith(".asset")))
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null || sprite.texture == null) throw new InvalidOperationException("Missing audited Sprite: " + path);
            string texturePath = AssetDatabase.GetAssetPath(sprite.texture);
            List<Sprite> list;
            if (!byTexture.TryGetValue(texturePath, out list)) byTexture[texturePath] = list = new List<Sprite>();
            list.Add(sprite);
        }
        var textures = new List<SourceTexture>();
        foreach (string path in retired.OrderBy(x => x, StringComparer.Ordinal))
        {
            ApprovedAsset asset = assets[path];
            ValidateHash(path, asset.sourceSha256, "Retirement source");
            Sprite direct = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            List<Sprite> sprites;
            if (!byTexture.TryGetValue(path, out sprites)) throw new InvalidOperationException("No audited Sprites: " + path);
            sprites.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            textures.Add(new SourceTexture { path = path, hash = GetSha256(path), metaHash = GetSha256(path + ".meta"),
                dependencyHash = AssetDatabase.GetAssetDependencyHash(path).ToString(), direct = DescribeSourceSprite(direct),
                sprites = sprites.Select(DescribeSourceSprite).ToArray() });
        }
        if (textures.Count != 611) throw new InvalidOperationException("Expected exactly 611 retired textures.");
        File.WriteAllText(SourceGeometryPath, JsonUtility.ToJson(new SourceGeometry { textures = textures.ToArray() }, true));
        File.WriteAllText(output + "/geometry-sha.txt", GetSha256(SourceGeometryPath));
        Debug.Log("SWORDSMAN_RETIRED_GEOMETRY_EXPORTED textures=" + textures.Count);
    }
    [Serializable] sealed class CleanupFiles { public string[] files; }
}
