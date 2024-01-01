using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickApple : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if(collision.gameObject.TryGetComponent<Player>(out player))
        {
            player.IncreaseCanEatWallAmount();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player;
        if (collision.gameObject.TryGetComponent<Player>(out player))
        {
            player.IncreaseCanEatWallAmount();
            Destroy(gameObject);
        }
    }
}
