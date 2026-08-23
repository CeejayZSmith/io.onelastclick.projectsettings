using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FinalClick.ProjectSettings.Editor
{
    public static class ProjectSettingsSettingsProvider
    {
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

        private static SettingsProvider CreateProvider(Type type)
        {
            var attribute = ProjectSettingsAttribute.GetProjectSettingsAttribute(type);
            var settings = ProjectSettingsDatabase.Get(type);

            UnityEditor.Editor editor = null;

            return new SettingsProvider(attribute.GetSettingsProviderPath(type), SettingsScope.Project)
            {
                label = attribute.GetSettingsProviderName(type),

                guiHandler = _ =>
                {
                    UnityEditor.Editor.CreateCachedEditor(settings, null, ref editor);
                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(settings);
                        ProjectSettingsDatabase.Save(settings);
                    }
                },

                deactivateHandler = () =>
                {
                    if (editor != null)
                    {
                        UnityEngine.Object.DestroyImmediate(editor);
                        editor = null;
                    }
                }
            };
        }
    }
}