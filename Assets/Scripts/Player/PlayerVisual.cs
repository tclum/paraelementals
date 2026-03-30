using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Apply current character sprite on start
        ApplyCurrentCharacter();
    }

    public void ApplyCurrentCharacter()
    {
        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("[PlayerVisual] PlayerStats.Instance is null");
            return;
        }

        if (PlayerStats.Instance.Stats == null)
        {
            Debug.LogWarning("[PlayerVisual] Stats is null");
            return;
        }

        Sprite sprite = PlayerStats.Instance.Stats.CharacterSprite;
        Debug.Log($"[PlayerVisual] Applying sprite: {(sprite != null ? sprite.name : "NULL")}");

        if (sprite != null && _spriteRenderer != null)
            _spriteRenderer.sprite = sprite;
        else if (sprite == null)
            Debug.LogWarning("[PlayerVisual] CharacterSprite is not assigned on CharacterStats");
    }
}
