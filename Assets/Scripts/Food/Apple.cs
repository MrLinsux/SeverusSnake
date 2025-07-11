using UnityEngine;

public class Apple : Food
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player;
        if (collision.transform.parent.parent.TryGetComponent(out player))
        {
            player.AddNextSegment();
            GameController.CurrentController.AppleEaten();
            PlayEatSound();
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player;
        if (collision.transform.parent.parent.TryGetComponent(out player))
        {
            player.AddNextSegment();
            GameController.CurrentController.AppleEaten();
            PlayEatSound();
            Destroy(gameObject);
        }
    }
}
