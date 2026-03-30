using UnityEngine;

public enum ItemType
{
    Material,
    Consumable,
    Weapon,
    Tool,
    Seed,
    BuildingMaterial,
    Quest
}

[CreateAssetMenu(menuName = "Para-Elementals/Items/Item Data")]
public class ItemData : ScriptableObject
{
    public string ItemId;
    public string DisplayName;
    
    [TextArea]
    public string Description;

    public Sprite Icon;

    public ItemType ItemType;

    public int MaxStack = 99;
}