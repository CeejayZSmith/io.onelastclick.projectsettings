using System;
using System.Collections.Generic;
using UnityEngine;

namespace OneLastClick.ProjectSettings
{
    public static class ProjectSettingsDatabase
    {
        private static readonly Dictionary<Type, ScriptableObject> CachedProjectSettings = new Dictionary<Type, ScriptableObject>();

        public static T Get<T>() where T : ScriptableObject
        {
            return Get(typeof(T)) as T;
        }

        public static ScriptableObject Get(Type type)
        {
            if (Application.isPlaying == false)
            {
                throw new InvalidOperationException("This should not be called outside of playmode as no settings will be registered yet. Use ProjectSettingsEditorDatabase instead.");
            }
            
            Debug.Assert(typeof(ScriptableObject).IsAssignableFrom(type), $"type is not assignable to scriptableobject from {type.FullName}");
            
            if (CachedProjectSettings.TryGetValue(type, out var cachedObject) == true)
            {
                return cachedObject;
            }
            
            throw new ArgumentException($"No project settings of type '{type}'");
        }

        internal static void Register(Type referenceType, ScriptableObject referenceValue)
        {
            Debug.Log("Registering " + referenceType.FullName + " to " + referenceValue);
            CachedProjectSettings.Add(referenceType, referenceValue);
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            Application.quitting += OnApplicationQuitting;
            ClearCachedProjectSettings();
        }
        
        private static void ClearCachedProjectSettings()
        {
            CachedProjectSettings.Clear();
        }

        private static void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            ClearCachedProjectSettings();
        }
    }
}