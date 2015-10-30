using UnityEngine;
using BnG.TrackData;
using System.Collections;

namespace BnG.Helpers
{
    /// <summary>
    /// Helpers for obtaining track data.
    /// </summary>
    public class TrackDataHelper
    {
        /// <summary>
        /// Get all tiles in a section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static TrTile[] SectionGetTiles(TrSection section)
        {
            return section.SECTION_TILES;
        }

        /// <summary>
        /// Get the rotation of a section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Quaternion SectionGetRotation(TrSection section)
        {
            // get positions
            Vector3 sectionPosition = section.SECTION_POSITION;
            Vector3 nextPosition = section.SECTION_NEXT.SECTION_POSITION;

            // get the forward direction from positions and then the normal
            Vector3 forward = (nextPosition - sectionPosition);
            Vector3 normal = section.SECTION_NORMAL;

            // return lookat
            return Quaternion.LookRotation(forward.normalized, normal.normalized);
        }

        /// <summary>
        /// Get the section a tile belongs to.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static TrSection TileGetSection(TrTile tile)
        {
            return tile.TILE_SECTION;
        }

        /// <summary>
        /// Get a tile from a vertex.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TrTile TileFromTriangleIndex(int index, E_TRACKMESH meshType, TrGenData data)
        {
            if (meshType == E_TRACKMESH.FLOOR)
            {
                if (data.TILES_FLOOR_MAPPED[index * 3] != null)
                    return data.TILES_FLOOR_MAPPED[index * 3];
                else
                    return null;
            }
            else if (meshType == E_TRACKMESH.WALL)
            {
                if (data.TILES_WALL_MAPPED[index * 3] != null)
                    return data.TILES_WALL_MAPPED[index * 3];
                else
                    return null;
            }
            else
            {
                return null;
            }

        }
    }
}
