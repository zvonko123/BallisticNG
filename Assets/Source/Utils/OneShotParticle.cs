using UnityEngine;
using System.Collections;

public class OneShotParticle : MonoBehaviour
{

    private float destroyTimer;
    private float lifeTimer;

    void Start()
    {
        if (!GetComponent<ParticleSystem>())
            Destroy(this.gameObject);
        else
            destroyTimer = GetComponent<ParticleSystem>().duration;
    }

    void Update()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > destroyTimer * 2)
            Destroy(this.gameObject);
    }

}