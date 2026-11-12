using OneLastClick.ProjectSettings;
using UnityEngine;
using UnityEngine.UI;

public class ExampleProjectSettingsRuntimeUI : MonoBehaviour
{
    [SerializeField] private Text m_numberText;
    
    private void Awake()
    {
        int number = ProjectSettingsDatabase.Get<ExampleProjectSettings>().Number;
        m_numberText.text = number.ToString();
    }

    private void Update()
    {
        int number = ProjectSettingsDatabase.Get<ExampleProjectSettings>().Number;
        m_numberText.text = number.ToString();
    }
}
