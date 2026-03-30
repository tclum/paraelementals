using System.Collections;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private int _spawnCount = 3;
    [SerializeField] private float _spawnDelay = 0.5f;
    [SerializeField] private bool _spawnOnStart = true;
    [SerializeField] private bool _spawnOnPlayerProximity = false;
    [SerializeField] private float _proximityRange = 10f;

    private bool _hasSpawned = false;
    private Transform _player;
    private int _enemiesAlive = 0;

    public bool IsCleared => _hasSpawned && _enemiesAlive <= 0;

    public event System.Action OnCleared;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        if (_spawnOnStart && !_spawnOnPlayerProximity)
            StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        if (_spawnOnPlayerProximity && !_hasSpawned && _player != null)
        {
            float dist = Vector2.Distance(transform.position, _player.position);
            if (dist <= _proximityRange)
                StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        _hasSpawned = true;

        for (int i = 0; i < _spawnCount; i++)
        {
            if (_enemyPrefab == null)
                yield break;

            GameObject enemy = Instantiate(_enemyPrefab, transform.position, Quaternion.identity);
            _enemiesAlive++;

            // Track death
            Health health = enemy.GetComponent<Health>();
            if (health != null)
                health.Died += OnEnemyDied;

            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    private void OnEnemyDied()
    {
        _enemiesAlive = Mathf.Max(0, _enemiesAlive - 1);

        if (_enemiesAlive <= 0 && _hasSpawned)
            OnCleared?.Invoke();
    }

    public void TriggerSpawn()
    {
        if (!_hasSpawned)
            StartCoroutine(SpawnWave());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (_spawnOnPlayerProximity)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, _proximityRange);
        }
    }
}
