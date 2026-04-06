using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int _maxSlots = 24;

    private List<InventorySlot> _slots = new List<InventorySlot>();

    public IReadOnlyList<InventorySlot> Slots => _slots;

    public event Action InventoryChanged;

    private void Awake()
    {
        if (FindObjectsByType<InventoryManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < _maxSlots; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlot slot = _slots[i];
            if (slot.Item == item && slot.Quantity < item.MaxStack)
            {
                int space = item.MaxStack - slot.Quantity;
                int toAdd = Mathf.Min(space, remaining);
                slot.Quantity += toAdd;
                remaining -= toAdd;

                if (remaining <= 0)
                {
                    InventoryChanged?.Invoke();
                    return true;
                }
                Debug.Log("Inventory + " + item.DisplayName + " x" + amount);
            }
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlot slot = _slots[i];
            if (slot.IsEmpty)
            {
                int toAdd = Mathf.Min(item.MaxStack, remaining);
                slot.Item = item;
                slot.Quantity = toAdd;
                remaining -= toAdd;

                if (remaining <= 0)
                {
                    InventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        InventoryChanged?.Invoke();
        return remaining == 0;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        if (!HasItem(item, amount))
            return false;

        int remaining = amount;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlot slot = _slots[i];
            if (slot.Item != item)
                continue;

            int toRemove = Mathf.Min(slot.Quantity, remaining);
            slot.Quantity -= toRemove;
            remaining -= toRemove;

            if (slot.Quantity <= 0)
            {
                slot.Item = null;
                slot.Quantity = 0;
            }

            if (remaining <= 0)
            {
                InventoryChanged?.Invoke();
                return true;
            }
        }

        InventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        int total = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item == item)
            {
                total += _slots[i].Quantity;
            }
        }

        return total >= amount;
    }

    public int GetItemCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return 0;

        int total = 0;

        for (int i = 0; i < _slots.Count; i++)
        {
            InventorySlot slot = _slots[i];

            if (slot.Item != null && slot.Item.ItemId == itemId)
            {
                total += slot.Quantity;
            }
        }

        return total;
    }

    public int GetItemCount(ItemData item)
    {
        if (item == null)
            return 0;

        int total = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (_slots[i].Item == item)
            {
                total += _slots[i].Quantity;
            }
        }

        return total;
    }
}