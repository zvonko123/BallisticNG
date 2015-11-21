using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {

    // references
    public AudioSource source;
    private AudioClip clip;

    void Start()
    {
        // create new audio source
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.0f;
        source.bypassReverbZones = true;

        // load music
        AudioSettings.LoadMusic();
    }

    void Update()
    {
        if (!source.isPlaying || Input.GetKeyDown(KeyCode.Period))
        {
            // get random number for track to load
            int rand = Random.Range(0, AudioSettings.musicLocations.Length);

            // load track
            clip = Resources.Load(AudioSettings.musicLocations[rand]) as AudioClip;
            source.clip = clip;

            // play
            source.Play();
        }

        // update volume
        source.volume = AudioSettings.VOLUME_MUSIC;
    }
}
