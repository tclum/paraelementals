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
            // Fallback if no death screen in scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Died -= HandleDeath;
    }
}
