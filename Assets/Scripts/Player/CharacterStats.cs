using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterStats", menuName = "Para-Elementals/Character Stats")]
public class CharacterStats : ScriptableObject
{
    [Header("Identity")]
    public string CharacterName;
    public Sprite CharacterSprite;

    [Header("Health")]
    public bool HasHealth = true;
    public int MaxHealth = 10;

    [Header("Stamina")]
    public bool HasStamina = false;
    public float MaxStamina = 100f;
    public float StaminaRegenRate = 10f;     // per second

    [Header("Mana")]
    public bool HasMana = false;
    public float MaxMana = 100f;
    public float ManaRegenRate = 5f;         // per second

    [Header("Rage")]
    public bool HasRage = false;
    public float MaxRage = 100f;
    public float RageBuildRate = 10f;        // per hit received
    public float RageDecayRate = 5f;         // per second when not in combat

    [Header("Combat")]
    public bool CanFight = true;
    public int BaseDamage = 1;
    public float MoveSpeed = 6f;
    public float JumpForce = 12f;
}
