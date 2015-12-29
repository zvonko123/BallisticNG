using UnityEngine;
using BnG.TrackData;
using BnG.Helpers;
using System.Collections;

public class ShipEffects : ShipBase {

    // ship color
    private Color shipColor;
    private Color32[] trackColors;
    private int[] trackTriangles;
    private Mesh m;
    private Mesh prevMesh;
    private TrTile tile;
    private Material shipMat;
    private Material droidMat;

    // audio settings
    public GameObject audioContainer;
    private AudioSource engineSFX;
    private AudioSource turbulanceSFX;
    private AudioSource droidSFX;
    private AudioSource scrapeSFX;
    private AudioSource rechargeSFX;
    private AudioSource spraySFX;

    // audio modifiers
    private float enginePitch = 0.5f;
    private float turbulanceVolume = 0.0f;

    // vape trails
    private float vapeTrailOpacity;
    private bool overWetSurface;
    private Vector3 sprayPosition;

    // engine effects
    private Material engineFlare;
    private Material engineTrail;

    private Color engineColor;
    private float engineColorTimer;
    private float engineColorTimer2;
    private Vector3 engineFlareSize;
    private GameObject aiTrail;
    private Material aiTrailMat;

    // droid settings
    public float droidBaseHeight;
    private float droidHeight;

    // shield
    public float shieldOsc;
    public float shieldTime;
    public float shieldAlpha;
    public Material shieldMat;

    public float shipDeadMult = 1.0f;

    public override void OnInit()
    {
        // get droid base height
        droidBaseHeight = r.settings.REF_DROID.transform.localPosition.y;

        // set shield material
        shieldMat = r.settings.REF_SHIELD.GetComponent<MeshRenderer>().material;

        // get ship material
        shipMat = r.settings.REF_MESH.GetComponent<MeshRenderer>().material;

        // get droid material
        droidMat = r.settings.REF_DROID.GetComponent<MeshRenderer>().material;

        // load ai trail
        aiTrail = Instantiate(Resources.Load("Prefabs/AITrail") as GameObject) as GameObject;
        aiTrail.transform.parent = r.settings.REF_ENGINE_TRAIL_PLAYER.transform.parent;
        aiTrail.transform.localPosition = r.settings.REF_ENGINE_TRAIL_PLAYER.transform.localPosition;
        aiTrailMat = aiTrail.GetComponent<TrailRenderer>().material;

        CreateAudioEffects();
        SetupEngineEffects();
    }

    public override void OnUpdate()
    {
        UpdateShipLighting();
        UpdateSounds();
        UpdateEngine();
        DroidManager();
        ShieldManager();
    }

    private void SetupEngineEffects()
    {
        engineFlare = r.settings.REF_ENGINE_FLARE.GetComponent<Renderer>().material;
        engineTrail = r.settings.REF_ENGINE_TRAIL_PLAYER.GetComponent<Renderer>().material;
        engineFlareSize = r.settings.REF_ENGINE_FLARE.transform.localScale;
    }

    private void UpdateShipLighting()
    {
        // get ship color from vertex colors of track mesh
        RaycastHit hit;
        Vector3 to = r.currentSection.SECTION_POSITION;
        to.y -= 5;

        r.recharging = false;
        overWetSurface = false;
        if (Physics.Linecast(transform.position, to, out hit, 1 << LayerMask.NameToLayer("TrackFloor")))
        {
            // get mesh information
            int tri = hit.triangleIndex;
            MeshCollider mc = hit.collider as MeshCollider;
            m = mc.sharedMesh;

            // if mesh changes then cache needed information (optimization - prevents the need for garbage collection)
            if (prevMesh != m)
            {
                trackColors = m.colors32;
                trackTriangles = m.triangles;
                prevMesh = m;
            }

            // get tile
            tile = TrackDataHelper.TileFromTriangleIndex(tri, E_TRACKMESH.FLOOR, RaceSettings.trackData.TRACK_DATA);

            // get color from track mesh (AI uses cached colors for performance, player ship grabs current color)
            if (r.isAI)
                shipColor = trackColors[trackTriangles[hit.triangleIndex * 3]];
            else
                shipColor = m.colors32[trackTriangles[hit.triangleIndex * 3]];

            if (tile.TILE_TYPE == E_TILETYPE.RECHARGE)
                r.recharging = true;

            if (tile.TILE_ISWET && r.sim.isShipGrounded)
            {
                overWetSurface = true;
                sprayPosition = hit.point;
                sprayPosition.x = transform.position.x;
                sprayPosition.z = transform.position.z;
            }

            if (tile.TILE_TYPE == E_TILETYPE.WEAPON)
                r.PickupItem();

            // boost pad management
            if (tile.TILE_TYPE == E_TILETYPE.BOOST)
            {
                TrSection section = TrackDataHelper.TileGetSection(tile);
                Quaternion padRot = TrackDataHelper.SectionGetRotation(section);
                Vector3 padDir = padRot * Vector3.forward;

                r.body.AddForce(padDir * 35, ForceMode.Acceleration);

                if (tile != r.lastBoost)
                {
                    r.HitSpeedPad();
                    r.lastBoost = tile;
                }
            } else
            {
                r.lastBoost = null;
            }
        }
        // dead color
        if (r.isDead)
        {
            shipDeadMult = Mathf.Lerp(shipDeadMult, 0.15f, Time.deltaTime * 3);
            shipColor *= shipDeadMult;
        }

        shipMat.SetColor("_Color", shipColor);

        if (r.recharging)
            rechargeSFX.volume = 1.0f;
        else
            rechargeSFX.volume = 0.0f;

    }

