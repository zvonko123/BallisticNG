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

    // audio modifiers
    private float enginePitch = 0.5f;
    private float turbulanceVolume = 0.0f;

    void Start()
    {
        CreateAudioEffects();
    }

    void FixedUpdate()
    {
        UpdateShipLighting();
        UpdateSounds();
    }

    private void UpdateShipLighting()
    {
        // get ship color from vertex colors of track mesh
        RaycastHit hit;
        Vector3 to = r.currentSection.SECTION_POSITION;
        to.y -= 5;

        r.recharging = false;
        if (Physics.Linecast(transform.position, to, out hit, 1 << LayerMask.NameToLayer("TrackFloor")))
        {
            int tri = hit.triangleIndex;
            MeshCollider mc = hit.collider as MeshCollider;
            Mesh m = mc.sharedMesh;

            // get tile
            TrTile tile = TrackDataHelper.TileFromTriangleIndex(tri, E_TRACKMESH.FLOOR, RaceSettings.trackData.TRACK_DATA);

            if (tile.TILE_TYPE == E_TILETYPE.BOOST)
                shipColor = Color.blue;
            else if (tile.TILE_TYPE == E_TILETYPE.WEAPON)
                shipColor = Color.red;
            else
                shipColor = Color.Lerp(shipColor, m.colors32[m.triangles[hit.triangleIndex * 3]], Time.deltaTime * 15);

            if (tile.TILE_TYPE == E_TILETYPE.RECHARGE)
                r.recharging = true;
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
        float wantedPitch = ((r.transform.InverseTransformDirection(r.body.velocity).z * 12) * Time.deltaTime);
        wantedPitch = Mathf.Clamp(wantedPitch, 0.5f, 1.3f);
        enginePitch = Mathf.Lerp(enginePitch, wantedPitch, Time.deltaTime * 5);
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
