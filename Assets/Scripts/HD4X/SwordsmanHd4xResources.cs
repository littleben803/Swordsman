using UnityEngine;

public static class SwordsmanHd4xResources
{
    public static GameObject LoadPrefab(string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab != null) SwordsmanHd4xReferenceBindings.ApplyAll(prefab);
        return prefab;
    }
}
