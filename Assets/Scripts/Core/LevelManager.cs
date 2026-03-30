using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Setup")]
    [SerializeField] private BossController _boss;
    [SerializeField] private bool _requireAllEnemiesForBoss = true;

    private List<EnemySpawnPoint> _spawnPoints = new List<EnemySpawnPoint>();
    private int _clearedSpawnPoints = 0;
    private bool _bossSpawned = false;
    private bool _levelComplete = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Find all spawn points in scene
        EnemySpawnPoint[] points = FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);
        foreach (var point in points)
        {
            _spawnPoints.Add(point);
            point.OnCleared += OnSpawnPointCleared;
        }

        // Hook up boss defeated event
        if (_boss != null)
            _boss.OnBossDefeated.AddListener(OnBossDefeated);

        Debug.Log($"[LevelManager] Found {_spawnPoints.Count} spawn points.");
    }

    private void OnSpawnPointCleared()
    {
        _clearedSpawnPoints++;
        Debug.Log($"[LevelManager] Spawn point cleared ({_clearedSpawnPoints}/{_spawnPoints.Count})");

        if (_requireAllEnemiesForBoss && _clearedSpawnPoints >= _spawnPoints.Count)
        {
            TriggerBossSpawn();
        }
    }

    private void TriggerBossSpawn()
    {
        if (_bossSpawned || _boss == null)
            return;

        _bossSpawned = true;
        _boss.gameObject.SetActive(true);
        Debug.Log("[LevelManager] Boss activated!");
    }

    private void OnBossDefeated()
    {
        if (_levelComplete)
            return;

        _levelComplete = true;
        Debug.Log("[LevelManager] Level complete!");

        if (ResultsScreen.Instance != null)
            ResultsScreen.Instance.ShowResults();
    }

    public void RegisterEnemyKill()
    {
        if (ResultsScreen.Instance != null)
            ResultsScreen.Instance.RegisterEnemyKill();
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
