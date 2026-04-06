using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Health))]
public class SideScrollPlayerRespawn : MonoBehaviour
{
    private Health _health;

    private void Awake()
    {
        _health = GetComponent<Health>();
        _health.Died += HandleDeath;
    }

    private void HandleDeath()
    {
        Debug.Log("Player died.");

        if (DeathScreen.Instance != null)
        {
            DeathScreen.Instance.ShowDeathScreen();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void DestroyPersistentPlayer()
    {
        // Find and destroy the DontDestroyOnLoad player so a fresh one spawns
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player.transform.root.gameObject);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Died -= HandleDeath;
    }
}
