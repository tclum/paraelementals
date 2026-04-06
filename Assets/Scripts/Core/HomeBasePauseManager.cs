using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class HomeBasePauseManager : MonoBehaviour
{
    public static HomeBasePauseManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _inventoryPanel;

    [Header("Pause Buttons")]
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _inventoryButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitButton;

    [Header("Inventory UI")]
    [SerializeField] private TextMeshProUGUI _shardCountText;
    [SerializeField] private Transform _inventorySlotContainer;
    [SerializeField] private GameObject _inventorySlotPrefab;
    [SerializeField] private Button _inventoryCloseButton;

    [Header("Settings")]
    [SerializeField] private string _mainMenuScene = "MainMenu";

    private bool _isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (_resumeButton != null)
            _resumeButton.onClick.AddListener(Resume);
        if (_inventoryButton != null)
            _inventoryButton.onClick.AddListener(OpenInventory);
        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(GoToMainMenu);
        if (_quitButton != null)
            _quitButton.onClick.AddListener(QuitGame);
        if (_inventoryCloseButton != null)
            _inventoryCloseButton.onClick.AddListener(CloseInventory);

        if (_pausePanel != null)
            _pausePanel.SetActive(false);
        if (_inventoryPanel != null)
            _inventoryPanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_inventoryPanel != null && _inventoryPanel.activeSelf)
                CloseInventory();
            else if (_isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        if (_pausePanel != null)
            _pausePanel.SetActive(true);
    }

    public void Resume()
    {
        _isPaused = false;
        Time.timeScale = 1f;
        if (_pausePanel != null)
            _pausePanel.SetActive(false);
        CloseInventory();
    }

    public void OpenInventory()
    {
        if (_inventoryPanel != null)
            _inventoryPanel.SetActive(true);

        RefreshInventory();
    }

    public void CloseInventory()
    {
        if (_inventoryPanel != null)
            _inventoryPanel.SetActive(false);
    }

    private void RefreshInventory()
    {
        // Update shard count
        if (_shardCountText != null)
        {
            int shards = CurrencyManager.Instance != null ? CurrencyManager.Instance.Shards : 0;
            _shardCountText.text = $"💎 {shards} Shards";
        }

        // Update inventory slots
        if (_inventorySlotContainer == null) return;

        foreach (Transform child in _inventorySlotContainer)
            Destroy(child.gameObject);

        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
        if (inventory == null) return;

        foreach (var slot in inventory.Slots)
        {
            if (slot.IsEmpty) continue;

            GameObject slotGO = _inventorySlotPrefab != null
                ? Instantiate(_inventorySlotPrefab, _inventorySlotContainer)
                : CreateDefaultSlot(_inventorySlotContainer);

            TextMeshProUGUI text = slotGO.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"{slot.Item.DisplayName}\nx{slot.Quantity}";

            Image icon = slotGO.GetComponentsInChildren<Image>().Length > 1
                ? slotGO.GetComponentsInChildren<Image>()[1]
                : null;
            if (icon != null && slot.Item.Icon != null)
                icon.sprite = slot.Item.Icon;
        }
    }

    private GameObject CreateDefaultSlot(Transform parent)
    {
        GameObject slot = new GameObject("InventorySlot");
        slot.transform.SetParent(parent, false);
        RectTransform rt = slot.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(160f, 60f);
        Image bg = slot.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.3f, 1f);
        LayoutElement le = slot.AddComponent<LayoutElement>();
        le.preferredWidth = 160f;
        le.preferredHeight = 60f;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(slot.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(8f, 4f);
        textRT.offsetMax = new Vector2(-8f, -4f);
        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 18f;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.MidlineLeft;

        return slot;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player.transform.root.gameObject);
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
