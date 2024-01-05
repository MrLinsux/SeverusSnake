using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleJellyApple : Food
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.gameObject.TryGetComponent(out player))
        {
            player.CanEatSegmentNow();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player;
        if (collision.gameObject.TryGetComponent(out player))
        {
            player.CanEatSegmentNow();
            Destroy(gameObject);
        }
    }
}
