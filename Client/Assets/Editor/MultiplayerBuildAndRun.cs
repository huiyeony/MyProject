using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MultiplayerBuildAndRun
{
    [MenuItem("Tools/BuildAndRun/2 Players")]
    static void PerformMacOSBuild2()
    {
        PerformMacOSBuild(2);
    }

    [MenuItem("Tools/BuildAndRun/3 Players")]
    static void PerformMacOSBuild3()
    {
        PerformMacOSBuild(3);
    }

    [MenuItem("Tools/BuildAndRun/4 Players")]
    static void PerformMacOSBuild4()
    {
        PerformMacOSBuild(4);
    }

    static void PerformMacOSBuild(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);

        for (int i = 1; i <= playerCount; i++)
        {
            
            BuildPlayerOptions options = new BuildPlayerOptions();
            options.scenes = GetScenePaths();
            options.locationPathName = "Builds/MACOS/" + GetProjectName() + i.ToString() + ".dmg";
            options.target = BuildTarget.StandaloneOSX;
            options.options = BuildOptions.AutoRunPlayer;
            BuildPipeline.BuildPlayer(options);

        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
