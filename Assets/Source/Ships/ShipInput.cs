using UnityEngine;
using System.Collections;

public class ShipInput : ShipBase {

    // inputs
    public float AXIS_STEER;
    public float AXIS_PITCH;
    public float AXIS_LEFTAIRBRAKE;
    public float AXIS_RIGHTAIRBRAKE;
    public float AXIS_BOTHAIRBRAKES;
    public bool ACTION_THRUST;
    public bool ACTION_SPECIAL;

    void Update()
    {
        if (r.isAI)
        {
            // ai method check here later
        } else
        {
            // if ship restrained then do not read any input
            if (!RaceSettings.shipsRestrained && !r.shipRestrained)
            {
                AXIS_STEER = Input.GetAxis("Steer");
                AXIS_PITCH = Input.GetAxis("Pitch");
                AXIS_LEFTAIRBRAKE = Input.GetAxis("Left Airbrake");
                AXIS_RIGHTAIRBRAKE = Input.GetAxis("Right Airbrake");
                ACTION_SPECIAL = Input.GetButton("Special");
            } else
            {
                AXIS_STEER = 0.0f;
                AXIS_PITCH = 0.0f;
                AXIS_RIGHTAIRBRAKE = 0.0f;
                AXIS_RIGHTAIRBRAKE = 0.0f;
            }
            
            // combine both airbrakes together
            AXIS_BOTHAIRBRAKES = AXIS_LEFTAIRBRAKE + AXIS_RIGHTAIRBRAKE;
            
            // thrust check
            ACTION_THRUST = Input.GetButton("Thrust");
        }
    }
}
