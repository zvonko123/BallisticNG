using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetworkSpawner : NetworkBehaviour {

	void Start ()
    {
        // create new ship
        GameObject newShip = new GameObject("SPAWNED SHIP");
        ShipConstructor c = newShip.AddComponent<ShipConstructor>();

        // position ship at spawn
        newShip.transform.position = RaceSettings.trackData.spawnPositions[0];
        newShip.transform.rotation = RaceSettings.trackData.spawnRotations[0];

        c.SpawnShip(false);

        // destroy this
        if (isServer)
            Destroy(this);
    }


}
