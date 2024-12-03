using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Segment;

public class Tail : MonoBehaviour
{
    Rigidbody2D _rb;
    [SerializeField]
    float currentT = 0.5f;
    public float CurrentT { get { return currentT; } }
    Player player;
    bool canMove = true;
    public bool CanMove { get { return canMove; } }
    void SetMove(bool canMove)
    {
        this.canMove = canMove;
    }

    public void Init()
    {
        Player.MoveSegmentsBackEvent += MoveSegmentToBackward;
        Player.CanMoveEvent += SetMove;
        player = GameObject.Find("Head").GetComponent<Player>();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        AddToDestroySegmentsEvent(MoveToPosition);
    }

    private void FixedUpdate()
    {
        if (currentT >= player.MaxEmptyRails)
        {
            // delete some emty rails
            GameController.CurrentRailway.DeleteFirst();
            Player.InvokeMoveSegmentToBack();
        }

        // movement
        if (CanMove)
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
    void MoveToPosition(float t)
    {
        // warn: this is no physic movement!
        currentT = t;
        var newPos = GameController.CurrentRailway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        transform.position = newPos;
    }

    void MoveSegmentToForward()
    {
        currentT++;
    }

    void MoveSegmentToBackward()
    {
        currentT--;
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
                GameController.CurrentController.GameOver(false);
            }
        }
    }

    private void OnDestroy()
    {
        Player.DecreaseLen();
        Player.MoveSegmentsBackEvent -= MoveSegmentToBackward;
    }
}
