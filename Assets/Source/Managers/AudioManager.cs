using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{

    private List<AudioSource> oneshots = new List<AudioSource>();
    private List<float> oneshotDuration = new List<float>();
    private List<float> oneshotTimers = new List<float>();

    void Update()
    {
        if (oneshots.Count > 0)
        {
            for (int i = 0; i < oneshots.Count; i++)
            {
                oneshotTimers[i] += Time.deltaTime;
                if (oneshotTimers[i] > oneshotDuration[i])
                {
                    // Delete oneshot
                    Destroy(oneshots[i].gameObject);

                    // Remove list items
                    oneshots.Remove(oneshots[i]);
                    oneshotDuration.Remove(oneshotDuration[i]);
                    oneshotTimers.Remove(oneshotTimers[i]);
                }
            }
        }
    }

    public void RegisterOneShot(AudioSource source)
    {
        // Register the audio source
        oneshots.Add(source);

        // Setup timers
        oneshotTimers.Add(0.0f);
        oneshotDuration.Add(source.clip.length);
    }
}
