using UnityEngine;

public interface IBehaviorPatrolWaypoints
{
    public Transform WaypointPatrolLeft { get; }
    public Transform WaypointPatrolRight { get; }

    public void Init(Rigidbody2D rb, Vector2 direction, float acceleration, float speedMax);
    public void TickPhysics();
}
