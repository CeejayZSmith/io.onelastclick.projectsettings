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
            return (T)Get(typeof(T));
        }

        public static ScriptableObject Get(Type type)
        {
            if (Application.isPlaying == false)
            {
                throw new InvalidOperationException($"Cannot read project settings of type '{type}' outside of play mode, as nothing is registered yet. Read the settings through the project settings window while editing.");
            }

            if (typeof(ScriptableObject).IsAssignableFrom(type) == false)
            {
                throw new ArgumentException($"'{type}' must derive from {nameof(ScriptableObject)} to be used as project settings.", nameof(type));
            }

            if (CachedProjectSettings.TryGetValue(type, out ScriptableObject cachedObject) == true)
            {
                return cachedObject;
            }

            throw new ArgumentException($"No project settings of type '{type}' are registered.", nameof(type));
        }

        internal static void Register(Type referenceType, ScriptableObject referenceValue)
        {
            if (referenceType == null)
            {
                throw new ArgumentNullException(nameof(referenceType));
            }

            if (referenceValue == null)
            {
                throw new ArgumentNullException(nameof(referenceValue));
            }

            CachedProjectSettings[referenceType] = referenceValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            Application.quitting -= OnApplicationQuitting;
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
