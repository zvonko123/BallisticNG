using UnityEngine;
using System.Collections;

// script originally copied from http://forum.unity3d.com/threads/b-splines.11951/
// additional changes made to suite needs
public class BSpline
{

    public int n = 2; // Degree of the curve

    public Vector3[] cachedControlPoints; // cached control points
    public int[] nV; // Node vector

    public void CreateSpline(Vector3[] cp)
    {
        cachedControlPoints = cp;

        nV = new int[cachedControlPoints.Length + 5];
        createNodeVector();
    }

    // Recursive deBoor algorithm.
    public Vector3 deBoor(int r, int i, float u)
    {
        if (r == 0)
        {
            return cachedControlPoints[i];
        }
        else
        {
            float pre = (u - nV[i + r]) / (nV[i + n + 1] - nV[i + r]); // Precalculation
            return ((deBoor(r - 1, i, u) * (1 - pre)) + (deBoor(r - 1, i + 1, u) * (pre)));
        }

    }

    public void createNodeVector()
    {
        int knoten = 0;
        for (int i = 0; i < (n + cachedControlPoints.Length + 1); i++) // n+m+1 = nr of nodes
        {
            if (i > n)
            {
                if (i <= cachedControlPoints.Length)
                    nV[i] = ++knoten;
                else
                    nV[i] = knoten;
            }
            else
            {
                nV[i] = knoten;
            }
        }
    }
}