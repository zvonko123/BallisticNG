using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BNGToggle : Button {

    private Text textComponent;
    private AudioSource audio;

    private string text;
    public bool toggled;
    public bool selected;

    public override void Select()
    {
        CheckComponents();

        // change color
        textComponent.color = new Color(1.0f, 0.68f, 0.0f, 1.0f);

        base.Select();
    }

    protected override void OnEnable()
    {
        CheckComponents();

        // change color
        textComponent.color = new Color(0.58f, 0.58f, 0.58f, 1.0f);

        base.OnEnable();
    }

    public void Toggle()
    {
        if (toggled)
            toggled = false;
        else if (!toggled)
            toggled = true;

        if (toggled)
            textComponent.text = text + " enabled";
        else
            textComponent.text = text + " disabled";
    }

    public void SetState(bool active)
    {
        CheckComponents();

        toggled = active;
        if (toggled)
            textComponent.text = text + " enabled";
        else
            textComponent.text = text + " disabled";
    }

    public override void OnSelect(BaseEventData eventData)
    {
        CheckComponents();

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
        selected = true;

        base.OnSelect(eventData);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        CheckComponents();

        // change color
        textComponent.color = new Color(0.58f, 0.58f, 0.58f, 1.0f);
        selected = false;

        base.OnDeselect(eventData);
    }

    private void CheckComponents()
    {
        if (textComponent == null)
        {
            // get text component
            textComponent = transform.Find("Text").GetComponent<Text>();
            if (textComponent == null)
            {
                Debug.LogError(gameObject.name + " (button) couldn't find a text component!");
                Destroy(this.gameObject);
            }
            text = textComponent.text;
        }

        if (audio == null)
        {
            // destroy any previous audiosources
            AudioSource[] source = GetComponents<AudioSource>();
            for (int i = 0; i < source.Length; ++i)
                DestroyImmediate(source[i]);

            // attach audio source for button sounds
            audio = gameObject.AddComponent<AudioSource>();
            audio.spatialBlend = 0.0f;
        }
    }
}
