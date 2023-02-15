using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TriggeredEvent : MonoBehaviour
{
    [Tooltip("Requires the player character to have the 'Player' tag assigned.")]
    public bool IsTriggeredByPlayer = true;
    public UnityEvent OnTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsTriggeredByPlayer && !collision.CompareTag(Tags.Player))
            return;

        OnTriggered?.Invoke();
    }
}
