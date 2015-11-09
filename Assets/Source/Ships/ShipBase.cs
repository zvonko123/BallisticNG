using UnityEngine;
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
    #endregion

    #region METHODS
    private void FixedUpdate()
    {
        if (recharging)
            shield += Time.deltaTime * 20;

        if (shield > settings.DAMAGE_SHIELD)
            shield = settings.DAMAGE_SHIELD;

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
    #endregion
}
