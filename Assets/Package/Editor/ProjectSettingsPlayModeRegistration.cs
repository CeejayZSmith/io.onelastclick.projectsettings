using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OneLastClick.ProjectSettings.Editor
{
    [InitializeOnLoad]
    internal static class ProjectSettingsPlayModeRegistration
    {
        static ProjectSettingsPlayModeRegistration()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;

            EditorApplication.delayCall -= OnDelayCall;
            EditorApplication.delayCall += OnDelayCall;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // EnteredPlayMode fires after the first scene has loaded, so settings
            // have to be registered here to be available to the first Awake calls.
            if (Application.isPlaying == false)
            {
                return;
            }

            RegisterRuntimeProjectSettings();
        }

        private static void OnDelayCall()
        {
            EditorApplication.delayCall -= OnDelayCall;
            CreateOrLoadAllProjectSettings();
        }

        private static void OnBeforeAssemblyReload()
        {
            // The runtime database is static state, so it cannot survive a domain reload.
            ProjectSettingsDatabase.Clear();
        }

        internal static void Refresh(ScriptableObject settings)
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            RegisterRuntimeProjectSetting(settings);
        }

        private static void CreateOrLoadAllProjectSettings()
        {
            Type[] types = ProjectSettingsBuildPreprocessor.GetProjectSettingsTypes();

            foreach (Type type in types)
            {
                ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
            }
        }

        private static void RegisterRuntimeProjectSettings()
        {
            ProjectSettingsDatabase.Clear();

            Type[] types = ProjectSettingsBuildPreprocessor.GetProjectSettingsTypes();

            foreach (Type type in types)
            {
                ScriptableObject settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                RegisterRuntimeProjectSetting(settings);
            }
        }

        private static void RegisterRuntimeProjectSetting(ScriptableObject settings)
        {
            Type type = settings.GetType();

            if (IsRuntimeProjectSetting(type) == false)
            {
                return;
            }

            ProjectSettingsDatabase.Register(type, settings);
        }

        private static bool IsRuntimeProjectSetting(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettingsEditorResolver.GetProjectSettingsAttribute(type);
            return attribute.EditorOnly == false;
        }
    }
}
