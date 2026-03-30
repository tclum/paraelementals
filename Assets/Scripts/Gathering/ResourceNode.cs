using UnityEngine;
using System.Collections;

public class ResourceNode : MonoBehaviour, IInteractable
{
    [Header("Resource Node")]
    [SerializeField] private ItemData _dropItem;
    [SerializeField] private int _dropAmount = 1;
    [SerializeField] private WorldItemPickup _pickupPrefab;

    [Header("Respawn")]
    [SerializeField] private float _respawnTime = 10f;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    public void Interact(PlayerInteractor interactor)
    {
        Gather();
    }

    public string GetInteractionPrompt()
    {
        return "Gather";
    }

    private void Gather()
    {
        if (_dropItem == null || _pickupPrefab == null)
            return;

        for (int i = 0; i < _dropAmount; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-3f, 3f),
                Random.Range(-3f, 3f),
                0f
            );

            WorldItemPickup pickup =
                Instantiate(_pickupPrefab, spawnPosition, Quaternion.identity);

            pickup.Initialize(_dropItem, 1);
        }

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        _spriteRenderer.enabled = false;
        _collider.enabled = false;

        yield return new WaitForSeconds(_respawnTime);

        _spriteRenderer.enabled = true;
        _collider.enabled = true;
    }
}