using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    [SerializeField] private Image _background;
    [SerializeField] private Image _characterImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private GameObject _lockIcon;
    [SerializeField] private Button _button;

    [Header("Colors")]
    [SerializeField] private Color _unlockedColor = new Color(0.15f, 0.15f, 0.35f);
    [SerializeField] private Color _lockedColor = new Color(0.08f, 0.08f, 0.08f);
    [SerializeField] private Color _selectedColor = new Color(0.3f, 0.5f, 0.8f);

    public CharacterEntry Entry { get; private set; }
    private CharacterSelectManager _manager;

    public void Setup(CharacterEntry entry, CharacterSelectManager manager)
    {
        Entry = entry;
        _manager = manager;

        if (_nameText != null)
            _nameText.text = entry.Stats != null ? entry.Stats.CharacterName : entry.CharacterId;

        if (_characterImage != null && entry.Stats?.CharacterSprite != null)
            _characterImage.sprite = entry.Stats.CharacterSprite;

        if (_lockIcon != null)
            _lockIcon.SetActive(!entry.IsUnlocked);

        if (_background != null)
            _background.color = entry.IsUnlocked ? _unlockedColor : _lockedColor;

        if (_button != null)
            _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (_manager != null)
            _manager.SelectCharacter(Entry);
    }

    public void SetSelected(bool selected)
    {
        if (_background != null)
            _background.color = selected ? _selectedColor :
                (Entry.IsUnlocked ? _unlockedColor : _lockedColor);
    }
}
