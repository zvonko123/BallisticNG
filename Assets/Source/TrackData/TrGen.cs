using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BnG.TrackData
{
    public class TrGen
    {
        public static TrGenData GenerateTrack(Mesh trackFloor, Mesh trackWall, Transform floorT, Transform floorW)
        {
            if (trackFloor == null || trackWall == null)
            {
                Debug.LogError("TRGEN: track floor and/or track wall meshes not present!");
                return null;
            }

            TrGenData gen = new TrGenData();

            #region TEMPS

            // verts
            Vector3[] verts = trackFloor.vertices;
            Vector3[] verts2 = trackWall.vertices;

            // section data
            TrSection newSection = new TrSection();
            TrTile[] tiles = new TrTile[2];

            // mesh triangles
            int[] tris = trackFloor.triangles;
            int[] tris2 = trackWall.triangles;

            // tiles to be mapped to indicies
            TrTile[] mappedFloor = new TrTile[tris.Length];
            TrTile[] mappedWall = new TrTile[tris.Length];

            // mesh normals
            Vector3[] normals = trackFloor.normals;
            Vector3[] normals2 = trackWall.normals;

            // vertex positions
            Vector3 p1 = Vector3.zero;
            Vector3 p2 = Vector3.zero;
            Vector3 pMid = Vector3.zero;

            // vertex normals
            Vector3 n1 = Vector3.zero;
            Vector3 n2 = Vector3.zero;
            Vector3 nMid = Vector3.zero;

            #endregion

            // create floor tiles
            int index = 0;
            for (int i = 0; i < tris.Length - 3; i += 6)
            {
                // create new tile
                TrTile newTile = new TrTile();

                // add tile indicies
                newTile.TILE_INDICES.Add(tris[i + 0]);
                newTile.TILE_INDICES.Add(tris[i + 1]);
                newTile.TILE_INDICES.Add(tris[i + 2]);
                newTile.TILE_INDICES.Add(tris[i + 4]);

                // get mid position of all vertices
                p1 = floorT.TransformPoint(verts[tris[i]]);
                p2 = floorT.TransformPoint(verts[tris[i + 1]]);
                pMid = (p1 + p2) / 2;

                // set default tile settings
                newTile.TILE_TYPE = E_TILETYPE.FLOOR;
                newTile.TILE_COLOR = Color.white;
                newTile.TILE_POSITION = pMid;
                newTile.TILE_INDEX = index;

                // add tile to list
                gen.TILES_FLOOR.Add(newTile);

                mappedFloor[i + 0] = newTile;
                mappedFloor[i + 1] = newTile;
                mappedFloor[i + 2] = newTile;
                mappedFloor[i + 3] = newTile;

                index++;
            }

            // create wall tiles
            index = 0;
            for (int i = 0; i < tris2.Length - 3; i += 6)
            {
                // create new tile
                TrTile newTile = new TrTile();

                // add tile indicies
                newTile.TILE_INDICES.Add(tris2[i + 0]);
                newTile.TILE_INDICES.Add(tris2[i + 1]);
                newTile.TILE_INDICES.Add(tris2[i + 2]);
                newTile.TILE_INDICES.Add(tris2[i + 4]);

                // get mid position of all vertices
                p1 = floorT.TransformPoint(verts2[tris2[i]]);
                p2 = floorT.TransformPoint(verts2[tris2[i + 1]]);
                pMid = (p1 + p2) / 2;

                // set default tile settings
                newTile.TILE_TYPE = E_TILETYPE.FLOOR;
                newTile.TILE_COLOR = Color.white;
                newTile.TILE_POSITION = pMid;
                newTile.TILE_INDEX = 0;

                // add tile to list
                gen.TILES_WALL.Add(newTile);

                mappedWall[i + 0] = newTile;
                mappedWall[i + 1] = newTile;
                mappedWall[i + 2] = newTile;
                mappedWall[i + 4] = newTile;

                index++;
            }

            // create sections
            index = 0;
            for (int i = 0; i < gen.TILES_FLOOR.Count - 1; i += 2)
            {
                // setup section and get tiles
                newSection = new TrSection();

                tiles[0] = gen.TILES_FLOOR[i + 0];
                tiles[1] = gen.TILES_FLOOR[i + 1];

                // set section defaults
                newSection.SECTION_TYPE = E_SECTIONTYPE.NORMAL;
                newSection.SECTION_TILES = tiles;
                newSection.SECTION_INDEX = index;

                // set section position
                p1 = floorT.transform.TransformPoint(verts[tiles[0].TILE_INDICES[0]]);
                p2 = floorT.transform.TransformPoint(verts[tiles[1].TILE_INDICES[1]]);
                pMid = (p1 + p2) / 2;

                newSection.SECTION_POSITION = pMid;

                // set section normal
                n1 = floorT.transform.TransformDirection(normals[tiles[0].TILE_INDICES[0]]);
                n2 = floorT.transform.TransformDirection(normals[tiles[1].TILE_INDICES[1]]);
                nMid = (n1 + n2) / 2;

                newSection.SECTION_NORMAL = nMid;

                // add section
                gen.SECTIONS.Add(newSection);

                // add section to tiles
                tiles[0].TILE_SECTION = newSection;
                tiles[1].TILE_SECTION = newSection;

                index++;
            }

            // set next sections
            for (int i = 0; i < gen.SECTIONS.Count; i++)
            {
                if (i == gen.SECTIONS.Count - 1)
                    gen.SECTIONS[i].SECTION_NEXT = gen.SECTIONS[0];
                else
                    gen.SECTIONS[i].SECTION_NEXT = gen.SECTIONS[i + 1];
            }

            // add mapped tiles to gendata
            for (int i = 0; i < mappedFloor.Length; i++)
                gen.TILES_FLOOR_MAPPED.Add(mappedFloor[i]);

            return gen;
        }
    }

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

    public enum E_TRACKMESH
    {
        FLOOR   = 0,
        WALL    = 1       
    }
}
