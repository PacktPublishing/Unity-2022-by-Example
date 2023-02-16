using UnityEngine;

public interface IBehaviorPatrolWaypoints
{
    Transform WaypointPatrolLeft { get; }
    Transform WaypointPatrolRight { get; }

    void Init(Rigidbody2D rb, Vector2 direction, float acceleration, float speedMax);
    void TickPhysics();
}
