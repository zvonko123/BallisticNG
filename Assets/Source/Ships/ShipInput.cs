using UnityEngine;
using BnG.TrackData;
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

    // ai settings
    private float xStableRatio = 1.0f;
    private float aiRacingLineRand;
    private float aiSteerSpeedRand;

    private Quaternion aiSteer;
    private float aiDrag;
    private float aiSteerTilt;
    private float rotDelta;
    private float prevRot;
    private float aiResistance;

    void Start()
    {
        // randomize AI
        xStableRatio = Random.Range(0.6f, 0.8f);
        aiRacingLineRand = Random.Range(0.8f, 1.5f);
        aiSteerSpeedRand = Random.Range(6, 10);
    }

    void Update()
    {
        if (r.isAI)
        {
            // cancel out all input
            AXIS_STEER = 0.0f;
            AXIS_PITCH = 0.0f;
            AXIS_LEFTAIRBRAKE = 0.0f;
            AXIS_RIGHTAIRBRAKE = 0.0f;
            AXIS_BOTHAIRBRAKES = 0.0f;
            ACTION_SPECIAL = false;
            AIUpdate();
        }
        else
        {
            if (r.isDead)
            {
                // no steering/pitch
                AXIS_STEER = 0;
                AXIS_PITCH = 0;

                // apply both brakes to slow the ship down
                AXIS_BOTHAIRBRAKES = 0.0f;
                AXIS_LEFTAIRBRAKE = 1.0f;
                AXIS_RIGHTAIRBRAKE = 1.0f;

                // no thrust
                ACTION_THRUST = false;

            }
            else
            {
                // if ship restrained then do not read any input
                if (!RaceSettings.shipsRestrained && !r.shipRestrained)
                {
                    AXIS_STEER = Input.GetAxis("Steer");
                    AXIS_PITCH = Input.GetAxis("Pitch");
                    AXIS_LEFTAIRBRAKE = Input.GetAxis("Left Airbrake");
                    AXIS_RIGHTAIRBRAKE = Input.GetAxis("Right Airbrake");
                    ACTION_SPECIAL = Input.GetButton("Special");
                }
                else
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

    void AIUpdate()
    {
        // ai always thrusts
        ACTION_THRUST = true;

        float lookAheadMult = 0.3f * aiRacingLineRand;
        int lookAheadAmount = Mathf.RoundToInt(transform.InverseTransformDirection(r.body.velocity).z * lookAheadMult);
        if (lookAheadAmount < 2)
            lookAheadAmount = 2;
        lookAheadAmount = 3;

        Vector3 sectionPos = AILookAhead(lookAheadAmount);
        sectionPos.y = 0;

        Vector3 flatPos = transform.position;
        flatPos.y = 0;

        // get lookat
        Vector3 lookPos = sectionPos - flatPos;
        aiSteer = Quaternion.Lerp(aiSteer, Quaternion.LookRotation(lookPos), Time.deltaTime * 20);
        Quaternion lookRot = aiSteer;

        // rotate ship
        Vector3 tempRot = transform.eulerAngles;
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 8f);
        transform.rotation = Quaternion.Euler(tempRot.x, transform.eulerAngles.y, tempRot.z);

        // rotation delta
        rotDelta = Mathf.DeltaAngle(transform.eulerAngles.y, prevRot);
        prevRot = transform.eulerAngles.y;

        // override ship tilt
        aiSteerTilt = Mathf.Lerp(aiSteerTilt, rotDelta * 20, Time.deltaTime * 5);
        aiSteerTilt = Mathf.Clamp(aiSteerTilt, -55.0f, 55.0f);
        r.axis.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, aiSteerTilt);

        // ai drag
        aiResistance = Mathf.Lerp(aiResistance, Mathf.Clamp(Mathf.Abs(rotDelta * 0.02f), 0.0f, 1.0f), Time.deltaTime * 5);

        Vector3 lv = transform.InverseTransformDirection(r.body.velocity);
        lv.z *= 1 - aiResistance;
        Vector3 wv = transform.TransformDirection(lv);
        r.body.velocity = wv;


        // stabalize
        r.body.AddForce(transform.right * (-transform.InverseTransformDirection(r.body.velocity).x * 2));
    }

    private Vector3 AILookAhead(int amount)
    {
        TrSection start = r.currentSection;
        TrSection next = start;
        for (int i = 0; i < amount; ++i)
        {
            next = next.SECTION_NEXT;
        }

        return next.SECTION_POSITION;
    }
}
