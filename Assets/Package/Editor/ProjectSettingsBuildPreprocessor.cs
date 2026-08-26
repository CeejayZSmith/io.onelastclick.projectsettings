using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneFinalClick.ProjectSettings.Editor
{
    public class ProjectSettingsBuildPreprocessor : IProcessSceneWithReport, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string GeneratedTempDirectoryName = "_FinalClickBuildTemp";
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

            ScriptableObject[] editorSettings = GetEditorReferencesOfProjectSettings();
            BuiltProjectSettingsContainer.CreateAtAssetPath(editorSettings, GeneratedContainerFilePath);
        }
        
        public void OnPostprocessBuild(BuildReport report)
        {
            AssetDatabase.DeleteAsset(GeneratedDirectory);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorPlaymodeInitialize()
        {
            ManuallyRegister();
        }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // EditorPlaymodeInitialize handles editor play mode, as it needs
            // to be created before Awake() of other objects.
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

        private static void ManuallyRegister()
        {
            ScriptableObject[] copies = GetEditorReferencesOfProjectSettings();

            foreach (ScriptableObject settings in copies)
            {
                ProjectSettingsDatabase.Register(settings.GetType(), settings);
            }
        }

        private static void CreateBootRegisterer(Scene scene)
        {
            var gameObject = new GameObject("ProjectSettingsRegisterer");
            SceneManager.MoveGameObjectToScene(gameObject, scene);
            var registerer = gameObject.AddComponent<ProjectSettingsRegisterer>();
            var container = AssetDatabase.LoadAssetAtPath<BuiltProjectSettingsContainer>(GeneratedContainerFilePath);
            if (container == null)
            {
                throw new InvalidOperationException($"Failed to load generated project settings container at '{GeneratedContainerFilePath}'.");
            }

            registerer.SetBuiltProjectSettings(container);
        }

        private static ScriptableObject[] GetEditorReferencesOfProjectSettings()
        {
            Type[] types =
                ProjectSettingsEditorResolver
                    .GetAllTypesWithProjectSettingAttribute()
                    .Where(type =>
                        ProjectSettingsEditorResolver
                            .GetProjectSettingsAttribute(type)
                            .EditorOnly == false)
                    .ToArray();

            ScriptableObject[] projectSettings = new ScriptableObject[types.Length];

            for (var i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                projectSettings[i] = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
            }

            return projectSettings;
        }
    }
}