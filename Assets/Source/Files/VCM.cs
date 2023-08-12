using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace BnG.Files
{
    /// <summary>
    /// Save/load vertex color maps.
    /// </summary>
    public class VCM
    {
        public static void Save(string trackName, VCMData[] toSave)
        {
            string path = Environment.CurrentDirectory + "/Lighting/" + trackName + ".vcm";

            if (!Directory.Exists(Environment.CurrentDirectory + "/Lighting/"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "/Lighting/");

            int i = 0;
            int j = 0;
            uint c;
            using (StreamWriter sr = new StreamWriter(path))
            {
                sr.WriteLine(toSave.Length);
                for (i = 0; i < toSave.Length; ++i)
                {
                    for (j = 0; j < toSave[i].colors.Count; ++j)
                    {
                        c = Color2uInt(toSave[i].colors[j]);
                        sr.WriteLine("C " + toSave[i].ID + " " + c);
                    }
                }
            }

        }

        public static VCMData[] Load(string trackName)
        {
            string path = Environment.CurrentDirectory + "/Lighting/" + trackName + ".vcm";

            if (!File.Exists(path))
            {
                Debug.Log("Didn't find " + path);
                return null;
            }
            List<VCMData> data = new List<VCMData>();

            using (StreamReader sr = new StreamReader(path))
            {
                char[] split = { ' ' };
                string[] lineArray;

                uint c;
                Color final;

                // read object count from first line
                string line = sr.ReadLine();
                int objectCount = System.Convert.ToInt32(line);
                for (int d = 0; d < objectCount; ++d)
                {
                    VCMData nd = new VCMData();
                    data.Add(nd);
                }

                int currentID;
                string line2 = "NA";

                while (line != null)
                {
                    if (line[0] == 'C')
                    {
                        // trim and split line
                        line.Trim();
                        lineArray = line.Split(split, 50);

                        // read ID from line
                        currentID = System.Convert.ToInt32(lineArray[1]);

                        // read uint and convert it to a color
                        c = System.Convert.ToUInt32(lineArray[2]);
                        final = uInt2Color(c);

                        // add new data to arrays
                        data[currentID].colors.Add(final);
                        data[currentID].ID = currentID;
                    }
                    line = sr.ReadLine();

                }
            }

            GameObject[] objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            int i = 0;
            int j = 0;
            int a = 0;
            Mesh m;
            Color32[] cols;

            for (i = 0; i < data.Count; ++i)
            {
                for (j = 0; j < objects.Length; ++j)
                {
                    if (objects[j].GetComponent<VCMID>())
                    {
                        if (objects[j].GetComponent<VCMID>().ID == data[i].ID)
                        {
                            m = objects[j].GetComponent<MeshFilter>().sharedMesh;
                            cols = new Color32[m.vertices.Length];
                            if (cols.Length == data[i].colors.Count)
                            {
                                for (a = 0; a < cols.Length; ++a)
                                {
                                    cols[a] = data[i].colors[a];
                                }
                                m.colors32 = cols;
                            }
                            else
                            {
                                Debug.Log("Array length mismatch on " + i.ToString());
                            }

                        }
                    }
                }
            }

            return data.ToArray();
        }

        public static Color32 uInt2Color(uint color)
        {
            byte r = ((byte)(color >> 24));
            byte g = ((byte)(color >> 16));
            byte b = ((byte)(color >> 8));

            return new Color32(r, g, b, 1);
        }

        public static uint Color2uInt(Color32 color)
        {
            return (uint)((color.r << 24 | color.g << 16 | color.b << 8 | color.a << 0));
        }
    }

    public class VCMData
    {
        public int ID;
        public List<Color32> colors = new List<Color32>();
    }
}