using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Food : MonoBehaviour
{
    AudioController audioController;
    [SerializeField]
    AudioClip eatSound;

    private void Awake()
    {
        audioController = Camera.allCameras[0].GetComponent<AudioController>();
    }

    private void Start()
    {
        transform.position = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
    }
}
