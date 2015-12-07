using UnityEngine;
using System.Collections;

public class MusicManager : ShipBase {

    // references
    public AudioSource source;
    private AudioClip clip;

    public float highpass;
    private AudioHighPassFilter HPFilter;
    private AudioReverbFilter reverbFilter;

    void Start()
    {
        // create new audio source
        source = gameObject.AddComponent<AudioSource>();
        source.spatialBlend = 0.0f;
        source.bypassReverbZones = true;

        HPFilter = gameObject.AddComponent<AudioHighPassFilter>();
        HPFilter.highpassResonanceQ = 3;

        reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = AudioReverbPreset.Dizzy;
        reverbFilter.enabled = false;

        // load music
        AudioSettings.LoadMusic();
    }

    void Update()
    {
        if (GameSettings.isPaused && source.isPlaying)
            source.Pause();
        else if (!GameSettings.isPaused && !source.isPlaying)
            source.UnPause();

        if ((!source.isPlaying && !GameSettings.isPaused) || Input.GetKeyDown(KeyCode.Period))
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

        // vape mode
        if (GameSettings.GS_VAPORWAVE)
            source.pitch = 0.35f;
        else
            source.pitch = 1.0f;

        reverbFilter.enabled = GameSettings.GS_VAPORWAVE;

        // highpass
        if (r.jumpHeight)
            HPFilter.cutoffFrequency = Mathf.Lerp(HPFilter.cutoffFrequency, 1500.0f, Time.deltaTime * 2);
        else
            HPFilter.cutoffFrequency = Mathf.Lerp(HPFilter.cutoffFrequency, 0.0f, Time.deltaTime * 5);
    }
}
