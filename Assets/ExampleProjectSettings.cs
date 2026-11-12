using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings(settingsProviderName: "Example Project Settings")]
public class ExampleProjectSettings : ScriptableObject
{
    [SerializeField] private int _number = 5;

    public int Number
    {
        get => _number;
    }
}
