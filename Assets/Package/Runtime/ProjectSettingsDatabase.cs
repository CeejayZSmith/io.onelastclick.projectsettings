using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OneLastClick.ProjectSettings
{
    public static class ProjectSettingsDatabase
    {
        internal static bool HasRegisteredProjectSettings = false;
        private static readonly Dictionary<Type, ScriptableObject> RegisteredProjectSettings = new Dictionary<Type, ScriptableObject>();

        public static T Get<T>() where T : ScriptableObject
        {
            return (T)Get(typeof(T));
        }

        public static ScriptableObject Get(Type type)
        {
            if (Application.isPlaying == false)
            {
                throw new InvalidOperationException($"Cannot read project settings of type '{type}' outside of play mode. Read the settings through the project settings window while editing.");
            }
            
            if (HasRegisteredProjectSettings == false)
            {
                throw new InvalidOperationException($"Cannot read project settings of type '{type}' as no Project Settings have been registered yet.");
            }

            if (typeof(ScriptableObject).IsAssignableFrom(type) == false)
            {
                throw new ArgumentException($"'{type}' must derive from {nameof(ScriptableObject)} to be used as project settings.", nameof(type));
            }

            if (RegisteredProjectSettings.TryGetValue(type, out ScriptableObject cachedObject) == true)
            {
                return cachedObject;
            }

            throw new ArgumentException($"No project settings of type '{type}' are registered.", nameof(type));
        }
        
        internal static ProjectSettingsAttribute GetProjectSettingsAttribute<T>() where T : ScriptableObject
        {
            return GetProjectSettingsAttribute(typeof(T));
        }

        internal static ProjectSettingsAttribute GetProjectSettingsAttribute(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException($"{type.FullName} must inherit from {nameof(ScriptableObject)} to use {nameof(ProjectSettingsAttribute)}.");
            }

            ProjectSettingsAttribute attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException($"{type.FullName} is missing [{nameof(ProjectSettingsAttribute)}].");
            }

            return attribute;
        }

        public static void RegisterProjectSettings(IReadOnlyCollection<ScriptableObject> settings)
        {
            Debug.Assert(HasRegisteredProjectSettings == false, "Settings already registered.");

            HasRegisteredProjectSettings = true;
            
            foreach (ScriptableObject reference in settings)
            {
                if (reference == null)
                {
                    Debug.LogError("Missing project settings reference.");
                    continue;
                }

                ScriptableObject duplicate = Object.Instantiate(reference);
                duplicate.name = reference.name;
                Register(duplicate);
            }    
        }

        private static void Register(ScriptableObject reference)
        {
            Type referenceType = reference.GetType();
            
            if (referenceType == null)
            {
                throw new ArgumentNullException(nameof(referenceType));
            }
            
            Debug.Assert(IsRuntimeProjectSetting(referenceType) == true, $"Reference type '{referenceType}' is not a runtime project setting. Its for editor only.");

            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            RegisteredProjectSettings[referenceType] = reference;
        }

        internal static void UnregisterAll()
        {
            foreach ((Type _, ScriptableObject settingInstance) in RegisteredProjectSettings)
            {
                if (Application.isPlaying == true)
                {
                    Object.Destroy(settingInstance);
                }
                else
                {
                    Object.DestroyImmediate(settingInstance);
                }
            }
            RegisteredProjectSettings.Clear();
        }
        
        private static bool IsRuntimeProjectSetting(Type type)
        {
            ProjectSettingsAttribute attribute = GetProjectSettingsAttribute(type);
            return attribute.EditorOnly == false;
        }
    }
}
