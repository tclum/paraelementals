using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackHitbox : MonoBehaviour
{
    private Collider2D _collider;
    private int _damage = 1;
    private Element _element = Element.None;
    private readonly HashSet<IDamageable> _hitTargets = new HashSet<IDamageable>();

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void SetDamage(int damage) => _damage = damage;
    public void SetElement(Element element) => _element = element;

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
        if (damageable == null || _hitTargets.Contains(damageable))
            return;

        _hitTargets.Add(damageable);

        int finalDamage = _damage;
        ElementalEntity targetElement = other.GetComponent<ElementalEntity>();

        if (targetElement != null && _element != Element.None)
        {
            finalDamage = targetElement.CalculateIncomingDamage(_damage, _element);

            if (targetElement.RollStatusEffect(_element))
            {
                ElementalStatus status = other.GetComponent<ElementalStatus>();
                if (status != null)
                {
                    StatusEffect effect = ElementalSystem.GetStatusEffect(_element);
                    Vector2 knockbackDir = (other.transform.position - transform.position).normalized;
                    status.ApplyStatus(effect, _element, knockbackDir);
                    Debug.Log($"[Elemental] Applied {effect} to {other.name}");
                }
            }

            float multiplier = ElementalSystem.GetMultiplier(_element, targetElement.EntityElement);
            if (multiplier > 1f)
                Debug.Log($"[Elemental] Super effective! {_element} vs {targetElement.EntityElement}");
            else if (multiplier < 1f)
                Debug.Log($"[Elemental] Not very effective... {_element} vs {targetElement.EntityElement}");
        }

        damageable.TakeDamage(finalDamage);
        Debug.Log($"Hit {other.name} for {finalDamage} ({_element})");
    }
}
