using UnityEngine;
using BnG.TrackData;
using BnG.TrackTools;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class RaceSettings
{
    // race references
    public static TrData trackData;
    public static Camera currentCamera;
    public static List<ShipRefs> SHIPS = new List<ShipRefs>();
    public static Transform[] overviewTransforms;
    public static RaceManager raceManager;

    // race settings
    public static int racers = 1;
    public static int laps = 1;
    public static E_SPEEDCLASS speedclass = E_SPEEDCLASS.SPECTRE;
    public static E_SHIPS playerShip = E_SHIPS.MTECHP1;
    public static E_GAMEMODES gamemode = E_GAMEMODES.TimeTrial;
    public static E_WEAPONCLASSES playerWClass = E_WEAPONCLASSES.MIXED;
    public static bool shipsRestrained = false;

    // countdown
    public static bool countdownReady;
    public static bool countdownFinished;

    public static Vector3 introCamStart;
    public static Vector3 introCamEnd;

    // player control
    public static int playerControlIndex;

    public static string customShipName;

    public static string trackToLoad;
}

public class TournamentSettings
{
    public bool isTournament;
    public List<E_TRACKS> tournamentTracks = new List<E_TRACKS>();
    public int tournamentIndex;
}

public class GameSettings
{
    // pause
    public static bool isPaused;

    public static void PauseToggle()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1.0f;
        } else if(!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0.0f;
        }
    }

    public static void PauseToggle(bool state)
    {
        isPaused = state;
        if (isPaused)
            Time.timeScale = 0.0f;
        else
            Time.timeScale = 1.0f;
    }

    // game settings
    public static Vector2 GS_RESOLUTION;
    public static int GS_FRAMECAP = 60;
    public static bool GS_BLOOM = false;
    public static bool GS_FXAA = false;
    public static bool GS_CRT = false;
    public static bool GS_VAPORWAVE = false;

    public static bool optionsClose = false;

    /// <summary>
    /// Cap the framerate.
    /// </summary>
    /// <param name="cap"></param>
    public static void CapFPS(int cap)
    {
        Application.targetFrameRate = cap;
    }

    /// <summary>
    /// Get the save directory of the game.
    /// </summary>
    /// <returns></returns>
    public static string GetDirectory()
    {
        // get path
        string path = Environment.CurrentDirectory + "/UserData/";

        // create directory if it doesn't exist
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        return path;
    }
}

public class AudioSettings
{
    // audio settings
    public static float VOLUME_MAIN = 1.0f;
    public static float VOLUME_MUSIC = 0.7f;
    public static float VOLUME_SFX = 1.0f;
    public static float VOLUME_VOICES = 1.0f;
    public static string[] musicLocations;

    // audio management
    public static AudioManager manager;
    public static void RegisterOneShot(AudioSource source)
    {
        if (manager == null)
        {
            // Create audio manager if it doesn't already exist
            GameObject managerOBJ = new GameObject();
            manager = managerOBJ.AddComponent<AudioManager>();
        }

        manager.RegisterOneShot(source);
    }

    public static void LoadMusic()
    {
        // read music list file from resources folder
        TextAsset musicList = Resources.Load("musiclist") as TextAsset;
        List<string> musicLocs = new List<string>();
        using (StringReader sr = new StringReader(musicList.ToString()))
        {
            string newLine = sr.ReadLine();

            while (newLine != null)
            {
                musicLocs.Add(newLine);
                newLine = sr.ReadLine();
            }
        }

        musicLocations = musicLocs.ToArray();
    }
}

#region Enumerators
public enum E_SPEEDCLASS
{
    SPARK = 0,
    TOXIC = 1,
    APEX = 2,
    HALBERD = 3,
    SPECTRE = 4
}

public enum E_GAMEMODES
{
    Arcade,
    TimeTrial,
    Survival,
    Tournament
}

public enum E_SAVELOCATIONS
{
    GHOSTS,
    TIMES,
    SHIPS,
    TRACKS
}

public enum E_SHIPS
{
    // main 8
    GTEK,
    WYVERN,
    HYPERION,
    SCORPIO,
    OMNICOM,
    DIAVOLT,
    NEXUS,
    TENRAI,
    // xpand ships
    PROTONIC,
    NX2000,
    MTECHP1,
    // secret ships
    GODTAMPON,
    BARRACUDA,
    CALIBURN,
    // others
    CUSTOM
}

public enum E_TRACKS
{
    // main 8
    UTAHPROJECT,
    ACIKNOVAE,
    ZEPHYRRIDGE,
    KAHAWAIBAY,
    HARPSTONE,
    OMEGAHARBOUR,
    ISHTARCITADEL,
    // remakes
    TERRAMAX,
    GAREDEUROPA,
    STANZAINTER,
    MANDRASHEE,
    BLUERIDGE,
    VOSTOKREEF,
    // prototypes
    _0x001,
    _0x002,
    _0x003,
    _0x004,
    _0x005,
    _0x006,
    _0x007,
    _0x008
}
#endregion