    private void UpdateSounds()
    {
        // engine sfx
        float wantedPitch = ((r.transform.InverseTransformDirection(r.body.velocity).z * 7) * Time.deltaTime);
        wantedPitch = Mathf.Clamp(wantedPitch, 0.5f, 2.0f);
        enginePitch = Mathf.Lerp(enginePitch, wantedPitch, Time.deltaTime * 1.5f);
        engineSFX.pitch = enginePitch;
        if (!r.isDead)
            engineSFX.volume = AudioSettings.VOLUME_MAIN;
        else
            engineSFX.volume = Mathf.Lerp(engineSFX.volume, 0.0f, Time.deltaTime * 5);

        // turbulance SFX
        if (r.input.AXIS_BOTHAIRBRAKES != 0)
        {
            turbulanceVolume = Mathf.Lerp(turbulanceVolume, Mathf.Abs(r.sim.airbrakeAmount * 3) * AudioSettings.VOLUME_MAIN, Time.deltaTime * 3);
            turbulanceSFX.panStereo = Mathf.Lerp(turbulanceSFX.panStereo, Mathf.Sign(r.sim.airbrakeAmount), Time.deltaTime * 5);
        }
        else
        {
            turbulanceVolume = Mathf.Lerp(turbulanceVolume, 0.0f, Time.deltaTime * 3);
            turbulanceSFX.panStereo = Mathf.Lerp(turbulanceSFX.panStereo, 0.0f, Time.deltaTime * 5);
        }
        turbulanceSFX.volume = turbulanceVolume;

        // scrape SFX
        if (r.sim.isShipScraping && !r.shieldActivate && !r.isDead)
        {
            if (!scrapeSFX.isPlaying)
                scrapeSFX.Play();

            scrapeSFX.volume = AudioSettings.VOLUME_MAIN;
        }
        else
        {
            scrapeSFX.volume = Mathf.Lerp(scrapeSFX.volume, 0.0f, Time.deltaTime * 3);
            if (scrapeSFX.volume < 0.1f & scrapeSFX.isPlaying)
                scrapeSFX.Stop();
        }

        // spray sfx
        if (overWetSurface)
        {
            spraySFX.volume = Mathf.Lerp(spraySFX.volume, 1.0f, Time.deltaTime * 3);
            r.settings.REF_SPRAYFX.enableEmission = true;
            r.settings.REF_SPRAYFX.transform.position = sprayPosition;
        }
        else
        {
            spraySFX.volume = Mathf.Lerp(spraySFX.volume, 0.0f, Time.deltaTime * 3);
            r.settings.REF_SPRAYFX.enableEmission = false;
        }
    }

