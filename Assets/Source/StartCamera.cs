using UnityEngine;
using System.Collections;

public class StartCamera : ShipBase {

    void Start()
    {
        // set camera position to start position
        transform.position = RaceSettings.introCamStart;
    }

    void FixedUpdate()
    {
        // lerp towards end transform
        transform.position = Vector3.Lerp(transform.position, RaceSettings.introCamEnd, Time.deltaTime);

        // lookat the target ship
        transform.LookAt(r.transform.position);

        // if close enough to the end transform then start the race
        if (Vector3.Distance(transform.position, RaceSettings.introCamEnd) < 1)
            Ready();
    }

    private void Ready()
    {
        gameObject.AddComponent<ShipCamera>();
        GetComponent<ShipCamera>().r = r;
        RaceSettings.countdownReady = true;
        RaceSettings.shipsRestrained = false;
        Destroy(this);
    }
}
