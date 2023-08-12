using UnityEngine;
using System.Collections;

public class TrTurbZone : MonoBehaviour {

    public Vector3 windDirection;
    public float windSpeed;

    void OnDrawGizmos()
    {
        Vector3 start = transform.position;
        Vector3 end = start + (windDirection * windSpeed);

        Gizmos.DrawLine(start, end);
    }
}
