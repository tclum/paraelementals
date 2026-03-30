using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _difficultyText;
    [SerializeField] private Image _lockIcon;
    [SerializeField] private Button _button;
    [SerializeField] private Image _background;

    [Header("Colors")]
    [SerializeField] private Color _unlockedColor = new Color(0.15f, 0.15f, 0.35f, 1f);
    [SerializeField] private Color _lockedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    [SerializeField] private Color _selectedColor = new Color(0.3f, 0.3f, 0.6f, 1f);

    private DungeonEntry _entry;
    private DungeonSelectManager _manager;

    public void Setup(DungeonEntry entry, DungeonSelectManager manager)
    {
        _entry = entry;
        _manager = manager;

        if (_nameText != null)
            _nameText.text = entry.DungeonName;

        if (_difficultyText != null)
            _difficultyText.text = entry.Difficulty;

        if (_lockIcon != null)
            _lockIcon.gameObject.SetActive(!entry.IsUnlocked);

        if (_background != null)
            _background.color = entry.IsUnlocked ? _unlockedColor : _lockedColor;

        if (_button != null)
        {
            _button.interactable = entry.IsUnlocked;
            _button.onClick.AddListener(OnClicked);
        }
    }

    private void OnClicked()
    {
        if (_manager != null)
            _manager.SelectDungeon(_entry);
    }

    public void SetSelected(bool selected)
    {
        if (_background != null)
            _background.color = selected ? _selectedColor :
                (_entry.IsUnlocked ? _unlockedColor : _lockedColor);
    }
}
