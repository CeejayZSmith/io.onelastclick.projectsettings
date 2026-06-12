using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FinalClick.ProjectSettings.Editor
{
    public static class ProjectSettingsSettingsProvider
    {
        private static readonly Dictionary<Type, UnityEditor.Editor> _editorCaches = new Dictionary<Type, UnityEditor.Editor>();
        
        [SettingsProviderGroup]
        public static SettingsProvider[] CreateSettingsProviders()
        {
            try
            {
                return ProjectSettingsAttribute.GetAllProjectSettingTypes().Select(CreateProvider).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public static SettingsProvider CreateProvider(Type type)
        {
            var attribute =
                ProjectSettingsAttribute.GetProjectSettingsAttribute(type);

            var settings =
                ProjectSettingsDatabase.Get(type);

            return new SettingsProvider(
                attribute.GetSettingsProviderPath(type),
                SettingsScope.Project)
            {
                label = attribute.GetSettingsProviderName(type),

                guiHandler = _ =>
                {
                    EditorGUI.BeginChangeCheck();
                    
                    if (_editorCaches.TryGetValue(type, out UnityEditor.Editor editor) == false || editor == null)
                    {
                        editor = UnityEditor.Editor.CreateEditor(settings);
                        _editorCaches[type] = editor;
                    }

                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        UnityEditor.EditorUtility.SetDirty(settings);                        
                        ProjectSettingsDatabase.Save(settings);
                    }

                }
            };
        }
    }
}