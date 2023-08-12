using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class ProfileManager : MonoBehaviour {

    /// <summary>
    /// Returns the profile directory for a given user.
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public static string GetProfileDirectory(string name)
    {
        return GameSettings.GetDirectory() + "Profiles/" + name;
    }

    public static string GetProfileDirectory()
    {
        return GameSettings.GetDirectory() + "Profiles/" + GameSettings.profileName;
    }

    /// <summary>
    /// Finds all created profiles.
    /// </summary>
    /// <returns></returns>
    public static string[] FindProfiles()
    {
        return Directory.GetDirectories(GameSettings.GetDirectory() + "Profiles/");
    }

    /// <summary>
    /// Creates a new profile directory.
    /// </summary>
    /// <returns></returns>
    public static string CreateProfile(string name)
    {
        // create the profile folder
        string profile = GameSettings.GetDirectory() + "/Profiles/" + name;
        if (!Directory.Exists(profile))
            Directory.CreateDirectory(profile);

        return profile;
    }

    /// <summary>
    /// Set the current profile name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static void SetProfile(string name)
    {
        GameSettings.profileName = name;
    }

    /// <summary>
    /// Update the last user.
    /// </summary>
    public static void WriteLastFile()
    {
        using (StreamWriter sw = new StreamWriter(GameSettings.GetDirectory() + "LastUser.txt"))
            sw.WriteLine(GameSettings.profileName);
    }

    public static string ReadLastFile(bool setUser)
    {
        if (!File.Exists(GameSettings.GetDirectory() + "LastUser.txt"))
            return "";

        string user = "";
        using (StreamReader sr = new StreamReader(GameSettings.GetDirectory() + "LastUser.txt"))
            user = sr.ReadLine();

        if (setUser && user != "")
            SetProfile(user);
        return user;
    }
}
