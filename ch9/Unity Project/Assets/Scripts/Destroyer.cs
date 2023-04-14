using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float Delay = 0f;

    public void DestroyMe()
    {
        if (Delay > 0)
            Invoke(nameof(DestroyNow), Delay);
        else
            DestroyNow();
    }

    private void DestroyNow() => Destroy(gameObject);
}