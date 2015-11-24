using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BNGButton : Button {

    private Text textComponent;
    private AudioSource audio;
    void Start()
    {
        // get text component
        textComponent = transform.Find("Text").GetComponent<Text>();
        if (textComponent == null)
        {
            Debug.LogError(gameObject.name + " (button) couldn't find a text component!");
            Destroy(this.gameObject);
        }

        // attach audio source for button sounds
        audio = gameObject.AddComponent<AudioSource>();
        audio.spatialBlend = 0.0f;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        // change color
        textComponent.color = new Color(1.0f, 0.68f, 0.0f, 1.0f);

        // load and play ui select sound
        AudioClip clip = Resources.Load("Audio/Interface/UIMOVE") as AudioClip;
        if (clip != null)
        {
            audio.PlayOneShot(clip);
        } else
        {
            Debug.LogError(gameObject.name + " (button) couldn't load sound: UIMOVE");
        }

        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        // change color
        textComponent.color = new Color(0.58f, 0.58f, 0.58f, 1.0f);

        base.OnDeselect(eventData);
    }
}
