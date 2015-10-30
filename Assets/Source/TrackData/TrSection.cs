using UnityEngine;
using System.Collections.Generic;

namespace BnG.TrackData
{
    /// <summary>
    /// Represents a segment of the track.
    /// </summary>
    public class TrSection
    {
        // the index of this section
        public int SECTION_INDEX;

        // what type of section this is
        public E_SECTIONTYPE SECTION_TYPE;

        // the position of this section
        public Vector3 SECTION_POSITION;

        // the tiles in this section
        public TrTile[] SECTION_TILES;

        // the normal of this section
        public Vector3 SECTION_NORMAL;

        // the next section
        public TrSection SECTION_NEXT;

        // the next junction
        private TrSection SECTION_JUNCTION;

        /// <summary>
        /// If this section is a junction then the junction will be returned.
        /// </summary>
        /// <returns>The next junction</returns>
        public TrSection GetJunction()
        {
            if (SECTION_TYPE == E_SECTIONTYPE.JUNCTION_START && SECTION_JUNCTION != null)
                return SECTION_JUNCTION;
            else
                return null;
        }
    }

    /// <summary>
    /// Types of sections a section can be.
    /// </summary>
    public enum E_SECTIONTYPE
    {
        NORMAL              = 0,
        JUMP_START          = 1,
        JUMP_END            = 2,
        JUNCTION_START      = 3,
        JUNCTION_END        = 4,
        PITLANE_ENTRANCE    = 5,
        LOGIC_STARTLINE     = 6
    }
}
