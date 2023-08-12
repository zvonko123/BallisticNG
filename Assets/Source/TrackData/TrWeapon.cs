using UnityEngine;
using System.Collections;

namespace BnG.TrackData
{
    public class TrWeapon : MonoBehaviour
    {
        public float disabledTimer;
        public int[] indicies;

        /// <summary>
        /// Disable the pad for t seconds;
        /// </summary>
        /// <param name="time"></param>
        public void DisablePad(float t)
        {
            disabledTimer = t;
        }

        /// <summary>
        /// Updates the weapon pad's disabled timer.
        /// </summary>
        public void PadUpdate()
        {
            disabledTimer -= Time.deltaTime;

            if (disabledTimer < 0)
                disabledTimer = 0;
        }
    }
}
