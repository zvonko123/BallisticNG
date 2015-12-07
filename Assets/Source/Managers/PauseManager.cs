using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseManager : MonoBehaviour {

    [Header("[ REFERENCES ]")]
    public Button toSelect;

    // for pause screen
    public GameObject container;
    public BNGButton optionsButton;

    // for results screen
    public ShipRefs r;
    public Text results;

    public void UnpauseGame()
    {
        RaceSettings.raceManager.PauseInput();
    }

    public void OpenOptions()
    {
        RaceSettings.raceManager.optionsManager.SetActive(true);
        RaceSettings.raceManager.optionsManager.GetComponent<OptionsManager>().OpenOptions();
        container.SetActive(false);
    }

    public void LoadResults()
    {
        // clear template text
        results.text = "";
        for (int i = 0; i < RaceSettings.laps; ++i)
        {
            results.text += string.Format("LAP {0} - {1}", i + 1, FloatToTime.Convert(r.laps[i], "0:00.00"));
            if (r.perfects[i] == true)
                results.text += "<color=red> P</color>";
            else
                results.text += "   ";

            results.text += "\n";
        }
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

    void Update()
    {
        if (GameSettings.optionsClose)
        {
            RaceSettings.raceManager.optionsManager.SetActive(false);
            container.SetActive(true);
            optionsButton.Select();

            GameSettings.optionsClose = false;
        }
    }

}
