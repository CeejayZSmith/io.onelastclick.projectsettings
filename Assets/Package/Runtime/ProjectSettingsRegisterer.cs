using System;
using UnityEngine;

namespace FinalClick.ProjectSettings
{
    [DefaultExecutionOrder(-1000)]
    internal partial class ProjectSettingsRegisterer : MonoBehaviour
    {
        [SerializeField] private ScriptableObject[] _references;

        private void Awake()
        {
            UnityEngine.Debug.Log("Registering project settings");
            DontDestroyOnLoad(gameObject);

            if (_references == null)
            {
                return;
            }
            
            foreach (ScriptableObject reference in _references)
            {
                Type projectSettingsType = reference.GetType();
                ProjectSettingsDatabase.Register(projectSettingsType, reference);
            }
        }
    }
}