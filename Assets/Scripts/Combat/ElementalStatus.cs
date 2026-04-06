using System.Collections;
using UnityEngine;

public class ElementalStatus : MonoBehaviour
{
    [Header("Status Settings")]
    [SerializeField] private float _burnDamagePerTick = 1f;
    [SerializeField] private float _burnTickRate = 0.5f;
    [SerializeField] private float _burnDuration = 3f;

    [SerializeField] private float _slowAmount = 0.5f;
    [SerializeField] private float _slowDuration = 3f;

    [SerializeField] private float _stunDuration = 1f;

    [SerializeField] private float _shockAmount = 0.5f;
    [SerializeField] private float _shockDuration = 3f;

    [SerializeField] private float _knockbackForce = 8f;

    // Current active effects
    private bool _isBurning;
    private bool _isSlowed;
    private bool _isStunned;
    private bool _isShocked;

    private Coroutine _burnCoroutine;
    private Coroutine _slowCoroutine;
    private Coroutine _stunCoroutine;
    private Coroutine _shockCoroutine;

    private Health _health;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;

    // Speed modifier accessors
    public bool IsStunned => _isStunned;
    public float SpeedMultiplier => _isSlowed ? _slowAmount : 1f;
    public float AttackSpeedMultiplier => _isShocked ? _shockAmount : 1f;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void ApplyStatus(StatusEffect effect, Element sourceElement, Vector2 attackDirection = default)
    {
        switch (effect)
        {
            case StatusEffect.Burn:
                ApplyBurn();
                break;
            case StatusEffect.Slow:
                ApplySlow();
                break;
            case StatusEffect.Stun:
                ApplyStun();
                break;
            case StatusEffect.Shock:
                ApplyShock();
                break;
            case StatusEffect.Knockback:
                ApplyKnockback(attackDirection);
                break;
        }
    }

    private void ApplyBurn()
    {
        if (_burnCoroutine != null)
            StopCoroutine(_burnCoroutine);
        _burnCoroutine = StartCoroutine(BurnRoutine());
    }

    private void ApplySlow()
    {
        if (_slowCoroutine != null)
            StopCoroutine(_slowCoroutine);
        _slowCoroutine = StartCoroutine(SlowRoutine());
    }

    private void ApplyStun()
    {
        if (_stunCoroutine != null)
            StopCoroutine(_stunCoroutine);
        _stunCoroutine = StartCoroutine(StunRoutine());
    }

    private void ApplyShock()
    {
        if (_shockCoroutine != null)
            StopCoroutine(_shockCoroutine);
        _shockCoroutine = StartCoroutine(ShockRoutine());
    }

    private void ApplyKnockback(Vector2 direction)
    {
        if (_rb == null) return;
        if (direction == Vector2.zero)
            direction = Vector2.right;
        _rb.AddForce(direction.normalized * _knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator BurnRoutine()
    {
        _isBurning = true;
        float elapsed = 0f;

        while (elapsed < _burnDuration)
        {
            if (_health != null)
                _health.TakeDamage(Mathf.RoundToInt(_burnDamagePerTick));

            FlashColor(new Color(1f, 0.3f, 0f));
            yield return new WaitForSeconds(_burnTickRate);
            elapsed += _burnTickRate;
        }

        _isBurning = false;
    }

    private IEnumerator SlowRoutine()
    {
        _isSlowed = true;
        FlashColor(new Color(0.3f, 0.6f, 1f));
        yield return new WaitForSeconds(_slowDuration);
        _isSlowed = false;
        ResetColor();
    }

    private IEnumerator StunRoutine()
    {
        _isStunned = true;
        FlashColor(new Color(0.8f, 0.7f, 0.2f));

        if (_rb != null)
            _rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(_stunDuration);
        _isStunned = false;
        ResetColor();
    }

    private IEnumerator ShockRoutine()
    {
        _isShocked = true;
        FlashColor(new Color(1f, 1f, 0.2f));
        yield return new WaitForSeconds(_shockDuration);
        _isShocked = false;
        ResetColor();
    }

    private void FlashColor(Color color)
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = color;
    }

    private void ResetColor()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.color = Color.white;
    }
}
