using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int startLen = 5;
    public int speed = 100;

    private void Start()
    {
        Segment.speed = speed;
        Segment.Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 1.5f), new Vector2(transform.position.x, transform.position.y - 0.5f));
        Segment.Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.position.x, transform.position.y + 0.5f));
        CreateBody();
    }

    public void CreateBody()
    {
        GetComponent<Segment>().AddNextSegments(startLen);
    }
}
