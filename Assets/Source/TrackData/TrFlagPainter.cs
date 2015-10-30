using UnityEngine;
using BnG.TrackData;
using System.Collections;

namespace BnG.TrackTools
{
    /// <summary>
    /// Empty class for the editor class to function.
    /// </summary>
    public class TrFlagPainter : MonoBehaviour
    {

        public TrGenData TRACK_DATA;
        public TrData DATA_FE;
    }

    public enum E_PAINTMODE
    {
        TILE        = 0,
        SECTION     = 1,
        LIGHTING    = 2
    }
}
