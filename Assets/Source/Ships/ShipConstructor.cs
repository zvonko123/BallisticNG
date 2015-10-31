using UnityEngine;
using System.Collections;

public class ShipConstructor : MonoBehaviour {

    void Start()
    {

    }

    private GameObject LoadShip(E_SHIPS ship)
    {
        return Instantiate(Resources.Load("Ships/" + ship.ToString()) as GameObject) as GameObject;
    }
}
