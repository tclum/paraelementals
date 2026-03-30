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

    [SerializeField] private WorldItemPickup _pickupPrefab;
    [SerializeField] private LootEntry[] _lootTable;

    public void DropLoot()
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
                    Random.Range(-3f, 3f),
                    Random.Range(-3f, 3f),
                    0f
                );

                WorldItemPickup pickup = Instantiate(_pickupPrefab, spawnPosition, Quaternion.identity);
                pickup.Initialize(entry.Item, Mathf.Max(1, entry.Amount));
            }
        }
    }
}