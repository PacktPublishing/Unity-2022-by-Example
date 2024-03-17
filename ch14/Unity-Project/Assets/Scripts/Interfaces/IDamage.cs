using UnityEngine;

internal interface IDamage
{
    int DamageAmount { get; }
    LayerMask DamageMask { get; }

    void DoDamage(Collider collision, bool isAffected);
}