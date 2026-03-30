using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject _mainPanel;
    [SerializeField] private GameObject _creditsPanel;

    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _creditsBackButton;
    [SerializeField] private Button _quitButton;

    [Header("Settings")]
    [SerializeField] private string _gameSceneName = "SideScroll_TestScene";
    [SerializeField] private float _fadeOutDuration = 0.8f;

    [Header("Fade")]
    [SerializeField] private CanvasGroup _fadePanel;

    private void Awake()
    {
        if (_playButton != null)
            _playButton.onClick.AddListener(OnPlayClicked);

        if (_creditsButton != null)
            _creditsButton.onClick.AddListener(OnCreditsClicked);

        if (_creditsBackButton != null)
            _creditsBackButton.onClick.AddListener(OnCreditsBack);

        if (_quitButton != null)
            _quitButton.onClick.AddListener(OnQuitClicked);

        // Start faded out then fade in
        if (_fadePanel != null)
        {
            _fadePanel.alpha = 1f;
            StartCoroutine(FadeIn());
        }

        ShowMainPanel();
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (_fadePanel != null)
                _fadePanel.alpha = 1f - Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        if (_fadePanel != null)
            _fadePanel.alpha = 0f;
    }

    private void OnPlayClicked()
    {
        StartCoroutine(LoadSceneWithFade(_gameSceneName));
    }

    private void OnCreditsClicked()
    {
        if (_mainPanel != null) _mainPanel.SetActive(false);
        if (_creditsPanel != null) _creditsPanel.SetActive(true);
    }

    private void OnCreditsBack()
    {
        ShowMainPanel();
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void ShowMainPanel()
    {
        if (_mainPanel != null) _mainPanel.SetActive(true);
        if (_creditsPanel != null) _creditsPanel.SetActive(false);
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Fade out
        float elapsed = 0f;
        while (elapsed < _fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            if (_fadePanel != null)
                _fadePanel.alpha = Mathf.Clamp01(elapsed / _fadeOutDuration);
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}
