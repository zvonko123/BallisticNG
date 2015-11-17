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

    public E_SPEEDCLASS speedClass;
    public E_SHIPS playerShip;

    public HUDManager RaceUI;

    #endregion

    void Start ()
    {
        // if using the scene settings then override racesettings
        if (useSceneSettings)
        {
            RaceSettings.racers = racerCount;
            RaceSettings.laps = lapCount;
            RaceSettings.playerShip = playerShip;
        }

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

        // cap framerate
        GameSettings.CapFPS(60);
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
