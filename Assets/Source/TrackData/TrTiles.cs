using UnityEngine;
using System.Collections.Generic;

namespace BnG.TrackData
{
    /// <summary>
    /// Represents a face on the track floor.
    /// </summary>
    public class TrTile
    {
        // the index of this tile
        public int TILE_INDEX;

        // the type of tile this is
        public E_TILETYPE TILE_TYPE;

        // the color of this tile (for per-vertex lighting)
        public Color32 TILE_COLOR;

        // the position of this tile
        public Vector3 TILE_POSITION;

        // the indicies of vertices that make this tile
        public List<int> TILE_INDICES = new List<int>();

        // the section this tile belongs to
        public TrSection TILE_SECTION;
    }

    /// <summary>
    /// Types of tiles a track tile can be.
    /// </summary>
    public enum E_TILETYPE
    {
        FLOOR       = 0,
        WALL        = 1,
        BOOST       = 2,
        WEAPON      = 3,
        RECHARGE    = 4,
        SPAWN       = 5
    }
}
