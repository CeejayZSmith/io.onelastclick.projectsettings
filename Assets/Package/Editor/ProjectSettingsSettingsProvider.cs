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
                    Debug.Assert(editor == null, "We should not have an editor when activating.");
                    editor = UnityEditor.Editor.CreateEditor(ProjectSettingsEditorDatabase.GetOrCreateDefault(type));
                    editor.serializedObject.Update();
                },

                guiHandler = _ =>
                {
                    // If editor target is lost, reload config object from project settings and recreate editor.
                    if (editor.target == null)
                    {
                        UpdateEditorWithNewUnityObject(ref editor, ProjectSettingsEditorDatabase.GetOrCreateDefault(type));
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
                        ProjectSettingsPlayModeRegistration.Refresh(settings);
                    }
                },

                deactivateHandler = () =>
                {
                    Debug.Assert(editor != null, "We should always have an editor when deactivating.");
                    UnityEngine.Object.DestroyImmediate(editor);
                    editor = null;
                }
            };
        }
        
        private static void UpdateEditorWithNewUnityObject(ref UnityEditor.Editor editor, UnityEngine.Object obj)
        {
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }
            
            editor = UnityEditor.Editor.CreateEditor(obj);
            editor.serializedObject.Update();
        }
    }
}
