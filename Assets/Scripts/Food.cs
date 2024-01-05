using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private void Start()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }

    private void OnDestroy()
    {
        GameController.ApplePoints++;
    }
}
