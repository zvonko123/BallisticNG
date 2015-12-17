using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CounterboardManager : MonoBehaviour {

    [Header("[ REFERENCES ]")]
    public Text txtCountDown;
    public Text txtInformation;
    public GameObject checkersParent;

    [Header("[ SETTINGS ]")]
    public float checkersMoveSpeed;
    private float checkersX;

    public void UpdateCountdown(string text)
    {
        txtCountDown.text = text;
    }

    void FixedUpdate()
    {
        // scroll checkers
        checkersX += Time.deltaTime * checkersMoveSpeed;
        if (checkersX > 200)
            checkersX = 0;

        // apply checkers scroll
        //checkersParent.transform.localPosition = new Vector3(checkersX, checkersParent.transform.localPosition.y, 0.0f);

        // update information
        txtInformation.text = RaceSettings.gamemode + " - " + RaceSettings.speedclass;

        // next lap notification
        if (RaceSettings.SHIPS[0].currentLap > 0 && RaceSettings.SHIPS[0].currentLap < RaceSettings.laps - 1)
            txtCountDown.text = "LAP " + (RaceSettings.SHIPS[0].currentLap + 1);

        // final lap notifcation
        if (RaceSettings.SHIPS[0].currentLap == RaceSettings.laps - 1)
            txtCountDown.text = "FINAL!";

        // finish notification
        if (RaceSettings.SHIPS[0].currentLap == RaceSettings.laps)
            txtCountDown.text = "FINISH!";
    }
}
