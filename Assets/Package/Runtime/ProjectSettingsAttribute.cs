using System;
using JetBrains.Annotations;

namespace OneLastClick.ProjectSettings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProjectSettingsAttribute : Attribute
    {
        private const string DefaultFileDirectory = "";
        
        [CanBeNull] public readonly string FileName;
        [CanBeNull] public readonly string SettingsProviderDirectory;
        [CanBeNull] public readonly string SettingsProviderName;
        public readonly bool EditorOnly;
        public readonly string FileDirectory;

        public ProjectSettingsAttribute(string fileName = null, string fileDirectory = null, string settingsProviderName = null, string settingsProviderDirectory = null, bool editorOnly = false)
        {
            FileDirectory = fileDirectory ?? DefaultFileDirectory;
            SettingsProviderDirectory = settingsProviderDirectory;
            FileName = fileName;
            SettingsProviderName = settingsProviderName;
            EditorOnly = editorOnly;
        }

    }
}