using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BnG.Files
{
    /// <summary>
    /// Save/load vertex color maps.
    /// </summary>
    public class VCM
    {
        public static void Save(string trackName, VCMData[] toSave)
        {
            if (!Directory.Exists(Application.dataPath + "/Resources/BakedLighting/"))
                Directory.CreateDirectory(Application.dataPath + "/Resources/BakedLighting/");

            string path = Application.dataPath + "/Resources/BakedLighting/" + trackName + ".vcm";

            using (StreamWriter sr = new StreamWriter(path))
            {
                for (int i = 0; i < toSave.Length; i++)
                {
                    sr.WriteLine("ID " + toSave[i].ID);
                    for (int j = 0; j < toSave[i].colors.Count; j++)
                    {
                        uint c = Color2uInt(toSave[i].colors[j]);
                        sr.WriteLine("C " + c);
                    }
                }
            }

        }

        public static VCMData[] Load(string trackName)
        {
            string path = Application.dataPath + "/Resources/BakedLighting/" + trackName + ".vcm";

            if (!File.Exists(path))
            {
                Debug.Log("Didn't find " + path);
                return null;
            }
            List<VCMData> data = new List<VCMData>();
            int index = -1;

            using (StreamReader sr = new StreamReader(path))
            {
                // read new line
                string line = sr.ReadLine();
                char[] split = { ' ' };
                string[] lineArray;

                while (line != null)
                {
                    line.Trim();
                    if (line.StartsWith("ID "))
                    {
                        data.Add(new VCMData());
                        index++;

                        lineArray = line.Split(split, 50);
                        data[index].ID = System.Convert.ToInt32(lineArray[1]);
                    }

                    if (line.StartsWith("C "))
                    {
                        lineArray = line.Split(split, 50);
                        uint c = System.Convert.ToUInt32(lineArray[1]);
                        Color final = uInt2Color(c);
                        data[index].colors.Add(final);
                    }
                    line = sr.ReadLine();
                }
            }

            GameObject[] objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < objects.Length; j++)
                {
                    if (objects[j].GetComponent<VCMID>())
                    {
                        if (objects[j].GetComponent<VCMID>().ID == data[i].ID)
                        {
                            Mesh m = objects[j].GetComponent<MeshFilter>().sharedMesh;
                            Color32[] cols = new Color32[m.vertices.Length];
                            for (int a = 0; a < m.vertices.Length; a++)
                            {
                                cols[a] = data[i].colors[a];
                            }
                            m.colors32 = cols;
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
