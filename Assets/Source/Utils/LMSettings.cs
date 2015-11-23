using UnityEngine;
using BnG.Files;
using System.Collections;

public class LMSettings : MonoBehaviour {

    [Header("[ SETTINGS ] ")]
    public Color32 ambientLight;
    public GameObject sceneParent;
    public string lightmapName;

    void Start()
    {
        CheckForExistingMap();
    }

    private void CheckForExistingMap()
    {
        string path = Application.dataPath + "/Resources/BakedLighting/" + lightmapName + ".vcm";
        if (System.IO.File.Exists(path))
        {
            VCM.Load(lightmapName);
        }
        else
        {
            Debug.Log("Couldn't find " + path + "!");
        }

    }
}
