using UnityEngine;
using BnG.TrackData;
using BnG.TrackTools;
using System.Collections;
using System.Collections.Generic;

public class RaceSettings
{
    // race references
    public static TrData trackData;
    public static Camera currentCamera;
    public static List<ShipBase> SHIPS = new List<ShipBase>();

    // race settings
    public static int racers = 1;
    public static int laps = 1;
    public static E_SPEEDCLASS speedclass = E_SPEEDCLASS.SPECTRE;
    public static E_SHIPS playerShip = E_SHIPS.MTECHP1;
    public static bool shipsRestrained = false;

    // countdown
    public static bool countdownReady;
    public static bool countdownFinished;

    public static Vector3 introCamStart;
    public static Vector3 introCamEnd;

    // player control
    public static int playerControlIndex;
}

public class GameSettings
{
    /// <summary>
    /// Cap the framerate.
    /// </summary>
    /// <param name="cap"></param>
    public static void CapFPS(int cap)
    {
        Application.targetFrameRate = cap;
    }
}

public class AudioSettings
{
    // audio settings
    public static float VOLUME_MAIN = 1.0f;
    public static float VOLUME_MUSIC;
    public static float VOLUME_SFX;
    public static float VOLUME_VOICES;

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
    BARRACUDA

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
