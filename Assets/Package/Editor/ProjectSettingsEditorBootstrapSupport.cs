using UnityEngine;

namespace OneLastClick.ProjectSettings.Editor
{
    internal static class ProjectSettingsEditorBootstrapSupport
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // NOTE: We need to invoke `RegisterRuntimeProjectSettings` before the Awake functions. However,
            //       the `EnteredPlayMode` event cant be used as it fires AFTER the Awake functions.
            //       Instead, we use the OnBeforeSceneLoad event and verify the application is playing.
            
            if (Application.isPlaying == false)
            {
                return;
            }

            EnsureRegisteredRuntimeProjectSettings();
        }

        private static void EnsureRegisteredRuntimeProjectSettings()
        {
            if (ProjectSettingsDatabase.HasRegisteredProjectSettings == true)
            {
                return;
            }
            
            using (DestroyOnDispose<ScriptableObject> settings = ProjectSettingsEditorDatabase.GetOrCreateRuntimeProjectSettings())
            {
                ProjectSettingsDatabase.RegisterProjectSettings(settings);
            }
        }
    }
}
