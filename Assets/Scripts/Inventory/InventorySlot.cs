[System.Serializable]
public class InventorySlot
{
    public ItemData Item;
    public int Quantity;

    public bool IsEmpty => Item == null || Quantity <= 0;

    public InventorySlot()
    {
        Item = null;
        Quantity = 0;
    }
}