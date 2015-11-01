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
        public bool resetTypes;
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

        // original vert lenghts
        private bool hasCachedVerts;
        private Mesh originalFloorMesh;
        private Mesh originalWallMesh;

        // spawning
        public List<Vector3> spawnPositions = new List<Vector3>();
        public List<Vector3> spawnCameraLocations = new List<Vector3>();
        public List<Quaternion> spawnRotations = new List<Quaternion>();
        public Vector3 cameraStart;
        public Vector3 cameraEnd;

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

        private void Update()
        {
            CheckPaintChange();
            UpdateTrackData();
            ResetTRTypes();
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

                // restore original meshes before regenerating track data
                if (!hasCachedVerts)
                {
                    originalFloorMesh = MESH_TRACKFLOOR.sharedMesh;
                    originalWallMesh = MESH_TRACKWALL.sharedMesh;
                    hasCachedVerts = true;
                } else if (hasCachedVerts)
                {
                    MESH_TRACKFLOOR.sharedMesh = originalFloorMesh;
                    MESH_TRACKWALL.sharedMesh = originalWallMesh;
                }

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

            }
        }

        public void FindSpawnTiles()
        {
            // clear any previous spawns
            spawnPositions.Clear();
            spawnRotations.Clear();

            // search through each tile for spawn tiles
            for (int i = 0; i < TRACK_DATA.TILES_FLOOR.Count; i++)
            {
                // if this tile is a spawn tile then add it to the spawn transform arrays
                if (TRACK_DATA.TILES_FLOOR[i].TILE_TYPE == E_TILETYPE.SPAWN)
                {
                    Vector3 spawnPos = TRACK_DATA.TILES_FLOOR[i].TILE_POSITION;
                    spawnPos.y += 0.5f;

                    spawnPositions.Add(spawnPos);
                    spawnRotations.Add(TrackDataHelper.SectionGetRotation(TRACK_DATA.TILES_FLOOR[i].TILE_SECTION));

                    Vector3 cameraPos = TRACK_DATA.TILES_FLOOR[i].TILE_SECTION.SECTION_POSITION;
                    cameraPos.y += 0.5f;
                    spawnCameraLocations.Add(cameraPos);

                }
            }
        }

        private void ResetTRTypes()
        {
            if (!resetTypes)
                return;

            for (int i = 0; i < TRACK_DATA.TILES_FLOOR.Count; i++)
                TRACK_DATA.TILES_FLOOR[i].TILE_TYPE = E_TILETYPE.FLOOR;

            for (int i = 0; i < TRACK_DATA.SECTIONS.Count; i++)
                TRACK_DATA.SECTIONS[i].SECTION_TYPE = E_SECTIONTYPE.NORMAL;

            resetTypes = false;
        }

#endregion


    }
}
