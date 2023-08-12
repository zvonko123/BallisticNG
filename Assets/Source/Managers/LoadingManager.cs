using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour {

    private AsyncOperation LEVEL;

    [Header(" [ REFERENCES ] ")]
    public GameObject[] ShipGMs;
    public string[] shipDescriptions;
    public E_SHIPS shipInfo;
    public bool overidePlayerShip;
    public Text shipDesc;

    void Start()
    {

        if (!overidePlayerShip)
            shipInfo = RaceSettings.playerShip;

        // disable all ships except for the ship to display
        for (int i = 0; i < ShipGMs.Length; ++i)
        {
            if (i == (int)shipInfo)
                ShipGMs[i].SetActive(true);
            else
                ShipGMs[i].SetActive(false);
        }

        // set ship description
        shipDesc.text = shipDescriptions[(int)shipInfo];

        // if there is no track set then load aciknovae normal
        if (RaceSettings.trackToLoad == "")
            RaceSettings.trackToLoad = "Aciknovae_Normal";

        // begin loading the level
        LoadLevel();
    }

    private void LoadLevel()
    {
        LEVEL = Application.LoadLevelAsync(RaceSettings.trackToLoad);
        LEVEL.allowSceneActivation = true;
    }
}
