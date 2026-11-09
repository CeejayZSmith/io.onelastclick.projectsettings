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

            return new SettingsProvider(ProjectSettingsEditorResolver.GetSettingsProviderPath(type), SettingsScope.Project)
            {
                label = ProjectSettingsEditorResolver.GetSettingsProviderName(type),

                activateHandler = (_, _) =>
                {
                    Debug.Assert(editor == null);
                    if (editor == null)
                    {
                        UnityEditor.Editor.CreateCachedEditor(
                            ProjectSettingsEditorDatabase.GetOrCreateDefault(type),
                            null,
                            ref editor);
                    }
                    editor.serializedObject.Update();
                },

                guiHandler = _ =>
                {
                    if (editor == null)
                    {
                        return;
                    }

                    editor.serializedObject.Update();

                    EditorGUI.BeginChangeCheck();

                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        editor.serializedObject.ApplyModifiedProperties();

                        ScriptableObject settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
                        EditorUtility.SetDirty(settings);
                        ProjectSettingsEditorDatabase.SaveProjectSetting(settings);
                        UnityEngine.Object.DestroyImmediate(editor);
                    }
                },

                deactivateHandler = () =>
                {
                    Debug.Assert(editor != null);
                    if (editor != null)
                    {
                        UnityEngine.Object.DestroyImmediate(editor);
                        editor = null;
                    }

                    ProjectSettingsEditorDatabase.ClearCacheForType(type);
                }
            };
        }
    }
}