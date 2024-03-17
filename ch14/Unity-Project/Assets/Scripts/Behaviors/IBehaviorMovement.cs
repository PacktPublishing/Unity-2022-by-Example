using UnityEngine;

internal interface IBehaviorMovement
{
    void Init(float speed);
    void TickPhysics();
    void SetTarget(Transform target);
}