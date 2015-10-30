using UnityEngine;
using UnityEditor;
using BnG.TrackData;
using BnG.Helpers;
using System.Collections;

namespace BnG.TrackTools
{
    [CustomEditor(typeof(TrFlagPainter))]
    public class TrFlagPainterEditor : Editor
    {

        E_TILETYPE currentTileType;
        E_SECTIONTYPE currentSectionType;

        void OnSceneGUI()
        {
            HandleUtility.Repaint();

            // do not allow selection
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            // get target class
            TrFlagPainter thisTarget = (TrFlagPainter)target;

            // cycle tile types
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.KeypadPlus)
            {
                if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.TILE)
                {
                    switch (currentTileType)
                    {
                        case E_TILETYPE.FLOOR:
                            currentTileType = E_TILETYPE.BOOST;
                            break;
                        case E_TILETYPE.BOOST:
                            currentTileType = E_TILETYPE.WEAPON;
                            break;
                        case E_TILETYPE.WEAPON:
                            currentTileType = E_TILETYPE.RECHARGE;
                            break;
                        case E_TILETYPE.RECHARGE:
                            currentTileType = E_TILETYPE.SPAWN;
                            break;
                        case E_TILETYPE.SPAWN:
                            currentTileType = E_TILETYPE.FLOOR;
                            break;
                    }
                }
                else if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.SECTION)
                {
                    switch (currentSectionType)
                    {
                        case E_SECTIONTYPE.NORMAL:
                            currentSectionType = E_SECTIONTYPE.LOGIC_STARTLINE;
                            break;
                        case E_SECTIONTYPE.LOGIC_STARTLINE:
                            currentSectionType = E_SECTIONTYPE.JUNCTION_START;
                            break;
                        case E_SECTIONTYPE.JUNCTION_START:
                            currentSectionType = E_SECTIONTYPE.JUNCTION_END;
                            break;
                        case E_SECTIONTYPE.JUNCTION_END:
                            currentSectionType = E_SECTIONTYPE.JUMP_START;
                            break;
                        case E_SECTIONTYPE.JUMP_START:
                            currentSectionType = E_SECTIONTYPE.JUMP_END;
                            break;
                        case E_SECTIONTYPE.JUMP_END:
                            currentSectionType = E_SECTIONTYPE.PITLANE_ENTRANCE;
                            break;
                        case E_SECTIONTYPE.PITLANE_ENTRANCE:
                            currentSectionType = E_SECTIONTYPE.NORMAL;
                            break;
                    }
                }
            }

            // raycast from scene view mouse position
            RaycastHit hit;
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            Mesh m = thisTarget.DATA_FE.MESH_TRACKFLOOR.sharedMesh;
            GUIStyle label = new GUIStyle();
            label.fontStyle = FontStyle.Bold;
            if (Physics.Raycast(ray, out hit, 1 << LayerMask.NameToLayer("TrackFloor")))
            {
                TrTile tile = TrackDataHelper.TileFromTriangleIndex(hit.triangleIndex, E_TRACKMESH.FLOOR, thisTarget.TRACK_DATA);

                if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.TILE)
                {

                    // draw mesh
                    Vector3[] verts = new Vector3[4];
                    int t = hit.triangleIndex * 3;

                    verts[0] = hit.transform.TransformPoint(m.vertices[m.triangles[t + 0]]);
                    verts[1] = hit.transform.TransformPoint(m.vertices[m.triangles[t + 1]]);
                    verts[2] = hit.transform.TransformPoint(m.vertices[m.triangles[t + 2]]);
                    verts[3] = hit.transform.TransformPoint(m.vertices[m.triangles[t + 0]]);

                    // change mesh color depending on face type
                    Color col = Color.grey;
                    Color outline = col;

                    switch (tile.TILE_TYPE)
                    {
                        case E_TILETYPE.BOOST:
                            col = Color.blue;
                            break;
                        case E_TILETYPE.WEAPON:
                            col = Color.red;
                            break;
                        case E_TILETYPE.SPAWN:
                            col = Color.yellow;
                            break;
                        case E_TILETYPE.RECHARGE:
                            col = Color.green;
                            break;
                    }

                    switch(currentTileType)
                    {
                        case E_TILETYPE.BOOST:
                            outline = Color.blue;
                            break;
                        case E_TILETYPE.WEAPON:
                            outline = Color.red;
                            break;
                        case E_TILETYPE.SPAWN:
                            outline = Color.yellow;
                            break;
                        case E_TILETYPE.RECHARGE:
                            outline = Color.green;
                            break;
                    }

                    outline *= 0.8f;
                    col.a = 0.2f;

                    Handles.DrawSolidRectangleWithOutline(verts, col, outline);

                    // draw label
                    string flip = tile.TILE_SECOND ? "Flipped" : "Normal";

                    Vector3 pos = tile.TILE_POSITION;
                    col.a = 1.0f;
                    label.normal.textColor = col;
                    Handles.Label(pos, "Current = " + tile.TILE_TYPE.ToString() + "_" + flip, label);

                    pos += tile.TILE_SECTION.SECTION_NORMAL * 0.4f;
                    outline.a = 1.0f;
                    label.normal.textColor = outline;
                    Handles.Label(pos, "Paint = " + currentTileType.ToString() + "_" + flip, label);

                    if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
                    {
                        // apply change
                        tile.TILE_TYPE = currentTileType;

                        // cache change
                        thisTarget.DATA_FE.CACHE_TILES[tile.TILE_INDEX] = currentTileType;
                    } else if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.R)
                    {
                        // apply change
                        tile.TILE_TYPE = E_TILETYPE.FLOOR;

                        // cache change
                        thisTarget.DATA_FE.CACHE_TILES[tile.TILE_INDEX] = currentTileType;
                    }
                }
                else if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.SECTION)
                {
                    thisTarget.DATA_FE.highlightedSection = tile.TILE_SECTION;

                    // change mesh color depending on section type
                    Color col = Color.grey;
                    Color outline = col;

                    switch (tile.TILE_SECTION.SECTION_TYPE)
                    {
                        case E_SECTIONTYPE.JUMP_START:
                            col = Color.green;
                            break;
                        case E_SECTIONTYPE.JUMP_END:
                            col = Color.green;
                            col *= 0.5f;
                            break;
                        case E_SECTIONTYPE.JUNCTION_START:
                            col = Color.red;
                            break;
                        case E_SECTIONTYPE.JUNCTION_END:
                            col = Color.red;
                            col *=0.5f;
                            break;
                        case E_SECTIONTYPE.LOGIC_STARTLINE:
                            col = Color.blue;
                            break;
                        case E_SECTIONTYPE.PITLANE_ENTRANCE:
                            col=  Color.yellow;
                            break;
                    }

                    switch (currentSectionType)
                    {
                        case E_SECTIONTYPE.JUMP_START:
                            outline = Color.green;
                            break;
                        case E_SECTIONTYPE.JUMP_END:
                            outline = Color.green;
                            outline *= 0.5f;
                            break;
                        case E_SECTIONTYPE.JUNCTION_START:
                            outline = Color.red;
                            break;
                        case E_SECTIONTYPE.JUNCTION_END:
                            outline = Color.red;
                            outline *= 0.5f;
                            break;
                        case E_SECTIONTYPE.LOGIC_STARTLINE:
                            outline = Color.blue;
                            break;
                        case E_SECTIONTYPE.PITLANE_ENTRANCE:
                            outline = Color.yellow;
                            break;
                    }

                    outline *= 0.8f;

                    outline.a = 1.0f;
                    col.a = 1.0f;

                    Vector3 pos = tile.TILE_SECTION.SECTION_POSITION;
                    label.normal.textColor = col;
                    Handles.Label(pos, "Current = " + tile.TILE_SECTION.SECTION_TYPE.ToString(), label);

                    pos += tile.TILE_SECTION.SECTION_NORMAL * 0.4f;
                    label.normal.textColor = outline;
                    Handles.Label(pos, "Paint = " + currentSectionType.ToString(), label);

                    if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
                    {
                        // apply change
                        tile.TILE_SECTION.SECTION_TYPE = currentSectionType;

                        // cache change
                        thisTarget.DATA_FE.CACHE_SECTIONS[tile.TILE_SECTION.SECTION_INDEX] = currentSectionType;
                    }
                }
            }

        }
    }
}