    private void UpdateEngine()
    {
        // vape trails
        float maxSpeed = 0.0f;
        switch(RaceSettings.speedclass)
        {
            case E_SPEEDCLASS.SPARK:
                maxSpeed = r.settings.ENGINE_MAXSPEED_SPARK;
                break;
            case E_SPEEDCLASS.TOXIC:
                maxSpeed = r.settings.ENGINE_MAXSPEED_TOXIC;
                break;
            case E_SPEEDCLASS.APEX:
                maxSpeed = r.settings.ENGINE_MAXSPEED_APEX;
                break;
            case E_SPEEDCLASS.HALBERD:
                maxSpeed = r.settings.ENGINE_MAXSPEED_HALBERD;
                break;
            case E_SPEEDCLASS.SPECTRE:
                maxSpeed = r.settings.ENGINE_MAXSPEED_SPECTRE;
                break;
        }

        float engineNorm = ((r.sim.engineThrust * 0.3f) / maxSpeed) * 0.3f;
        vapeTrailOpacity = engineNorm * 0.5f;
        r.settings.REF_VAPE_LEFT.material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, vapeTrailOpacity));
        r.settings.REF_VAPE_RIGHT.material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, vapeTrailOpacity));
        r.settings.REF_VAPE_LEFT.gameObject.SetActive(!r.isAI);
        r.settings.REF_VAPE_RIGHT.gameObject.SetActive(!r.isAI);

        // engine flare and trail
        engineNorm = (Mathf.Clamp01((r.sim.enginePower * 5)) / 1.0f);
        Vector3 size = Mathf.Clamp(engineNorm, 0.7f, 1.0f) * engineFlareSize;
        r.settings.REF_ENGINE_FLARE.transform.localScale = size;

        engineColorTimer2 += Time.deltaTime;
        float colSinSin = Mathf.Abs(Mathf.Sin(engineColorTimer2));
        colSinSin = Mathf.Clamp(colSinSin, 0.3f, 1.0f);

        engineColorTimer += Time.deltaTime * 20;
        float colSin = Mathf.Sin(engineColorTimer);

        engineColor = Color.Lerp(r.settings.REF_ENGINECOL, r.settings.REF_ENGINECOL_BRIGHT, Mathf.Abs(colSin) * colSinSin);
        engineColor.a = engineNorm * 0.4f;
        engineFlare.SetColor("_TintColor", engineColor);

        engineNorm = (Mathf.Clamp01((r.sim.enginePower * 2)) / 1.0f);
        engineColor.a = engineNorm * r.settings.REF_ENGINECOL.a;
        engineTrail.SetColor("_TintColor", engineColor);
        aiTrailMat.SetColor("_TintColor", engineColor);

        r.settings.REF_ENGINE_FLARE.transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 90.0f, transform.eulerAngles.y, 0.0f);

        // toggle engine objects
        r.settings.REF_ENGINE_FLARE.SetActive(!r.isAI);
        r.settings.REF_ENGINE_TRAIL_PLAYER.SetActive(!r.isAI);
        aiTrail.gameObject.SetActive(r.isAI);

    }

    private void DroidManager()
    {
        if (r.isRespawning || RaceSettings.shipsRestrained)
        {
            droidHeight = droidBaseHeight;
        } else
        {
            droidHeight = Mathf.Lerp(droidHeight, droidBaseHeight * 15, Time.deltaTime * 0.8f);
        }

        if (droidHeight > droidBaseHeight * 13 && r.settings.REF_DROID.activeSelf)
            r.settings.REF_DROID.SetActive(false);
        else if (droidHeight < droidBaseHeight * 13 && !r.settings.REF_DROID.activeSelf)
            r.settings.REF_DROID.SetActive(true);

        r.settings.REF_DROID.transform.localPosition = new Vector3(0.0f, droidHeight, 0.0f);

        droidMat.SetColor("_Color", shipColor);
    }

    private void ShieldManager()
    {
        shieldTime += Time.deltaTime * 5;
        shieldOsc = Mathf.Sin(shieldTime) * 0.4f;
        shieldMat.SetTextureOffset("_MainTex", new Vector2(0.0f, shieldOsc));

        Color tint = r.shieldColor;

        if (r.shieldActivate)
            shieldAlpha = 0.5f;
        else
            shieldAlpha = Mathf.Lerp(shieldAlpha, 0.0f, Time.deltaTime * 5);

        tint.a = shieldAlpha;
        shieldMat.SetColor("_TintColor", tint);
    }

    private void CreateAudioEffects()
    {
        float aiMinDistance = 0.5f;
        float aiMaxDistance = 3.0f;
        audioContainer = new GameObject("AudioContainer");
        audioContainer.transform.parent = transform;
        audioContainer.transform.localPosition = Vector3.zero;

        engineSFX = AttachNewSound(r.settings.SFX_ENGINE, r.isAI, aiMinDistance, aiMaxDistance, true, true);
        turbulanceSFX = AttachNewSound(r.settings.SFX_TURBULENCE, r.isAI, aiMinDistance, aiMaxDistance, true, true);
        scrapeSFX = AttachNewSound(r.settings.SFX_SCRAPE, r.isAI, aiMinDistance, aiMaxDistance, true, true);
        scrapeSFX.volume = 0.0f;
        rechargeSFX = AttachNewSound(r.settings.SFX_RECHARGE, r.isAI, aiMinDistance, aiMaxDistance, true, true);
        rechargeSFX.volume = 0.0f;
        spraySFX = AttachNewSound(r.settings.SFX_SPRAY, r.isAI, aiMinDistance, aiMaxDistance, true, true);
        spraySFX.volume = 0.0f;
        
    }

    private AudioSource AttachNewSound(AudioClip clip, bool isAi, float minDistance, float maxDistance, bool loop, bool play)
    {
        AudioSource newSound = audioContainer.AddComponent<AudioSource>();
        newSound.clip = clip;

        if (isAi)
            newSound.spatialBlend = 1;
        else
            newSound.spatialBlend = 0;
        newSound.maxDistance = maxDistance;
        newSound.minDistance = minDistance;

        if (loop)
            newSound.loop = true;
        if (play)
            newSound.Play();

        return newSound;

    }
}
