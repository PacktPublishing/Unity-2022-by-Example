using UnityEngine;
using UnityEngine.Events;

public class PickupHeal : MonoBehaviour, IHeal
{
    public int HealAmount => _healAmount;
    public LayerMask HealMask => _healMask;

    [SerializeField] private int _healAmount = 10;
    [SerializeField] private LayerMask _healMask;

    public UnityEvent<GameObject> OnHealEvent;

    public void DoHeal(GameObject healedObject)
        => OnHealEvent?.Invoke(healedObject);
}