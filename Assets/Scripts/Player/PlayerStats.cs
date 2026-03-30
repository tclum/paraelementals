using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Character")]
    [SerializeField] private CharacterStats _characterStats;

    // Current values
    private float _currentStamina;
    private float _currentMana;
    private float _currentRage;

    // Events
    public event Action<float, float> OnStaminaChanged;
    public event Action<float, float> OnManaChanged;
    public event Action<float, float> OnRageChanged;

    // Public accessors
    public CharacterStats Stats => _characterStats;

    public float CurrentStamina => _currentStamina;
    public float MaxStamina => _characterStats != null ? _characterStats.MaxStamina : 0f;

    public float CurrentMana => _currentMana;
    public float MaxMana => _characterStats != null ? _characterStats.MaxMana : 0f;

    public float CurrentRage => _currentRage;
    public float MaxRage => _characterStats != null ? _characterStats.MaxRage : 0f;

    public bool HasStamina => _characterStats != null && _characterStats.HasStamina;
    public bool HasMana => _characterStats != null && _characterStats.HasMana;
    public bool HasRage => _characterStats != null && _characterStats.HasRage;
    public bool CanFight => _characterStats != null && _characterStats.CanFight;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeStats();
    }

    private void InitializeStats()
    {
        if (_characterStats == null) return;

        _currentStamina = _characterStats.MaxStamina;
        _currentMana = _characterStats.MaxMana;
        _currentRage = 0f;

        OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
        OnManaChanged?.Invoke(_currentMana, MaxMana);
        OnRageChanged?.Invoke(_currentRage, MaxRage);
    }

    private void Update()
    {
        if (_characterStats == null) return;

        float dt = Time.deltaTime;

        // Stamina regen
        if (_characterStats.HasStamina && _currentStamina < _characterStats.MaxStamina)
        {
            ModifyStamina(_characterStats.StaminaRegenRate * dt);
        }

        // Mana regen
        if (_characterStats.HasMana && _currentMana < _characterStats.MaxMana)
        {
            ModifyMana(_characterStats.ManaRegenRate * dt);
        }

        // Rage decay
        if (_characterStats.HasRage && _currentRage > 0f)
        {
            ModifyRage(-_characterStats.RageDecayRate * dt);
        }
    }

    public void SetCharacter(CharacterStats stats)
    {
        _characterStats = stats;
        InitializeStats();
    }

    public bool UseStamina(float amount)
    {
        if (!HasStamina || _currentStamina < amount)
            return false;

        ModifyStamina(-amount);
        return true;
    }

    public bool UseMana(float amount)
    {
        if (!HasMana || _currentMana < amount)
            return false;

        ModifyMana(-amount);
        return true;
    }

    public void AddRage(float amount)
    {
        if (!HasRage) return;
        ModifyRage(amount);
    }

    private void ModifyStamina(float amount)
    {
        _currentStamina = Mathf.Clamp(_currentStamina + amount, 0f, MaxStamina);
        OnStaminaChanged?.Invoke(_currentStamina, MaxStamina);
    }

    private void ModifyMana(float amount)
    {
        _currentMana = Mathf.Clamp(_currentMana + amount, 0f, MaxMana);
        OnManaChanged?.Invoke(_currentMana, MaxMana);
    }

    private void ModifyRage(float amount)
    {
        _currentRage = Mathf.Clamp(_currentRage + amount, 0f, MaxRage);
        OnRageChanged?.Invoke(_currentRage, MaxRage);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
