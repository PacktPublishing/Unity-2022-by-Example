using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthSystem))]
public class ConstantDamage : MonoBehaviour, IDamage
{
    public LayerMask DamageMask => _damageMask;
    [SerializeField] private LayerMask _damageMask;

    public int DamageAmount => _damageAmount;
    [SerializeField] private int _damageAmount = 1;

    [SerializeField] private float _damageInterval = 5f;

    private void Start() => StartCoroutine(ApplyDamageOverTime());

    private IEnumerator ApplyDamageOverTime()
    {
        var healthSystem = GetComponent<HealthSystem>();
        while (true)
        {
            healthSystem.HandleDamageCollision(null, this);
            yield return new WaitForSeconds(_damageInterval);
        }
    }

    public void DoDamage(Collider collision, bool isAffected)
    {
        // Do something when the damage is applied.
    }
}