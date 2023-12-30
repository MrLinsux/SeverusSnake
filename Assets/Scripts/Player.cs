using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int startLen = 5;
    public int speed = 1;

    private void Start()
    {
        Segment.speed = speed;
        // add start rails for head and tail
        Segment.Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 1.5f), new Vector2(transform.position.x, transform.position.y - 0.5f));
        Segment.Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.position.x, transform.position.y + 0.5f));
        CreateBody();
    }

    public void CreateBody()
    {
        // spawn n segments
        // n=0 then only head and tail
        GetComponent<Segment>().AddNextSegments(startLen);
    }
}
