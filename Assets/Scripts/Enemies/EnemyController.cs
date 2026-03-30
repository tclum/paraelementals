using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectionRange = 6f;
    [SerializeField] private float _attackRange = 1f;
    [SerializeField] private int _contactDamage = 1;
    [SerializeField] private float _attackCooldown = 1f;

    private Rigidbody2D _rb;
    private Health _health;
    private Transform _player;
    private float _lastAttackTime = -999f;
    private bool _isDead;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<Health>();
        _health.Died += HandleDeath;
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
    }

    private void FixedUpdate()
    {
        if (_isDead || _player == null)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > _detectionRange)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        if (distance > _attackRange)
        {
            Vector2 direction = (_player.position - transform.position).normalized;
            _rb.linearVelocity = direction * _moveSpeed;
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            TryAttackPlayer();
        }
    }

    private void TryAttackPlayer()
    {
        if (Time.time < _lastAttackTime + _attackCooldown)
            return;

        _lastAttackTime = Time.time;

        IDamageable damageable = _player.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(_contactDamage);
            Debug.Log("Enemy hit player for " + _contactDamage);
        }
    }

    private void HandleDeath()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        LootDropper lootDropper = GetComponent<LootDropper>();
        if (lootDropper != null)
        {
            lootDropper.DropLoot();
        }

        Debug.Log(gameObject.name + " died");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.Died -= HandleDeath;
        }
    }
}