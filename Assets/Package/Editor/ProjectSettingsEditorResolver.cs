using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OneFinalClick.ProjectSettings.Editor
{
    public static class ProjectSettingsEditorResolver
    {
        public static Type[] GetAllTypesWithProjectSettingAttribute() => AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetTypesSafe)
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetCustomAttribute<ProjectSettingsAttribute>() != null).ToArray();

        
        public static string GetFilePathToSettingsAsset<T>() where T : ScriptableObject => GetFilePathToSettingsAsset(typeof(T));

        public static string GetFilePathToSettingsAsset(Type type)
        {
            ProjectSettingsAttribute attribute = GetProjectSettingsAttribute(type);
            return Path.Combine("ProjectSettings", attribute.FileDirectory, $"{GetFileName(type)}.asset");
        }

        public static string GetSettingsProviderPath<T>()
        {
            return GetSettingsProviderPath(typeof(T));
        }
        
        public static string GetSettingsProviderName(Type type)
        {
            ProjectSettingsAttribute attribute = GetProjectSettingsAttribute(type);
            
            if (string.IsNullOrEmpty(attribute.SettingsProviderName) == true)
            {
                return GetFileName(type);
            }

            return attribute.SettingsProviderName;
        }
        
        public static string GetSettingsProviderPath(Type type)
        {
            var name = GetSettingsProviderName(type);
            ProjectSettingsAttribute attribute = GetProjectSettingsAttribute(type);
            
            if (string.IsNullOrEmpty(attribute.SettingsProviderDirectory) == true)
            {
                return $"Project/{name}";
            }

            return $"Project/{attribute.SettingsProviderDirectory}/{name}";
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null);
            }
        }
        
        public static ProjectSettingsAttribute GetProjectSettingsAttribute<T>() where T : ScriptableObject
        {
            return GetProjectSettingsAttribute(typeof(T));
        }

        public static ProjectSettingsAttribute GetProjectSettingsAttribute(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException($"{type.FullName} must inherit from {nameof(ScriptableObject)} to use {nameof(ProjectSettingsAttribute)}.");
            }

            var attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException($"{type.FullName} is missing [{nameof(ProjectSettingsAttribute)}].");
            }

            return attribute;
        }

        private static string GetFileName<T>() where T : ScriptableObject
        {
            return GetFileName(typeof(T));
        }

        private static string GetFileName(Type type)
        {
            var attribute = GetProjectSettingsAttribute(type);
            if (string.IsNullOrEmpty(attribute.FileName) == true)
            {
                return type.Name;
            }

            return attribute.FileName;
        }
    }
}