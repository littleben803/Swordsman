using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static partial class SwordsmanHd4xBuild
{
    const string RetirementOutput = "Logs/hd-cleanup-20261116";

    public static void DeleteRetiredSourcesAndValidate()
    {
        ValidateRetiredReferenceGeometry();
        if (File.ReadAllText(RetirementOutput + "/incoming-references.json").Trim() != "[]")
            throw new InvalidOperationException("External source references remain.");
        string[] files = JsonUtility.FromJson<CleanupFiles>(File.ReadAllText(RetirementOutput + "/delete-input.json")).files;
        var requested = new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);
        var expected = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var source in frozenSources.Values)
        {
            expected.Add(source.path);
            foreach (var sprite in source.sprites) expected.Add(sprite.path);
            if (source.direct != null && source.path.StartsWith("Assets/Resources/", StringComparison.OrdinalIgnoreCase)) expected.Add(Path.ChangeExtension(source.path, ".asset").Replace('\\', '/'));
        }
        foreach (string path in expected.ToArray()) expected.Add(path + ".meta");
        if (expected.Count != 5164 || !expected.SetEquals(requested)) throw new InvalidOperationException("Deletion scope differs from frozen provenance.");
        long bytes = files.Sum(path => new FileInfo(path).Length);
        AssetDatabase.StartAssetEditing();
        try
        {
            foreach (string path in files.Where(p => !p.EndsWith(".meta")).OrderBy(p => p.EndsWith(".asset") ? 0 : 1))
                if (!AssetDatabase.DeleteAsset(path)) throw new InvalidOperationException("Failed to retire " + path);
        }
        finally { AssetDatabase.StopAssetEditing(); }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string path in files) if (File.Exists(path)) throw new InvalidOperationException("Retired source remains: " + path);
        File.WriteAllText(RetirementOutput + "/deleted.json", "{\"textures\":611,\"sprites\":1971,\"metas\":2582,\"files\":5164,\"diskBytes\":" + bytes + "}");
        ValidateAfterRetirement();
    }

    public static void ValidateAfterRetirement()
    {
        // Builder validates against the frozen, hash-locked original geometry first.
        ValidateExistingApprovedBundle("7c2fded688c9ddb5a5ef9b24d80d8797e5ed32e4ad75d87ffd587d65389cb23b");
        string recipe = GetHdCacheRecipeHash();
        List<HdCacheInput> inputs = CreateHdCacheInputs(recipe);
        HdCacheState state = ReadHdCacheState();
        HdCachePlan plan = PlanHdCacheChanges(inputs, state);
        string bundle = GetAbsoluteProjectPath(BundleOutputRelativePath + "/" + BundleName);
        if (!CanReuseHdCacheBundle(state, recipe, GetHdCacheInputsHash(inputs), bundle) || plan.changed.Count != 0 || plan.removed.Count != 0)
            throw new InvalidOperationException("Post-retirement HD cache is not reusable without import.");
        SwordsmanHd4xRequiredValidation.Validate();
        SwordsmanHd4xReferenceMigration.Validate();
        File.WriteAllText(RetirementOutput + "/validation.json", "{\"passed\":true,\"frozenTextures\":611,\"referenceDescriptors\":1971,\"bindings\":3804,\"scenes\":4,\"missingScripts\":0,\"playerScripts\":true,\"hdCacheReused\":true,\"reimported\":0}");
        Debug.Log("SWORDSMAN_RETIRED_SOURCE_VALIDATION passed textures=611 sprites=1971 scenes=4 missingScripts=0 reimported=0");
    }
}
