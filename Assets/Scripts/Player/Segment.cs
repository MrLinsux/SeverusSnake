using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public static float speed = 1f;
    [SerializeField]
    GameObject spawnApplePrefab;
    protected Rigidbody2D _rb;
    [SerializeField]
    float currentT = 1;
    public float CurrentT 
    {  get { return currentT; } set { currentT = value; } }
    Player player;

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

    private void Awake()
    {
        Player.MoveSegmentsBack += MoveSegmentToBackward;
        DestroySegments += DestroySegment;
        player = GameObject.Find("Head").GetComponent<Player>();
    }

    protected void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
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

    void DestroySegment(float startT)
    {
        if (startT >= currentT)
        {
            DestroySegments -= DestroySegment;
            Instantiate(spawnApplePrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("SegmentEater"))
        {
            if (player.CanEatSegment)
            {
                player.CantEatSegmentNow();
                DestroySegments.Invoke(currentT + 1);
            }
            else
                Debug.Break();
        }
    }

    void OnDestroy()
    {
        Player.DecreaseLen();
    }


    public static class Railway
    {
        public static Rail LastRail 
        { 
            get 
            { 
                return rails.Last(); 
            } 
            set 
            {
                rails[rails.Count - 1] = value;
            }
        }
        // all rails
        static List<Rail> rails = new List<Rail>();
        public static float MaxT { get { return rails.Count; } }
        public static void AddRail(Vector2 from, Vector2 to)
        {
            rails.Add(new Rail(from, to));
        }
        public static void DeleteFirst()
        {
            rails.RemoveAt(0);
        }
        public static Vector2 GetPositionOnRailway(float t)
        {
            int _t = (int)t;
            try
            {
                return rails[_t].GetRailPos(t - _t);
            }
            catch
            {
                return rails[0].GetRailPos(0);
            }
        }
        public static Vector2 GetPositionOnRailway(float t, out Vector2 direction)
        {
            int _t = (int)t;
            try
            {
                return rails[_t].GetRailPos(t - _t, out direction);
            }
            catch
            {
                return rails[0].GetRailPos(0, out direction);
            }
        }

        public class Rail
        {
            // class of a rail
            public Rail(Vector2 from, Vector2 to)
            {
                this.from = from; this.to = to;
            }

            public bool IsCircle
            {
                get { return !(MathF.Abs(from.x - to.x) <= 0.001f | MathF.Abs(from.y - to.y)<=0.001f); }
            }

            public Vector2 From { get { return from; } }
            public Vector2 To { get { return to; } }

            Vector2 from, to;
            public Vector2 GetRailPos(float t)
            {
                // t is from 0 to 1 only!
                Vector2 res;
                if (!IsCircle)
                {
                    // is line
                    res = Vector2.Lerp(from, to, t);
                }
                else
                {
                    // is circle
                    Vector2 cell, center, sign;
                    if (Math.Abs(from.x - Mathf.Round(from.x)) <= 0.01f)
                    {
                        cell = new Vector2(from.x, to.y);
                        center = new Vector2(to.x, from.y);
                    }
                    else
                    {
                        cell = new Vector2(to.x, from.y);
                        center = new Vector2(from.x, to.y);
                    }

                    sign = 2 * (cell - center);

                    res = new Vector2(sign.x * 0.5f,0) + center;
                    if((res-from).magnitude > 0.01f)
                    {
                        t = 1 - t;
                    }

                    res = SplineIntepolate(from, cell, to, t);
                }

                return res;
            }

            public Vector2 GetRailPos(float t, out Vector2 direction)
            {
                // t is from 0 to 1 only!
                // direction is direction of speed vector
                // calculate as derivative of position parametric function
                Vector2 res;
                if (!IsCircle)
                {
                    // is line
                    res = Vector2.Lerp(from, to, t);
                    direction = to - from;
                }
                else
                {
                    // is circle
                    Vector2 cell, center, sign;
                    if (Math.Abs(from.x - Mathf.Round(from.x)) <= 0.01f)
                    {
                        cell = new Vector2(from.x, to.y);
                        center = new Vector2(to.x, from.y);
                    }
                    else
                    {
                        cell = new Vector2(to.x, from.y);
                        center = new Vector2(from.x, to.y);
                    }

                    sign = 2*(cell - center);
                    res = new Vector2(sign.x * 0.5f, 0) + center;
                    if ((res - from).magnitude > 0.01f)
                    {
                            direction = SplineIntepolateDirection(from, cell, to, t);
                    }
                    else
                    {
                            direction = SplineIntepolateDirection(from, cell, to, t);
                    }
                    res = SplineIntepolate(from, cell, to, t);
                    Debug.DrawLine(res, res + direction, Color.blue);
                }

                direction.Normalize();
                return res;
            }

            Vector2 SplineIntepolate(Vector2 a,  Vector2 b, Vector2 c, float t)
            {
                // curve bezie
                return (1 - t) * (1 - t) * a + 2 * t * (1 - t) * b + t * t * c;
                //return (1 - t) * (1 - t)* (1 - t) * a + 3 * t * (1 - t)* (1 - t) * b + 3 * t*t * (1 - t) * b + t * t * t*c;
            }
            Vector2 SplineIntepolateDirection(Vector2 a, Vector2 b, Vector2 c, float t)
            {
                // curve bezie
                return -2*a*(1-t)+2*b*(1-t)-2*b*t+2*c*t;
                //return -3 * a * (1 - t) * (1 - t) + 3 * b * (1 - t) * (1 - t) - 3 * b * t * t + 3 * c * t * t;
            }
        }
    }
}
