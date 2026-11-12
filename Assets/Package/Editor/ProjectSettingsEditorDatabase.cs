using System;
using System.IO;
using UnityEngine;

namespace OneLastClick.ProjectSettings.Editor
{
    public static class ProjectSettingsEditorDatabase
    {
        public static ScriptableObject[] GetOrCreateRuntimeProjectSettings()
        {
            Type[] types = ProjectSettingsEditorResolver.GetAllRuntimeTypesWithProjectSettingAttribute();

            ScriptableObject[] projectSettings = new ScriptableObject[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];

                projectSettings[i] = GetOrCreateDefault(type);
            }

            return projectSettings;
        }
        
        public static T GetOrCreateDefault<T>() where T : class
        {
            return GetOrCreateDefault(typeof(T)) as T;
        }

        public static ScriptableObject GetOrCreateDefault(Type type)
        {
            Debug.Assert(typeof(ScriptableObject).IsAssignableFrom(type), $"type is not assignable to scriptableobject from {type.FullName}");

            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);

            if (attribute == null)
            {
                throw new ArgumentException("The specified type is not a project settings type. Add the attribute [ProjectSettings] to the type if you would like it to be a project settings." + type.FullName);
            }

            string path = ProjectSettingsEditorResolver.GetFilePathToSettingsAsset(type);

            string directory = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(directory) == false)
            {
                Directory.CreateDirectory(directory);
            }

            // Nothing is cached here on purpose. Reading from disk every time means changes made to the
            // settings file outside of Unity, such as discarding the file in git, are always picked up.
            UnityEngine.Object[] loadedObjects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(path);
            if (loadedObjects.Length > 0)
            {
                Debug.Assert(loadedObjects.Length == 1, "Too many objects were loaded.");
                Debug.Assert(type.IsAssignableFrom(loadedObjects[0].GetType()), $"saved object is not a {type.FullName}");
                return loadedObjects[0] as ScriptableObject;
            }

            ScriptableObject settings = ScriptableObject.CreateInstance(type);
            SaveProjectSetting(settings);
            Debug.Log("Creating new Settings asset.");
            return settings;
        }

        public static void SaveProjectSetting(ScriptableObject configObject)
        {
            Type projectSettingsType = configObject.GetType();

            if (configObject is IProjectSettingsPreSaveProcessor preSaveProcessor)
            {
                preSaveProcessor.OnPreSave();
            }

            string filePath = ProjectSettingsEditorResolver.GetFilePathToSettingsAsset(projectSettingsType);
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { configObject }, filePath, true);
        }
    }
}
