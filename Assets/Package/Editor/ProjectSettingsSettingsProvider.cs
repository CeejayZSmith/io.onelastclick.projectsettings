using System;
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
                return ProjectSettingsEditorResolver.GetAllTypesWithProjectSettingAttribute().Select(CreateProvider).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private static SettingsProvider CreateProvider(Type type)
        {

            UnityEditor.Editor editor = null;

            return new SettingsProvider(ProjectSettingsEditorResolver.GetSettingsProviderPath(type), SettingsScope.Project)
            {
                label = ProjectSettingsEditorResolver.GetSettingsProviderName(type),

                guiHandler = _ =>
                {
                    var settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                    UnityEditor.Editor.CreateCachedEditor(settings, null, ref editor);
                    EditorGUI.BeginChangeCheck();
                    
                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        ProjectSettingsEditorDatabase.SaveProjectSetting(settings);
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