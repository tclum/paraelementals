using UnityEngine;

public class LootDropper : MonoBehaviour
{
    [System.Serializable]
    public class LootEntry
    {
        public ItemData Item;
        public int Amount = 1;
        [Range(0f, 1f)] public float DropChance = 1f;
    }

    [Header("Item Drops")]
    [SerializeField] private WorldItemPickup _pickupPrefab;
    [SerializeField] private LootEntry[] _lootTable;

    [Header("Shard Drops")]
    [SerializeField] private ShardPickup _shardPrefab;
    [SerializeField] private int _minShards = 0;
    [SerializeField] private int _maxShards = 0;
    [SerializeField] private int _shardBurstCount = 3;

    public void DropLoot()
    {
        DropItems();
        DropShards();
    }

    private void DropItems()
    {
        if (_pickupPrefab == null || _lootTable == null)
            return;

        for (int i = 0; i < _lootTable.Length; i++)
        {
            LootEntry entry = _lootTable[i];
            if (entry == null || entry.Item == null)
                continue;

            if (Random.value <= entry.DropChance)
            {
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0.5f, 1.5f),
                    0f
                );

                WorldItemPickup pickup = Instantiate(_pickupPrefab, spawnPosition, Quaternion.identity);
                pickup.Initialize(entry.Item, Mathf.Max(1, entry.Amount));
            }
        }
    }

    private void DropShards()
    {
        if (_shardPrefab == null || _maxShards <= 0)
            return;

        int totalShards = Random.Range(_minShards, _maxShards + 1);
        if (totalShards <= 0) return;

        // Spread shards across burst count pickups
        int remaining = totalShards;
        for (int i = 0; i < _shardBurstCount && remaining > 0; i++)
        {
            int amount = (i == _shardBurstCount - 1)
                ? remaining
                : Random.Range(1, remaining + 1);

            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-1.5f, 1.5f),
                Random.Range(0.5f, 2f),
                0f
            );

            ShardPickup shard = Instantiate(_shardPrefab, spawnPos, Quaternion.identity);
            shard.Initialize(amount);
            remaining -= amount;
        }
    }
}