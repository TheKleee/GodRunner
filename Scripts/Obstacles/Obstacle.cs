using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision player)
    {
        if (player.transform.GetComponent<PlayerController>() != null)
        {
            player.transform.GetComponent<PlayerController>().Revive();
        }
    }
}
