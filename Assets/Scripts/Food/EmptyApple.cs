using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyApple : Food
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.transform.parent.parent.TryGetComponent(out player))
        {
            GameController.CurrentController.AppleEaten();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player;
        if (collision.transform.parent.parent.TryGetComponent(out player))
        {
            GameController.CurrentController.AppleEaten();
            Destroy(gameObject);
        }
    }
}
