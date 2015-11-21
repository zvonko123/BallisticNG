using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDManager : ShipBase {

    [Header("[ COLORS ]")]
    public Color accentColor;
    public Image[] accentPanels;

    [Header("[ MESSAGES ]")]
    public Text msgPerfectLap;
    public Text msgFinalLap;
    public Text msgWrongWay;

    [Header("[ SPEED PANELS ]")]
    public Text txtShield;
    public Image imgShield;
    public RectTransform tShield;
    public Image imgPwr;
    public RectTransform tPwr;
    public Image imgSpeed;
    public Text txtSpeed;
    public RectTransform tSpeed;

    [Header("[ TIME PANELS ]")]
    public Text[] txtLaps;
    public Image[] imgLaps;
    public GameObject[] gmLapPanels;
    public GameObject[] gmPerfectText;
    public Text txtTotalTime;
    public Text txtCurrentTime;
    public Text txtBestTime;
    public Text txtChkPoint;
    public Text txtCurrenLap;
    public Text txtMaxLap;

    public Image[] lapCounters;
    
    void Start()
    {
        // disable un-used lap counters
        for (int i = RaceSettings.laps; i < gmLapPanels.Length; i++)
            gmLapPanels[i].SetActive(false);
    }

    void Update()
    {
        // update accent color
        int i = 0;
        accentColor.a = 0.5f;
        for (i = 0; i < accentPanels.Length; i++)
        {
            accentPanels[i].color = accentColor;
        }

        // wrong way indicator
        if (r.facingFoward)
            msgWrongWay.text = "";
        else
            msgWrongWay.text = "WRONG WAY";

        // shield bar
        float shieldPercent = r.shield / r.settings.DAMAGE_SHIELD;
        shieldPercent = Mathf.Clamp(shieldPercent, 0.0f, 1.0f);
        tShield.localScale = new Vector3(shieldPercent, 1.0f, 1.0f);

        // shield text
        float displayShield = Mathf.Round(shieldPercent * 100) / 100;
        txtShield.text = (displayShield * 100).ToString() + "%";

        // shield color
        imgShield.color = new Color((1.0f - accentColor.r) * 0.4f, (1.0f - accentColor.g) * 0.4f, (1.0f - accentColor.b) * 0.4f, 1.0f);

        // power bar
        tPwr.localScale = new Vector3(r.sim.enginePower, 1.0f, 1.0f);

        // powercolor
        imgSpeed.color = new Color((1.0f - accentColor.r) * 0.6f, (1.0f - accentColor.g) * 0.6f, accentColor.r * 0.6f, 1.0f);

        // speed bar
        float speed = r.transform.InverseTransformDirection(r.body.velocity).z / r.settings.ENGINE_MAXSPEED_SPECTRE;
        speed = Mathf.Clamp(speed, 0.0f, 1.0f) * 3;
        tSpeed.localScale = new Vector3(speed, 1.0f, 1.0f);

        // speed text
        txtSpeed.text = Mathf.RoundToInt((r.transform.InverseTransformDirection(r.body.velocity).z * 20)).ToString();

        // speed color
        imgPwr.color = new Color((1.0f - accentColor.r) * 0.8f, (1.0f - accentColor.g) * 0.8f, (1.0f - accentColor.b) * 0.8f, 1.0f);

        // update total time
        txtTotalTime.text = FloatToTime.Convert(r.totalTime, "0:00.00");

        // update best time
        if (r.hasBestTime)
            txtBestTime.text = FloatToTime.Convert(r.bestLap, "0:00.00");
        else
            txtBestTime.text = "-:--.--";

        // update current time
        for (i = 0; i < txtLaps.Length; i++)
        {
            if (i < r.currentLap)
                txtLaps[i].text = FloatToTime.Convert(r.laps[i], "0:00.00");

            if (r.perfects[i])
                gmPerfectText[i].SetActive(true);
            else
                gmPerfectText[i].SetActive(false);

            if (i == r.currentLap - 1)
                txtLaps[i].text = FloatToTime.Convert(r.currentTime, "0:00.00");

            if (i == r.currentLap - 1)
                imgLaps[i].color = accentColor;
            else
                imgLaps[i].color = new Color(1.0f, 1.0f, 1.0f, accentColor.a);
        }

        // update lap text
        txtCurrenLap.text = r.currentLap.ToString();
        txtMaxLap.text = RaceSettings.laps.ToString();

        // notifications
        if (r.finalLapPopup > 0)
            msgFinalLap.gameObject.SetActive(true);
        else
            msgFinalLap.gameObject.SetActive(false);

        if (r.perfectLapPopup > 0)
            msgPerfectLap.gameObject.SetActive(true);
        else
            msgPerfectLap.gameObject.SetActive(false);
    }
}
