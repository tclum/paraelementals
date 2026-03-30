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
        Debug.Log("Player died - restarting scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.Died -= HandleDeath;
        }
    }
}
