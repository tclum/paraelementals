using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CharacterRoster _roster;

    [Header("UI")]
    [SerializeField] private GameObject _panel;
    [SerializeField] private Transform _characterListContainer;
    [SerializeField] private GameObject _characterCardPrefab;

    [Header("Selected Info")]
    [SerializeField] private Image _previewImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _statsText;
    [SerializeField] private TextMeshProUGUI _unlockHintText;

    [Header("Buttons")]
    [SerializeField] private Button _confirmButton;
    [SerializeField] private Button _closeButton;

    private CharacterEntry _selectedEntry;
    private List<CharacterCardUI> _cards = new List<CharacterCardUI>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_confirmButton != null)
            _confirmButton.onClick.AddListener(OnConfirm);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(Close);

        if (_panel != null)
            _panel.SetActive(false);
    }

    public void Open()
    {
        if (_panel != null)
            _panel.SetActive(true);

        PopulateCharacterList();

        CharacterEntry current = _roster?.GetSelectedCharacter();
        if (current != null)
            SelectCharacter(current);
    }

    public void Close()
    {
        if (_panel != null)
            _panel.SetActive(false);
    }

    private void PopulateCharacterList()
    {
        if (_characterListContainer == null || _characterCardPrefab == null || _roster == null)
            return;

        foreach (Transform child in _characterListContainer)
            Destroy(child.gameObject);

        _cards.Clear();

        foreach (CharacterEntry entry in _roster.Characters)
        {
            GameObject cardGO = Instantiate(_characterCardPrefab, _characterListContainer);
            CharacterCardUI card = cardGO.GetComponent<CharacterCardUI>();
            if (card != null)
            {
                card.Setup(entry, this);
                _cards.Add(card);
            }
        }
    }

    public void SelectCharacter(CharacterEntry entry)
    {
        _selectedEntry = entry;

        foreach (var card in _cards)
            card.SetSelected(card.Entry == entry);

        if (_nameText != null)
            _nameText.text = entry.Stats != null ? entry.Stats.CharacterName : entry.CharacterId;

        if (_previewImage != null && entry.Stats?.CharacterSprite != null)
            _previewImage.sprite = entry.Stats.CharacterSprite;

        if (_statsText != null && entry.Stats != null)
        {
            string stats = "";
            if (entry.Stats.HasHealth) stats += $"HP: {entry.Stats.MaxHealth}\n";
            if (entry.Stats.HasStamina) stats += $"Stamina: {entry.Stats.MaxStamina}\n";
            if (entry.Stats.HasMana) stats += $"Mana: {entry.Stats.MaxMana}\n";
            if (entry.Stats.HasRage) stats += $"Rage: {entry.Stats.MaxRage}\n";
            if (!entry.Stats.CanFight) stats += "Cannot fight";
            _statsText.text = stats.TrimEnd();
        }

        bool isUnlocked = entry.IsUnlocked;

        if (_unlockHintText != null)
        {
            _unlockHintText.gameObject.SetActive(!isUnlocked);
            _unlockHintText.text = isUnlocked ? "" : "🔒 " + entry.UnlockHint;
        }

        if (_confirmButton != null)
            _confirmButton.interactable = isUnlocked;
    }

    private void OnConfirm()
    {
        if (_selectedEntry == null || !_selectedEntry.IsUnlocked || _roster == null)
            return;

        _roster.SelectCharacter(_selectedEntry.CharacterId);

        if (PlayerStats.Instance != null && _selectedEntry.Stats != null)
            PlayerStats.Instance.SetCharacter(_selectedEntry.Stats);

        // Update player visual
        PlayerVisual visual = FindFirstObjectByType<PlayerVisual>();
        if (visual != null)
            visual.ApplyCurrentCharacter();

        Debug.Log($"[CharacterSelect] Confirmed: {_selectedEntry.CharacterId}");
        Close();
    }
}
