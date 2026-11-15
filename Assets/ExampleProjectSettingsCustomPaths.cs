using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings(fileName: "CustomPathsSettings", fileDirectory: "Custom", settingsProviderName: "Custom Paths", settingsProviderDirectory: "Examples")]
public class ExampleProjectSettingsCustomPaths : ScriptableObject
{
    [SerializeField] private string _apiUrl = "https://example.com/api";
    [SerializeField] private int _requestTimeoutSeconds = 10;

    public string ApiUrl
    {
        get => _apiUrl;
    }

    public int RequestTimeoutSeconds
    {
        get => _requestTimeoutSeconds;
    }
}
