using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Segment;

public class Player : MonoBehaviour
{
    [SerializeField]
    bool canEatSegment = false;
    public bool CanEatSegment { get { return canEatSegment; } }
    [SerializeField]
    bool canEatWall = false;
    public void CanEatWallNow()
    {
        canEatWall=true;
    }
    //[SerializeField]
    float distBetweenSegments = 1;
    public int startLen = 5;
    public int speed = 1;
    [SerializeField]
    GameObject eater;
    [SerializeField]
    Tail tail;
    [SerializeField]
    int maxEmptyRails = 100;
    protected Rigidbody2D _rb;
    [SerializeField]
    protected GameObject segmentPref;
    public static int SnakeLen { get { return snakeLen; } }
    static int snakeLen;
    public static void DecreaseLen() => snakeLen--;

    public delegate void MoveSegment();
    public static event MoveSegment MoveSegmentsBack;
    public static event MoveSegment HeadMoved;
    [SerializeField]
    float currentT = 1.5f;
    public float CurrentT {  get { return currentT; } }

    private void Awake()
    {
        MoveSegmentsBack += MoveSegmentToBackward;
        tail.distBetweenSegments = distBetweenSegments;
        Segment.distBetweenSegments = distBetweenSegments;
        currentT += distBetweenSegments;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Segment.speed = speed;
        // add start rails for head and tail
        Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 1.5f), new Vector2(transform.position.x, transform.position.y - 0.5f));
        //Railway.AddRail(new Vector2(transform.position.x, transform.position.y - 0.5f), new Vector2(transform.position.x, transform.position.y + 0.5f));
        CreateBody();
        tail.speed = speed;
        tail.maxEmptyRails = maxEmptyRails;
        //eater.SetActive(true);
    }

    private void FixedUpdate()
    {
        Vector2 nearFrom = Railway.LastRail.GetRailPos(0, out Vector2 nearFromDirection);
        Vector2 lastPoint = Railway.LastRail.GetRailPos(1, out Vector2 lastPointDirection);
        Vector2 newDir;
        // if on last rail then add new rail
        if (currentT >= Railway.MaxT - 1)
        {
            Railway.AddRail(lastPoint, lastPoint + lastPointDirection);
            nearFrom = Railway.LastRail.GetRailPos(0, out nearFromDirection);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            newDir = Input.GetAxis("Vertical") > 0 ? Vector2.up : Vector2.down;
            if (Mathf.Abs(nearFromDirection.y) <= 0.001f)
            {
                Railway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection / 2 + newDir / 2);
            }
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            newDir = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
            if (Mathf.Abs(nearFromDirection.x) <= 0.001f)
            {
                Railway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection / 2 + newDir / 2);
            }
        }
        else if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized == (Vector2)transform.up)
        {
            // if press forward
            Railway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection);
        }

        // movement
        currentT += speed * Time.fixedDeltaTime * 1.12f;
        var newPos = Railway.GetPositionOnRailway(currentT, out Vector2 moveVector);
        // rotate
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.up, moveVector));
        // move
        //_rb.MovePosition(newPos);
        transform.position = newPos;
        HeadMoved.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddNextSegment();
        }
        for (float i = 0; i < currentT; i += 0.02f)
        {
            Debug.DrawLine(Railway.GetPositionOnRailway(i), Railway.GetPositionOnRailway(i + 0.02f), Color.blue);
        }
    }

    protected GameObject AddNextSegment()
    {
        GameObject newSegment = Instantiate(segmentPref, Railway.GetPositionOnRailway(currentT-distBetweenSegments), transform.rotation, transform.parent);
        newSegment.GetComponent<Segment>().CurrentT = currentT;
        currentT+=distBetweenSegments;
        MoveSegmentsBack.Invoke();

        snakeLen++;

        return newSegment;
    }
    public GameObject AddNextSegments(int n)
    {
        GameObject newSegment = null;
        for (int i = 0; i < n; i++)
        {
            newSegment = AddNextSegment();
        }

        return newSegment;
    }

    public void CreateBody()
    {
        // spawn n segments
        // n=0 then only head and tail
        AddNextSegments(startLen);
    }
    void MoveSegmentToBackward()
    {
        currentT -= distBetweenSegments;
    }
    public static void InvokeMoveSegmentToBack()
    {
        MoveSegmentsBack.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            AddNextSegment();
            Destroy(collision.gameObject);
        }
        // is wall
        if (canEatWall)
        {
            Tilemap walls;
            if(collision.gameObject.TryGetComponent(out walls))
            {
                var grid = walls.layoutGrid;
                var worldPos = new Vector3((float)Math.Round(Railway.LastRail.GetRailPos(0.5f).x), (float)Math.Round(Railway.LastRail.GetRailPos(0.5f).y));
                Vector3Int eatedWallPos = grid.WorldToCell(worldPos);
                walls.SetTile(eatedWallPos, null);
            }
            canEatWall = false;
        }
        else
        {
            MoveSegmentsBack.Invoke();
            MoveSegmentsBack.Invoke();
            Debug.Break();
        }
    }
}
