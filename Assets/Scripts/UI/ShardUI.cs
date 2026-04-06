using UnityEngine;
using TMPro;

public class ShardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _shardText;

    private void Start()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnShardsChanged += UpdateDisplay;
            UpdateDisplay(CurrencyManager.Instance.Shards);
        }
    }

    private void UpdateDisplay(int amount)
    {
        if (_shardText != null)
            _shardText.text = $"{amount} Shards";
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnShardsChanged -= UpdateDisplay;
    }
}
