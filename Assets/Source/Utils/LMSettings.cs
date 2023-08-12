using UnityEngine;
using BnG.Files;
using BnG.TrackData;
using System.Collections;
using System;

public class LMSettings : MonoBehaviour {

    [Header("[ SETTINGS ] ")]
    public Color32 ambientLight;
    public GameObject sceneParent;
    public string lightmapName;
    public TrData data;

    void Start()
    {
        CheckForExistingMap();
    }

    private void CheckForExistingMap()
    {
        string path = Environment.CurrentDirectory + "/Lighting/" + lightmapName + ".vcm";
        if (System.IO.File.Exists(path))
        {
            VCM.Load(lightmapName);
            data.setTileColors = true;
        }
        else
        {
            Debug.Log("Couldn't find " + path + "!");
        }

    }
}
