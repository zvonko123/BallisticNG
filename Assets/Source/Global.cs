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
    public static ServerReferences serverReferences;

    // race settings
    public static int racers = 1;
    public static int laps = 1;
    public static E_SPEEDCLASS speedclass = E_SPEEDCLASS.SPECTRE;
    public static E_SHIPS playerShip = E_SHIPS.MTECHP1;
    public static E_GAMEMODES gamemode = E_GAMEMODES.TimeTrial;
    public static E_WEAPONCLASSES playerWClass = E_WEAPONCLASSES.MIXED;
    public static bool shipsRestrained = false;
    public static bool hyperSpeed = false;

    // countdown
    public static bool countdownReady;
    public static bool countdownFinished;

    public static Vector3 introCamStart;
    public static Vector3 introCamEnd;

    // player control
    public static int playerControlIndex;

    public static string customShipName;

    public static string trackToLoad;
    public static bool isNetworked;

    public static void StartRace()
    {
        // start race and unrestrain ships
        countdownFinished = true;
        shipsRestrained = false;

        // get all ships to check if they can start boost
        for (int i = 0; i < SHIPS.Count; ++i)
            SHIPS[i].CheckStartBoost();
    }
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
    public static int GS_DRAWDISTANCE = 60;
    public static bool GS_FULLSCREEN = true;
    public static bool GS_BLOOM = false;
    public static bool GS_FXAA = false;
    public static bool GS_CRT = false;
    public static bool GS_VAPORWAVE = false;

    public static bool optionsClose = false;

    public static string profileName = "Developer";

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

    /// <summary>
    /// Quits the game.
    /// </summary>
    public static void QuitGame()
    {
        Application.Quit();
    }
}

public class UserProgress
{
    // ship unlocks
    public static bool unlockedBarracuda;
    public static bool unlockedPrototype1;
    public static bool unlockedPrototype2;
    public static bool unlockedPrototype3;
    public static bool unlockedGodTampon;

    // campaign progress
    public static Medal c1e1;
    public static Medal c1e2;
    public static Medal c1e3;
    public static Medal c1e4;
    public static Medal c1e5;
    public static Medal c1e6;
    public static Medal c1e7;
    public static Medal c1e8;
    public static Medal c2e1;
    public static Medal c2e2;
    public static Medal c2e3;
    public static Medal c2e4;
    public static Medal c2e5;
    public static Medal c2e6;
    public static Medal c2e7;
    public static Medal c2e8;
    public static Medal c3e1;
    public static Medal c3e2;
    public static Medal c3e3;
    public static Medal c3e4;
    public static Medal c3e5;
    public static Medal c3e6;
    public static Medal c3e7;
    public static Medal c3e8;

    // track unlocks
    public static bool proto1;
    public static bool proto2;
    public static bool proto3;
    public static bool proto4;
    public static bool proto5;
    public static bool proto6;
    public static bool proto7;
    public static bool proto8;
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
    LUNA,
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
