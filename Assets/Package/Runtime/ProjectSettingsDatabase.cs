using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FinalClick.ProjectSettings
{
    public static class ProjectSettingsDatabase
    {
        private static readonly Dictionary<Type, UnityEngine.Object> _cachedProjectSettings = new Dictionary<Type, UnityEngine.Object>();

        public static T Get<T>() where T : ScriptableObject
        {
            return Get(typeof(T)) as T;
        }
        public static UnityEngine.Object Get(Type type)
        {
            Debug.Assert(typeof(ScriptableObject).IsAssignableFrom(type), $"type is not assignable to scriptableobject from {type.FullName}");
            
            if (_cachedProjectSettings.TryGetValue(type, out var cachedObject) == true)
            {
                return cachedObject;
            }
            
#if UNITY_EDITOR
            var attribute = ProjectSettingsAttribute.GetProjectSettingsAttribute(type);
            
            var loadedObjects = UnityEditorInternal.InternalEditorUtility
                .LoadSerializedFileAndForget(attribute.GetFilePathToSettingsAsset(type));
            if (loadedObjects.Length > 0)
            {
                Debug.Assert(loadedObjects.Length == 1, "Too many objects were loaded.");
                Debug.Assert(type.IsAssignableFrom(loadedObjects[0].GetType()), $"saved object is not a {type.FullName}");
                return loadedObjects[0];
            }
            
            ScriptableObject settings = ScriptableObject.CreateInstance(type);
            Save(settings);
            Debug.Log("Creating new Settings asset.");
            return settings;
#else
            throw new NotImplementedException();
#endif
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Save(UnityEngine.Object configObject)
        {
#if UNITY_EDITOR
            var attribute = ProjectSettingsAttribute.GetProjectSettingsAttribute(configObject.GetType());

            if (configObject is IProjectSettingsPreSaveProcessor preSaveProcessor)
            {
                preSaveProcessor.OnPreSave();
            }
            
            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(new Object[] { configObject }, attribute.GetFilePathToSettingsAsset(configObject.GetType()), true);
#endif
        }

        internal static void Register(Type referenceType, ScriptableObject referenceValue)
        {
            UnityEngine.Debug.Log("Registering " + referenceType.FullName + " to " + referenceValue);
            _cachedProjectSettings.Add(referenceType, referenceValue);
        }
    }
}