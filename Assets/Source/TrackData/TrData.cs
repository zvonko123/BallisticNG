using UnityEngine;
using System.Collections.Generic;

namespace BnG.TrackData
{
    /// <summary>
    /// Represents all information about the track.
    /// </summary>
    [ExecuteInEditMode]
    public class TrData : MonoBehaviour
    {
        #region VARS-PUBLIC

        // track tiles and sections
        public TrGenData TRACK_DATA;

        [Header("[ MESH REFERENCES ]")]
        public MeshFilter MESH_TRACKFLOOR;
        private MeshFilter MESH_TRACKFLOOR_PREV;
        public MeshFilter MESH_TRACKWALL;
        private MeshFilter MESH_TRACKWALL_PREV;
        
        #endregion

        #region OVERRIDES
        private void OnDrawGizmos()
        {
            if (TRACK_DATA == null)
                return;

            Vector3 pos = Vector3.zero;
            Vector3 pos2 = Vector3.zero;
            for (int i = 0; i < TRACK_DATA.SECTIONS.Count; i++)
            {
                // get positions to draw line
                pos = TRACK_DATA.SECTIONS[i].SECTION_POSITION;
                pos2 = pos + TRACK_DATA.SECTIONS[i].SECTION_NORMAL * 1;

                // draw sphere and line
                Gizmos.DrawWireSphere(pos, 0.2f);
                Gizmos.DrawWireSphere(pos2, 0.2f);
                Gizmos.DrawLine(pos, pos2);

                // draw line between sections
                if (i < TRACK_DATA.SECTIONS.Count - 1)
                {
                    pos2 = TRACK_DATA.SECTIONS[i + 1].SECTION_POSITION;
                    Gizmos.DrawLine(pos, pos2);
                }

                // draw orientation
                pos = pos + TRACK_DATA.SECTIONS[i].SECTION_NORMAL * 0.5f;
                pos2 = pos + (SectionGetRotation(TRACK_DATA.SECTIONS[i]) * Vector3.forward) * 1;
                Gizmos.DrawLine(pos, pos2);
            }
            
        }

        private void Start()
        {
            UpdateTrackData();
        }

        private void Update()
        {
            UpdateTrackData();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Forces the track data to be regenerated.
        /// </summary>
        public void UpdateTrackData()
        {
            // if there is no track data or the meshes have changed then generate new track data
            if (TRACK_DATA == null || MESH_TRACKFLOOR_PREV != MESH_TRACKFLOOR || MESH_TRACKWALL != MESH_TRACKWALL_PREV)
            {
                // makes sure both track and wall meshes are present before updating
                if (MESH_TRACKFLOOR == null || MESH_TRACKWALL == null)
                    return;

                TRACK_DATA = TrGen.GenerateTrack(MESH_TRACKFLOOR.sharedMesh, MESH_TRACKWALL.sharedMesh, MESH_TRACKFLOOR.transform, MESH_TRACKWALL.transform);
                MESH_TRACKFLOOR_PREV = MESH_TRACKFLOOR;
                MESH_TRACKWALL_PREV = MESH_TRACKWALL;
            }
        }

        /// <summary>
        /// Get all tiles in a section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public TrTile[] SectionGetTiles(TrSection section)
        {
            return section.SECTION_TILES;
        }

        /// <summary>
        /// Get the rotation of a section.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public Quaternion SectionGetRotation(TrSection section)
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
        public TrSection TileGetSection(TrTile tile)
        {
            return tile.TILE_SECTION;
        }

        /// <summary>
        /// Get a tile from a vertex.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TrTile TileFromVertex(int index, E_TRACKMESH meshType)
        {
            if (meshType == E_TRACKMESH.FLOOR)
            {
                if (TRACK_DATA.TILES_FLOOR_MAPPED[index] != null)
                    return TRACK_DATA.TILES_FLOOR_MAPPED[index];
                else
                    return null;
            } else if (meshType == E_TRACKMESH.WALL)
            {
                if (TRACK_DATA.TILES_WALL_MAPPED[index] != null)
                    return TRACK_DATA.TILES_WALL_MAPPED[index];
                else
                    return null;
            } else
            {
                return null;
            }

        }

        #endregion


    }
}
