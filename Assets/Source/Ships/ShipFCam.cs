using UnityEngine;
using System.Collections;

public class ShipFCam : ShipBase{

    // camera mode
    private int camMode;
    private float cameraSwitchTimer;

    void FixedUpdate()
    {
        // camera switch
        cameraSwitchTimer += Time.deltaTime;
        if(cameraSwitchTimer > 3)
        {
            camMode++;
            cameraSwitchTimer = 0;
        }
        if (camMode > 2)
            camMode = 0;

        if (camMode == 0)
        {
            // no parent
            transform.parent = null;

            // go to next camera mode if there are no overview transforms
            if (RaceSettings.overviewTransforms.Length == 0)
                camMode++;

            int index = 0;
            float distance = 10000;
            int i = 0;

            // find nearest transform
            for (i = 0; i < RaceSettings.overviewTransforms.Length; ++i)
            {
                float p2s = Vector3.Distance(r.transform.position, RaceSettings.overviewTransforms[i].position);
                if (p2s < distance)
                {
                    distance = p2s;
                    if (Physics.Linecast(RaceSettings.overviewTransforms[i].transform.position, r.transform.position))
                        index = i;
                }
            }

            // position camera at current transform
            transform.position = RaceSettings.overviewTransforms[index].position;

            // lookat ship
            transform.LookAt(r.transform.position);

            // alter FoV based on distance
            distance = Vector3.Distance(transform.position, r.transform.position) * 3;
            distance = Mathf.Clamp(distance, 0.0f, 85.0f);
            r.cam.fieldOfView = 100 - distance;

        } else if (camMode == 1)
        {
            camMode++;
        } else if (camMode == 2)
        {
            camMode++;
        }
    }
}
