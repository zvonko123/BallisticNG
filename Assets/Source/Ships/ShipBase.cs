﻿using UnityEngine;
using BnG.TrackData;
using System.Collections;

public class ShipBase : MonoBehaviour {

    public ShipRefs r;
}

public class ShipRefs : MonoBehaviour
{
    #region VARIABLES
    // ai
    public bool isAI;
    private float aiSoundMinDistance = 0.1f;
    private float aiSoundMaxDistance = 1.0f;

    // damage
    public bool isDead;
    public float shield;

    // linked Classes
    public ShipSettings settings;
    public ShipPosition position;
    public ShipEffects effects;
    public ShipInput input;
    public ShipSim sim;

    // components
    public GameObject axis;
    public GameObject anim;
    public GameObject mesh;
    public Camera cam;
    public Rigidbody body;

    // track data
    public TrSection currentSection;
    public TrSection initialSection;

    public int tCurrentIndex;

    // others
    public bool shipRestrained;
    public bool isRespawning;
    public bool facingFoward;
    public bool recharging;
    public bool jumpHeight;

    // race times
    public float[] laps;
    public bool[] perfects;
    public float bestLap;
    public int currentLap;

    public bool passedMid;
    public bool perfectLap;
    public float currentTime;
    public float totalTime;
    public float checkpoint;

    public bool hasBestTime;

    // UI timers
    public float perfectLapPopup;
    public float finalLapPopup;

    #endregion

    #region METHODS
    void Start()
    {
        // set lap count
        laps = new float[RaceSettings.laps];
        perfects = new bool[RaceSettings.laps];

        // TEMP: remove once countdown has been added
        RaceSettings.countdownFinished = true;
    }

    private void RaceTimers()
    {
        totalTime += Time.deltaTime;
        currentTime += Time.deltaTime;

        checkpoint -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (recharging)
            shield += Time.deltaTime * 20;

        if (shield > settings.DAMAGE_SHIELD)
            shield = settings.DAMAGE_SHIELD;

        if (shield < 0)
            shield = 0;

        if (recharging)
        {
            if (!settings.REF_RECHARGEFX.activeSelf)
            {
                settings.REF_RECHARGEFX.SetActive(true);
                settings.REF_RECHARGEFX.transform.rotation = Quaternion.Euler(90.0f, transform.eulerAngles.y, 0.0f);
            }
        } else
        {
            if (settings.REF_RECHARGEFX.activeSelf)
            {
                settings.REF_RECHARGEFX.SetActive(false);
            }
        }

        if (currentLap > 0)
            RaceTimers();

        if (perfectLapPopup > -1)
            perfectLapPopup -= Time.deltaTime;

        if (finalLapPopup > -1)
            finalLapPopup -= Time.deltaTime;
    }
    public void PlayOneShot(AudioClip clip)
    {
        if (isAI)
        {
            OneShot.CreateOneShot(clip, transform, AudioSettings.VOLUME_MAIN, 1.0f, aiSoundMinDistance, aiSoundMaxDistance);
        }
        else
        {
            OneShot.CreateOneShot(clip, AudioSettings.VOLUME_MAIN, 1.0f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // respawn
        if (other.tag == "Respawn")
            isRespawning = true;

        if (other.tag == "SLine")
        {
            if (passedMid || currentLap == 0)
            {
                UpdateLap();
                passedMid = false;
            }
        }

        if (other.tag == "MLine")
            passedMid = true;

        if (other.tag == "MReset")
            passedMid = false;
    }

    private void UpdateLap()
    {
        if (currentLap > 0 && currentLap < RaceSettings.laps)
        {
            // set lap time
            laps[currentLap - 1] = currentTime;

            // set perfect laps
            perfects[currentLap - 1] = perfectLap;

            // compare best time
            if (currentTime < bestLap || !hasBestTime)
            {
                bestLap = currentTime;
                hasBestTime = true;
            }

            // notify of perfect lap
            if (perfectLap)
                perfectLapPopup = 1.0f;
        }

        // reset current time
        currentTime = 0;

        // reset perfect lap
        perfectLap = true;

        // increment lap
        currentLap++;

        // notify of perfect lap
        if (currentLap == RaceSettings.laps)
            finalLapPopup = 1.0f;
    }
    #endregion
}
