using UnityEngine;
using UnityEngine.Networking;
using BnG.TrackData;
using System.Collections;

public class ShipBase : NetworkBehaviour {

    public ShipRefs r;
}

public class ShipRefs : NetworkBehaviour
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

    public bool shieldActivate;
    public float shieldTimer;
    public Color shieldColor;

    public int boostState;
    public float boostAccel;
    public float boostTimer;
    public TrTile lastBoost;

    public bool attachedFinalCam;
    public PickupBase pickup;
    public E_WEAPONS weaponClass = E_WEAPONS.NONE;

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
    public bool loadedBestTime;
    public bool finished;

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

        // try to fetch best time
        if (!isAI)
        {
            TimeData td = SaveData.ReadTime(Application.loadedLevelName, RaceSettings.speedclass);
            if (td.dataFound)
            {
                loadedBestTime = true;
                bestLap = td.time;
            } else
            {
                Debug.Log("No time data found!");
            }
        }

    }

    void Update()
    {
        // weapon update
        if (pickup != null)
            pickup.OnUpdate();
    }

    private void RaceTimers()
    {
        if (!finished)
        {
            totalTime += Time.deltaTime;
            currentTime += Time.deltaTime;

            checkpoint -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (recharging)
            shield += Time.deltaTime * 20;

        if (shield > settings.DAMAGE_SHIELD)
            shield = settings.DAMAGE_SHIELD;

        if (shield < 0)
        {
            shield = 0;

            // ship is dead
            if (!isDead)
            {
                isDead = true;

                // make ship fall to ground
                settings.AG_HOVER_HEIGHT *= 0.5f;

                // play explosion particle
                GameObject particle = Instantiate(Resources.Load("Particles/EXPLOSION") as GameObject) as GameObject;
                particle.transform.parent = transform;
                particle.transform.localPosition = Vector3.zero;
            }
        }

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

        if (shieldTimer > 0)
            shieldTimer -= Time.deltaTime;

        shieldActivate = shieldTimer > 0;

        // lerp shield color back to engine color
        shieldColor = Color.Lerp(shieldColor, settings.REF_ENGINECOL, Time.deltaTime * 2);

        // boost timer
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
        }
        else
        {
            boostState = 0;
            boostAccel = 0;
        }
    }

    public void PickupItem()
    {
        // if the ship already has a pickup then return
        if (weaponClass != E_WEAPONS.NONE)
            return;

        // get length of weapons enum (minus none at the end)
        int max = System.Enum.GetNames(typeof(E_WEAPONS)).Length - 1;
        int rand = Random.Range(0, max);
        E_WEAPONS weap = (E_WEAPONS)rand;
        switch (weap)
        {
            case E_WEAPONS.AUTOPILOT:
                PickupAutopilot ap = new PickupAutopilot();
                ap.OnInit(0.0f, this, weap);
                pickup = ap;
                break;
            case E_WEAPONS.BOMB:
                PickupBomb bo = new PickupBomb();
                bo.OnInit(0.0f, this, weap);
                pickup = bo;
                break;
            case E_WEAPONS.CANNONS:
                PickupCannons ca = new PickupCannons();
                ca.OnInit(0.0f, this, weap);
                pickup = ca;
                break;
            case E_WEAPONS.CLOAK:
                PickupCloak cl = new PickupCloak();
                cl.OnInit(0.0f, this, weap);
                pickup = cl;
                break;
            case E_WEAPONS.ENERGYWALL:
                PickupEnergyWall ew = new PickupEnergyWall();
                ew.OnInit(0.0f, this, weap);
                pickup = ew;
                break;
            case E_WEAPONS.EPACK:
                PickupEPack ep = new PickupEPack();
                ep.OnInit(0.0f, this, weap);
                pickup = ep;
                break;
            case E_WEAPONS.FSHIELD:
                PickupFShield fs = new PickupFShield();
                fs.OnInit(0.0f, this, weap);
                pickup = fs;
                break;
            case E_WEAPONS.IMPULSE:
                PickupImpulse im = new PickupImpulse();
                im.OnInit(0.0f, this, weap);
                pickup = im;
                break;
            case E_WEAPONS.MINES:
                PickupMines mi = new PickupMines();
                mi.OnInit(0.0f, this, weap);
                pickup = mi;
                break;
            case E_WEAPONS.MISSILES:
                PickupMissiles mis = new PickupMissiles();
                mis.OnInit(0.0f, this, weap);
                pickup = mis;
                break;
            case E_WEAPONS.PLASMABOLT:
                PickupPlasmaBolt pl = new PickupPlasmaBolt();
                pl.OnInit(0.0f, this, weap);
                pickup = pl;
                break;
            case E_WEAPONS.REFLECTOR:
                PickupReflector re = new PickupReflector();
                re.OnInit(0.0f, this, weap);
                pickup = re;
                break;
            case E_WEAPONS.ROCKETS:
                PickupRockets ro = new PickupRockets();
                ro.OnInit(0.0f, this, weap);
                pickup = ro;
                break;
            case E_WEAPONS.SHIELD:
                PickupShield sh = new PickupShield();
                sh.OnInit(0.0f, this, weap);
                pickup = sh;
                break;
            case E_WEAPONS.TRIPLASER:
                PickupTripLaser tl = new PickupTripLaser();
                tl.OnInit(0.0f, this, weap);
                pickup = tl;
                break;
        }
    }

    public void ActivateShield(float time)
    {
        shieldTimer = time;
    }

    public void ShieldDamage()
    {
        if (shieldActivate)
        {
            PlayOneShot(settings.SFX_SHIELDHIT);
            shieldColor = new Color(1.0f - settings.REF_ENGINECOL.r, 1.0f - settings.REF_ENGINECOL.g, 1.0f - settings.REF_ENGINECOL.b, 0.5f);
        }
    }

    public void TakeDamage(float amount)
    {
        shield -= amount * settings.DAMAGE_MULT;
    }

    public void SetAudioEnabled(bool enabled)
    {
        effects.audioContainer.SetActive(enabled);
    }

    public void HitSpeedPad()
    {
        if (boostState < 3)
        {
            boostTimer += 1.5f;
            boostAccel += sim.engineThrust * 0.1f;
            boostState++;
        } else
        {
            boostTimer += 0.5f;
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
        if (currentLap > 0 && currentLap <= RaceSettings.laps)
        {
            // set lap time
            laps[currentLap - 1] = currentTime;

            // set perfect laps
            perfects[currentLap - 1] = perfectLap;

            // compare best time
            if ((currentTime < bestLap || !hasBestTime) && !loadedBestTime)
            {
                bestLap = currentTime;
                hasBestTime = true;
            }

            // notify of perfect lap
            if (perfectLap)
                perfectLapPopup = 1.0f;
        }

        if (currentLap >= RaceSettings.laps)
        {
            if (!finished)
            {
                finished = true;
                if (!isAI)
                {
                    // enable and setup results UI
                    RaceSettings.raceManager.FinishedUI.gameObject.SetActive(true);
                    RaceSettings.raceManager.FinishedUI.MenuEnabled();
                    RaceSettings.raceManager.FinishedUI.LoadResults();

                    // disable race UI
                    RaceSettings.raceManager.RaceUI.gameObject.SetActive(false);

                    // show cursor
                    Cursor.visible = true;

                    // save time
                    bool canWrite = false;
                    if (loadedBestTime)
                    {
                        if (totalTime < bestLap)
                            canWrite = true;
                    } else
                    {
                        canWrite = true;
                    }

                    if (canWrite)
                     SaveData.WriteTime(Application.loadedLevelName, RaceSettings.speedclass, totalTime);
                }
            }

            if (!attachedFinalCam)
            {
                // destroy camera
                Destroy(cam.GetComponent<ShipCamera>());

                // add final camera
                cam.gameObject.AddComponent<ShipFCam>();
                cam.GetComponent<ShipFCam>().r = this;
                attachedFinalCam = true;
                isAI = true;
            }
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
