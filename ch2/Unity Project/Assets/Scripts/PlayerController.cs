using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private bool _shouldMoveForward;

    public Rigidbody2D Rb;
    public float MoveSpeed = 10f;
    public float SpriteRotationOffset = -90f;
    public float LookAtSpeed = 2f;
    
    private float _speedStartValue;

    // TODO: Cache the transform.


    // Update is called once per frame - process input
    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;  // Mouse added for bonus task?

        // Keyboard connected?
        if (keyboard == null || mouse == null)  // Mouse connected for bonus task?
            return;     // No keyboard connected so stop running code.

        if (keyboard.spaceKey.IsPressed() || mouse.leftButton.IsPressed())  // Mouse condition added for bonus task?
        {
            // Move while holding spacebar key down.
            _shouldMoveForward = true;
        }
        else if (keyboard.spaceKey.wasReleasedThisFrame || mouse.leftButton.wasReleasedThisFrame)   // Mouse condition added for bonus task.
        {
            // The spacebar key was released - stop moving.
            _shouldMoveForward = false;
        }

        LookAtMousePointer();
    }

    // FixedUpdate is called every physics fixed timestep
    private void FixedUpdate()
    {
        if (_shouldMoveForward)
        {
            // Process physics movement.
            // Up is the direction the object sprite is currently facing.
            Rb.velocity = transform.up * MoveSpeed;
        }
        else
        {
            // Stop movement.
            Rb.velocity = Vector2.zero;
        }
    }

    private void LookAtMousePointer()
    {
        var mouse = Mouse.current;
        if (mouse == null)
            return;

        var mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());

        var direction = (Vector2)mousePos - Rb.position;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + SpriteRotationOffset;

        // Direct rotation.
        //Rb.rotation = angle;

        // Interpolated rotation – smoothed.
        // The forward (Z-axis) is want we want to rotate on.
        var q = Quaternion.AngleAxis(angle, Vector3.forward);
        Rb.transform.rotation = Quaternion.Slerp(Rb.transform.rotation, q, Time.deltaTime * LookAtSpeed);
    }
}

