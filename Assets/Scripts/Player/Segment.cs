using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class Segment : MonoBehaviour
{
    AudioController audioController;
    [SerializeField]
    AudioClip eatSound;

    [SerializeField]
    GameObject spawnApplePrefab;
    [SerializeField]
    float startT = 1;

    Rigidbody2D _rb;
    Player player;

    float currentT = 1;
    public float CurrentT 
    {  get { return currentT; } private set { currentT = value; } }

    bool canMove = true;
    public bool CanMove { get { return canMove; } }
    void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }

    // for cutting of tail
    public delegate void DestroySegmentsFromStartT(float startT);
    static event DestroySegmentsFromStartT DestroySegments;
    public static void AddToDestroySegmentsEvent(DestroySegmentsFromStartT method)
    {
        DestroySegments += method;
    }
    public static void InvokeDestroySegmentsEvent(float startT)
    {
        DestroySegments.Invoke(startT);
    }

    public void Init(float startT)
    {
        this.startT = startT;
    }

    private void Start()
    {
        currentT = startT;
    }

    private void Awake()
    {
        audioController = Camera.allCameras[0].GetComponent<AudioController>();
        _rb = GetComponent<Rigidbody2D>();
        Player.MoveSegmentsBackEvent += MoveSegmentToBackward;
        Player.CanMoveEvent += SetMove;
        DestroySegments += DestroySegment;
        player = GameObject.Find("Head").GetComponent<Player>();
    }

    void FixedUpdate()
    {
        // movement
        if(CanMove)
        {
            currentT += player.Speed * Time.fixedDeltaTime * 1.12f;
            MoveToPosition();
        }
    }

    void MoveToPosition()
    {
        var newPos = Railway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        _rb.MovePosition(newPos);
    }

    void MoveSegmentToForward()
    {
        currentT++;
    }

    void MoveSegmentToBackward()
    {
        currentT--;
    }

    void DestroySegment(float startT)
    {
        // startT is T + 1 of segment-invoker
        if(currentT<=startT)
        {
            DestroySegments -= DestroySegment;
            Destroy(gameObject);
        }
        if(currentT <= startT-1)
        {
            Instantiate(spawnApplePrefab, Railway.GetPositionOnRailway(currentT-1), Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SegmentEater"))
        {
            if (player.CanEatSegment)
            {
                player.CantEatSegmentNow();
                DestroySegments.Invoke(currentT + 1);
                GameController.ApplePoints++;
            }
            else
            {
                audioController.PlayDeadSound();
                Debug.Break();
                GameController.GameOver(false);
            }
        }
    }

    void OnDestroy()
    {
        audioController.PlayAudio(eatSound);
        Player.DecreaseLen();
        Player.MoveSegmentsBackEvent -= MoveSegmentToBackward;
    }
}
