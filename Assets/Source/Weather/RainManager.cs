using UnityEngine;
using System.Collections;

public class RainManager : MonoBehaviour {

    [Header("[ RAIN SETTINGS ]")]
    public float gridSnapSize;
    public float rainHeight;

    private GameObject targetCamera;
    private ParticleSystem rainSystem;
    private bool hasTarget;

    void Start()
    {
        // try to get rain particle system
        rainSystem = GetComponent<ParticleSystem>();
        if (rainSystem == null)
            Destroy(this.gameObject);
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

        rainSystem.enableEmission = !Physics.Raycast(targetCamera.transform.position, Vector3.up);

    }
}
