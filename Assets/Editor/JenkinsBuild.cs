using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class JenkinsBuild
{
    static string[] SCENES = FindEnabledEditorScenes();
    static string APP_NAME = "Cache-Cache";
    static string TARGET_DIR = "buildScript";

    [MenuItem("Custom/CI/Build PC")]
    static void PerformPCBuild()
    {
        string target_dir = APP_NAME + ".exe";
        GenericBuild(SCENES, TARGET_DIR + "/" + target_dir, BuildTarget.StandaloneWindows, BuildOptions.None);
    }

    static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes , string target_dir, BuildTarget buildTarget, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, buildTarget);

        string str = BuildPipeline.BuildPlayer(scenes, target_dir, buildTarget, build_options).ToString();
    }
}
