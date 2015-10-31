using UnityEngine;
using System.Collections;

public class ShipSettings : MonoBehaviour {

    [Header("[ ENGINE FORCE ]")]
    public float ENGINE_MAXSPEED_SPARK;
    public float ENGINE_MAXSPEED_TOXIC;
    public float ENGINE_MAXSPEED_APEX;
    public float ENGINE_MAXSPEED_HALBERD;
    public float ENGINE_MAXSPEED_SPECTRE;

    [Header("[ ACCELERATION ]")]
    public float ENGINE_ACCELERATION_SPARK;
    public float ENGINE_ACCELERATION_TOXIC;
    public float ENGINE_ACCELERATION_APEX;
    public float ENGINE_ACCELERATION_HALBERD;
    public float ENGINE_ACCELERATION_SPECTRE;

    [Header("[ ENGINE POWER ]")]
    public float ENGINE_POWER_GAIN_SPARK;
    public float ENGINE_POWER_FALLOFF_SPARK;
    public float ENGINE_POWER_GAIN_TOXOIC;
    public float ENGINE_POWER_FALLOFF_TOXIC;
    public float ENGINE_POWER_GAIN_APEX;
    public float ENGINE_POWER_FALLOFF_APEX;
    public float ENGINE_POWER_GAIN_HALBERD;
    public float ENGINE_POWER_FALLOFF_HALBERD;
    public float ENGINE_POWER_GAIN_SPECTRE;
    public float ENGINE_POWER_FALLOFF_SPECTRE;

    [Header("[ TURNING ]")]
    public float TURN_SPEED;
    public float TURN_GAIN;
    public float TURN_FALLOFF;
    public float TILT_GAIN;
    public float TILT_FALLOFF;

    [Header("[ AIRBRAKES ]")]
    public float AIRBRAKE_SPEED;
    public float AIRBRAKE_GAIN;
    public float AIRBRAKE_FALLOFF;
    [Space(5)]
    public float AIRBRAKE_DRAG_GAIN;
    public float AIRBRAKE_DRAG_FALLOFF;
    public float AIRBRAKE_DRAG_MULT;

    [Header("[ GRAVITY ]")]
    public float GRAVITY_FORCE;
    public float GRAVITY_WEIGHT;
    [Range(0, 1)]
    public float GRAVITY_RESISTANCE;

    [Header("[ ANTI-GRAVITY ]")]
    public float AG_HOVER_HEIGHT;
    public float AG_HOVER_FORCE;
    public float AG_HOVER_DAMP;
    public float AG_REBOUND_THRESHOLD;
    public float AG_REBOUND_MULTIPLIER;
    public float AG_ROTATION_SPEED;
    public float AG_PITCH_AMOUNT;


    [Header("[ DRAG ]")]
    public float AG_GRIP;
    public float AG_SLIP_AMOUNT;
    public float AG_SLIP_GAIN;
    public float AG_SLIP_FALLOFF;
    public float AG_SLIDE_AMOUNT;
    public float AG_SLIDE_GAIN;
    public float AG_SLIDE_FALLOFF;

    [Header("[ CAMERA ]")]
    public Vector3 CAMERA_OFFSET_TRACK;
    public Vector3 CAMERA_OFFSET_SENSITIVITY;
    public Vector3 CAMERA_OFFSET_SPEED;

    [Header("[ RACE ]")]
    public float DAMAGE_SHIELD;
    public float DAMAGE_MULT;
    public bool TOGGLE_PICKUPSALLOWED;

    [Header("[ SFX ]")]
    public AudioClip SFX_ENGINE;
    public AudioClip SFX_TURBULENCE;
    public AudioClip SFX_AIRBRAKE;
    public AudioClip SFX_SCRAPE;
    public AudioClip SFX_WALLHIT;
    public AudioClip SFX_STARTBOOST;

    [Header("[ REFERENCES ]")]
    public TrailRenderer REF_VAPE_LEFT;
    public TrailRenderer REF_VAPE_RIGHT;
    public GameObject REF_ENGINE_FLARE;
    public GameObject REF_ENGINE_TRAIL_PLAYER;
    public TrailRenderer REF_ENGINE_TRAIL_AI;

}
