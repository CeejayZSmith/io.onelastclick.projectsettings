using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace FinalClick.ProjectSettings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProjectSettingsAttribute : Attribute
    {
        private const string DefaultFileDirectory = "";
        
        [CanBeNull] private readonly string _fileName;
        [CanBeNull] private readonly string _settingsProviderDirectory;
        [CanBeNull] private readonly string _settingsProviderName;
        private readonly string _fileDirectory;

        public static Type[] GetAllProjectSettingTypes() => AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetTypesSafe)
            .Where(type =>
                type.IsClass &&
                !type.IsAbstract &&
                type.GetCustomAttribute<ProjectSettingsAttribute>() != null).ToArray();
        
        public string GetFilePathToSettingsAsset<T>() where T : ScriptableObject => GetFilePathToSettingsAsset(typeof(T));
        public string GetFilePathToSettingsAsset(Type type) => Path.Combine("ProjectSettings", _fileDirectory, $"{GetFileName(type)}.asset");

        public string GetSettingsProviderPath<T>()
        {
            return GetSettingsProviderPath(typeof(T));
        }
        
        public string GetSettingsProviderName(Type type)
        {
            if (string.IsNullOrEmpty(_settingsProviderName) == true)
            {
                return GetFileName(type);
            }

            return _settingsProviderName;
        }
        
        public string GetSettingsProviderPath(Type type)
        {
            var name = GetSettingsProviderName(type);
            
            if (string.IsNullOrEmpty(_settingsProviderDirectory) == true)
            {
                return $"Project/{name}";
            }

            return $"Project/{_settingsProviderDirectory}/{name}";
        }
        public ProjectSettingsAttribute(string fileName = null, string fileDirectory = null, string settingsProviderName = null, string settingsProviderDirectory = null)
        {
            _fileDirectory = fileDirectory ?? DefaultFileDirectory;
            _settingsProviderDirectory = settingsProviderDirectory;
            _fileName = fileName;
            _settingsProviderName = settingsProviderName;
        }

        public static ProjectSettingsAttribute GetProjectSettingsAttribute<T>() where T : ScriptableObject
        {
            return GetProjectSettingsAttribute(typeof(T));
        }

        public static ProjectSettingsAttribute GetProjectSettingsAttribute(Type type)
        {
            if (typeof(ScriptableObject).IsAssignableFrom(type) == false)
            {
                throw new InvalidOperationException(
                    $"{type.FullName} must inherit from {nameof(ScriptableObject)} to use {nameof(ProjectSettingsAttribute)}.");
            }

            var attribute = type.GetCustomAttribute<ProjectSettingsAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException(
                    $"{type.FullName} is missing [{nameof(ProjectSettingsAttribute)}].");
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
            if (string.IsNullOrEmpty(attribute._fileName) == true)
            {
                return type.Name;
            }

            return attribute._fileName;
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
        
    }
}