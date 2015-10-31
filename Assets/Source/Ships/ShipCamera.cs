using UnityEngine;
using BnG.TrackData;
using BnG.Helpers;
using System.Collections;

public class ShipCamera : ShipBase {

    // CAMERA SETTINGS
    public Vector3 cOffset = new Vector3(0.0f, 0.15f, -1f);
    public float cFoV = 75.0f;
    public float tcSpeed = 13.0f;
    public float sensitivity = 0.15f;

    private float tcDirectionLag;
    private Vector3 tcActualOffset;
    private float tcPitchOffset;
    private float tcInputOffset;

    private Vector3 tcTrackOffset;
    private Vector3 tcFinalOffset;

    private float tcSectionLerp;
    private Vector3 tcSectionOffset;
    private float tcFallOffset;

    private float tcX;
    private float tcY;
    private float tcZ;

    private int cameraMode;
    private bool lookingBehind;

    // FALL LAG
    private float tcFallTimer;
    private float tcFallTimerGain;
    private float tcFallLagY;
    private float tcFallLagZ;

    private GameObject cameraHelper;

    void Start()
    {
        // Create new transform helper
        cameraHelper = new GameObject("_TRACKHELPER");
    }

    void Update()
    {
        // Cycle camera mode
        if (Input.GetButtonDown("Camera"))
            cameraMode++;

        if (cameraMode > 1)
            cameraMode = 0;

        // Look Behind Checks
        if (Input.GetButtonDown("Look Behind"))
        {
            r.mesh.GetComponent<MeshRenderer>().enabled = true;
            lookingBehind = true;
        }

        if (Input.GetButtonUp("Look Behind"))
        {
            if (cameraMode == 0)
                r.mesh.GetComponent<MeshRenderer>().enabled = false;
            lookingBehind = false;
        }
    }

    void FixedUpdate()
    {
        if (r.isRespawning && cameraMode == 0)
        {
            ReverseChase();
        }
        else
        {
            if (lookingBehind)
            {
                ReverseChase();
            }
            else
            {
                switch (cameraMode)
                {
                    case 0:
                        // Enable ship mesh
                        r.mesh.GetComponent<MeshRenderer>().enabled = true;
                        FollowTrack();
                        break;
                    case 1:
                        // Disable ship mesh
                        r.mesh.GetComponent<MeshRenderer>().enabled = false;
                        InternalCamera();
                        break;
                }
            }
        }
    }

    private void ReverseChase()
    {
        // parent camera
        transform.parent = r.transform;

        // offset camera
        transform.localPosition = new Vector3(0.0f, cOffset.y, -cOffset.z * 0.6f);
        transform.localRotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);

        // update FoV
        r.cam.fieldOfView = 75.0f;
    }

    private void FollowTrack()
    {
        // parent camera
        transform.parent = r.transform;

        // only follow track if current section exist
        if (r.currentSection == null)
            return;

        // position and rotate track helper (this is so we have a transform to work with)
        TrSection current = r.currentSection;
        Vector3 currentSegment = current.SECTION_POSITION;
        cameraHelper.transform.position = currentSegment;

        Quaternion currentRot = TrackDataHelper.SectionGetRotation(current);
        cameraHelper.transform.rotation = currentRot;

        // figure out the camera's offset to the track
        Vector3 cameraOffset = cameraHelper.transform.InverseTransformPoint(transform.position);

        // figure out which way the ship is facing and interpolate track direction dot product value to it
        Vector3 trackForward = cameraHelper.transform.forward;
        tcDirectionLag = Mathf.Lerp(tcDirectionLag, Vector3.Dot(transform.forward, trackForward), Time.deltaTime * (tcSpeed * 0.5f));

        tcTrackOffset = Vector3.Lerp(tcTrackOffset, cameraOffset * tcDirectionLag, Time.deltaTime * (tcSpeed * 0.65f));
        tcFinalOffset = Vector3.Lerp(tcFinalOffset, cameraOffset, Time.deltaTime * (tcSpeed * 0.65f));

        // figure out which side of the track the camera is on (this is for positioning reasons)
        Vector3 trackSide = cameraHelper.transform.right;
        float sideDot = Vector3.Dot(transform.forward, trackSide);
        float sideFinal = Mathf.Sign(cameraOffset.x) * -Mathf.Sign(sideDot);

        // interpolate positions
        tcX = Mathf.Lerp(tcX, -tcTrackOffset.x * r.settings.CAMERA_OFFSET_SENSITIVITY.x, Time.deltaTime * (r.settings.CAMERA_OFFSET_SPEED.x));
        tcY = Mathf.Lerp(tcY, Mathf.Abs(tcFinalOffset.x) * r.settings.CAMERA_OFFSET_SENSITIVITY.y, Time.deltaTime * (r.settings.CAMERA_OFFSET_SPEED.y * 0.15f));
        tcZ = Mathf.Lerp(tcZ, (Mathf.Abs(cameraOffset.x) * r.settings.CAMERA_OFFSET_SENSITIVITY.z) * (sideFinal * (1 - Mathf.Abs(tcDirectionLag))), Time.deltaTime * (r.settings.CAMERA_OFFSET_SPEED.z * 0.4f));

        // increase/decrease distance to ship on slopes
        float upDir = Vector3.Dot(Vector3.up, r.transform.forward);
        tcPitchOffset = Mathf.Lerp(tcPitchOffset, upDir * 0.2f, Time.deltaTime * tcSpeed);

        // fall Offsets
        if (r.sim.isShipGrounded)
        {
            tcFallTimer = 0;
            tcFallTimerGain = 0;
        }
        else
        {
            tcFallTimer += Time.deltaTime;
        }

        if (tcFallTimer > 0.2f)
        {
            tcFallTimerGain = Mathf.Lerp(tcFallTimerGain, 0.8f, Time.deltaTime);
            tcFallLagY = Mathf.Lerp(tcFallLagY, 0.4f, Time.deltaTime * tcFallTimerGain);
            tcFallLagZ = Mathf.Lerp(tcFallLagZ, -0.8f, Time.deltaTime * (tcFallTimerGain * 4));
        }
        else
        {
            tcFallTimerGain = 0;
            tcFallLagY = Mathf.Lerp(tcFallLagY, 0.0f, Time.deltaTime * 4);
            tcFallLagZ = Mathf.Lerp(tcFallLagZ, 0.0f, Time.deltaTime * 4);
        }

        // apply camera offset
        transform.localPosition = new Vector3(r.settings.CAMERA_OFFSET_TRACK.x + tcX, r.settings.CAMERA_OFFSET_TRACK.y + tcY + tcFallLagY, r.settings.CAMERA_OFFSET_TRACK.z + tcZ + tcPitchOffset + tcFallLagZ);

        // update Rotation
        transform.rotation = r.transform.rotation;

        // update FoV
        r.cam.fieldOfView = 75.0f;
    }

    private void InternalCamera()
    {
        // parent camera
        transform.parent = r.axis.transform;

        // zero out transform
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // update FoV
        r.cam.fieldOfView = 75.0f;
    }

}
