using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TrackLaser : MonoBehaviour {

    public bool update;

    [Header("[ REFERENCES ]")]
    public GameObject emitterLeft;
    public GameObject emitterRight;
    public LineRenderer laserRenderer;
    public ParticleSystem laserOccludePS;

    private Vector3 emissionPoint;
    private Vector3 targetPoint;
    private Vector3 actualTarget;

    void Start()
    {
        // force laser to update
        update = true;
    }

    void Update()
    {
        // check for collisions
        RaycastHit hit;
        Vector3 final = targetPoint;
        bool emission = false;
        if (Physics.Linecast(emissionPoint, targetPoint, out hit, ~(1 << LayerMask.NameToLayer("TrackWall"))))
        {
            final = hit.point;
            laserOccludePS.transform.position = final;
            emission = true;
        }

        laserOccludePS.enableEmission = emission;
        laserRenderer.SetPosition(1, final);


        if (!update)
            return;

        // raycast left for left emitter
        Vector3 p0 = Vector3.zero;
        Vector3 p1 = Vector3.zero;
        if (Physics.Raycast(transform.position, -transform.right, out hit, 1 << LayerMask.NameToLayer("TrackWall")))
        {
            emitterLeft.transform.position = hit.point;
            emitterLeft.transform.LookAt(transform.position);
            p0 = hit.point;
            emissionPoint = p0;
        }

        // raycast right for right emitter
        if (Physics.Raycast(transform.position, transform.right, out hit, 1 << LayerMask.NameToLayer("TrackWall")))
        {
            emitterRight.transform.position = hit.point;
            emitterRight.transform.LookAt(transform.position);
            p1 = hit.point;
            targetPoint = p1;
        }

        // update laser renderer
        laserRenderer.SetPosition(0, p0);
        laserRenderer.SetPosition(1, p1);

        update = false;

    }
}
