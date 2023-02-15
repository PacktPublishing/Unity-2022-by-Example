using UnityEngine;
using UnityEngine.Events;

public class CollectItem : MonoBehaviour
{
    // Added ch3 - Update Pickup Count
    public static event UnityAction OnItemCollected;
    private const string PlayerTag = "Player";

    void Start()
    {
        Debug.Log($"{gameObject.name}'s Start called", gameObject);

        // Added ch3 - Update Pickup Count
        GameManager.Instance.AddCollectibleItem();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // UNDONE: Make sure only the player can collect (not tool boxes) - bonus for reader?
        if (!collision.CompareTag(PlayerTag))
            return;

        // Added ch3 - Update Pickup Count
        OnItemCollected?.Invoke();

        Debug.Log($"Collision message event triggered on {gameObject.name}", gameObject);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Debug.Log($"Destroyed {gameObject.name}", gameObject);
    }
}
