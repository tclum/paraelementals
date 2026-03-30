using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 5;
    public float spawnRange = 10f;

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 pos = new Vector2(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y
            );

            Instantiate(enemyPrefab, pos, Quaternion.identity);
        }
    }
}