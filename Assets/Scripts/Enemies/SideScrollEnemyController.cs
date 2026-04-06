using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Health))]
public class SideScrollEnemyController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _detectionRange = 6f;
    [SerializeField] private float _attackRange = 1f;

    [Header("Combat")]
    [SerializeField] private int _contactDamage = 1;
    [SerializeField] private float _attackCooldown = 1f;

    [Header("Visuals")]
    [SerializeField] private Transform _visualRoot;

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
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            return;
        }

        float distance = Vector2.Distance(transform.position, _player.position);

        if (distance > _detectionRange)
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            return;
        }

        float deltaX = _player.position.x - transform.position.x;
        UpdateFacing(deltaX);

        if (Mathf.Abs(deltaX) > _attackRange)
        {
            float moveDir = Mathf.Sign(deltaX);
            _rb.linearVelocity = new Vector2(moveDir * _moveSpeed, _rb.linearVelocity.y);
        }
        else
        {
            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            TryAttackPlayer();
        }
    }

    private void UpdateFacing(float deltaX)
    {
        if (_visualRoot == null)
            return;

        if (Mathf.Abs(deltaX) < 0.01f)
            return;

        Vector3 scale = _visualRoot.localScale;
        scale.x = Mathf.Abs(scale.x) * (deltaX >= 0f ? 1f : -1f);
        _visualRoot.localScale = scale;
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
            Debug.Log(gameObject.name + " hit player for " + _contactDamage);
        }
    }

    private void HandleDeath()
    {
        _isDead = true;
        _rb.linearVelocity = Vector2.zero;

        if (LevelManager.Instance != null)
            LevelManager.Instance.RegisterEnemyKill();

        LootDropper lootDropper = GetComponent<LootDropper>();
        if (lootDropper != null)
            lootDropper.DropLoot();

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
