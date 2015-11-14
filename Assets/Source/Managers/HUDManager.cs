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

    public Image[] lapCounters;

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
        imgShield.color = new Color(accentColor.b * 0.4f, accentColor.g * 0.4f, accentColor.r * 0.4f, 1.0f);

        // power bar
        tPwr.localScale = new Vector3(r.sim.enginePower, 1.0f, 1.0f);

        // powercolor
        imgSpeed.color = new Color(accentColor.b * 0.6f, accentColor.g * 0.6f, accentColor.r * 0.6f, 1.0f);

        // speed bar
        float speed = r.transform.InverseTransformDirection(r.body.velocity).z / r.settings.ENGINE_MAXSPEED_SPECTRE;
        speed = Mathf.Clamp(speed, 0.0f, 1.0f) * 3;
        tSpeed.localScale = new Vector3(speed, 1.0f, 1.0f);

        // speed text
        txtSpeed.text = Mathf.RoundToInt((r.transform.InverseTransformDirection(r.body.velocity).z * 20)).ToString();

        // speed color
        imgPwr.color = new Color(accentColor.b * 0.8f, accentColor.g * 0.8f, accentColor.r * 0.8f, 1.0f);
    }
}
