using UnityEngine;
using System.Collections;

public class VapeHover : MonoBehaviour {

    public AudioClip clip;
    public BNGToggle btn;
    private AudioSource source;
    private float timer;

    void Start()
    {
        // create audiosource
        source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = 0.0f;
        source.loop = true;
        source.Play();
    }

    void Update()
    {
        if (btn.selected)
        {
            timer += 0.005f;
            if (timer > 0.08f)
                source.volume = 0.2f * (AudioSettings.VOLUME_MUSIC);
        }
        else
        {
            timer = 0;
            source.volume = 0.0f;
        }
    }
}
