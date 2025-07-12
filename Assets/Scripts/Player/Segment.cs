using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class Segment : MonoBehaviour
{
    AudioController audioController;
    [SerializeField]
    AudioClip eatSound;
    [SerializeField]
    AudioClip wallEatSound;

    [SerializeField]
    GameObject spawnApplePrefab;

    Rigidbody2D _rb;
    Player player;

    [SerializeField]
    float currentT = 1;
    public float CurrentT 
    {  get { return currentT; } private set { currentT = value; } }

    bool canMove = true;
    public bool CanMove { get { return canMove; } }
    public void SetMove(bool canMove)
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

    public void Init(float startT, Player player)
    {
        audioController = Camera.allCameras[0].GetComponent<AudioController>();
        _rb = GetComponent<Rigidbody2D>();
        DestroySegments += DestroySegment;
        this.player = player;
        currentT = startT;
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
        var newPos = GameController.CurrentRailway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        _rb.MovePosition(newPos);
    }

    public void MoveSegmentToForward()
    {
        currentT++;
    }

    public void MoveSegmentToBackward()
    {
        currentT--;
    }

    void DestroySegment(float startT)
    {
        // startT is T + 1 of segment-invoker
        if(currentT<=startT)
        {
            DestroySegments -= DestroySegment;
            if (player != null)
                player.DecreaseLen(this);
            Destroy(gameObject);
        }
        if(currentT <= startT-1)
        {
            Instantiate(spawnApplePrefab, GameController.CurrentRailway.GetPositionOnRailway(currentT-1), Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SegmentEater"))
        {
            if (player.CanEatSegment)
            {
                player.SetCanEatSegment(false);
                audioController.PlayAudio(eatSound);
                GameController.CurrentController.AppleEaten();
                DestroySegments.Invoke(currentT + 1);
            }
            else
            {
                audioController.PlayDeadSound();
                Debug.Break();
                GameController.CurrentController.GameOver(false, "try to eat standart segment");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // is wall
        if (player.CanEatWall)
        {
            Tilemap walls;
            if (collision.gameObject.TryGetComponent(out walls))
            {
                var grid = walls.layoutGrid;
                Vector3 worldPos = Vector3.zero;
                if (collision.contactCount == 2)
                {
                    worldPos = new Vector3((float)Math.Round(GameController.CurrentRailway.LastRail.GetRailPos(0.5f).x), (float)Math.Round(GameController.CurrentRailway.LastRail.GetRailPos(0.5f).y));
                }
                else if (collision.contactCount == 1)
                {
                    var toPos = GameController.CurrentRailway.GetPositionOnRailway(Mathf.Ceil(CurrentT), out var toDir);
                    toDir.Normalize();
                    toDir = toDir / 2 + toPos;
                    worldPos = new Vector3((float)Math.Round(toDir.x), (float)Math.Round(toDir.y));
#if UNITY_EDITOR
                    Debug.DrawLine(transform.position, worldPos, Color.red, 10);
#endif
                }
                Vector3Int eatedWallPos = grid.WorldToCell(worldPos);
                walls.SetTile(eatedWallPos, null);
                audioController.PlayAudio(wallEatSound);
            }
            player.SetCanEatWall(false);
        }
        else
        {
            GameController.CurrentController.GameOver(false, "kiss bricks without protection");
        }
    }
}
