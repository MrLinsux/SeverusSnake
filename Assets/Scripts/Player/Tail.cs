using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Segment;

public class Tail : MonoBehaviour
{
    public int speed = 1;
    public int maxEmptyRails = 100;
    protected Rigidbody2D _rb;
    [SerializeField]
    float currentT = 0.5f;
    public float CurrentT { get { return currentT; } }
    Player player;

    private void Awake()
    {
        Player.MoveSegmentsBack += MoveSegmentToBackward;
        player = GameObject.Find("Head").GetComponent<Player>();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        AddToDestroySegmentsEvent(MoveToPosition);
    }

    private void FixedUpdate()
    {
        if (currentT >= maxEmptyRails)
        {
            // delete some emty rails
            Railway.DeleteFirst();
            Player.InvokeMoveSegmentToBack();
        }

        // movement
        currentT += speed * Time.fixedDeltaTime * 1.12f;
        MoveToPosition();
    }

    void MoveToPosition()
    {
        var newPos = Railway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        _rb.MovePosition(newPos);
    }
    void MoveToPosition(float t)
    {
        // warn: this is no physic movement!
        currentT = t;
        var newPos = Railway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        transform.position = newPos;
    }

    void MoveSegmentToForward()
    {
        currentT++;
        MoveToPosition();
    }

    void MoveSegmentToBackward()
    {
        currentT--;
        //MoveToPosition();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("SegmentEater"))
        {
            if (player.CanEatSegment)
            {
                player.CantEatSegmentNow();
                InvokeDestroySegmentsEvent(currentT + 1);
            }
            else
            {
                Debug.Break();
                GameController.GameOver(false);
            }
        }
    }
}
