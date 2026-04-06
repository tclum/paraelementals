using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _quitButton;

    [Header("Settings")]
    [SerializeField] private string _mainMenuScene = "MainMenu";

    private bool _isPaused;

    public bool IsPaused => _isPaused;

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

        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (_quitButton != null)
            _quitButton.onClick.AddListener(QuitGame);

        if (_pausePanel != null)
            _pausePanel.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (_isPaused)
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
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        _isPaused = false;

        // Destroy persistent player
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
