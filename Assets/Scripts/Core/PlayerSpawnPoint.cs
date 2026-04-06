using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [SerializeField] private bool _movePlayerOnStart = true;

    private void Start()
    {
        if (!_movePlayerOnStart) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = transform.position;
            Debug.Log("[SpawnPoint] Player moved to spawn: " + transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 2f);
    }
}
