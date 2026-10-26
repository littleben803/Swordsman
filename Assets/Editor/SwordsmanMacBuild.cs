using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class SwordsmanMacBuild
{
    private static readonly string[] Scenes =
    {
        "Assets/Scenes/level0.unity",
        "Assets/Scenes/level1.unity",
        "Assets/Scenes/level2.unity",
        "Assets/Scenes/level3.unity",
    };

    public static void BuildDevelopmentPlayer()
    {
        string outputPath = Path.GetFullPath(
            Path.Combine(Application.dataPath, "../Builds/macOS/Swordsman.app"));
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

        var options = new BuildPlayerOptions
        {
            scenes = Scenes,
            locationPathName = outputPath,
            target = BuildTarget.StandaloneOSX,
            options = BuildOptions.Development | BuildOptions.AllowDebugging,
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        Debug.LogFormat(
            "SWORDSMAN_MAC_BUILD result={0} errors={1} warnings={2} size={3} output={4}",
            summary.result,
            summary.totalErrors,
            summary.totalWarnings,
            summary.totalSize,
            summary.outputPath);

        if (summary.result != BuildResult.Succeeded)
        {
            throw new InvalidOperationException(
                "macOS Development Player build failed: " + summary.result);
        }
    }
}
