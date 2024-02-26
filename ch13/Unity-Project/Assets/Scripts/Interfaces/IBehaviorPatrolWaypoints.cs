using UnityEngine;

public interface IBehaviorPatrolWaypoints
{
    void Init(float acceleration, float speedMax);

    void TickPhysics();
}