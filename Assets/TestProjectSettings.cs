using OneFinalClick.ProjectSettings;
using UnityEngine;

[ProjectSettings]
public class TestProjectSettings : ScriptableObject
{
    [SerializeField] private int _number = 5;

    public int Number
    {
        get => _number;
    }
}

