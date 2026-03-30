using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class BossController : MonoBehaviour
{
    public enum BossPhase { Idle, PhaseOne, PhaseTwo, Dead }

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _phase2SpeedBoost = 1.5f;

    [Header("Combat")]
    [SerializeField] private int _contactDamage = 2;
    [SerializeField] private float _attackCooldown = 1.5f;
    [SerializeField] private float _chargeSpeed = 8f;
    [SerializeField] private float _chargeDuration = 0.4f;

    [Header("Phase Transition")]
    [SerializeField] private float _phase2HealthPercent = 0.5f;

    [Header("Visuals")]
    [SerializeField] private Transform _visualRoot;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Color _phase2Color = Color.red;

    [Header("Events")]
    public UnityEngine.Events.UnityEvent OnBossDefeated;

    private Rigidbody2D _rb;
    private Health _health;
    private Transform _player;
    private BossPhase _currentPhase = BossPhase.Idle;
    private float _lastAttackTime = -999f;
    private bool _isCharging;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _health.Died += HandleDeath;
        _health.HealthChanged += CheckPhaseTransition;
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        _currentPhase = BossPhase.PhaseOne;
    }

    private void FixedUpdate()
    {
        if (_isDead || _player == null || _isCharging)
            return;

        float deltaX = _player.position.x - transform.position.x;
        UpdateFacing(deltaX);

        float speed = _currentPhase == BossPhase.PhaseTwo
            ? _moveSpeed * _phase2SpeedBoost
            : _moveSpeed;

        _rb.linearVelocity = new Vector2(Mathf.Sign(deltaX) * speed, _rb.linearVelocity.y);

        TryAttack(deltaX);
    }

    private void TryAttack(float deltaX)
    {
        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        float dist = Mathf.Abs(deltaX);

        if (dist < 1.5f)
        {
            // Contact damage
            _lastAttackTime = Time.time;
            IDamageable damageable = _player.GetComponent<IDamageable>();
            damageable?.TakeDamage(_contactDamage);
        }
        else if (_currentPhase == BossPhase.PhaseTwo && dist < 6f)
        {
            // Phase 2 charge attack
            _lastAttackTime = Time.time;
            StartCoroutine(ChargeAttack(Mathf.Sign(deltaX)));
        }
    }

    private IEnumerator ChargeAttack(float direction)
    {
        _isCharging = true;
        float elapsed = 0f;

        while (elapsed < _chargeDuration)
        {
            _rb.linearVelocity = new Vector2(direction * _chargeSpeed, _rb.linearVelocity.y);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isCharging = false;
    }

    private void CheckPhaseTransition(int current, int max)
    {
        if (_currentPhase == BossPhase.PhaseOne &&
            current <= Mathf.RoundToInt(max * _phase2HealthPercent))
        {
            EnterPhaseTwo();
        }
    }

    private void EnterPhaseTwo()
    {
        _currentPhase = BossPhase.PhaseTwo;
        _attackCooldown *= 0.6f;

        if (_spriteRenderer != null)
            _spriteRenderer.color = _phase2Color;

        Debug.Log("[Boss] Entering Phase 2!");
    }

    private void UpdateFacing(float deltaX)
    {
        if (_visualRoot == null || Mathf.Abs(deltaX) < 0.01f)
            return;

        Vector3 scale = _visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (deltaX >= 0f ? 1f : -1f);
        _visualRoot.localScale = scale;
    }

    private void HandleDeath()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;
        _currentPhase = BossPhase.Dead;

        Debug.Log("[Boss] Defeated!");
        OnBossDefeated?.Invoke();

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        // Flash effect
        if (_spriteRenderer != null)
        {
            for (int i = 0; i < 6; i++)
            {
                _spriteRenderer.enabled = !_spriteRenderer.enabled;
                yield return new WaitForSeconds(0.15f);
            }
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.Died -= HandleDeath;
            _health.HealthChanged -= CheckPhaseTransition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
        Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, 6f);
    }
}
