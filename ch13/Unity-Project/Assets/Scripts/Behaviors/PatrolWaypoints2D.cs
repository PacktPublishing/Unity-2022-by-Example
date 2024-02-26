using UnityEngine;

public class PatrolWaypoints2D : MonoBehaviour, IBehaviorPatrolWaypoints2D
{
    public Transform WaypointPatrolLeft => _waypointPatrolLeft;
    public Transform WaypointPatrolRight => _waypointPatrolRight;

    [Header("Patrol Waypoints")]
    [SerializeField] private Transform _waypointPatrolLeft;

    [SerializeField] private Transform _waypointPatrolRight;

    private Rigidbody2D _rb;
    private Vector2 _movementDirection;
    private float _acceleration;
    private float _speedMax;

    public void Init(Rigidbody2D rb, Vector2 direction, float acceleration, float speedMax)
    {
        _rb = rb;
        _movementDirection = direction;
        _acceleration = acceleration;
        _speedMax = speedMax;
    }

    public void TickPhysics()
    {
        UpdateDirection();
        UpdateVelocity();
    }

    private void UpdateDirection()
    {
        // Flip the direction of the object depending on direction of movement using scale.
        transform.localScale = Vector3.one;
        if (_movementDirection.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    private void UpdateVelocity()
    {
        if (transform.position.x >= _waypointPatrolRight.position.x)
        {
            _movementDirection = Vector2.left;
        }
        else if (transform.position.x <= _waypointPatrolLeft.position.x)
        {
            _movementDirection = Vector2.right;
        }

        var velocity = _rb.velocity;
        velocity += _acceleration * Time.fixedDeltaTime * _movementDirection;
        velocity.x = Mathf.Clamp(velocity.x, -_speedMax, _speedMax);
        _rb.velocity = velocity;
    }

    // ADDED: Chapter 8 - Enemy Spawner
    public void SetWaypoints(Transform left, Transform right)
    {
        _waypointPatrolLeft = left;
        _waypointPatrolRight = right;
    }
}