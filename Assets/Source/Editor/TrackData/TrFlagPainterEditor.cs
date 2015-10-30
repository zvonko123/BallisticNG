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
                            currentSectionType = E_SECTIONTYPE.JUNCTION_END;
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

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1 << LayerMask.NameToLayer("TrackFloor")))
            {
                TrTile tile = TrackDataHelper.TileFromTriangleIndex(hit.triangleIndex, E_TRACKMESH.FLOOR, thisTarget.TRACK_DATA);

                if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.TILE)
                {
                    Handles.Label(tile.TILE_POSITION, tile.TILE_TYPE.ToString());

                    if (Event.current.type == EventType.mouseUp && Event.current.button == 0)
                    {
                        // apply change
                        tile.TILE_TYPE = currentTileType;

                        // cache change
                        thisTarget.DATA_FE.CACHE_TILES[tile.TILE_INDEX] = currentTileType;
                    }
                }
                else if (thisTarget.DATA_FE.PAINT_MODE == E_PAINTMODE.SECTION)
                {
                    Handles.Label(tile.TILE_SECTION.SECTION_POSITION, tile.TILE_SECTION.SECTION_TYPE.ToString());

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