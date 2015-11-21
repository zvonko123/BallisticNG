using UnityEngine;
using BnG.TrackData;
using BnG.Helpers;
using System.Collections;

public class ShipEffects : ShipBase {

    // ship color
    private Color shipColor;

    // audio settings
    private GameObject audioContainer;
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

    // droid settings
    public float droidBaseHeight;
    private float droidHeight;

    void Start()
    {
        // get droid base height
        droidBaseHeight = r.settings.REF_DROID.transform.localPosition.y;

        CreateAudioEffects();
        SetupEngineEffects();
    }

    void FixedUpdate()
    {
        UpdateShipLighting();
        UpdateSounds();
        UpdateEngine();
        DroidManager();
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
            int tri = hit.triangleIndex;
            MeshCollider mc = hit.collider as MeshCollider;
            Mesh m = mc.sharedMesh;

            // get tile
            TrTile tile = TrackDataHelper.TileFromTriangleIndex(tri, E_TRACKMESH.FLOOR, RaceSettings.trackData.TRACK_DATA);

            shipColor = m.colors32[m.triangles[hit.triangleIndex * 3]];
            if (tile.TILE_TYPE == E_TILETYPE.RECHARGE)
                r.recharging = true;

            if (tile.TILE_ISWET && r.sim.isShipGrounded)
            {
                overWetSurface = true;
                sprayPosition = hit.point;
                sprayPosition.x = transform.position.x;
                sprayPosition.z = transform.position.z;
            }

            // boost pad management
            if (tile.TILE_TYPE == E_TILETYPE.BOOST)
            {
                TrSection section = TrackDataHelper.TileGetSection(tile);
                Quaternion padRot = TrackDataHelper.SectionGetRotation(section);
                Vector3 padDir = padRot * Vector3.forward;

                r.body.AddForce(padDir * 35, ForceMode.Acceleration);
            }
        }
        r.mesh.GetComponent<Renderer>().material.SetColor("_Color", shipColor);

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
        engineSFX.volume = AudioSettings.VOLUME_MAIN;

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
        if (r.sim.isShipScraping)
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
        vapeTrailOpacity = engineNorm;
        r.settings.REF_VAPE_LEFT.material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, vapeTrailOpacity));
        r.settings.REF_VAPE_RIGHT.material.SetColor("_TintColor", new Color(1.0f, 1.0f, 1.0f, vapeTrailOpacity));

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

        r.settings.REF_ENGINE_FLARE.transform.rotation = Quaternion.Euler(transform.eulerAngles.x - 90.0f, transform.eulerAngles.y, 0.0f);

    }

    private void DroidManager()
    {
        if (r.isRespawning)
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
    }

    private void CreateAudioEffects()
    {
        float aiMinDistance = 0.5f;
        float aiMaxDistance = 3.0f;
        audioContainer = new GameObject("AudioContainer");
        audioContainer.transform.parent = transform;

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
