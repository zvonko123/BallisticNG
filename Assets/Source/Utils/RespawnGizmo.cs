using UnityEngine;
using System.Collections;

public class RespawnGizmo : MonoBehaviour {

    [Header(" DESTROY THIS ONCE DONE ")]
    public bool draw = true;

    void OnDrawGizmos()
    {
        if (draw)
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.4f);

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
        }
    }
}
