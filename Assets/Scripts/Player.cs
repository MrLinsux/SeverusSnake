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
        CreateBody();
    }

    public void CreateBody()
    {
        GetComponent<Segment>().AddNextSegments(startLen);
    }
}
