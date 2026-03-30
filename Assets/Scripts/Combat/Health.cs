using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHealth = 10;

    private int _currentHealth;

    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        _currentHealth = _maxHealth;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || _currentHealth <= 0)
            return;

        _currentHealth -= amount;
        if (_currentHealth < 0)
            _currentHealth = 0;

        HealthChanged?.Invoke(_currentHealth, _maxHealth);

        if (_currentHealth == 0)
        {
            Died?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || _currentHealth <= 0)
            return;

        _currentHealth += amount;
        if (_currentHealth > _maxHealth)
            _currentHealth = _maxHealth;

        HealthChanged?.Invoke(_currentHealth, _maxHealth);
    }
}