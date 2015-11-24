using UnityEngine;
using BnG.TrackData;
using System.Collections;

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

    public Transform[] trackCameraPoints;

    public HUDManager RaceUI;
    public MusicManager musicManager;

    #endregion

    void Start ()
    {
        // if using the scene settings then override racesettings
        if (useSceneSettings)
        {
            RaceSettings.speedclass = speedClass;
            RaceSettings.racers = racerCount;
            //RaceSettings.laps = lapCount;
            RaceSettings.playerShip = playerShip;
        }

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


        // spawn the ships
        SpawnShips();

        // create new HUD
        GameObject newUI = Instantiate(Resources.Load("RaceUI") as GameObject) as GameObject;
        RaceUI = newUI.GetComponent<HUDManager>();
        RaceUI.r = RaceSettings.SHIPS[0];
        RaceUI.accentColor = RaceSettings.SHIPS[0].settings.REF_HUDCOL;

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
        // pause check
        if (Input.GetButtonDown("Pause"))
        {
            GameSettings.PauseToggle();
            for (int i = 0; i < RaceSettings.SHIPS.Count; ++i)
                RaceSettings.SHIPS[i].SetAudioEnabled(!GameSettings.isPaused);
        }
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
