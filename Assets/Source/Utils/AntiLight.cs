using UnityEngine;
using System.Collections;

public class AntiLight : MonoBehaviour {

    [Header("[ FLAGS ]")]
    public bool allowAmbient;
    public bool isArea;

    [Header("[ SETTINGS  ]")]
    [Range(0, 1)]
    public float intensity;

    public float range;
    public Vector3 area;
    public GameObject[] exclusions;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (isArea)
        {
            Gizmos.DrawWireCube(transform.position, area);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }

}
