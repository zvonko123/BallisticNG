using UnityEngine;
using System.Collections;

public class FloatToTime
{

    public static string Convert(float toConvert, string format)
    {
        string newString = "";
        switch (format)
        {
            case "00.0":
                newString = (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                    Mathf.Floor((toConvert * 10) % 10).ToString("0")/*miliseconds*/;
                return newString;
            case "#0.0":
                newString = (Mathf.Floor(toConvert) % 60).ToString("#0")/*seconds*/+ "." +
                    Mathf.Floor((toConvert * 10) % 10).ToString("0")/*miliseconds*/;
                return newString;
            case "00.00":
                newString = (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                    Mathf.Floor((toConvert * 100) % 100).ToString("00")/*miliseconds*/;
                return newString;
            case "00.000":
                newString = (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                    Mathf.Floor((toConvert * 1000) % 1000).ToString("000")/*miliseconds*/;
                return newString;
            case "#00.000":
                newString = (Mathf.Floor(toConvert) % 60).ToString("#00")/*seconds*/+ "." +
                    Mathf.Floor((toConvert * 1000) % 1000).ToString("000")/*miliseconds*/;
                return newString;
            case "0:00.0":
                newString = (Mathf.Floor(toConvert / 60)).ToString("0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 10) % 10).ToString("0")/*miliseconds*/;
                return newString;
            case "#0:00":
                newString = (Mathf.Floor(toConvert / 60)).ToString("0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/;
                return newString;
            case "#00:00":
                newString = (Mathf.Floor(toConvert / 60)).ToString("#00")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/;
                return newString;
            case "#0:00.0":
                newString = (Mathf.Floor(toConvert / 60)).ToString("#0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 10) % 10).ToString("0")/*miliseconds*/;
                return newString;
            case "0:00.00":
                newString = (Mathf.Floor(toConvert / 60)).ToString("0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 100) % 100).ToString("00")/*miliseconds*/;
                return newString;
            case "#0:00.00":
                newString = (Mathf.Floor(toConvert / 60)).ToString("#0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 100) % 100).ToString("00")/*miliseconds*/;
                return newString;
            case "0:00.000":
                newString = (Mathf.Floor(toConvert / 60)).ToString("0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 1000) % 1000).ToString("000")/*miliseconds*/;
                return newString;
            case "#0:00.000":
                newString = (Mathf.Floor(toConvert / 60)).ToString("#0")/*minutes*/+ ":" +
                    (Mathf.Floor(toConvert) % 60).ToString("00")/*seconds*/+ "." +
                        Mathf.Floor((toConvert * 1000) % 1000).ToString("000")/*miliseconds*/;
                return newString;
        }
        return newString;
    }
}