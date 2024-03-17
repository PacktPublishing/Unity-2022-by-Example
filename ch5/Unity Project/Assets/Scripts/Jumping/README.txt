Jumping README
___
The PlayerController.cs, as shown in this project, still needs to implement jumping. You can refer to the scripts provided in this folder while implementing jumping based on where the PlayerController script left off in Chapter 5 by following these steps:

1. In PlayerController.cs add the following variable declarations (note you’ll be experimenting with the jump force value as you playtest):

    [Header("Jump")]
    [SerializeField] private float _jumpForce = 85f;
    private IGroundCheck _groundCheck;

2. Add the following lines to the Awake() method to find and assign the ground check component – we cannot allow the player to jump unless we know he’s standing on the ground so we use a ground checking approach to determine the player’s “is grounded” state (we’ll add the required objects to the Player object in the following steps):

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        _groundCheck = GetComponentInChildren<IGroundCheck>();
        if (_groundCheck == null)
            Debug.LogWarning("No ground check component found on the Player - you cannot jump!");
    }

3. Add an Update() method and create the new method for updating animation parameters (yes, you’ll have to make a jump animation in addition to the idle and run animations):

    void Update() => UpdateAnimatorParameters();

    private void UpdateAnimatorParameters()
    {
        _animator.SetBool("Running", _movementInput.x != 0f);
        _animator.SetBool("Jumping", !_groundCheck.IsGrounded);
    }
}

4. Now add the OnJump() method what will respond to the input action for jumping – we don’t need to do anything else for this to be called since it’s already included in our action map:

    void OnJump()
    {
        if (_groundCheck == null || !_groundCheck.IsGrounded)
            return;     // Short circuit.
        
        _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _groundCheck.SetGrounded(false);
    }

5. In the UpdateVelocity() method, comment out (or delete) setting the animation parameter for running since we accounted for it in the UpdateAnimatorParameter() method we added to the class in step #3 above:

    private void UpdateVelocity()
    {
        …
        //_animator.SetBool("Running", _movementInput.x != 0f);
    }

6. Create a new script to implement the ground check as an interface – we don’t discuss interfaces until Chapter 6 so don’t worry too much about what you’re doing here for now if you’re not yet familiar with interfaes. Create the script in the Assets/Scripts/Interfaces folder and name it “IGroundCheck” with the following code:

internal interface IGroundCheck
{
    bool IsGrounded { get; }
    void SetGrounded(bool grounded);
}

7. Now we’ll create the class that will implements the interface. In the Assets/Scripts folder, create a new script named “GroundCheckTrigger” with the following code:

using UnityEngine;

public class GroundCheckTrigger : MonoBehaviour, IGroundCheck
{
    public bool IsGrounded => _isGrounded;
    private bool _isGrounded;

    [SerializeField] private LayerMask _groundMask;
    public void SetGrounded(bool grounded) => _isGrounded = grounded;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_groundMask.IsLayerInMask(collision.gameObject.layer))
        {
            _isGrounded = true;
        }
    }
}

8. Now, in the Unity Editor find and open the Player Prefab in Prefab mode.
  a. Add a new child GameObject and name it “GroundCheck”
  b. Add the GroundCheckTrigger script to it and select “Ground” in the GroundMask layer mask dropdown.
  c. Add a CircleCollider2D component and set its Radius to 0.3 (for a starting value, you’re going to have to playtest to fine tune the ground check for the best playability)

And that’s it!
