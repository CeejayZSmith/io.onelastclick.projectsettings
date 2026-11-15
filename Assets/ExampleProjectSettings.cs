using System;
using System.Collections.Generic;
using OneLastClick.ProjectSettings;
using UnityEngine;

[ProjectSettings(settingsProviderName: "Example Project Settings")]
public class ExampleProjectSettings : ScriptableObject
{
    public enum DifficultyLevel
    {
        Easy = 0,
        Normal = 1,
        Hard = 2,
    }

    [Serializable]
    public class EnemySpawn
    {
        [SerializeField] private string _enemyName = "Enemy";
        [SerializeField] private int _count = 1;

        public string EnemyName
        {
            get => _enemyName;
        }

        public int Count
        {
            get => _count;
        }
    }

    [Serializable]
    public class NetworkSettings
    {
        [SerializeField] private string _host = "localhost";
        [SerializeField] private int _port = 7777;

        public string Host
        {
            get => _host;
        }

        public int Port
        {
            get => _port;
        }
    }

    [Header("Primitives")]
    [SerializeField] private int _number = 5;
    [SerializeField] private float _spawnRadius = 2.5f;
    [SerializeField] private bool _enableDebugLogs = true;
    [SerializeField] private string _logPrefix = "Example";

    [Header("Unity Types")]
    [SerializeField] private Vector3 _cameraOffset = new Vector3(0f, 2f, -5f);
    [SerializeField] private Color _tintColor = new Color(0.2f, 0.6f, 1f, 1f);

    [Header("Collections")]
    [SerializeField] private DifficultyLevel _difficulty = DifficultyLevel.Normal;
    [SerializeField] private string[] _levelNames = { "Tutorial", "Arena", "Boss" };
    [SerializeField] private List<EnemySpawn> _enemySpawns = new List<EnemySpawn>();
    [SerializeField] private NetworkSettings _network = new NetworkSettings();

    public int Number
    {
        get => _number;
    }

    public float SpawnRadius
    {
        get => _spawnRadius;
    }

    public bool EnableDebugLogs
    {
        get => _enableDebugLogs;
    }

    public string LogPrefix
    {
        get => _logPrefix;
    }

    public Vector3 CameraOffset
    {
        get => _cameraOffset;
    }

    public Color TintColor
    {
        get => _tintColor;
    }

    public DifficultyLevel Difficulty
    {
        get => _difficulty;
    }

    public string[] LevelNames
    {
        get => _levelNames;
    }

    public List<EnemySpawn> EnemySpawns
    {
        get => _enemySpawns;
    }

    public NetworkSettings Network
    {
        get => _network;
    }
}
