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

    private void Awake()
    {
        Player.MoveSegmentsBack += MoveSegmentToBackward;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        AddToDestroySegmentsEvent((float startT) => currentT = startT);
    }

    private void FixedUpdate()
    {
        if (currentT >= maxEmptyRails)
        {
            // delete some emty rails
            Railway.DeleteFirst();
            Player.InvokeMpveSegmentToBack();
        }

        // movement
        currentT += speed * Time.fixedDeltaTime * 1.12f;
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
}
