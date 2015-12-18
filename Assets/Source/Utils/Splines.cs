using UnityEngine;
using System.Collections;

public class Splines {

    public static Vector3 GetPoint(Vector3[] pts, float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;

        return pts[0] * (omt2 * omt) +
            pts[1] * (3f * omt2 * t) +
            pts[2] * (3f * omt * t2) +
            pts[3] * (t2 * t);
    }
}
