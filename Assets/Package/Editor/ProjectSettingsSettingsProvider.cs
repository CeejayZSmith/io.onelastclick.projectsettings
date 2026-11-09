using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OneLastClick.ProjectSettings.Editor
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
            ScriptableObject settings = null;

            return new SettingsProvider(ProjectSettingsEditorResolver.GetSettingsProviderPath(type), SettingsScope.Project)
            {
                label = ProjectSettingsEditorResolver.GetSettingsProviderName(type),

                activateHandler = (_, _) =>
                {
                    if (settings == null)
                    {
                        settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                    }

                    if (editor == null)
                    {
                        UnityEditor.Editor.CreateCachedEditor(
                            settings,
                            null,
                            ref editor);
                    }

                    editor.serializedObject.Update();
                },

                guiHandler = _ =>
                {
                    if (editor.target == null)
                    {
                        return;
                    }
                    
                    editor.serializedObject.Update();

                    EditorGUI.BeginChangeCheck();

                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        editor.serializedObject.ApplyModifiedProperties();
                        settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                        EditorUtility.SetDirty(settings);
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

                    settings = null;

                    ProjectSettingsEditorDatabase.ClearCacheForType(type);
                }
            };
        }
    }
}
