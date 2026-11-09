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
    }
}
#endif