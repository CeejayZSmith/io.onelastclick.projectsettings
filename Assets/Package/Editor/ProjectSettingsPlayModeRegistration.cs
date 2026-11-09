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

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            EditorApplication.delayCall -= OnDelayCall;
            EditorApplication.delayCall += OnDelayCall;
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

        private static void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            if (playModeStateChange == PlayModeStateChange.EnteredPlayMode)
            {
                RegisterRuntimeProjectSettings();
            }
        }

        internal static void Refresh(ScriptableObject settings)
        {
            if (Application.isPlaying == false)
            {
                return;
            }

            Type type = settings.GetType();

            if (IsRuntimeProjectSetting(type) == false)
            {
                return;
            }

            ProjectSettingsDatabase.Register(type, settings);
        }

        private static void CreateOrLoadAllProjectSettings()
        {
            Type[] types = ProjectSettingsBuildPreprocessor.GetProjectSettingsTypes();

            foreach (Type type in types)
            {
                ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
            }

            if (Application.isPlaying == true)
            {
                RegisterRuntimeProjectSettings();
            }
        }

        private static void RegisterRuntimeProjectSettings()
        {
            ProjectSettingsDatabase.Clear();

            Type[] types = ProjectSettingsBuildPreprocessor.GetProjectSettingsTypes();

            foreach (Type type in types)
            {
                if (IsRuntimeProjectSetting(type) == false)
                {
                    continue;
                }

                ScriptableObject settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                ProjectSettingsDatabase.Register(type, settings);
            }
        }

        private static bool IsRuntimeProjectSetting(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettingsEditorResolver.GetProjectSettingsAttribute(type);
            return attribute.EditorOnly == false;
        }
    }
}
