using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings]
public class ExampleProjectSettingsMinimal : ScriptableObject
{
    [SerializeField] private string _greeting = "Hello, World!";

    public string Greeting
    {
        get => _greeting;
    }
}
