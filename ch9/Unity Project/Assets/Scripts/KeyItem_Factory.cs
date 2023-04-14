using UnityEngine;

public class KeyItem_Factory : MonoBehaviour
{
    private int _id;

    public void Init(int id) => _id = id;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Tags.Player))
        {
            //print($"Key Item '{gameObject.name}' ({_id}) Collected!");
            EventSystem.Instance.TriggerEvent<int>(EventConstants.OnKeyCollected, _id);
            Destroy(gameObject);
        }
    }
}