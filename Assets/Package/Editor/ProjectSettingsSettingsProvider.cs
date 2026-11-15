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
            bool isSettingsRuntimeInstance = false;

            // NOTE: Editor only settings are never registered in ProjectSettingsDatabase, so the settings
            //       loaded from disk are always the ones to display and edit, whether play mode is active or not.
            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);
            bool isEditorOnly = attribute.EditorOnly;

            return new SettingsProvider(ProjectSettingsEditorResolver.GetSettingsProviderPath(type), SettingsScope.Project)
            {
                label = ProjectSettingsEditorResolver.GetSettingsProviderName(type),

                // NOTE: When Application.isPlaying is FALSE
                //       The settings are loaded from disk when the provider opens and are kept only while it is open.
                //       Every other read of the settings reloads from disk, so edits made outside of Unity, such as
                //       discarding the settings file in git, are picked up the next time the provider opens.
                
                // NOTE: When Application.isPlaying is TRUE
                //       We get the runtime settings instance, but we do not save any changes to disk. Instead, we
                //       display a warning box to the Developer that their changes will be lost on exiting playmode

                // NOTE: For editor only settings
                //       The settings are never registered in ProjectSettingsDatabase, so play mode makes no difference.
                //       The settings loaded from disk are kept and edited as usual, with no reload on isPlaying changes.
                activateHandler = (_, _) => ReloadEditor(type, isEditorOnly, ref settings, ref isSettingsRuntimeInstance, ref editor),

                guiHandler = _ =>
                {
                    // If editor target is lost, reload config object from project settings and recreate editor.
                    bool isPlayingStateChanged = isEditorOnly == false && isSettingsRuntimeInstance != Application.isPlaying;

                    if (editor == null || editor.target == null || isPlayingStateChanged == true)
                    {
                        ReloadEditor(type, isEditorOnly, ref settings, ref isSettingsRuntimeInstance, ref editor);
                    }

                    editor.serializedObject.Update();

                    if (isSettingsRuntimeInstance == true)
                    {
                        DisplayPlaymodeWarning(settings);
                    }
                    
                    EditorGUI.BeginChangeCheck();

                    editor.OnInspectorGUI();
                    
                    if (EditorGUI.EndChangeCheck() == true)
                    {
                        editor.serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(settings);
                        if (isSettingsRuntimeInstance == false)
                        {
                            ProjectSettingsEditorDatabase.SaveProjectSetting(settings);
                        }
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

        private static void DisplayPlaymodeWarning(ScriptableObject settings)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            GUILayout.Label(
                EditorGUIUtility.IconContent("console.warnicon"),
                GUILayout.Width(24),
                GUILayout.Height(36)
            );

            EditorGUILayout.LabelField(
                "Changes made during Play Mode will be reverted when exiting Play Mode.",
                EditorStyles.wordWrappedLabel
            );

            if (GUILayout.Button(
                    "Save to Project",
                    GUILayout.Width(120),
                    GUILayout.Height(36)
                ) == true)
            {
                ProjectSettingsEditorDatabase.SaveProjectSetting(settings);
            }

            EditorGUILayout.EndHorizontal();
        }

        private static void ReloadEditor(Type type, bool isEditorOnly, ref ScriptableObject settings, ref bool isRuntimeSettingsInstance, ref UnityEditor.Editor editor)
        {
            if (editor != null)
            {
                UnityEngine.Object.DestroyImmediate(editor);
                editor = null;
            }

            if (isEditorOnly == false && Application.isPlaying == true && ProjectSettingsDatabase.HasRegisteredProjectSettings == true)
            {
                isRuntimeSettingsInstance = true;
                settings = ProjectSettingsDatabase.Get(type);
            }
            else
            {
                isRuntimeSettingsInstance = false;
                settings = ProjectSettingsEditorDatabase.GetOrCreateDefault(type);
            }

            editor = UnityEditor.Editor.CreateEditor(settings);
            editor.serializedObject.Update();
        }
    }
}
