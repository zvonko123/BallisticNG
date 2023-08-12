﻿using UnityEngine;
using UnityEngine.Networking;
using BnG.TrackData;
using UnityStandardAssets.ImageEffects;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour {

    #region VARIABLES
    [Header("[ TRACK DATA ]")]
    public TrData trackData;
    public CounterboardManager[] cbManagers;

    [Header("[ RACE SETTINGS ] ")]
    public bool isNetworked;
    public bool useSceneSettings = true;
    public int racerCount = 1;
    public int lapCount = 1;
    public bool hyperSpeed;
    public bool musicManagerEnabled;

    public E_SPEEDCLASS speedClass;
    public E_SHIPS playerShip;
    public E_GAMEMODES gamemode;

    public Transform[] trackCameraPoints;

    public HUDManager RaceUI;
    public PauseManager PauseUI;
    public PauseManager DeadUI;
    public PauseManager FinishedUI;
    public MusicManager musicManager;
    public GhostManager ghostManager;
    public GameObject optionsManager;

    public List<GameObject> ambSounds = new List<GameObject>();

    public bool playerDied;

    private List<Bloom> ppBlooms = new List<Bloom>();
    private List<Antialiasing> ppAA = new List<Antialiasing>();
    private List<ColorCorrectionLookup> ccl = new List<ColorCorrectionLookup>();

    [Header("[ COUNTDOWN ] ")]
    public float countdownStartDelay;
    public float countdownStageDelay;
    private float countdownTimer;
    private int countDownStage;

    #endregion

    void Start ()
    {
        // cap framerate
        GameSettings.CapFPS(60);

        ResetRaceStates();
        GatherAmbientSounds();
        ApplyRaceSettings();
        SetupTrackData();
        SpawnShips();
        SetupInterfaces();
        SetupManagers();
        AttachImageEffects();
	}

    private void ResetRaceStates()
    {

        // set reference to race manager
        RaceSettings.raceManager = this;

        // reset race values
        RaceSettings.shipsRestrained = true;
        RaceSettings.countdownFinished = false;
        RaceSettings.countdownReady = false;

        // clear ships list
        RaceSettings.SHIPS.Clear();

        // if networked then create the network references
        if (RaceSettings.isNetworked)
            RaceSettings.serverReferences = gameObject.AddComponent<ServerReferences>();
    }

    private void GatherAmbientSounds()
    {
        // gather all the sounds in the scene (for pausing)
        GameObject[] allObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < allObjects.Length; ++i)
        {
            if (allObjects[i].GetComponent<AudioSource>())
                ambSounds.Add(allObjects[i]);
        }
    }

    private void ApplyRaceSettings()
    {
        // if using the scene settings then override racesettings
        if (useSceneSettings)
        {
            RaceSettings.speedclass = speedClass;
            RaceSettings.racers = racerCount;
            RaceSettings.gamemode = gamemode;
            RaceSettings.playerShip = playerShip;
            RaceSettings.isNetworked = isNetworked;
            RaceSettings.hyperSpeed = hyperSpeed;
        }


        // set laps based on speed class
        switch (RaceSettings.speedclass)
        {
            case E_SPEEDCLASS.SPARK:
                RaceSettings.laps = 2;
                break;
            case E_SPEEDCLASS.TOXIC:
                RaceSettings.laps = 2;
                break;
            case E_SPEEDCLASS.APEX:
                RaceSettings.laps = 3;
                break;
            case E_SPEEDCLASS.HALBERD:
                RaceSettings.laps = 4;
                break;
            case E_SPEEDCLASS.SPECTRE:
                RaceSettings.laps = 5;
                break;
        }
    }

    private void SetupTrackData()
    {
        // copy camera points to settings
        RaceSettings.overviewTransforms = trackCameraPoints;

        // set global reference to track data
        RaceSettings.trackData = trackData;

        // find spawn tiles
        trackData.UpdateTrackData();
        trackData.FindSpawnTiles();

        // set camera points
        int startIndex = (trackData.spawnCameraLocations.Count - 1) / 2;
        RaceSettings.introCamStart = trackData.spawnCameraLocations[startIndex];
        RaceSettings.introCamEnd = trackData.spawnCameraLocations[0];
    }

    private void SetupInterfaces()
    {
        // create new HUD
        GameObject newUI = Instantiate(Resources.Load("RaceUI") as GameObject) as GameObject;
        RaceUI = newUI.GetComponent<HUDManager>();
        RaceUI.r = RaceSettings.SHIPS[0];
        RaceUI.accentColor = RaceSettings.SHIPS[0].settings.REF_HUDCOL;

        // create pause UI
        GameObject pauseUI = Instantiate(Resources.Load("PauseUI") as GameObject) as GameObject;
        PauseUI = pauseUI.GetComponent<PauseManager>();
        pauseUI.SetActive(false);

        // create dead UI
        GameObject deadUI = Instantiate(Resources.Load("DeadUI") as GameObject) as GameObject;
        DeadUI = deadUI.GetComponent<PauseManager>();
        deadUI.SetActive(false);

        // create finished UI
        GameObject finishedUI = Instantiate(Resources.Load("ResultsUI") as GameObject) as GameObject;
        FinishedUI = finishedUI.GetComponent<PauseManager>();
        FinishedUI.r = RaceSettings.SHIPS[0];
        finishedUI.SetActive(false);

        // create options UI
        GameObject optionsUI = Instantiate(Resources.Load("OptionsUI") as GameObject) as GameObject;
        optionsManager = optionsUI;
        optionsUI.SetActive(false);
    }

    private void SetupManagers()
    {
        // create ghost manager
        if (RaceSettings.gamemode == E_GAMEMODES.TimeTrial)
        {
            ghostManager = gameObject.AddComponent<GhostManager>();
            ghostManager.r = RaceSettings.SHIPS[0];
        }

        // hide mouse cursor
        Cursor.visible = false;

        // create music manager
        if (musicManagerEnabled)
        {
            GameObject newObj = new GameObject("Music Manager");
            musicManager = newObj.AddComponent<MusicManager>();
            musicManager.r = RaceSettings.SHIPS[0];
        }
    }

    private void AttachImageEffects()
    {
        for (int i = 0; i < RaceSettings.SHIPS.Count; ++i)
        {
            // bloom
            if (RaceSettings.SHIPS[i].cam != null)
            {

                // bloom
                Bloom b = RaceSettings.SHIPS[i].cam.gameObject.AddComponent<Bloom>();
                b.lensFlareShader = Shader.Find("Hidden/LensFlareCreate");
                b.screenBlendShader = Shader.Find("Hidden/BlendForBloom");
                b.blurAndFlaresShader = Shader.Find("Hidden/BlurAndFlares");
                b.brightPassFilterShader = Shader.Find("Hidden/BrightPassFilter2");
                b.bloomIntensity = 0.5f;
                b.bloomThreshold = 0.3f;
                b.bloomBlurIterations = 4;
                b.sepBlurSpread = 10;
                ppBlooms.Add(b);

                // fxaa
                Antialiasing aa = RaceSettings.SHIPS[i].cam.gameObject.AddComponent<Antialiasing>();
                aa.ssaaShader = Shader.Find("Hidden/SSAA");
                aa.dlaaShader = Shader.Find("Hidden/DLAA");
                aa.nfaaShader = Shader.Find("Hidden/NFAA");
                aa.shaderFXAAPreset2 = Shader.Find("Hidden/FXAA Preset 2");
                aa.shaderFXAAPreset3 = Shader.Find("Hidden/FXAA Preset 3");
                aa.shaderFXAAII = Shader.Find("Hidden/FXAA II");
                aa.shaderFXAAIII = Shader.Find("Hidden/FXAA III (Console)");
                ppAA.Add(aa);

                // color correction
                ColorCorrectionLookup cc = RaceSettings.SHIPS[i].cam.gameObject.AddComponent<ColorCorrectionLookup>();
                cc.shader = Shader.Find("Hidden/ColorCorrection3DLut");
                cc.Convert(Resources.Load("VapeLookup") as Texture2D, "Resources/VapeLookup.tga");
                ccl.Add(cc);
            }
        }
    }

    void Update()
    {
        // pause check (pause button, alt-tab and default steam overlay combination)
        if (Input.GetButtonDown("Pause") || (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Tab)) ||
            (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab)))
        {
            if (!RaceSettings.SHIPS[0].isDead && !RaceSettings.SHIPS[0].finished)
                PauseInput();
        }

        // player death check
        if (!playerDied && RaceSettings.SHIPS[0].isDead)
        {
            playerDied = true;

            // update UI's
            DeadUI.gameObject.SetActive(true);
            RaceUI.gameObject.SetActive(false);
            DeadUI.MenuEnabled();

            // show cursor
            Cursor.visible = true;
        }

        UpdateTrackCameras();
        UpdateImageEffects();
    }

    private void UpdateTrackCameras()
    {
        int i = 0;
        for (i = 0; i < trackCameraPoints.Length; ++i)
        {
            // get closest ship
            float distance = 10000.0f;
            float tempDistance = 0.0f;
            int j = 0;
            int index = 0;
            for (j = 0; j < RaceSettings.SHIPS.Count; ++j)
            {
                tempDistance = Vector3.Distance(trackCameraPoints[i].position, RaceSettings.SHIPS[j].transform.position);
                if (tempDistance < distance)
                {
                    distance = tempDistance;
                    index = j;
                }
            }

            // lookat closest ship
            trackCameraPoints[i].LookAt(RaceSettings.SHIPS[index].transform.position);
        }
    }

    private void UpdateImageEffects()
    {
        int i = 0;
        for (i = 0; i < ppBlooms.Count; ++i)
            ppBlooms[i].enabled = GameSettings.GS_BLOOM;
        for (i = 0; i < ppAA.Count; ++i)
            ppAA[i].enabled = GameSettings.GS_FXAA;
        for (i = 0; i < ccl.Count; ++i)
            ccl[i].enabled = GameSettings.GS_VAPORWAVE;
    }

    public void PauseInput()
    {
        if (optionsManager.activeSelf)
            return;

        // toggle pause
        GameSettings.PauseToggle();
        
        // toggle sounds for ships
        for (int i = 0; i < RaceSettings.SHIPS.Count; ++i)
            RaceSettings.SHIPS[i].SetAudioEnabled(!GameSettings.isPaused);

        // show/hide pause UI
        PauseUI.gameObject.SetActive(GameSettings.isPaused);
        if (GameSettings.isPaused)
        {
            // load and play pause sound
            AudioClip clip = Resources.Load("Audio/Interface/PAUSE") as AudioClip;
            if (clip != null)
            {
                OneShot.CreateOneShot(clip, 1.0f, 1.0f);
            }
            else
            {
                Debug.LogError(gameObject.name + " (Manager) couldn't load sound: PAUSE");
            }
            PauseUI.MenuEnabled();
        } else
        {
            // load and play pause sound
            AudioClip clip = Resources.Load("Audio/Interface/UNPAUSE") as AudioClip;
            if (clip != null)
            {
                OneShot.CreateOneShot(clip, 1.0f, 1.0f);
            }
            else
            {
                Debug.LogError(gameObject.name + " (Manager) couldn't load sound: UNPAUSE");
            }
        }

        // show/hide race HUD
        RaceUI.gameObject.SetActive(!GameSettings.isPaused);

        // toggle ambient sounds
        for (int i = 0; i < ambSounds.Count; ++i)
            ambSounds[i].SetActive(!GameSettings.isPaused);

        // toggle mouse cursor visibility
        Cursor.visible = GameSettings.isPaused;
    }

    private void SpawnShips()
    {
        if (isNetworked)
            return;

        // make sure racers is at least 1
        if (racerCount < 1)
            racerCount = 1;

        for (int i = 0; i < RaceSettings.racers; i++)
        {
            // if there arn't any spawns then we can't spawn the ship
            if (i > (RaceSettings.trackData.spawnPositions.Count - 1))
            {
                Debug.LogError("Could not spawn ship! Reason: Not enough spawn locations.");
                return;
            }

            bool isAI = true;

            // racer index 0 is always player
            if (i == 0)
                isAI = false;

            // TODO: random ship for AI (once all ships are setup)
            GameObject newShip = new GameObject("SPAWNED SHIP");
            ShipConstructor c = newShip.AddComponent<ShipConstructor>();

            // position ship at spawn
            newShip.transform.position = RaceSettings.trackData.spawnPositions[i];
            newShip.transform.rotation = RaceSettings.trackData.spawnRotations[i];

            c.SpawnShip(isAI);
        }
    }

    void OnApplicationFocus(bool focusState)
    {
        if (!Application.isEditor)
        {
            // pause when game looses focus
            if (!focusState && !GameSettings.isPaused && !RaceSettings.SHIPS[0].isDead && !RaceSettings.SHIPS[0].finished)
                PauseInput();
        }
    }

    void FixedUpdate()
    {
        if (!RaceSettings.countdownFinished)
        {
            if (RaceSettings.countdownReady)
            {
                if (countDownStage == 0)
                {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStartDelay)
                    {
                        countdownTimer = 0.0f;
                        countDownStage++;
                        UpdateCounterboards("3");
                    }
                }
                else if (countDownStage < 4)
                {
                    countdownTimer += Time.deltaTime;
                    if (countdownTimer > countdownStageDelay)
                    {
                        countdownTimer = 0.0f;
                        countDownStage++;

                        if (countDownStage == 2)
                            UpdateCounterboards("2");
                        if (countDownStage == 3)
                            UpdateCounterboards("1");
                    }
                }

                if (countDownStage == 4)
                {
                    RaceSettings.StartRace();
                    UpdateCounterboards("GO!");
                }
            }
        }
    }

    private void UpdateCounterboards(string text)
    {
        for (int i = 0; i < cbManagers.Length; ++i)
            cbManagers[i].UpdateCountdown(text);
    }
}
