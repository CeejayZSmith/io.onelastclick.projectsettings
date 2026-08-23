#if UNITY_EDITOR
using System;
using System.Diagnostics;
using UnityEngine;

namespace FinalClick.ProjectSettings
{
    internal partial class ProjectSettingsRegisterer
    {
        [Conditional("UNITY_EDITOR")]
        public void UpdateReferences()
        {
            Type[] types = ProjectSettingsAttribute.GetAllProjectSettingTypes();
            _references = new ScriptableObject[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                _references[i] = ProjectSettingsDatabase.Get(type);
            }

            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
}
#endif