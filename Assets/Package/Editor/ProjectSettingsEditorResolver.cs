using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OneLastClick.ProjectSettings.Editor
{
    public static class ProjectSettingsEditorResolver
    {
        public static Type[] GetAllRuntimeTypesWithProjectSettingAttribute() => AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetTypesSafe)
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetCustomAttribute<ProjectSettingsAttribute>() != null &&
                type.GetCustomAttribute<ProjectSettingsAttribute>().EditorOnly == false
                ).ToArray();
        
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
            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);
            return Path.Combine("ProjectSettings", attribute.FileDirectory, $"{GetFileName(type)}.asset");
        }

        public static string GetSettingsProviderPath<T>()
        {
            return GetSettingsProviderPath(typeof(T));
        }

        public static string GetSettingsProviderName(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);

            if (string.IsNullOrEmpty(attribute.SettingsProviderName) == true)
            {
                return GetFileName(type);
            }

            return attribute.SettingsProviderName;
        }

        public static string GetSettingsProviderPath(Type type)
        {
            string name = GetSettingsProviderName(type);
            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);

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

        private static string GetFileName<T>() where T : ScriptableObject
        {
            return GetFileName(typeof(T));
        }

        private static string GetFileName(Type type)
        {
            ProjectSettingsAttribute attribute = ProjectSettingsDatabase.GetProjectSettingsAttribute(type);
            if (string.IsNullOrEmpty(attribute.FileName) == true)
            {
                return type.Name;
            }

            return attribute.FileName;
        }
    }
}
