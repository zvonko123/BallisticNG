using UnityEngine;
using BnG.TrackData;
using System.Collections.Generic;

public class TrGenData
{
    // tiles
    public List<TrTile> TILES_FLOOR = new List<TrTile>();
    public List<TrTile> TILES_FLOOR_MAPPED = new List<TrTile>();
    public List<TrTile> TILES_WALL = new List<TrTile>();
    public List<TrTile> TILES_WALL_MAPPED = new List<TrTile>();

    // sections
    public List<TrSection> SECTIONS = new List<TrSection>();
}
