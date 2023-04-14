using UnityEngine;

public class Player : MonoBehaviour, IHaveHealth
{
    // HealthSystem implemented methods.

    //public void HealthChanged(int amount) =>
    //    print($"{gameObject.name} health changed {amount}");
    public void HealthChanged(int amount)
    {
        //throw new System.NotImplementedException();
        print($"The '{gameObject.name}' health has changed: {amount}");
    }

    //public void Died() =>
    //    print($"{gameObject.name} died");
    public void Died()
    {
        //throw new System.NotImplementedException();
        print($"The '{gameObject.name}' has died.");
    }
}