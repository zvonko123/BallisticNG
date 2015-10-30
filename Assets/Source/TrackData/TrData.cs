using UnityEngine;
using BnG.Helpers;
using BnG.TrackTools;
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
        public bool reloadTrackData;
        public MeshFilter MESH_TRACKFLOOR;
        private MeshFilter MESH_TRACKFLOOR_PREV;
        public MeshFilter MESH_TRACKWALL;
        private MeshFilter MESH_TRACKWALL_PREV;

        [Header("[ FLAG PAINTING ] ")]
        public bool PAINT_FLAGS;
        private TrFlagPainter TR_FLAGPAINTER;
        public Color PAINT_VERTEXCOLOR;
        public E_PAINTMODE PAINT_MODE;
        [Space(10)]

        // cached tile types
        public E_TILETYPE[] CACHE_TILES;
        public E_SECTIONTYPE[] CACHE_SECTIONS;
        public Color32[] LIGHTS_TILES_FLOOR;
        public Color32[] LIGHTS_TILES_WALL;

        // editor references
        [HideInInspector]
        public TrSection highlightedSection;
        
#endregion

#region OVERRIDES
        private void OnDrawGizmos()
        {
            if (TRACK_DATA == null)
                return;

            Vector3 pos = Vector3.zero;
            Vector3 pos2 = Vector3.zero;
            Color gizmoColor = Color.white;
            for (int i = 0; i < TRACK_DATA.SECTIONS.Count; i++)
            {
                // get positions to draw line
                pos = TRACK_DATA.SECTIONS[i].SECTION_POSITION;
                pos2 = pos + TRACK_DATA.SECTIONS[i].SECTION_NORMAL * 1;

                // draw sphere and line
                gizmoColor = Color.white;
                if (TR_FLAGPAINTER != null && TRACK_DATA.SECTIONS[i] == highlightedSection && PAINT_MODE == E_PAINTMODE.SECTION) 
                {
                    gizmoColor = Color.red;
                }
                Gizmos.color = gizmoColor;
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
                pos2 = pos + (TrackDataHelper.SectionGetRotation(TRACK_DATA.SECTIONS[i]) * Vector3.forward) * 1;
                Gizmos.DrawLine(pos, pos2);
            }
        }

        private void Start()
        {
            UpdateTrackData();
        }

        private void Update()
        {
            CheckPaintChange();
            UpdateTrackData();
        }

#endregion

#region METHODS


        private void CheckPaintChange()
        {
            if (PAINT_FLAGS && TR_FLAGPAINTER == null)
                TR_FLAGPAINTER = gameObject.AddComponent<TrFlagPainter>();
            else if (!PAINT_FLAGS && TR_FLAGPAINTER != null)
                DestroyImmediate(TR_FLAGPAINTER);

            if (PAINT_FLAGS)
            {
                TR_FLAGPAINTER.TRACK_DATA = TRACK_DATA;
                TR_FLAGPAINTER.DATA_FE = this;
            }
        }

        /// <summary>
        /// Forces the track data to be regenerated.
        /// </summary>
        public void UpdateTrackData()
        {
            // if there is no track data or the meshes have changed then generate new track data
            if (TRACK_DATA == null || MESH_TRACKFLOOR_PREV != MESH_TRACKFLOOR || MESH_TRACKWALL != MESH_TRACKWALL_PREV || reloadTrackData)
            {
                reloadTrackData = false;

                // makes sure both track and wall meshes are present before updating
                if (MESH_TRACKFLOOR == null || MESH_TRACKWALL == null)
                    return;

                TRACK_DATA = TrGen.GenerateTrack(MESH_TRACKFLOOR.sharedMesh, MESH_TRACKWALL.sharedMesh, MESH_TRACKFLOOR.transform, MESH_TRACKWALL.transform);
                MESH_TRACKFLOOR_PREV = MESH_TRACKFLOOR;
                MESH_TRACKWALL_PREV = MESH_TRACKWALL;

                // fetch tile and section types from cache
                if (CACHE_TILES.Length > 0)
                {
                    for (int i = 0; i < CACHE_TILES.Length; i++)
                    {
                        TRACK_DATA.TILES_FLOOR[i].TILE_TYPE = CACHE_TILES[i];
                    }
                }

                if (CACHE_SECTIONS.Length > 0)
                {
                    for (int i = 0; i < CACHE_SECTIONS.Length; i++)
                    {
                        TRACK_DATA.SECTIONS[i].SECTION_TYPE = CACHE_SECTIONS[i];
                    }
                }

                // set cache array lengths
                if (CACHE_TILES.Length != TRACK_DATA.TILES_FLOOR.Count)
                    CACHE_TILES = new E_TILETYPE[TRACK_DATA.TILES_FLOOR.Count];
                if (CACHE_SECTIONS.Length != TRACK_DATA.SECTIONS.Count)
                    CACHE_SECTIONS = new E_SECTIONTYPE[TRACK_DATA.SECTIONS.Count];

                // fetch colors for tiles
                if (LIGHTS_TILES_FLOOR.Length > 0)
                {
                    Color32[] newCols = MESH_TRACKFLOOR.sharedMesh.colors32;
                    for (int i = 0; i < MESH_TRACKFLOOR.sharedMesh.colors32.Length; i++)
                    {
                        newCols[i] = LIGHTS_TILES_FLOOR[i];
                    }
                }

                if (LIGHTS_TILES_WALL.Length > 0)
                {
                    Color32[] newCols = MESH_TRACKWALL.sharedMesh.colors32;
                    for (int i = 0; i < MESH_TRACKWALL.sharedMesh.colors32.Length; i++)
                    {
                        newCols[i] = LIGHTS_TILES_WALL[i];
                    }
                }

                // set light array lengths
                if (LIGHTS_TILES_FLOOR.Length != MESH_TRACKFLOOR.sharedMesh.colors32.Length)
                    LIGHTS_TILES_FLOOR = new Color32[MESH_TRACKFLOOR.sharedMesh.colors32.Length];
                if (LIGHTS_TILES_WALL.Length != MESH_TRACKWALL.sharedMesh.colors32.Length)
                    LIGHTS_TILES_WALL = new Color32[MESH_TRACKWALL.sharedMesh.colors32.Length];

            }
        }

#endregion


    }
}
