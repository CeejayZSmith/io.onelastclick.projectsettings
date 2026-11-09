using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneLastClick.ProjectSettings.Editor
{
    [InitializeOnLoad]
    public class ProjectSettingsBuildPreprocessor : IProcessSceneWithReport, IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string GeneratedTempDirectoryName = "_OneLastClickBuildTemp";
        private const string GeneratedDirectory = "Assets/" + GeneratedTempDirectoryName;
        private const string GeneratedContainerFilePath = GeneratedDirectory + "/BuiltProjectSettingsContainer.asset";

        public int callbackOrder => -1;

        static ProjectSettingsBuildPreprocessor()
        {
            // Editor play mode never runs the build pipeline, so hand the
            // runtime a way to build an equivalent container on demand.
            ProjectSettingsRegisterer.EditorPlaymodeContainerProvider = CreateEditorPlaymodeContainer;
        }

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

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            // Only inject into the first boot scene.
            if (SceneManager.GetSceneAt(0) != scene)
            {
                return;
            }

            CreateBootRegisterer(scene);

            Debug.Log($"Injecting ProjectSettings into {scene.name}");
        }

        private static BuiltProjectSettingsContainer CreateEditorPlaymodeContainer()
        {
            ScriptableObject[] editorSettings = GetEditorReferencesOfProjectSettings();

            BuiltProjectSettingsContainer container = ScriptableObject.CreateInstance<BuiltProjectSettingsContainer>();
            container.SetSettings(editorSettings);
            return container;
        }

        private static void CreateBootRegisterer(Scene scene)
        {
            GameObject gameObject = new GameObject("ProjectSettingsRegisterer");
            SceneManager.MoveGameObjectToScene(gameObject, scene);

            ProjectSettingsRegisterer registerer = gameObject.AddComponent<ProjectSettingsRegisterer>();
            BuiltProjectSettingsContainer container = AssetDatabase.LoadAssetAtPath<BuiltProjectSettingsContainer>(GeneratedContainerFilePath);
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

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                projectSettings[i] = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
            }

            return projectSettings;
        }
    }
}
