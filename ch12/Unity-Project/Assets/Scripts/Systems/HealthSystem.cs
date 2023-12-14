using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int _healthMax = 100;
    private int _healthCurrent;
    private IHaveHealth _objectWithHealth;


    private void Awake()
    {
        _healthCurrent = _healthMax;
        _objectWithHealth = GetComponent<IHaveHealth>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Test for a collision with a component that can damage us.
        if (collision.TryGetComponent<IDamage>(out var damage))
        {
            HandleDamageCollision(collision, damage);
        }
        // Test for a collision with a component that can heal us.
        else if (collision.TryGetComponent<IHeal>(out var heal))
        {
            HandleHealCollision(heal);
        }
    }

    internal void HandleDamageCollision(Collider collision, IDamage damage)
    {
        // We can use this variable to determine what type of effect the object that has collided with us should have.
        // And then whether to actually take damage from the object.
        var isAffected = IsLayerInLayerMask(gameObject.layer, damage.DamageMask);
        damage.DoDamage(collision, isAffected);

        //if ((damage.DamageMask & (1 << gameObject.layer)) != 0)
        //if (IsLayerInLayerMask(gameObject.layer, damage.DamageMask))
        if (isAffected)
            TakeDamage(damage.DamageAmount);
    }

    internal void HandleHealCollision(IHeal heal)
    {
        if (IsLayerInLayerMask(gameObject.layer, heal.HealMask))
        {
            heal.DoHeal(gameObject);
            ApplyHealing(heal.HealAmount);
        }
    }


    private void TakeDamage(int amount)
    {
        // If the health is already zero then we don't want to process health.
        // Some player actions may be called in quick succession that could try to
        // update health further before a health becoming zero condition can be processed.
        if (_healthCurrent == 0)
            return;

        _healthCurrent = Mathf.Max(_healthCurrent - amount, 0);
        //HealthChanged(-amount);
        HealthChanged();
    }

    private void ApplyHealing(int amount)
    {
        _healthCurrent = Mathf.Min(_healthCurrent + amount, _healthMax);
        //HealthChanged(amount);
        HealthChanged();
    }

    //private void HealthChanged(int delta)
    private void HealthChanged()
    {
        if (_objectWithHealth == null)
        {
            Debug.LogWarning($"HealthSystem on {gameObject.name}' requires a sibling component that inherits from IHaveHealth!",
                gameObject);
            return;
        }

        if (_healthCurrent > 0)
            _objectWithHealth.HealthChanged(_healthCurrent);
        else
            _objectWithHealth.Died();
    }

    
    // TODO: Convert this to an int ExtensionMethod as an example? Or simply a static Utilities class?
    // Use the | operator instead of & operator for Default layer (0) to return true.
    private bool IsLayerInLayerMask(int layer, LayerMask mask)
        => (mask & (1 << layer)) != 0;


    #region TESTING

    [ContextMenu("Tests/TakeDamage(5)")]
    public void Test_TakeDamage()
    {
        TakeDamage(5);
        print($"{_healthCurrent}/{_healthMax}");
    }

    [ContextMenu("Tests/Heal(10)")]
    public void Test_Heal()
    {
        ApplyHealing(10);
        print($"{_healthCurrent}/{_healthMax}");
    }

    #endregion TESTING
}