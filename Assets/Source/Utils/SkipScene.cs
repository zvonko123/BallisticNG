using UnityEngine;
using System.Collections;

public class SkipScene : MonoBehaviour {

    [Header("[ SETTINGS ]")]
    public bool capFPS;
    public string nextScene;
    public float waitTime;
    public bool loadSettings;

    private float waitTimer;

    void Start()
    {
        // default fps to 60fps
        if (capFPS)
            GameSettings.CapFPS(60);

        if (loadSettings)
            GameOptions.LoadGameSettings();
    }

    void Update()
    {
        // wait timer
        waitTimer += Time.deltaTime;
        if (waitTimer > waitTime)
            Application.LoadLevel(nextScene);

        // player skip
        if (Input.anyKeyDown)
            Application.LoadLevel(nextScene);
    }
}
