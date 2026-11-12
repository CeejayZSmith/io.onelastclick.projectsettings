using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    // Unregister as late as possible to prevent null ref errors if using ProjectSettings in OnDestroy.
    [DefaultExecutionOrder(int.MaxValue)]
    internal class ProjectSettingsUnregisterer : MonoBehaviour
    {
        private void OnDestroy()
        {
            ProjectSettingsDatabase.UnregisterAll();
        }
    }
}
