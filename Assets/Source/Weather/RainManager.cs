using UnityEngine;
using System.Collections;

public class RainManager : MonoBehaviour {

    [Header("[ RAIN SETTINGS ]")]
    public float gridSnapSize;
    public float rainHeight;

    private GameObject targetCamera;
    private ParticleSystem rainSystem;
    private bool hasTarget;
    private AudioSource rain;

    private bool canRain;

    void Start()
    {
        // try to get rain particle system
        rainSystem = GetComponent<ParticleSystem>();
        if (rainSystem == null)
            Destroy(this.gameObject);

        // create rain audiosource
        rain = gameObject.AddComponent<AudioSource>();
        rain.loop = true;
        rain.spatialBlend = 0.0f;
        rain.clip = Resources.Load("Audio/Env/Rain") as AudioClip;
        rain.volume = 0.0f;
        rain.Play();
    }

    void Update()
    {
        // get camera target
        if (!hasTarget)
        {
            targetCamera = RaceSettings.SHIPS[0].cam.gameObject;
            hasTarget = (targetCamera != null);
        }

        // position rain
        Vector3 pos = new Vector3(targetCamera.transform.position.x, targetCamera.transform.position.y + rainHeight, targetCamera.transform.position.z);
        pos.x = Mathf.RoundToInt(pos.x / gridSnapSize) * gridSnapSize;
        pos.y = Mathf.RoundToInt(pos.y / gridSnapSize) * gridSnapSize;
        pos.z = Mathf.RoundToInt(pos.z / gridSnapSize) * gridSnapSize;
        transform.position = pos;

        canRain = !Physics.Raycast(targetCamera.transform.position, Vector3.up); ;
        rainSystem.enableEmission = canRain;

        if (canRain)
            rain.volume = Mathf.Lerp(rain.volume, 1.0f * (AudioSettings.VOLUME_MAIN * AudioSettings.VOLUME_SFX), Time.deltaTime * 3);
        else
            rain.volume = Mathf.Lerp(rain.volume, 0.0f, Time.deltaTime * 3);

    }
}
