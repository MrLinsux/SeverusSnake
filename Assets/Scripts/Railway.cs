using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Railway
{
    public Rail LastRail
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
    List<Rail> rails = new List<Rail>();
    public float MaxT { get { return rails.Count; } }
    public void AddRail(Vector2 from, Vector2 to)
    {
        rails.Add(new Rail(from, to));
    }
    public void DeleteFirst()
    {
        rails.RemoveAt(0);
    }
    public Vector2 GetPositionOnRailway(float t)
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
    public Vector2 GetPositionOnRailway(float t, out Vector2 direction, bool isHead = false)
    {
        int _t = (int)t;
        try
        {
            return rails[_t].GetRailPos(t - _t, out direction, isHead);
        }
        catch
        {
            return rails[0].GetRailPos(0, out direction, isHead);
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
            get { return !(MathF.Abs(from.x - to.x) <= 0.001f | MathF.Abs(from.y - to.y) <= 0.001f); }
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
                Vector2 cell, center;
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

                res = SplineIntepolate(from, cell, to, t);
            }

            return res;
        }

        public Vector2 GetRailPos(float t, out Vector2 direction, bool isHead = false)
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
                Vector2 cell, center;
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

                direction = SplineIntepolateDirection(from, cell, to, t, isHead);
                res = SplineIntepolate(from, cell, to, t, isHead);
                Debug.DrawLine(res, res + direction, Color.blue);
            }

            direction.Normalize();
            return res;
        }

        Vector2 SplineIntepolate(Vector2 a, Vector2 b, Vector2 c, float t, bool fourPoints = false)
        {
            // curve bezie
            if (!fourPoints)
                return (1 - t) * (1 - t) * a + 2 * t * (1 - t) * b + t * t * c;
            else
                return (1 - t) * (1 - t) * (1 - t) * a + 3 * t * (1 - t) * (1 - t) * b + 3 * t * t * (1 - t) * b + t * t * t * c;
        }
        Vector2 SplineIntepolateDirection(Vector2 a, Vector2 b, Vector2 c, float t, bool fourPoints = false)
        {
            // curve bezie
            if (!fourPoints)
                return -2 * a * (1 - t) + 2 * b * (1 - t) - 2 * b * t + 2 * c * t;
            else
                return -3 * a * (1 - t) * (1 - t) + 3 * b * (1 - t) * (1 - t) - 3 * b * t * t + 3 * c * t * t;
        }
    }
}
