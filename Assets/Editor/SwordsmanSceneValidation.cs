using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SwordsmanSceneValidation
{
    private static readonly string[] ScenePaths =
    {
        "Assets/Scenes/level0.unity",
        "Assets/Scenes/level1.unity",
        "Assets/Scenes/level2.unity",
        "Assets/Scenes/level3.unity",
    };

    public static void ValidateScenes()
    {
        int totalMissingScripts = 0;

        foreach (string path in ScenePaths)
        {
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                throw new InvalidOperationException($"Failed to load scene: {path}");
            }

            int missingScripts = 0;
            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (Transform transform in root.GetComponentsInChildren<Transform>(true))
                {
                    missingScripts += GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(
                        transform.gameObject
                    );
                }
            }

            totalMissingScripts += missingScripts;
            Debug.Log(
                $"SWORDSMAN_SCENE_VALIDATION path={path} roots={scene.rootCount} " +
                $"missingScripts={missingScripts}"
            );
        }

        Debug.Log(
            $"SWORDSMAN_SCENE_VALIDATION_COMPLETE scenes={ScenePaths.Length} " +
            $"missingScripts={totalMissingScripts}"
        );
    }
}
