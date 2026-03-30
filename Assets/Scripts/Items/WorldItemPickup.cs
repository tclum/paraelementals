using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WorldItemPickup : MonoBehaviour
{
    [SerializeField] private ItemData _item;
    [SerializeField] private int _amount = 1;

    public void Initialize(ItemData item, int amount)
    {
        _item = item;
        _amount = amount;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        InventoryManager inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
            return;

        if (inventory.AddItem(_item, _amount))
        {
            Destroy(gameObject);
        }
    }
}