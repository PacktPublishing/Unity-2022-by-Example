using UnityEngine;
using UnityEngine.Events;

public class ProjectileDamage : MonoBehaviour, IDamage
{
    public int DamageAmount => _damageAmount;
    public LayerMask DamageMask => _damageMask;

    [SerializeField] private int _damageAmount = 5;
    [SerializeField] private LayerMask _damageMask;

    public UnityEvent<Collider, bool> OnDamageEvent;

    public void DoDamage(Collider collision, bool isAffected)
        => OnDamageEvent?.Invoke(collision, isAffected);
}