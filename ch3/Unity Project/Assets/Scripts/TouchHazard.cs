using UnityEngine;

public class TouchHazard : MonoBehaviour
{
    [Header("Make sure the 'Player' tag is also assigned!")]
    public PlayerController Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Player.tag))
        {
            Debug.Log("You touched a toxic puddle!");

            // TODO: Refactor to not use a Player scene reference assigned in the Inspector.
            Player.SlowPlayerSpeed();
        }
    }
}
