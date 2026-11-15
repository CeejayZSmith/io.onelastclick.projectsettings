using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings(settingsProviderName: "Example Project Settings (Editor Only)", editorOnly: true)]
public class ExampleProjectSettingsEditorOnly : ScriptableObject
{
    [SerializeField] private string _publishingNotes = "Only used while working in the editor";
    [SerializeField] private int _sceneIndexToOpen = 0;

    public string PublishingNotes
    {
        get => _publishingNotes;
    }

    public int SceneIndexToOpen
    {
        get => _sceneIndexToOpen;
    }
}
