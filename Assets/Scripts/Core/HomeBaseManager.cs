using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HomeBaseManager : MonoBehaviour
{
    public static HomeBaseManager Instance { get; private set; }

    [Header("Scene Settings")]
    [SerializeField] private string _dungeonSceneName = "SideScroll_TestScene";

    [Header("UI Panels")]
    [SerializeField] private GameObject _craftingPanel;
    [SerializeField] private GameObject _storagePanel;
    [SerializeField] private GameObject _shopPanel;
    [SerializeField] private GameObject _skillsPanel;
    [SerializeField] private GameObject _interactPrompt;

    [Header("Interaction")]
    [SerializeField] private float _interactRadius = 2f;

    private GameObject _activePanel;
    private bool _anyPanelOpen => _activePanel != null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Close any open panel with Escape
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame && _anyPanelOpen)
            CloseAllPanels();
    }

    public void EnterDungeon()
    {
        Debug.Log("[HomeBase] Entering dungeon: " + _dungeonSceneName);
        SceneManager.LoadScene(_dungeonSceneName);
    }

    public void OpenCrafting()
    {
        TogglePanel(_craftingPanel);
    }

    public void OpenStorage()
    {
        TogglePanel(_storagePanel);
    }

    public void OpenShop()
    {
        TogglePanel(_shopPanel);
    }

    public void OpenSkills()
    {
        TogglePanel(_skillsPanel);
    }

    public void CloseAllPanels()
    {
        SetPanel(_craftingPanel, false);
        SetPanel(_storagePanel, false);
        SetPanel(_shopPanel, false);
        SetPanel(_skillsPanel, false);
        _activePanel = null;
    }

    private void TogglePanel(GameObject panel)
    {
        if (panel == null) return;

        if (_activePanel == panel)
        {
            CloseAllPanels();
            return;
        }

        CloseAllPanels();
        SetPanel(panel, true);
        _activePanel = panel;
    }

    private void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    public void ShowInteractPrompt(string text, bool show)
    {
        if (_interactPrompt != null)
        {
            _interactPrompt.SetActive(show);
            var tmp = _interactPrompt.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null && show)
                tmp.text = text;
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
