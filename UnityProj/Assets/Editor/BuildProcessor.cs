using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

namespace Editor
{
    public class BuildProcessor
    {
        [PostProcessBuild(1000)]
        private static void OnPostProcessBuild(BuildTarget target, string path)
        {
// #if UNITY_IOS
//             if (target != BuildTarget.iOS)
//                 return;
//
//             var projPath = PBXProject.GetPBXProjectPath(path);
//
//             var proj = new PBXProject();
//             proj.ReadFromString(File.ReadAllText(projPath));
//             proj.SetBuildPropertyForConfig(proj.BuildConfigByName(proj.GetUnityFrameworkTargetGuid(), "Release"),
//                 "DEBUG_INFORMATION_FORMAT", "dwarf-with-dsym");
//             proj.WriteToFile(projPath);
// #endif
        }
    }
}