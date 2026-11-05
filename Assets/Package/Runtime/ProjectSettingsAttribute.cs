using System;

namespace OneLastClick.ProjectSettings
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ProjectSettingsAttribute : Attribute
    {
        private const string DefaultFileDirectory = "";
        
        public readonly string FileName;
        public readonly string SettingsProviderDirectory;
        public readonly string SettingsProviderName;
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