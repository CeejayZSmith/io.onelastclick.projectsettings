using OneLastClick.ProjectSettings;
using UnityEngine;
using UnityEngine.UI;

public class TestProjectSettingsGetterUI : MonoBehaviour
{
    [SerializeField] private Text m_numberText;
    
    private void Awake()
    {
        int number = ProjectSettingsDatabase.Get<TestProjectSettings>().Number;
        m_numberText.text = number.ToString();
    }

    private void Update()
    {
        int number = ProjectSettingsDatabase.Get<TestProjectSettings>().Number;
        m_numberText.text = number.ToString();
    }
}
