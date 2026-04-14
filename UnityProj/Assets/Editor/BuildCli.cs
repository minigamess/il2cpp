using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEditor.Build.Reporting;

namespace Editor
{
    public class BuildCli
    {
        [MenuItem("Tools/Build")]
        private static void PerformBuild()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
            
            var buildPlayerOptions = new BuildPlayerOptions();

            buildPlayerOptions.target = BuildTarget.Android;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.locationPathName = "Builds/empty.apk";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
            // PlayerSettings.gcIncremental = false;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/empty.unity", true) };

            EditorSceneManager.OpenScene("Assets/empty.unity", OpenSceneMode.Single);
            
#if UNITY_2021_1_OR_NEWER
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;
#elif UNITY_2019_1_OR_NEWER
            EditorUserBuildSettings.androidCreateSymbolsZip = true;
#else
#endif
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;
            
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"构建失败: {report.summary.result}");
                if (report.steps != null && report.steps.Length > 0)
                {
                    foreach (var step in report.steps)
                    {
                        if (step.messages != null)
                        {
                            foreach (var msg in step.messages)
                            {
                                if (msg.type == LogType.Error || 
                                    msg.type == LogType.Exception)
                                {
                                    Debug.LogError(msg.content);
                                }
                            }
                        }
                    }
                }
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log("构建成功");
                EditorApplication.Exit(0);
            }
        }
        
        [MenuItem("Tools/BuildIpa")]
        private static void PerformBuildIpa()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            
            var buildPlayerOptions = new BuildPlayerOptions();

            buildPlayerOptions.target = BuildTarget.iOS;
            buildPlayerOptions.targetGroup = BuildTargetGroup.iOS;
            buildPlayerOptions.locationPathName = "Builds/empty";
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
            // PlayerSettings.gcIncremental = false;
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/empty.unity", true) };
            EditorSceneManager.OpenScene("Assets/empty.unity", OpenSceneMode.Single);
            // iOS 构建不需要设置 Android 相关配置
            
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"构建失败: {report.summary.result}");
                if (report.steps != null && report.steps.Length > 0)
                {
                    foreach (var step in report.steps)
                    {
                        if (step.messages != null)
                        {
                            foreach (var msg in step.messages)
                            {
                                if (msg.type == LogType.Error || 
                                    msg.type == LogType.Exception)
                                {
                                    Debug.LogError(msg.content);
                                }
                            }
                        }
                    }
                }
                EditorApplication.Exit(1);
            }
            else
            {
                Debug.Log("构建成功");
                EditorApplication.Exit(0);
            }
        }

    }
}