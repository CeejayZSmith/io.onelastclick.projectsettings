using System;
using System.Diagnostics;
using UnityEngine;

namespace FinalClick.ProjectSettings
{
    [DefaultExecutionOrder(-1000)]
    internal class ProjectSettingsRegisterer : MonoBehaviour
    {
        [Serializable]
        private struct ProjectSettingReference
        {
            [SerializeField] private string _type;
            [SerializeField] private ScriptableObject _value;
            
            public ScriptableObject Value => _value;
            public Type Type => Type.GetType(_type, throwOnError: true)!;

            public ProjectSettingReference(Type type, ScriptableObject value)
            {
                _type = type.AssemblyQualifiedName;
                _value = value;
            }
        }

        [SerializeField] private ProjectSettingReference[] _references;

        private void Awake()
        {
            UnityEngine.Debug.Log("Registering project settings");
            DontDestroyOnLoad(gameObject);
            foreach (var reference in _references)
            {
                ProjectSettingsDatabase.Register(reference.Type, reference.Value);
            }
        }

        [Conditional("UNITY_EDITOR")]
        public void UpdateReferences()
        {
#if UNITY_EDITOR
            var types = ProjectSettingsAttribute.GetAllProjectSettingTypes();
            _references = new ProjectSettingReference[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                _references[i] = new ProjectSettingReference(type, ProjectSettingsDatabase.Get(type) as ScriptableObject);
            }

            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}