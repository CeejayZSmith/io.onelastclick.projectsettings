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

                // The settings are loaded from disk when the provider opens and are kept only while it is open.
                // Every other read of the settings reloads from disk, so edits made outside of Unity, such as
                // discarding the settings file in git, are picked up the next time the provider opens.
                activateHandler = (_, _) => ReloadEditor(type, ref settings, ref editor),

                guiHandler = _ =>
                {
                    // If editor target is lost, reload config object from project settings and recreate editor.
                    if (editor == null || editor.target == null)
                    {
                        ReloadEditor(type, ref settings, ref editor);
                    }

                    editor.serializedObject.Update();

                    EditorGUI.BeginChangeCheck();

                    editor.OnInspectorGUI();

                    if (EditorGUI.EndChangeCheck())
                    {
                        editor.serializedObject.ApplyModifiedProperties();

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

                    // The settings reference is dropped rather than destroyed because it can still be
                    // registered in ProjectSettingsDatabase for the current play mode session.
                    settings = null;
                }
            };
        }

        private static void ReloadEditor(Type type, ref ScriptableObject settings, ref UnityEditor.Editor editor)
        {
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }

            settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);

            editor = UnityEditor.Editor.CreateEditor(settings);
            editor.serializedObject.Update();
        }
    }
}
