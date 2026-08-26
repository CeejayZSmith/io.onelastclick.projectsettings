using UnityEngine;

namespace OneFinalClick.ProjectSettings
{
    internal partial class BuiltProjectSettingsContainer : ScriptableObject
    {
        [SerializeField] private ScriptableObject[] _settings;
        
        public ScriptableObject[] Settings => _settings;
    }
}