using UnityEngine;
using System.Collections;

public class OneShot {

    public static AudioSource CreateOneShot(AudioClip clip, float volume, float pitch)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;
        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    public static AudioSource CreateOneShot(AudioClip clip, Transform parent, float volume, float pitch)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 0;
        source.gameObject.name = clip.name;

        source.transform.parent = parent;
        source.transform.localPosition = Vector3.zero;
        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    public static AudioSource CreateOneShot(AudioClip clip, Vector3 position, float volume, float pitch)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.gameObject.name = clip.name;

        source.transform.position = position;
        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    public static AudioSource CreateOneShot(AudioClip clip, Vector3 position, float volume, float pitch, float minDistance, float maxDistance)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.maxDistance = maxDistance;
        source.minDistance = minDistance;
        source.gameObject.name = clip.name;
        source.spatialBlend = 1.0f;

        source.transform.position = position;
        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    public static AudioSource CreateOneShot(AudioClip clip, Vector3 position, Transform parent, float volume, float pitch, float minDistance, float maxDistance, bool localSpace)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.maxDistance = maxDistance;
        source.minDistance = minDistance;
        source.gameObject.name = clip.name;
        source.spatialBlend = 1.0f;

        source.transform.parent = parent;
        if (localSpace)
            source.transform.localPosition = position;
        else
            source.transform.position = position;

        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    public static AudioSource CreateOneShot(AudioClip clip, Transform parent, float volume, float pitch, float minDistance, float maxDistance)
    {
        // Create source
        AudioSource source = CreateSource();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.maxDistance = maxDistance;
        source.minDistance = minDistance;
        source.gameObject.name = clip.name;
        source.spatialBlend = 1.0f;

        source.transform.parent = parent;
        source.transform.localPosition = Vector3.zero;
        source.Play();

        // Register it
        AudioSettings.RegisterOneShot(source);

        return source;
    }

    private static AudioSource CreateSource()
    {
        GameObject newSound = new GameObject();
        return newSound.AddComponent<AudioSource>();
    }
}
