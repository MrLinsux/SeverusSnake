using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Segment;

public class Player : MonoBehaviour
{
    [SerializeField]
    Sprite brickSnakeSprite;          // head
    [SerializeField]
    Sprite[] jellySnakeSprites;        // 0 - segment; 1 - tail
    [SerializeField]
    Sprite[] standartSnakeSprites;     // 0 - head; 1 - segment; 2 - tail
    AudioController _audioController;

    Segment tail, head;
    List<Segment> body = new List<Segment>();

    [SerializeField]
    GameObject segmentPref;
    [SerializeField]
    int startLen = 5;
    [SerializeField]
    float speed = 1;
    public float Speed { get { return speed; } }

    [SerializeField]
    bool canEatWall = false;
    public bool CanEatWall { get { return canEatWall; } }
    public void SetCanEatWall(bool canEat)
    {
        SpriteRenderer headSprite = head.GetComponent<SpriteRenderer>();
        headSprite.sprite = canEat ? brickSnakeSprite : standartSnakeSprites[0];
        canEatWall = canEat;
    }
    [SerializeField]
    bool canEatSegment = false;
    public bool CanEatSegment { get { return canEatSegment; } }
    public void SetCanEatSegment(bool canEat)
    {
        SpriteRenderer[] bodySprites = new SpriteRenderer[body.Count];
        for (int i = 0; i < bodySprites.Length; i++)
            bodySprites[i] = body[i].GetComponent<SpriteRenderer>();

        for (int i = 0; i < bodySprites.Length; i++)
        {
            bodySprites[i].sprite = canEat ? jellySnakeSprites[0] : standartSnakeSprites[1];   // segments of snake
        }
        segmentPref.GetComponent<SpriteRenderer>().sprite = canEat ? jellySnakeSprites[0] : standartSnakeSprites[1];   // segment prefab
        tail.GetComponent<SpriteRenderer>().sprite = canEat ? jellySnakeSprites[1] : standartSnakeSprites[2];      // tail
        canEatSegment = canEat;
    }

    [SerializeField]
    bool canPlayRotationSound = true;

    bool canMove = true;
    public bool CanMove { get { return canMove; } }
    void SetMove(bool canMove)
    {
        foreach(Segment segment in body)
        {
            segment.SetMove(canMove);
        }
        head.SetMove(canMove);
        tail.SetMove(canMove);

        this.canMove = canMove;
    }

    public int SnakeLen { get { return snakeLen; } set { snakeLen = value; GameController.CurrentController.SetLengthPoints(snakeLen); } }
    static int snakeLen;
    public void DecreaseLen(Segment segment)
    {
        body.Remove(segment);
        SetSegmentAsTail(body[0]);
        SnakeLen--;
    }

    public float CurrentT {  get { return head.CurrentT; } }        // main parameter is t of head

    private void Start()
    {
        // add start rails for head and tail
        GameController.CurrentRailway.AddRail(new Vector2(transform.position.x, transform.position.y - 1.5f), new Vector2(transform.position.x, transform.position.y - 0.5f));
        snakeLen = 0;
        _audioController = Camera.allCameras[0].GetComponent<AudioController>();
        head = GetComponentInChildren<Segment>();
        head.Init(1.5f, this);
        CreateBody();
        
        SetCanEatSegment(canEatSegment);
        SetCanEatWall(canEatWall);
        SetMove(false);
    }

    private void FixedUpdate()
    {
        Vector2 nearFrom = GameController.CurrentRailway.LastRail.GetRailPos(0, out Vector2 nearFromDirection);
        Vector2 lastPoint = GameController.CurrentRailway.LastRail.GetRailPos(1, out Vector2 lastPointDirection);
        Vector2 newDir;
        // if on last rail then add new rail
        if (CurrentT >= GameController.CurrentRailway.MaxT - 0.55f)
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
                    _audioController.PlaySnakeRotationSound();
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
                    _audioController.PlaySnakeRotationSound();
                    canPlayRotationSound = false;
                }
            }
        }
        else if (new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized == (Vector2)transform.up)
        {
            // if press forward
            GameController.CurrentRailway.LastRail = new Railway.Rail(nearFrom, nearFrom + nearFromDirection);
        }
    }

    void MoveBodyToForward()
    {
        foreach(var segment in body)
            segment.MoveSegmentToForward();
        tail.MoveSegmentToForward();
    }

    void MoveBodyToBackward()
    {
        foreach (var segment in body)
            segment.MoveSegmentToBackward();
        tail.MoveSegmentToBackward();
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
        for (float i = 0; i < CurrentT; i += 0.02f)
        {
            Debug.DrawLine(GameController.CurrentRailway.GetPositionOnRailway(i), GameController.CurrentRailway.GetPositionOnRailway(i + 0.02f), Color.blue);
        }
#endif
    }

    Segment AddTailSegment()
    {
        Segment tail = Instantiate(
            segmentPref, 
            GameController.CurrentRailway.GetPositionOnRailway(CurrentT - 1), 
            head.transform.rotation, 
            transform).GetComponent<Segment>();
        tail.Init(CurrentT-1, this);

        tail.AddComponent<SpriteMask>().sprite = standartSnakeSprites[1];   // sprite of mask is standart segment
        tail.GetComponent<SpriteRenderer>().sprite = standartSnakeSprites[2];
        tail.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;

        return tail;
    }

    void SetSegmentAsTail(Segment segment)
    {
        if (segment == null)
            return;
        segment.AddComponent<SpriteMask>().sprite = standartSnakeSprites[1];   // sprite of mask is standart segment
        segment.GetComponent<SpriteRenderer>().sprite = standartSnakeSprites[2];
        segment.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        tail = segment;
        body.Remove(segment);
    }

    public Segment AddNextSegment()
    {
        Segment newSegment = Instantiate(
            segmentPref, 
            GameController.CurrentRailway.GetPositionOnRailway(CurrentT - 1), 
            head.transform.rotation, 
            transform).GetComponent<Segment>();
        newSegment.Init(CurrentT-1, this);
        MoveBodyToBackward();

        SnakeLen +=1;

        body.Add(newSegment);

        return newSegment;
    }

    public Segment AddNextSegments(int n)
    {
        Segment newSegment = null;
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
        tail = AddTailSegment();
        AddNextSegments(startLen);
    }
}
