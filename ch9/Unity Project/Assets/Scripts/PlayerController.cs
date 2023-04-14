using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Actor")]
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField] private float _acceleration = 0.0f;
    [SerializeField] private float _speedMax = 0.0f;

    // ADDED: Jump added in Chapter 9.
    [Header("Jump")]
    [SerializeField] private float _jumpForce = 0.0f;
    private IGroundCheck _groundCheck;

    private Rigidbody2D _rb;
    private Vector2 _movementInput;


    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // ADDED: Jump added in Chapter 9.
        _groundCheck = GetComponentInChildren<IGroundCheck>();
        if (_groundCheck == null)
            Debug.LogWarning("No ground check component found on the Player - you cannot jump!");
    }

    // ADDED: Jump added in Chapter 9.
    void Update() => UpdateAnimatorParameters();

    void FixedUpdate() => UpdateVelocity();


    void OnMove(InputValue value)
    {
        var move = value.Get<Vector2>();
        _movementInput = (move.x != 0f) ? new Vector2(move.x, 0f) : Vector2.zero;

        UpdateDirection();
    }

    // ADDED: Jump added in Chapter 9.
    void OnJump()
    {
        if (_groundCheck == null || !_groundCheck.IsGrounded)
            return;     // Short circuit.
        
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _groundCheck.SetGrounded(false);
    }


    private void UpdateVelocity()
    {
        var velocity = _rb.velocity;
        velocity += _acceleration * Time.fixedDeltaTime * _movementInput;
        velocity.x = Mathf.Clamp(velocity.x, -_speedMax, _speedMax);
        _rb.velocity = velocity;

        // COMMENTED: Jump added in Chapter 9.
        //_animator.SetBool("Running", _movementInput.x != 0f);
    }

    private void UpdateDirection()
    {
        // only update the direction if the player is moving
        if (_movementInput.x != 0f)
        {
            // flip the direction of the player depending on direction of movement using scale
            transform.localScale = Vector3.one;
            if (_movementInput.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }


    // ADDED: Jump added in Chapter 9.
    private void UpdateAnimatorParameters()
    {
        _animator.SetBool("Running", _movementInput.x != 0f);
        _animator.SetBool("Jumping", !_groundCheck.IsGrounded);
    }
}