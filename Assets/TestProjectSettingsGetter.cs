using FinalClick.ProjectSettings;
using UnityEngine;

public class TestProjectSettingsGetter : MonoBehaviour
{
    void Awake()
    {
        int number = ProjectSettingsDatabase.Get<TestProjectSettings>().Number;
        Debug.Log(number);
    }
}
