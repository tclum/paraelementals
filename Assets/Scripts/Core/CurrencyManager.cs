using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private int _shards = 0;

    public int Shards => _shards;

    public event Action<int> OnShardsChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved shards
        _shards = PlayerPrefs.GetInt("Shards", 0);
    }

    public void AddShards(int amount)
    {
        if (amount <= 0) return;
        _shards += amount;
        PlayerPrefs.SetInt("Shards", _shards);
        PlayerPrefs.Save();
        OnShardsChanged?.Invoke(_shards);
        Debug.Log($"[Currency] +{amount} shards. Total: {_shards}");
    }

    public bool SpendShards(int amount)
    {
        if (_shards < amount) return false;
        _shards -= amount;
        PlayerPrefs.SetInt("Shards", _shards);
        PlayerPrefs.Save();
        OnShardsChanged?.Invoke(_shards);
        return true;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
