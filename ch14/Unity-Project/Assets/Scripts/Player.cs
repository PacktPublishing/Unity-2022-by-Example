using UnityEngine;

public class Player : MonoBehaviour, IHaveHealth
{
    public void HealthChanged(int amount)
    {
        throw new System.NotImplementedException();
    }

    public void Died()
        => EventSystem.Instance.TriggerEvent(
            EventConstants.OnPlayerDied, true);
}
