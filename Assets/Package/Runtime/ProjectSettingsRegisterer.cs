using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    // Register as early as possible so ProjectSettings are accessible in Awake functions.
    [DefaultExecutionOrder(int.MinValue)] 
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

            ProjectSettingsDatabase.RegisterProjectSettings(_builtProjectSettingsContainer.Settings);
        }
    }
}
