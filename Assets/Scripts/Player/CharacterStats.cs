using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Para-Elementals/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Identity")]
    public string CharacterName;
    public Sprite CharacterSprite;

    [Header("Element")]
    public Element CharacterElement = Element.None;

    [Header("Health")]
    public bool HasHealth = true;
    public int MaxHealth = 10;

    [Header("Stamina")]
    public bool HasStamina = false;
    public float MaxStamina = 100f;
    public float StaminaRegenRate = 10f;

    [Header("Mana")]
    public bool HasMana = false;
    public float MaxMana = 100f;
    public float ManaRegenRate = 5f;

    [Header("Rage")]
    public bool HasRage = false;
    public float MaxRage = 100f;
    public float RageBuildRate = 10f;
    public float RageDecayRate = 5f;

    [Header("Combat")]
    public bool CanFight = true;
    public int BaseDamage = 1;
    public float MoveSpeed = 6f;
    public float JumpForce = 12f;
}
