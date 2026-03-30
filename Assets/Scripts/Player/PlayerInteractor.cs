using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius = 0.75f;
    [SerializeField] private LayerMask _interactableMask;

    private IInteractable _currentInteractable;

    public IInteractable CurrentInteractable => _currentInteractable;

    private void Update()
    {
        RefreshCurrentInteractable();

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            _currentInteractable?.Interact(this);
        }
    }

    private void RefreshCurrentInteractable()
    {
        if (_interactionPoint == null)
            return;

        Collider2D hit = Physics2D.OverlapCircle(_interactionPoint.position, _interactionRadius, _interactableMask);

        if (hit != null)
        {
            _currentInteractable = hit.GetComponent<IInteractable>();
        }
        else
        {
            _currentInteractable = null;
        }
    }

    public Vector3 GetInteractionPosition()
    {
        return _interactionPoint != null ? _interactionPoint.position : transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        if (_interactionPoint == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_interactionPoint.position, _interactionRadius);
    }
}