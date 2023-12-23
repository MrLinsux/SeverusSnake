using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment : MonoBehaviour
{
    [SerializeField]
    protected GameObject segmentPref;
    public static float speed = 1f;
    public static int snakeLen;
    protected Rigidbody2D _rb;
    protected Vector2 nextCell;
    protected float nextTurn;
    [SerializeField] bool isTail = false;
    [SerializeField] bool isHead = false;

    public Segment ForwardSegment
    {
        get { return forwardSegment; }
    }
    [SerializeField]
    protected Segment forwardSegment;

    public Segment BackwardSegment
    {
        get { return backwardSegment; }
    }
    [SerializeField]
    protected Segment backwardSegment;

    protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector3 moveVector = transform.up;
        Vector2 newDir;

        if(isHead)
        {
            if (Input.GetAxis("Vertical") != 0)
            {
                newDir = Input.GetAxis("Vertical") > 0 ? Vector2.up : Vector2.down;
                if (Mathf.Abs(moveVector.y) <= 0.001f)
                {
                    nextTurn = Vector2.SignedAngle(Vector2.up, newDir);
                }
            }
            else if (Input.GetAxis("Horizontal") != 0)
            {
                newDir = Input.GetAxis("Horizontal") > 0 ? Vector2.right : Vector2.left;
                if (Mathf.Abs(moveVector.x) <= 0.001f)
                {
                    nextTurn = Vector2.SignedAngle(Vector2.up, newDir);
                }
            }
        }
        if (!isTail)
        {
            BackwardSegment.nextTurn = Vector2.SignedAngle(Vector2.up, moveVector);
        }

        if (Vector2.Distance(transform.position, nextCell) <= 0.001f)
        {
            transform.position = nextCell;
            transform.eulerAngles = new Vector3(0, 0, nextTurn);
            nextCell = transform.position + transform.up;
            moveVector = transform.up;
        }
        _rb.velocity = moveVector * speed * Time.fixedDeltaTime;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if(isHead)
            {
                AddNextSegment();
            }
        }
    }

    protected GameObject AddNextSegment()
    {
        Segment newSegment;
        newSegment = Instantiate(segmentPref, backwardSegment.transform.position, backwardSegment.transform.rotation).GetComponent<Segment>();
        backwardSegment.MoveSegmentToBackward();
        newSegment.forwardSegment = this;
        newSegment.backwardSegment = backwardSegment;
        backwardSegment.forwardSegment = newSegment;
        backwardSegment = newSegment;
        snakeLen++;
        

        return newSegment.gameObject;
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

    void MoveSegmentToForward()
    {
        if(!isHead)
        {
            transform.position = forwardSegment.transform.position;
            if (!isTail)
                backwardSegment.MoveSegmentToForward();
        }
    }

    void MoveSegmentToBackward()
    {
        if (isTail)
        {
            transform.position = transform.position - transform.up;
        }
        else
        {
            transform.position = backwardSegment.transform.position;
            nextCell = BackwardSegment.nextCell;
            transform.rotation = BackwardSegment.transform.rotation;
            backwardSegment.MoveSegmentToBackward();
        }

    }

    void OnDestroy()
    {
        backwardSegment.forwardSegment = forwardSegment;
        backwardSegment.MoveSegmentToForward();
        snakeLen--;
    }
}
