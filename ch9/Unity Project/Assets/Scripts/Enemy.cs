using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IHaveHealth
{
    private event UnityAction _onDestroyed;

    internal void Init(UnityAction destroyedCallback)
        => _onDestroyed = destroyedCallback;

    private void OnDestroy()
        => _onDestroyed?.Invoke();


    /// <summary>
    /// HealthSystem implemented method.
    /// </summary>
    /// <param name="amount"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    //public void HealthChanged(int amount) =>
    //    print($"{gameObject.name} health changed {amount}");
    public void HealthChanged(int amount)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// HealthSystem implemented method.
    /// </summary>
    //public void Died() =>
    //    print($"{gameObject.name} died");
    public void Died()
    {
        //throw new System.NotImplementedException();
        Destroy(gameObject);
    }
}
