using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SideScrollPlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SideScrollPlayerController _controller;
    [SerializeField] private AttackHitbox _attackHitbox;
    [SerializeField] private Transform _attackPivot;

    [Header("Combat")]
    [SerializeField] private int _baseDamage = 1;
    [SerializeField] private float _attackCooldown = 0.35f;
    [SerializeField] private float _attackActiveTime = 0.12f;
    [SerializeField] private float _attackOffsetX = 0.8f;

    private float _lastAttackTime = -999f;
    private bool _isAttacking;

    private void Update()
    {
        UpdateHitboxPosition();

        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            TryAttack();
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void UpdateHitboxPosition()
    {
        if (_attackPivot == null || _controller == null)
            return;

        Vector3 localPos = _attackPivot.localPosition;
        localPos.x = _controller.FacingRight ? Mathf.Abs(_attackOffsetX) : -Mathf.Abs(_attackOffsetX);
        _attackPivot.localPosition = localPos;
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
