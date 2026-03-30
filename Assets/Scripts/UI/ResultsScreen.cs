using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ResultsScreen : MonoBehaviour
{
    public static ResultsScreen Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _enemiesKilledText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Settings")]
    [SerializeField] private float _fadeInDuration = 1f;
    [SerializeField] private string _mainMenuScene = "MainMenu";
    [SerializeField] private string _nextScene = "";

    // Stats tracking
    private int _enemiesKilled = 0;
    private float _levelStartTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _levelStartTime = Time.time;

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        if (_continueButton != null)
            _continueButton.onClick.AddListener(Continue);

        if (_mainMenuButton != null)
            _mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void RegisterEnemyKill()
    {
        _enemiesKilled++;
    }

    public void ShowResults()
    {
        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float elapsed = 0f;

        float timeTaken = Time.time - _levelStartTime;
        int minutes = Mathf.FloorToInt(timeTaken / 60f);
        int seconds = Mathf.FloorToInt(timeTaken % 60f);

        if (_enemiesKilledText != null)
            _enemiesKilledText.text = $"Enemies Defeated: {_enemiesKilled}";

        if (_timeText != null)
            _timeText.text = $"Time: {minutes:00}:{seconds:00}";

        while (elapsed < _fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            if (_canvasGroup != null)
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
            yield return null;
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
    }

    private void Continue()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(_nextScene))
            SceneManager.LoadScene(_nextScene);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuScene);
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
