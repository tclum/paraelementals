using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private AttackHitbox _attackHitbox;
    [SerializeField] private int _baseDamage = 1;
    [SerializeField] private float _attackCooldown = 0.4f;
    [SerializeField] private float _attackActiveTime = 0.15f;

    private float _lastAttackTime = -999f;
    private bool _isAttacking;

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryAttack();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (_isAttacking)
            return;

        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        _lastAttackTime = Time.time;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        if (_attackHitbox != null)
        {
            _attackHitbox.SetDamage(_baseDamage);
            _attackHitbox.EnableHitbox();
        }

        yield return new WaitForSeconds(_attackActiveTime);

        if (_attackHitbox != null)
        {
            _attackHitbox.DisableHitbox();
        }

        _isAttacking = false;
    }
}