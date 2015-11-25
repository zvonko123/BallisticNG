using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SaveData {

    /// <summary>
    /// Write a new file that contains save data.
    /// </summary>
    /// <param name="sav"></param>
    /// <param name="filename"></param>
    public static void WriteSav(SAVDAT sav, string filename)
    {
        // create/get path
        string path = GameSettings.GetDirectory() + string.Format("/{0}/", sav.saveLocation);
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string file = path + string.Format("{0}.SAV", filename);

        // write file
        using (StreamWriter sw = new StreamWriter(file))
        {
            int i = 0;
            for (i = 0; i < sav.lines.Length; ++i)
            {
                sw.WriteLine(sav.lines[i]);
            }
        }
    }

    /// <summary>
    /// Read a file that contains save data.
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="filename"></param>
    /// <returns></returns>
    public static SAVDAT ReadSav(E_SAVELOCATIONS dir, string filename)
    {
        // get path
        string path = GameSettings.GetDirectory() + string.Format("/{0}/", dir) + string.Format("{0}.SAV", filename);

        // if the file doesn't exist then return and throw an error
        if (!File.Exists(path))
        {
            Debug.LogError("Couldn't load file " + path + "!");
            SAVDAT noSav = new SAVDAT();
            noSav.exists = false;
            return noSav;
        }

        // read file
        SAVDAT newSAV = new SAVDAT();
        using (StreamReader sr = new StreamReader(path))
        {
            List<string> readStrings = new List<string>();

            // read file
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                readStrings.Add(line);
            }

            // convert strings to array
            newSAV.lines = readStrings.ToArray();
        }
        newSAV.exists = true;

        return newSAV;
    }

    /// <summary>
    /// Sava ghost information to a SAV file.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="speed"></param>
    /// <param name="data"></param>
    public static void SaveGhost(string trackName, E_SPEEDCLASS speed,  GhostData data)
    {
        List<string> positions = new List<string>();
        List<string> rotations = new List<string>();
        List<string> lines = new List<string>();

        // populate data to write
        int i = 0;
        for (i = 0; i < data.positions.Length; ++i)
            positions.Add(string.Format("{0} {1} {2}", data.positions[i].x, data.positions[i].y, data.positions[i].z));

        for (i = 0; i < data.rotations.Length; ++i)
            rotations.Add(string.Format("{0} {1} {2} {4}", data.rotations[i].x, data.rotations[i].y, data.rotations[i].z, data.rotations[i].w));

        // pack into single string array
        lines.Add(((int)data.ship).ToString());
        for (i = 0; i < positions.Count; ++i)
            lines.Add(positions[i]);
        for (i = 0; i < rotations.Count;  ++i)
            lines.Add(rotations[i]);

        // create new save data and write
        SAVDAT sav = new SAVDAT();
        sav.lines = lines.ToArray();
        sav.saveLocation = E_SAVELOCATIONS.GHOSTS;

        WriteSav(sav, trackName + "_" + speed);
    }

    /// <summary>
    /// Load ghost information from a SAV file.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static GhostData LoadGhost(string trackName, E_SPEEDCLASS speed)
    {
        // create new ghost data and load file
        GhostData ghost = new GhostData();
        SAVDAT data = ReadSav(E_SAVELOCATIONS.GHOSTS, trackName + "_" + speed);

        // get ship from first line
        if (data.exists)
        {
            ghost.ship = (E_SHIPS)System.Convert.ToInt32(data.lines[0]);

            // read positions and rotations
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();
            int i = 0;
            int alt = 0;
            for (i = 1; i < data.lines.Length; ++i)
            {
                // get current line and remove whitespace
                string line = data.lines[i];
                line.Trim();

                // split it
                char[] split = { ' ' };
                string[] lineArray = line.Split(split, 50);

                // load positions/rotations
                if (alt == 0)
                    positions.Add(new Vector3(System.Convert.ToInt32(lineArray[0]), System.Convert.ToInt32(lineArray[1]), System.Convert.ToInt32(lineArray[2])));
                else if (alt == 1)
                    rotations.Add(new Quaternion(float.Parse(lineArray[0]), float.Parse(lineArray[1]), float.Parse(lineArray[2]), float.Parse(lineArray[3])));

                // increase alternate
                alt++;
                if (alt > 1)
                    alt = 0;
            }

            // convert to ghost data arrays
            ghost.positions = positions.ToArray();
            ghost.rotations = rotations.ToArray();
        }

        ghost.dataFound = data.exists;

        return ghost;
    }

    /// <summary>
    /// Write a time record to a SAV file.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="speed"></param>
    /// <param name="time"></param>
    public static void WriteTime(string trackName, E_SPEEDCLASS speed, float time)
    {
        SAVDAT data = new SAVDAT();
        data.saveLocation = E_SAVELOCATIONS.TIMES;
        data.lines = new string[1] { time.ToString() };

        WriteSav(data, trackName + "_" + speed);
    }

    /// <summary>
    /// Read a time record from a SAV file.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static TimeData ReadTime(string trackName, E_SPEEDCLASS speed)
    {
        // read file
        SAVDAT data = ReadSav(E_SAVELOCATIONS.TIMES, trackName + "_" + speed);

        // return time
        TimeData td = new TimeData();
        if (data.exists)
            td.time = float.Parse(data.lines[0]);

        td.dataFound = data.exists;

        return td;
    }
}

public class SAVDAT
{
    public string[] lines;
    public E_SAVELOCATIONS saveLocation;
    public bool exists;
}

public class GhostData
{
    public E_SHIPS ship;
    public Vector3[] positions;
    public Quaternion[] rotations;
    public bool dataFound;
}

public class TimeData
{
    public float time;
    public bool dataFound;
}
