using System;
using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    [DefaultExecutionOrder(-1000)]
    internal partial class ProjectSettingsRegisterer : MonoBehaviour
    {
        [SerializeField] private BuiltProjectSettingsContainer _builtProjectSettingsContainer;

        private void Awake()
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            
            Debug.Log("Registering project settings");
            DontDestroyOnLoad(gameObject);

            if (_builtProjectSettingsContainer == null)
            {
                Debug.LogError("No project settings container found.");
                return;
            }
            
            foreach (ScriptableObject reference in _builtProjectSettingsContainer.Settings)
            {
                if (reference == null)
                {
                    Debug.LogError("Missing project settings reference.");
                    continue;
                }
                Type projectSettingsType = reference.GetType();
                ProjectSettingsDatabase.Register(projectSettingsType, reference);
            }
        }
    }
}