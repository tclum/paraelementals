using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    private Collider2D _collider;
    private int _damage = 1;
    private readonly HashSet<IDamageable> _hitTargets = new HashSet<IDamageable>();

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    public void EnableHitbox()
    {
        _hitTargets.Clear();
        _collider.enabled = true;
        Debug.Log("Attack hitbox enabled");
    }

    public void DisableHitbox()
    {
        _collider.enabled = false;
        _hitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
            return;

        if (_hitTargets.Contains(damageable))
            return;

        _hitTargets.Add(damageable);
        damageable.TakeDamage(_damage);

        Debug.Log("Hit " + other.name + " for " + _damage);
    }
}