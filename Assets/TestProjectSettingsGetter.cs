using OneLastClick.ProjectSettings;
using UnityEngine;

public class TestProjectSettingsGetter : MonoBehaviour
{
    private void Awake()
    {
        int number = ProjectSettingsDatabase.Get<TestProjectSettings>().Number;
        Debug.Log(number);
    }

    private void Update()
    {
        int number = ProjectSettingsDatabase.Get<TestProjectSettings>().Number;
        Debug.Log(number);
    }
}
