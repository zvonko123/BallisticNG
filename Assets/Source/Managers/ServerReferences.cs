using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class ServerReferences : NetworkBehaviour {

    public List<ShipRefs> players = new List<ShipRefs>();
}
