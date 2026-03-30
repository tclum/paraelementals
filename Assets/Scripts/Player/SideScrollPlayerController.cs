using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class SideScrollPlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask _groundLayer;

    [Header("Visuals")]
    [SerializeField] private Transform _visualRoot;

    private Rigidbody2D _rb;
    private float _moveInput;
    private bool _jumpQueued;
    private bool _isGrounded;
    private bool _facingRight = true;

    public float MoveInput => _moveInput;
    public bool IsGrounded => _isGrounded;
    public bool FacingRight => _facingRight;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _moveInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) _moveInput -= 1f;
            if (Keyboard.current.dKey.isPressed) _moveInput += 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame && _isGrounded)
            {
                _jumpQueued = true;
            }
        }

        UpdateGroundedState();
        UpdateFacing();
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_moveInput * _moveSpeed, _rb.linearVelocity.y);

        if (_jumpQueued)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            _jumpQueued = false;
        }
    }

    private void UpdateGroundedState()
    {
        if (_groundCheck == null)
        {
            _isGrounded = false;
            return;
        }

        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer) != null;
    }

    private void UpdateFacing()
    {
        if (_moveInput > 0.01f)
        {
            SetFacing(true);
        }
        else if (_moveInput < -0.01f)
        {
            SetFacing(false);
        }
    }

    private void SetFacing(bool faceRight)
    {
        if (_facingRight == faceRight)
            return;

        _facingRight = faceRight;

        if (_visualRoot != null)
        {
            Vector3 scale = _visualRoot.localScale;
            scale.x = Mathf.Abs(scale.x) * (_facingRight ? 1f : -1f);
            _visualRoot.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_groundCheck == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
    }
}
