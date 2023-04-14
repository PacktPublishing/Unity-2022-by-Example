using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            //print($"Key Item '{gameObject.name}' ({_id}) Collected!");
            EventSystem.Instance.TriggerEvent(EventConstants.OnKeyCollected, false);
            Destroy(gameObject);
        }
    }
}