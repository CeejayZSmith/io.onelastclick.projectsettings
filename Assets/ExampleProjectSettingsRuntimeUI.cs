using System.Collections.Generic;
using System.Text;
using OneLastClick.ProjectSettings;
using UnityEngine;
using UnityEngine.UI;

public class ExampleProjectSettingsRuntimeUI : MonoBehaviour
{
    [SerializeField] private Text m_numberText;
    [SerializeField] private Text m_spawnRadiusText;
    [SerializeField] private Text m_difficultyText;
    [SerializeField] private Text m_levelNamesText;
    [SerializeField] private Text m_enemySpawnsText;
    [SerializeField] private Text m_networkText;
    [SerializeField] private Text m_tintColorText;
    [SerializeField] private Text m_customPathsText;
    [SerializeField] private Text m_minimalText;

    private void Awake()
    {
        Refresh();
    }

    private void Update()
    {
        Refresh();
    }

    private void Refresh()
    {
        ExampleProjectSettings settings = ProjectSettingsDatabase.Get<ExampleProjectSettings>();
        ExampleProjectSettingsCustomPaths customPaths = ProjectSettingsDatabase.Get<ExampleProjectSettingsCustomPaths>();
        ExampleProjectSettingsMinimal minimal = ProjectSettingsDatabase.Get<ExampleProjectSettingsMinimal>();

        int number = settings.Number;
        float spawnRadius = settings.SpawnRadius;
        ExampleProjectSettings.DifficultyLevel difficulty = settings.Difficulty;
        string levelNames = string.Join(", ", settings.LevelNames);
        string enemySpawns = DescribeEnemySpawns(settings.EnemySpawns);
        string network = $"{settings.Network.Host}:{settings.Network.Port}";
        string tintColor = settings.TintColor.ToString();
        string apiUrl = $"{customPaths.ApiUrl} ({customPaths.RequestTimeoutSeconds}s)";
        string greeting = minimal.Greeting;

        SetText(m_numberText, number.ToString());
        SetText(m_spawnRadiusText, spawnRadius.ToString("0.0"));
        SetText(m_difficultyText, difficulty.ToString());
        SetText(m_levelNamesText, levelNames);
        SetText(m_enemySpawnsText, enemySpawns);
        SetText(m_networkText, network);
        SetText(m_tintColorText, tintColor);
        SetText(m_customPathsText, apiUrl);
        SetText(m_minimalText, greeting);
    }

    private static string DescribeEnemySpawns(List<ExampleProjectSettings.EnemySpawn> enemySpawns)
    {
        StringBuilder builder = new StringBuilder();

        foreach (ExampleProjectSettings.EnemySpawn enemySpawn in enemySpawns)
        {
            if (builder.Length > 0)
            {
                builder.Append('\n');
            }

            builder.Append(enemySpawn.EnemyName).Append(" x").Append(enemySpawn.Count);
        }

        return builder.ToString();
    }

    private static void SetText(Text text, string value)
    {
        // Skipped so a label can be wired up in the inspector without the others breaking.
        if (text == null)
        {
            return;
        }

        text.text = value;
    }
}
