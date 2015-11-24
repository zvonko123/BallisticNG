using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour {

    [Header("[ REFERENCES ]")]
    public Button toSelect;

    public void UnpauseGame()
    {
        RaceSettings.raceManager.PauseInput();
    }

    public void RestartLevel()
    {
        // unpause game
        GameSettings.PauseToggle(false);

        // load level
        Application.LoadLevel(Application.loadedLevel);
    }

    public void MenuEnabled()
    {
        toSelect.Select();
    }

    public void Quit()
    {
        // back to menu here
    }

}
