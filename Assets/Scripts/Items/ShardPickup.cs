using System.Collections;
using UnityEngine;

public class ShardPickup : MonoBehaviour
{
    [SerializeField] private int _amount = 1;
    [SerializeField] private float _lifetime = 15f;
    [SerializeField] private float _bobSpeed = 2f;
    [SerializeField] private float _bobHeight = 0.15f;
    [SerializeField] private float _attractRadius = 2f;
    [SerializeField] private float _attractSpeed = 8f;

    private Transform _player;
    private Vector3 _startPos;
    private bool _attracting;
    private float _spawnTime;

    public void Initialize(int amount)
    {
        _amount = amount;
    }

    private void Start()
    {
        _startPos = transform.position;
        _spawnTime = Time.time;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;

        StartCoroutine(LifetimeFade());
    }

    private void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        if (dist <= _attractRadius)
        {
            _attracting = true;
        }

        if (_attracting)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, _player.position, _attractSpeed * Time.deltaTime);

            if (dist < 0.2f)
                Collect();
        }
        else
        {
            // Bob up and down
            float bobOffset = Mathf.Sin((Time.time - _spawnTime) * _bobSpeed) * _bobHeight;
            transform.position = new Vector3(_startPos.x, _startPos.y + bobOffset, _startPos.z);
        }
    }

    private void Collect()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.AddShards(_amount);

        Destroy(gameObject);
    }

    private IEnumerator LifetimeFade()
    {
        yield return new WaitForSeconds(_lifetime * 0.7f);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        float elapsed = 0f;
        float fadeDuration = _lifetime * 0.3f;
        Color startColor = sr.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _attractRadius);
    }
}
