using UnityEngine;
using System.Collections;
using System.IO;

public class GameOptions {

    public static void SaveGameSettings()
    {
        INIParser ini = new INIParser();
        ini.Open(GameSettings.GetDirectory() + "settings.ini");
        {
            ini.WriteValue("Display", "Screen Width", Screen.width);
            ini.WriteValue("Display", "Screen Height", Screen.height);
            ini.WriteValue("Display", "Fullscreen", GameSettings.GS_FULLSCREEN);
            ini.WriteValue("Display", "Framecap", GameSettings.GS_FRAMECAP);

            ini.WriteValue("Rendering", "Draw Distance", GameSettings.GS_DRAWDISTANCE);
            ini.WriteValue("Rendering", "Bloom", GameSettings.GS_BLOOM);
            ini.WriteValue("Rendering", "FXAA", GameSettings.GS_FXAA);
            ini.WriteValue("Rendering", "CRT", GameSettings.GS_CRT);
            ini.WriteValue("Rendering", "Vape", GameSettings.GS_VAPORWAVE);

            ini.WriteValue("Audio", "Master Volume", Mathf.RoundToInt(AudioSettings.VOLUME_MAIN));
            ini.WriteValue("Audio", "SFX Volume", Mathf.RoundToInt(AudioSettings.VOLUME_SFX));
            ini.WriteValue("Audio", "Voices Volume", Mathf.RoundToInt(AudioSettings.VOLUME_VOICES));
            ini.WriteValue("Audio", "Music Volume", Mathf.RoundToInt(AudioSettings.VOLUME_MUSIC));
        }
        ini.Close();
    }

    public static void LoadGameSettings()
    {
        // create the file if it doesn't already exist
        if (!File.Exists(GameSettings.GetDirectory() + "settings.ini"))
            SaveGameSettings();

        INIParser ini = new INIParser();
        ini.Open(GameSettings.GetDirectory() + "settings.ini");
        {
            GameSettings.GS_RESOLUTION.x = ini.ReadValue("Display", "Screen Width", Screen.width);
            GameSettings.GS_RESOLUTION.y = ini.ReadValue("Display", "Screen Height", Screen.height);
            GameSettings.GS_FULLSCREEN = ini.ReadValue("Display", "Fullscreen", GameSettings.GS_FULLSCREEN);
            GameSettings.GS_FRAMECAP = ini.ReadValue("Display", "Framecap", GameSettings.GS_FRAMECAP);

            GameSettings.GS_DRAWDISTANCE = ini.ReadValue("Rendering", "Draw Distance", GameSettings.GS_DRAWDISTANCE);
            GameSettings.GS_BLOOM = ini.ReadValue("Rendering", "Bloom", GameSettings.GS_BLOOM);
            GameSettings.GS_FXAA = ini.ReadValue("Rendering", "FXAA", GameSettings.GS_FXAA);
            GameSettings.GS_CRT  = ini.ReadValue("Rendering", "CRT", GameSettings.GS_CRT);
            GameSettings.GS_VAPORWAVE = ini.ReadValue("Rendering", "Vape", GameSettings.GS_VAPORWAVE);

            AudioSettings.VOLUME_MAIN= ini.ReadValue("Audio", "Master Volume", Mathf.RoundToInt(AudioSettings.VOLUME_MAIN));
            AudioSettings.VOLUME_SFX = ini.ReadValue("Audio", "SFX Volume", Mathf.RoundToInt(AudioSettings.VOLUME_SFX));
            AudioSettings.VOLUME_VOICES = ini.ReadValue("Audio", "Voices Volume", Mathf.RoundToInt(AudioSettings.VOLUME_VOICES));
            AudioSettings.VOLUME_MUSIC = ini.ReadValue("Audio", "Music Volume", Mathf.RoundToInt(AudioSettings.VOLUME_MUSIC));
        }
        ini.Close();
    }
}
