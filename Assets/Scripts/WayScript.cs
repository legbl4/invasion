using UnityEngine;
using System.Collections;

public class WayScript
{
    public Vector3[] points;

    public int CurveCount
    {
        get
        {
            return (points.Length - 1) / 3;
        }
    }

    public Vector3 GetPoint(float t)
    {

        if (t >= 1f)
            t = 1f;
        else
            t = Mathf.Clamp01(t) * 1;

        //bezier
        //return Bezier.GetPoint(points[0], points[1], points[2], points[3], t);
        return Vector3.Lerp(points[0], points[3], t);
    }
}
