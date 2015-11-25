using UnityEngine;
using BnG.TrackData;
using System.Collections;
using System.Collections.Generic;

public class RaceManager : MonoBehaviour {

    #region VARIABLES
    [Header("[ TRACK DATA ]")]
    public TrData trackData;

    [Header("[ RACE SETTINGS ] ")]
    public bool useSceneSettings = true;
    public int racerCount = 1;
    public int lapCount = 1;
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

    public List<GameObject> ambSounds = new List<GameObject>();

    public bool playerDied;

    #endregion

    void Start ()
    {
        // gather all the sounds in the scene (for pausing)
        GameObject[] allObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        for (int i = 0; i < allObjects.Length; ++i)
        {
            if (allObjects[i].GetComponent<AudioSource>())
                ambSounds.Add(allObjects[i]);
        }

        // if using the scene settings then override racesettings
        if (useSceneSettings)
        {
            RaceSettings.speedclass = speedClass;
            RaceSettings.racers = racerCount;
            RaceSettings.gamemode = gamemode;
            RaceSettings.playerShip = playerShip;
        }

        // set reference to race manager
        RaceSettings.raceManager = this;

        // clear ships list
        RaceSettings.SHIPS.Clear();

        // set laps based on speed class
        switch(RaceSettings.speedclass)
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

        // ships start restrained
        RaceSettings.shipsRestrained = true;

        // spawn the ships
        SpawnShips();

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

        // cap framerate
        GameSettings.CapFPS(60);
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
    }

    public void PauseInput()
    {
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
}
