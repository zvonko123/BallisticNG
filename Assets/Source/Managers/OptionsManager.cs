using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OptionsManager : MonoBehaviour {

    [Header("[ SELECTION SCREEN ]")]
    public BNGButton btnGraphics;

    [Header("[ GRAPHICS ]")]
    public Dropdown drdwnResolution;
    public Slider slidFrameCap;
    public Text txtFrameCap;

    public BNGToggle tglBloom;
    public BNGToggle tglFXAA;
    public BNGToggle tglCRT;
    public BNGToggle tglVape;

    [Header("[ AUDIO ]")]
    public Slider slidMasterVolume;
    public Text txtMasterVolume;
    public Slider slidSFXVolume;
    public Text txtSFXVolume;
    public Slider slidMusicVolume;
    public Text txtMusicVolume;
    public Slider slidVoiceVolume;
    public Text txtVoiceVolume;

    private Resolution[] resolutions = new Resolution[0];

    public void UpdateFrameCapValue()
    {
        GameSettings.GS_FRAMECAP = Mathf.RoundToInt(slidFrameCap.value);
        txtFrameCap.text = GameSettings.GS_FRAMECAP.ToString();
    }

    public void UpdateVolumes()
    {
        txtMasterVolume.text = System.Math.Round(slidMasterVolume.value, 2).ToString();
        txtSFXVolume.text = System.Math.Round(slidSFXVolume.value, 2).ToString();
        txtMusicVolume.text = System.Math.Round(slidMusicVolume.value, 2).ToString();
        txtVoiceVolume.text = System.Math.Round(slidVoiceVolume.value, 2).ToString();
    }

    public void InitGraphicsMenu()
    {
        // get resolutions
        GetResolutions();
        SetResolutionDropDown();

        // frame cap
        slidFrameCap.value = GameSettings.GS_FRAMECAP;

        // toggles
        tglBloom.SetState(GameSettings.GS_BLOOM);
        tglFXAA.SetState(GameSettings.GS_FXAA);
        tglCRT.SetState(GameSettings.GS_CRT);
        tglVape.SetState(GameSettings.GS_VAPORWAVE);
    }

    public void InitAudioMenu()
    {
        slidMasterVolume.value = AudioSettings.VOLUME_MAIN;
        slidSFXVolume.value = AudioSettings.VOLUME_SFX;
        slidMusicVolume.value = AudioSettings.VOLUME_MUSIC;
        slidVoiceVolume.value = AudioSettings.VOLUME_VOICES;
    }

    public void GetResolutions()
    {
        resolutions = Screen.resolutions;
        List<Dropdown.OptionData> od = new List<Dropdown.OptionData>();

        for (int i = 0; i < resolutions.Length; ++i)
            od.Add(new Dropdown.OptionData(string.Format("{0}x{1}", resolutions[i].width, resolutions[i].height)));

        drdwnResolution.options = od;
    }

    public void GetAudio()
    {

    }

    public void CloseOptions()
    {
        GameSettings.optionsClose = true;
    }

    public void OpenOptions()
    {
        btnGraphics.Select();
    }

    public void SetResolutionDropDown()
    {
        int val = 0;
        for (int i = 0; i < resolutions.Length; ++i)
        {
            int width = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x);
            int height = Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y);

            if (resolutions[i].width == width && resolutions[i].height == height)
                val = i;
        }
        drdwnResolution.value = val;
    }

    public void GetRefreshRates()
    {

    }

    public void UpdateSettings()
    {
        // resolution
        if (resolutions.Length > 0)
        {
            GameSettings.GS_RESOLUTION.x = Mathf.RoundToInt(resolutions[drdwnResolution.value].width);
            GameSettings.GS_RESOLUTION.y = Mathf.RoundToInt(resolutions[drdwnResolution.value].height);

            // frame cap
            GameSettings.GS_FRAMECAP = Mathf.RoundToInt(slidFrameCap.value);
            GameSettings.CapFPS(GameSettings.GS_FRAMECAP);

            // toggles
            GameSettings.GS_BLOOM = tglBloom.toggled;
            GameSettings.GS_FXAA = tglFXAA.toggled;
            GameSettings.GS_CRT = tglCRT.toggled;
            GameSettings.GS_VAPORWAVE = tglVape.toggled;
        }

        // audio
        AudioSettings.VOLUME_MAIN = slidMasterVolume.value;
        AudioSettings.VOLUME_SFX = slidSFXVolume.value;
        AudioSettings.VOLUME_MUSIC = slidMusicVolume.value;
        AudioSettings.VOLUME_VOICES = slidVoiceVolume.value;

        // apply resolution
        Screen.SetResolution(Mathf.RoundToInt(GameSettings.GS_RESOLUTION.x), Mathf.RoundToInt(GameSettings.GS_RESOLUTION.y), GameSettings.GS_FULLSCREEN);

        // save setings to disk
        GameOptions.SaveGameSettings();
    }
}
