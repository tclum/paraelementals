using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private GameObject _healthBarRoot;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private Image _healthFill;
    [SerializeField] private Color _healthColor = new Color(0.9f, 0.15f, 0.15f);

    [Header("Stamina Bar")]
    [SerializeField] private GameObject _staminaBarRoot;
    [SerializeField] private Slider _staminaSlider;
    [SerializeField] private TextMeshProUGUI _staminaText;
    [SerializeField] private Image _staminaFill;
    [SerializeField] private Color _staminaColor = new Color(0.2f, 0.8f, 0.2f);

    [Header("Mana Bar")]
    [SerializeField] private GameObject _manaBarRoot;
    [SerializeField] private Slider _manaSlider;
    [SerializeField] private TextMeshProUGUI _manaText;
    [SerializeField] private Image _manaFill;
    [SerializeField] private Color _manaColor = new Color(0.2f, 0.4f, 0.9f);

    [Header("Rage Bar")]
    [SerializeField] private GameObject _rageBarRoot;
    [SerializeField] private Slider _rageSlider;
    [SerializeField] private TextMeshProUGUI _rageText;
    [SerializeField] private Image _rageFill;
    [SerializeField] private Color _rageColor = new Color(0.9f, 0.3f, 0.05f);

    private Health _playerHealth;
    private PlayerStats _playerStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Apply fill colors
        SetFillColor(_healthFill, _healthColor);
        SetFillColor(_staminaFill, _staminaColor);
        SetFillColor(_manaFill, _manaColor);
        SetFillColor(_rageFill, _rageColor);

        FindAndBindPlayer();
    }

    private void FindAndBindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;

        // Bind health
        _playerHealth = playerObj.GetComponent<Health>();
        if (_playerHealth != null)
        {
            _playerHealth.HealthChanged += OnHealthChanged;
            OnHealthChanged(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
        }

        // Bind stats
        _playerStats = playerObj.GetComponent<PlayerStats>();
        if (_playerStats == null)
            _playerStats = PlayerStats.Instance;

        if (_playerStats != null)
        {
            _playerStats.OnStaminaChanged += OnStaminaChanged;
            _playerStats.OnManaChanged += OnManaChanged;
            _playerStats.OnRageChanged += OnRageChanged;

            RefreshHUDVisibility();

            // Fire initial values
            if (_playerStats.HasStamina)
                OnStaminaChanged(_playerStats.CurrentStamina, _playerStats.MaxStamina);
            if (_playerStats.HasMana)
                OnManaChanged(_playerStats.CurrentMana, _playerStats.MaxMana);
            if (_playerStats.HasRage)
                OnRageChanged(_playerStats.CurrentRage, _playerStats.MaxRage);
        }
    }

    private void RefreshHUDVisibility()
    {
        if (_playerStats == null) return;

        bool hasHealth = _playerHealth != null;
        SetActive(_healthBarRoot, hasHealth);
        SetActive(_staminaBarRoot, _playerStats.HasStamina);
        SetActive(_manaBarRoot, _playerStats.HasMana);
        SetActive(_rageBarRoot, _playerStats.HasRage);
    }

    private void OnHealthChanged(int current, int max)
    {
        UpdateBar(_healthSlider, _healthText, current, max);
    }

    private void OnStaminaChanged(float current, float max)
    {
        UpdateBar(_staminaSlider, _staminaText, current, max);
    }

    private void OnManaChanged(float current, float max)
    {
        UpdateBar(_manaSlider, _manaText, current, max);
    }

    private void OnRageChanged(float current, float max)
    {
        UpdateBar(_rageSlider, _rageText, current, max);
    }

    private void UpdateBar(Slider slider, TextMeshProUGUI text, float current, float max)
    {
        if (slider != null)
        {
            slider.minValue = 0f;
            slider.maxValue = max;
            slider.value = current;
        }

        if (text != null)
            text.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
    }

    private void SetFillColor(Image fill, Color color)
    {
        if (fill != null)
            fill.color = color;
    }

    private void SetActive(GameObject go, bool active)
    {
        if (go != null)
            go.SetActive(active);
    }

    private void OnDestroy()
    {
        if (_playerHealth != null)
            _playerHealth.HealthChanged -= OnHealthChanged;

        if (_playerStats != null)
        {
            _playerStats.OnStaminaChanged -= OnStaminaChanged;
            _playerStats.OnManaChanged -= OnManaChanged;
            _playerStats.OnRageChanged -= OnRageChanged;
        }

        if (Instance == this)
            Instance = null;
    }
}
