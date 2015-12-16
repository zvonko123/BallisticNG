using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

    [Header("[ REFERENCES ]")]
    public OptionsManager options;
    public GameObject menuRoot;
    public BNGButton menuDefaultSelection;
    [Space(10)]
    public BNGButton trackDefaultSelection;
    public GameObject trackRoot;
    public GameObject gamemodeRoot;
    [Space(10)]
    public BNGButton teamDefaultSelection;
    public GameObject teamRoot;
    [Space(10)]
    public GameObject[] Teams;
    [Space(10)]
    public Text statSpeed;
    public Text statHandling;
    public Text statAcceleration;
    public Text statShield;
    public Text statFirepower;

    private int currentTeam;

    void Start()
    {
        menuDefaultSelection.Select();
        Cursor.visible = true;
    }

    void Update()
    {
        // options close
        if (GameSettings.optionsClose)
        {
            options.gameObject.SetActive(false);
            menuRoot.SetActive(true);
            menuDefaultSelection.Select();
            GameSettings.optionsClose = false;
        }

        // update team mesh display
        bool enabled = teamRoot.activeSelf;
        for (int i = 0; i < Teams.Length; ++i)
        {
            if(enabled && i == currentTeam)
            {
                Teams[i].SetActive(true);
            } else
            {
                Teams[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// Set the current gamemode.
    /// </summary>
    /// <param name="gamemode"></param>
    public void SetGameMode(int gamemode)
    {
        RaceSettings.gamemode = (E_GAMEMODES)gamemode;
    }

    public void SetTrack(int track)
    {
        E_TRACKS toTrack = (E_TRACKS)track;
        switch(toTrack)
        {
            case E_TRACKS.ACIKNOVAE:
                RaceSettings.trackToLoad = "Aciknovae_Normal";
                break;
            case E_TRACKS.BLUERIDGE:
                RaceSettings.trackToLoad = "BlueRidge_Normal";
                break;
            case E_TRACKS.GAREDEUROPA:
                RaceSettings.trackToLoad = "GareDE_Normal";
                break;
            case E_TRACKS.HARPSTONE:
                RaceSettings.trackToLoad = "Harpstone_Normal";
                break;
            case E_TRACKS.ISHTARCITADEL:
                RaceSettings.trackToLoad = "Ishtar_Normal";
                break;
            case E_TRACKS.KAHAWAIBAY:
                RaceSettings.trackToLoad = "KBay_Normal";
                break;
            case E_TRACKS.MANDRASHEE:
                RaceSettings.trackToLoad = "Mandrashee_Normal";
                break;
            case E_TRACKS.OMEGAHARBOUR:
                RaceSettings.trackToLoad = "Omega_Normal";
                break;
            case E_TRACKS.STANZAINTER:
                RaceSettings.trackToLoad = "Stanza_Normal";
                break;
            case E_TRACKS.TERRAMAX:
                RaceSettings.trackToLoad = "Terramax_Normal";
                break;
            case E_TRACKS.UTAHPROJECT:
                RaceSettings.trackToLoad = "UtahProject_Normal";
                break;
            case E_TRACKS.VOSTOKREEF:
                RaceSettings.trackToLoad = "VOSTOKREEF_Normal";
                break;
            case E_TRACKS.ZEPHYRRIDGE:
                RaceSettings.trackToLoad = "ZephyrRidge_Normal";
                break;
            case E_TRACKS.LUNA:
                RaceSettings.trackToLoad = "Luna_Normal";
                break;
            case E_TRACKS._0x001:
                RaceSettings.trackToLoad = "0x001_Normal";
                break;
            case E_TRACKS._0x002:
                RaceSettings.trackToLoad = "0x002_Normal";
                break;
            case E_TRACKS._0x003:
                RaceSettings.trackToLoad = "0x003_Normal";
                break;
            case E_TRACKS._0x004:
                RaceSettings.trackToLoad = "0x004_Normal";
                break;
            case E_TRACKS._0x005:
                RaceSettings.trackToLoad = "0x005_Normal";
                break;
            case E_TRACKS._0x006:
                RaceSettings.trackToLoad = "0x006_Normal";
                break;
            case E_TRACKS._0x007:
                RaceSettings.trackToLoad = "0x007_Normal";
                break;
            case E_TRACKS._0x008:
                RaceSettings.trackToLoad = "0x008_Normal";
                break;

        }
    }

    public void SetTeam(int team)
    {
        currentTeam = team;
        RaceSettings.playerShip = (E_SHIPS)team;

        switch(RaceSettings.playerShip)
        {
            case E_SHIPS.BARRACUDA:
                statSpeed.text = "10/10";
                statAcceleration.text = "10/10";
                statHandling.text = "10/10";
                statShield.text = "10/10";
                statFirepower.text = "0/10";
                break;
            case E_SHIPS.CALIBURN:
                statSpeed.text = "8/10";
                statAcceleration.text = "3/10";
                statHandling.text = "9/10";
                statShield.text = "5/10";
                statFirepower.text = "7/10";
                break;
            case E_SHIPS.CUSTOM:
                break;
            case E_SHIPS.DIAVOLT:
                statSpeed.text = "8/10";
                statAcceleration.text = "9/10";
                statHandling.text = "3/10";
                statShield.text = "8/10";
                statFirepower.text = "8/10";
                break;
            case E_SHIPS.GODTAMPON:
                statSpeed.text = "10/10";
                statAcceleration.text = "10/10";
                statHandling.text = "10/10";
                statShield.text = "10/10";
                statFirepower.text = "10/10";
                break;
            case E_SHIPS.GTEK:
                statSpeed.text = "4/10";
                statAcceleration.text = "6/10";
                statHandling.text = "8/10";
                statShield.text = "6/10";
                statFirepower.text = "6/10";
                break;
            case E_SHIPS.HYPERION:
                statSpeed.text = "6/10";
                statAcceleration.text = "8/10";
                statHandling.text = "6/10";
                statShield.text = "5/10";
                statFirepower.text = "6/10";
                break;
            case E_SHIPS.MTECHP1:
                statSpeed.text = "6/10";
                statAcceleration.text = "6/10";
                statHandling.text = "5/10";
                statShield.text = "6/10";
                statFirepower.text = "6/10";
                break;
            case E_SHIPS.NEXUS:
                statSpeed.text = "6/10";
                statAcceleration.text = "8/10";
                statHandling.text = "6/10";
                statShield.text = "6/10";
                statFirepower.text = "8/10";
                break;
            case E_SHIPS.NX2000:
                statSpeed.text = "10/10";
                statAcceleration.text = "10/10";
                statHandling.text = "8/10";
                statShield.text = "1/10";
                statFirepower.text = "0/10";
                break;
            case E_SHIPS.OMNICOM:
                statSpeed.text = "6/10";
                statAcceleration.text = "8/10";
                statHandling.text = "6/10";
                statShield.text = "7/10";
                statFirepower.text = "6/10";
                break;
            case E_SHIPS.PROTONIC:
                statSpeed.text = "8/10";
                statAcceleration.text = "8/10";
                statHandling.text = "3/10";
                statShield.text = "6/10";
                statFirepower.text = "8/10";
                break;
            case E_SHIPS.SCORPIO:
                statSpeed.text = "6/10";
                statAcceleration.text = "9/10";
                statHandling.text = "6/10";
                statShield.text = "7/10";
                statFirepower.text = "8/10";
                break;
            case E_SHIPS.TENRAI:
                statSpeed.text = "7/10";
                statAcceleration.text = "7/10";
                statHandling.text = "10/10";
                statShield.text = "4/10";
                statFirepower.text = "3/10";
                break;
            case E_SHIPS.WYVERN:
                statSpeed.text = "7/10";
                statAcceleration.text = "5/10";
                statHandling.text = "7/10";
                statShield.text = "6/10";
                statFirepower.text = "7/10";
                break;
        }
    }

    public void SetSpeedClass(int speed)
    {
        RaceSettings.speedclass = (E_SPEEDCLASS)speed;
    }

    /// <summary>
    /// Opens the options menu and selects the graphics button.
    /// </summary>
    public void OpenOptions()
    {
        options.gameObject.SetActive(true);
        options.btnGraphics.Select();
    }

    /// <summary>
    /// Opens the track selection (should be done from the gamemode selection screen)
    /// </summary>
    public void OpenTrackSelection()
    {
        gamemodeRoot.SetActive(false);
        trackRoot.SetActive(true);
        trackDefaultSelection.Select();
    }

    public void OpenTeamSelection()
    {
        trackRoot.SetActive(false);
        teamRoot.SetActive(true);
        teamDefaultSelection.Select();
        SetTeam(0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScreen");
    }

    /// <summary>
    /// Calls the global quit function.
    /// </summary>
    public void Quit()
    {
        GameSettings.QuitGame();
    }
}
