#if UNITY_EDITOR
using System;
using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    public partial class ProjectSettingsRegisterer
    {
        internal void SetBuiltProjectSettings(BuiltProjectSettingsContainer builtProjectSettingsContainer)
        {
            if (Application.isPlaying == true)
            {
                throw new InvalidOperationException("Cannot set runtime project settings while running as Awake call will have already happened.");
            }
            _builtProjectSettingsContainer = builtProjectSettingsContainer;
        }

        // Editor play mode does not go through the build pipeline, so nothing
        // injects a registerer into the scene. The editor assembly installs a
        // provider during its own initialization, and this hook runs before
        // any user Awake so ProjectSettingsDatabase.Get works immediately.
        internal static Func<BuiltProjectSettingsContainer> EditorPlaymodeContainerProvider;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EditorPlaymodeInitialize()
        {
            if (EditorPlaymodeContainerProvider == null)
            {
                return;
            }

            BuiltProjectSettingsContainer container = EditorPlaymodeContainerProvider();
            if (container == null)
            {
                return;
            }

            RegisterContainer(container);
        }
    }
}
#endif
