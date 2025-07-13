using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("SegmentEater"))
        {
            Player player = collision.transform.parent.parent.GetComponent<Player>();
            if(player && !player.CanFlight)
                GameController.CurrentController.GameOver(false, "fall to Kirby");
        }
    }
}
