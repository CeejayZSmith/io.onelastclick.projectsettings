using OneLastClick.ProjectSettings;
using UnityEngine;

public class ExampleProjectSettingsLogNumber : MonoBehaviour
{
    private void Awake()
    {
        int number = ProjectSettingsDatabase.Get<ExampleProjectSettings>().Number;
        Debug.Log(number);
    }

    private void Update()
    {
        int number = ProjectSettingsDatabase.Get<ExampleProjectSettings>().Number;
        Debug.Log(number);
    }
}
