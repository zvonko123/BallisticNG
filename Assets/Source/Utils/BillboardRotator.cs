using UnityEngine;
using System.Collections;

public class BillboardRotator : MonoBehaviour
{

    [Header("[ ROTATION SETTINGS ]")]
    [Range(0.0f, 360.0f)]
    public float rotStart;
    public float rotSpeed;
    public float rotWait;

    [Header("[ ROTATION FLAGS ]")]
    public bool isReverse;
    public bool rotSmooth;

    private float yRot;
    private float rotAmount;
    private float rotWaitTimer;

    void Start()
    {
        // set start rotations
        yRot = rotStart;
        rotAmount = rotSpeed;
    }

    void FixedUpdate()
    {
        rotWaitTimer += Time.deltaTime;
        if (rotWaitTimer > rotWait)
        {
            // update rotation
            if (isReverse)
            {
                yRot -= 180.0f;

                if (rotSmooth)
                    rotAmount = (yRot < -360) ? 0 : rotAmount;
                yRot = (yRot < -360) ? 0 : yRot;
            }
            else
            {
                yRot += 180.0f;

                if (rotSmooth)
                    rotAmount = (yRot > 360) ? 0 : rotAmount;
                yRot = (yRot > 360) ? 0 : yRot;
            }

            // reset timer
            rotWaitTimer = 0.0f;
        }

        // apply rotation
        if (rotSmooth)
            rotAmount = Mathf.Lerp(rotAmount, yRot, Time.deltaTime * rotSpeed);
        else
            rotAmount = Mathf.MoveTowardsAngle(rotAmount, yRot, Time.deltaTime * rotSpeed);
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rotAmount, transform.eulerAngles.z);
        
    }
}
