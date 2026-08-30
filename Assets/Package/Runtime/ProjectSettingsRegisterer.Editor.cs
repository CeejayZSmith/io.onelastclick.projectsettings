#if UNITY_EDITOR
using System;
using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    internal partial class ProjectSettingsRegisterer
    {
        public void SetBuiltProjectSettings(BuiltProjectSettingsContainer builtProjectSettingsContainer)
        {
            if (Application.isPlaying == true)
            {
                throw new InvalidOperationException("Cannot set runtime project settings while running as Awake call will have alraedy happened.");
            }
            _builtProjectSettingsContainer = builtProjectSettingsContainer;
        }
    }
}
#endif