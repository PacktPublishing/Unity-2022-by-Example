using UnityEngine;

internal interface IBehaviorAttack
{
    void Init(Transform origin);

    void TickPhysics();
}