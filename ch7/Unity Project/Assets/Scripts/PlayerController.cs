using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Actor")]
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField] private float _acceleration = 0.0f;
    [SerializeField] private float _speedMax = 0.0f;

    private Rigidbody2D _rb;
    private Vector2 _movementInput;

    void Awake() => _rb = GetComponent<Rigidbody2D>();
    void FixedUpdate() => UpdateVelocity();


    void OnMove(InputValue value)
    {
        var move = value.Get<Vector2>();
        _movementInput = (move.x != 0f) ? new Vector2(move.x, 0f) : Vector2.zero;

        UpdateDirection();
    }

    private void UpdateVelocity()
    {
        var velocity = _rb.velocity;
        velocity += _acceleration * Time.fixedDeltaTime * _movementInput;
        velocity.x = Mathf.Clamp(velocity.x, -_speedMax, _speedMax);
        _rb.velocity = velocity;

        _animator.SetBool("Running", _movementInput.x != 0f);
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
}