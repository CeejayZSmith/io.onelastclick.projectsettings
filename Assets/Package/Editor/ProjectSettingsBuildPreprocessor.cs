using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneLastClick.ProjectSettings.Editor
{
    public class ProjectSettingsBuildPreprocessor : IProcessSceneWithReport, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string GeneratedTempDirectoryName = "_OneLastClickBuildTemp";
        private const string GeneratedDirectory = "Assets/" + GeneratedTempDirectoryName;
        private const string GeneratedContainerFilePath = GeneratedDirectory + "/BuiltProjectSettingsContainer.asset";

        public int callbackOrder => -1;

        public void OnPreprocessBuild(BuildReport report)
        {
            AssetDatabase.DeleteAsset(GeneratedDirectory);

            if (AssetDatabase.IsValidFolder(GeneratedDirectory) == false)
            {
                AssetDatabase.CreateFolder("Assets", GeneratedTempDirectoryName);
            }

            ScriptableObject[] settings = ProjectSettingsEditorDatabase.GetOrCreateRuntimeProjectSettings();
            BuiltProjectSettingsContainer.CreateAtAssetPath(settings, GeneratedContainerFilePath);
            DestroySettings(settings);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            AssetDatabase.DeleteAsset(GeneratedDirectory);
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // Editor play mode is handled by ProjectSettingsPlayModeRegistration,
            // which registers before the first scene loads its Awake calls.
            if (Application.isPlaying == true)
            {
                return;
            }

            // Only inject into the first boot scene.
            if (SceneManager.GetSceneAt(0) != scene)
            {
                return;
            }

            CreateBootRegisterer(scene);

            Debug.Log($"Injecting ProjectSettings into {scene.name}");
        }

        private static void CreateBootRegisterer(Scene scene)
        {
            GameObject gameObject = new GameObject("ProjectSettingsRegisterer");
           SceneManager.MoveGameObjectToScene(gameObject, scene);
            ProjectSettingsRegisterer registerer = gameObject.AddComponent<ProjectSettingsRegisterer>();
            ProjectSettingsUnregisterer _ = gameObject.AddComponent<ProjectSettingsUnregisterer>();
            
            BuiltProjectSettingsContainer container = AssetDatabase.LoadAssetAtPath<BuiltProjectSettingsContainer>(GeneratedContainerFilePath);
            if (container == null)
            {
                throw new InvalidOperationException($"Failed to load generated project settings container at '{GeneratedContainerFilePath}'.");
            }

            registerer.SetBuiltProjectSettings(container);
        }

        private static void DestroySettings(ScriptableObject[] projectSettings)
        {
            foreach (ScriptableObject settings in projectSettings)
            {
                if (settings == null)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(settings);
            }
        }
    }
}
