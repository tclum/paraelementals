using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    private Vector2 _lastMoveDirection = Vector2.down;

    public Vector2 MoveInput => _moveInput;
    public Vector2 LastMoveDirection => _lastMoveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) _moveInput.y += 1f;
            if (Keyboard.current.sKey.isPressed) _moveInput.y -= 1f;
            if (Keyboard.current.aKey.isPressed) _moveInput.x -= 1f;
            if (Keyboard.current.dKey.isPressed) _moveInput.x += 1f;
        }

        _moveInput = _moveInput.normalized;

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _lastMoveDirection = _moveInput;
        }

        Debug.Log("Move input: " + _moveInput);
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = _moveInput * _moveSpeed;
    }
}