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
    Sprite brickSnake;          // head
    [SerializeField]
    Sprite[] jellySnake;        // 0 - segment; 1 - tail
    [SerializeField]
    Sprite[] standartSnake;     // 0 - head; 1 - segment; 2 - tail
    Rigidbody2D _rb;
    AudioController audioController;

    [SerializeField]
    GameObject eater;
    [SerializeField]
    Tail tail;
    [SerializeField]
    int maxEmptyRails = 100;
    [SerializeField]
    GameObject segmentPref;
    [SerializeField]
    int startLen = 5;
    [SerializeField]
    int speed = 1;
    public int Speed { get { return speed; } }

    [SerializeField]
    bool canEatWall = false;
    public bool CanEatWall { get { return canEatSegment; } }
    public void CanEatWallNow()
    {
        SpriteRenderer bodySprites = transform.parent.GetComponentInChildren<SpriteRenderer>();
        bodySprites.sprite = brickSnake;
        canEatWall = true;
    }
    public void CantEatWallNow()
    {
        SpriteRenderer bodySprites = transform.parent.GetComponentInChildren<SpriteRenderer>();
        bodySprites.sprite = standartSnake[0];
        canEatWall = false;
    }
    [SerializeField]
    bool canEatSegment = false;
    public bool CanEatSegment { get { return canEatSegment; } }
    public void CanEatSegmentNow()
    {
        SpriteRenderer[] bodySprites = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        for(int i = 2; i < bodySprites.Length; i++)
        {
            bodySprites[i].sprite = jellySnake[0];   // segments of snake
        }
        segmentPref.GetComponent<SpriteRenderer>().sprite = jellySnake[0];   // segment prefab
        bodySprites[1].sprite = jellySnake[1];      // tail
        canEatSegment = true;
    }
    public void CantEatSegmentNow()
    {
        SpriteRenderer[] bodySprites = transform.parent.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 2; i < bodySprites.Length; i++)
        {
            bodySprites[i].sprite = standartSnake[1];   // segments of snake
        }
        segmentPref.GetComponent<SpriteRenderer>().sprite = standartSnake[1];   // segment prefab
        bodySprites[1].sprite = standartSnake[2];      // tail
        canEatSegment = false;
    }

    bool canPlayRotationSound = true;


    public int MaxEmptyRails { get {  return maxEmptyRails; } }
    bool canMove = true;
    public bool CanMove { get { return canMove; } }
    void SetMove(bool canMove)
    {
        this.canMove = canMove;
        CanMoveEvent(canMove);
    }

    public delegate void CanMoveDelegate(bool canMove);
    public static event CanMoveDelegate CanMoveEvent;

    public static int SnakeLen { get { return snakeLen; } set { snakeLen = value; GameController.CurrentController.SetLengthPoints(snakeLen); } }
    static int snakeLen;
    public static void DecreaseLen() => SnakeLen--;

    public delegate void MoveSegmentDelegate();
    public static event MoveSegmentDelegate MoveSegmentsBackEvent;
    float currentT = 1.5f;
    public float CurrentT {  get { return currentT; } }

    private void Awake()
    {
        snakeLen = 0;
        MoveSegmentsBackEvent = null;
        CanMoveEvent = null;
        MoveSegmentsBackEvent += MoveSegmentToBackward;
        CantEatSegmentNow();
        CantEatWallNow();
        tail.Init();
    }

    private void Start()
    {
        audioController = Camera.allCameras[0].GetComponent<AudioController>();
        _rb = GetComponent<Rigidbody2D>();
        // add start rails for head and tail
        GameController.CurrentRailway.AddRail(new Vector2(transform.position.x, transform.position.y - 1.5f), new Vector2(transform.position.x, transform.position.y - 0.5f));
        CreateBody();
        SetMove(false);
    }

    private void FixedUpdate()
    {
        Vector2 nearFrom = GameController.CurrentRailway.LastRail.GetRailPos(0, out Vector2 nearFromDirection);
        Vector2 lastPoint = GameController.CurrentRailway.LastRail.GetRailPos(1, out Vector2 lastPointDirection);
        Vector2 newDir;
        // if on last rail then add new rail
        if (currentT >= GameController.CurrentRailway.MaxT - 0.55f)
        {
            GameController.CurrentRailway.AddRail(lastPoint, lastPoint + lastPointDirection);
            canPlayRotationSound = true;
            nearFrom = GameController.CurrentRailway.LastRail.GetRailPos(0, out nearFromDirection);
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            // rotation to up or down
            newDir = Input.GetAxis("Vertical") > 0 ? Vector2.up : Vector2.down;
            if (Mathf.Abs(nearFromDirection.y) <= 0.001f)
            {
                GameController.CurrentRailway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection / 2 + newDir / 2);
                if (canPlayRotationSound)
                {
                    audioController.PlaySnakeRotationSound();
                    canPlayRotationSound = false;
                }
            }
        }
        else if (Input.GetAxis("Horizontal") != 0)
        {
            // rotation to right ot left
            newDir = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
            if (Mathf.Abs(nearFromDirection.x) <= 0.001f)
            {
                GameController.CurrentRailway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection / 2 + newDir / 2);
                if(canPlayRotationSound)
                {
                    audioController.PlaySnakeRotationSound();
                    canPlayRotationSound = false;
                }
            }
        }
        else if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized == (Vector2)transform.up)
        {
            // if press forward
            GameController.CurrentRailway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection);
        }

        // movement
        if (CanMove)
        {
            currentT += speed * Time.fixedDeltaTime * 1.12f;
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

    void MoveSegmentToForward()
    {
        currentT++;
    }

    void MoveSegmentToBackward()
    {
        currentT--;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SetMove(!CanMove);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            AddNextSegment();
        }
#if UNITY_EDITOR
        for (float i = 0; i < currentT; i += 0.02f)
        {
            Debug.DrawLine(GameController.CurrentRailway.GetPositionOnRailway(i), GameController.CurrentRailway.GetPositionOnRailway(i + 0.02f), Color.blue);
        }
#endif
    }

    protected GameObject AddNextSegment()
    {
        GameObject newSegment = Instantiate(segmentPref, GameController.CurrentRailway.GetPositionOnRailway(currentT-1), transform.rotation, transform.parent);
        newSegment.GetComponent<Segment>().Init(currentT);
        MoveSegmentsBackEvent?.Invoke();
        currentT++;

        SnakeLen+=1;

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

    public static void InvokeMoveSegmentToBack()
    {
        MoveSegmentsBackEvent?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // is wall
        if (canEatWall)
        {
            Tilemap walls;
            if(collision.gameObject.TryGetComponent(out walls))
            {
                var grid = walls.layoutGrid;
                Vector3 worldPos = Vector3.zero;
                if (collision.contactCount == 2)
                {
                    worldPos = new Vector3((float)Math.Round(GameController.CurrentRailway.LastRail.GetRailPos(0.5f).x), (float)Math.Round(GameController.CurrentRailway.LastRail.GetRailPos(0.5f).y));
                }
                else if (collision.contactCount == 1)
                {
                    var toPos = GameController.CurrentRailway.GetPositionOnRailway(Mathf.Ceil(currentT), out var toDir);
                    toDir.Normalize();
                    toDir = toDir/2 + toPos;
                    worldPos = new Vector3((float)Math.Round(toDir.x), (float)Math.Round(toDir.y));
                    Debug.DrawLine(transform.position, worldPos, Color.red, 10);
                }
                Vector3Int eatedWallPos = grid.WorldToCell(worldPos);
                walls.SetTile(eatedWallPos, null);
            }
            CantEatWallNow();
        }
        else
        {
            audioController.PlayDeadSound();
            GameController.CurrentController.GameOver(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Tilemap wells))
        {
            Debug.Log("This is well!");
        }
    }

    private void OnDestroy()
    {
        MoveSegmentsBackEvent += MoveSegmentToBackward;
    }
}
