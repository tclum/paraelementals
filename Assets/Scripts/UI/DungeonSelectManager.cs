using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DungeonEntry
{
    public string DungeonName;
    public string SceneName;
    public string Description;
    public string Difficulty;
    public bool IsUnlocked;
    public Sprite PreviewImage;
}

public class DungeonSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform _dungeonListContainer;
    [SerializeField] private GameObject _dungeonButtonPrefab;
    [SerializeField] private TextMeshProUGUI _selectedNameText;
    [SerializeField] private TextMeshProUGUI _selectedDescriptionText;
    [SerializeField] private TextMeshProUGUI _selectedDifficultyText;
    [SerializeField] private Image _selectedPreviewImage;
    [SerializeField] private Button _enterButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private CanvasGroup _fadePanel;

    [Header("Dungeons")]
    [SerializeField] private DungeonEntry[] _dungeons;

    [Header("Settings")]
    [SerializeField] private string _homeBaseScene = "HomeBase";
    [SerializeField] private float _fadeDuration = 0.5f;

    private DungeonEntry _selectedDungeon;

    private void Awake()
    {
        if (_enterButton != null)
            _enterButton.onClick.AddListener(OnEnterClicked);

        if (_backButton != null)
            _backButton.onClick.AddListener(OnBackClicked);

        if (_fadePanel != null)
        {
            _fadePanel.alpha = 1f;
            StartCoroutine(FadeIn());
        }
    }

    private void Start()
    {
        PopulateDungeonList();

        // Auto-select first unlocked dungeon
        if (_dungeons != null)
        {
            foreach (var d in _dungeons)
            {
                if (d.IsUnlocked)
                {
                    SelectDungeon(d);
                    break;
                }
            }
        }

        UpdateEnterButton();
    }

    private void PopulateDungeonList()
    {
        if (_dungeonListContainer == null || _dungeonButtonPrefab == null || _dungeons == null)
            return;

        foreach (Transform child in _dungeonListContainer)
            Destroy(child.gameObject);

        foreach (DungeonEntry dungeon in _dungeons)
        {
            GameObject btn = Instantiate(_dungeonButtonPrefab, _dungeonListContainer);
            DungeonButton db = btn.GetComponent<DungeonButton>();
            if (db != null)
                db.Setup(dungeon, this);
        }
    }

    public void SelectDungeon(DungeonEntry dungeon)
    {
        _selectedDungeon = dungeon;

        if (_selectedNameText != null)
            _selectedNameText.text = dungeon.DungeonName;

        if (_selectedDescriptionText != null)
            _selectedDescriptionText.text = dungeon.Description;

        if (_selectedDifficultyText != null)
            _selectedDifficultyText.text = "Difficulty: " + dungeon.Difficulty;

        if (_selectedPreviewImage != null && dungeon.PreviewImage != null)
            _selectedPreviewImage.sprite = dungeon.PreviewImage;

        UpdateEnterButton();
    }

    private void UpdateEnterButton()
    {
        if (_enterButton != null)
            _enterButton.interactable = _selectedDungeon != null && _selectedDungeon.IsUnlocked;
    }

    public void OnEnterClicked()
    {
        if (_selectedDungeon == null || !_selectedDungeon.IsUnlocked)
            return;

        StartCoroutine(LoadSceneWithFade(_selectedDungeon.SceneName));
    }

    public void OnBackClicked()
    {
        StartCoroutine(LoadSceneWithFade(_homeBaseScene));
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (_fadePanel != null)
                _fadePanel.alpha = 1f - Mathf.Clamp01(elapsed / _fadeDuration);
            yield return null;
        }
        if (_fadePanel != null)
            _fadePanel.alpha = 0f;
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (_fadePanel != null)
                _fadePanel.alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
