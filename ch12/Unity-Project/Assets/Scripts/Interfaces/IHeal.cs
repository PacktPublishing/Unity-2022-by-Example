using UnityEngine;

internal interface IHeal
{
    int HealAmount { get; }
    LayerMask HealMask { get; }

    void DoHeal(GameObject healedObject);
}