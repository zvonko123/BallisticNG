using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostManager : ShipBase
{

    // ship mesh
    public GameObject ghostGM;
    public Material ghostMat;

    // ghost control
    public bool foundGhost;
    public float ghostPlayback;
    public float ghostIndex;
    public List<Vector3> ghostPosition = new List<Vector3>();
    public List<Quaternion> ghostRotation = new List<Quaternion>();

    void Start()
    {
        // load ghost ship
        ghostGM = Instantiate(Resources.Load("Ships/" + RaceSettings.playerShip + "GHOST") as GameObject) as GameObject;
        if (ghostGM != null)
            ghostMat = ghostGM.GetComponent<GhostRef>().mesh.gameObject.GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        // color ghost ship blue
        if (ghostGM != null)
            ghostMat.SetColor("_Color", new Color(0.0f, 0.5f, 1.0f));
    }
}
