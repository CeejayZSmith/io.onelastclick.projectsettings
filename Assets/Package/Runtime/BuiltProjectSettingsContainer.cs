using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    internal partial class BuiltProjectSettingsContainer : ScriptableObject
    {
        [SerializeField] private ScriptableObject[] _settings;
        
        public ScriptableObject[] Settings => _settings;
    }
}