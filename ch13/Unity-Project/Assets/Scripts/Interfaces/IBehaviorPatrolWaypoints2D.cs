using UnityEngine;

public interface IBehaviorPatrolWaypoints2D
{
    Transform WaypointPatrolLeft { get; }
    Transform WaypointPatrolRight { get; }

    void Init(Rigidbody2D rb, Vector2 direction, float acceleration, float speedMax);

    void TickPhysics();

    // ADDED: Chapter 8.
    void SetWaypoints(Transform left, Transform right);
}