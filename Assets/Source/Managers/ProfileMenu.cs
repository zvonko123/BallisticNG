using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileMenu : MonoBehaviour {

    [Header("[ SETTINGS ]")]
    public string nextScene;
    public InputField fldName;

    void Start()
    {
        // load next scene if user already exists
        string name = ProfileManager.ReadLastFile(true);
        if (name != "")
            Application.LoadLevel(nextScene);
    }

    public void CreateProfile()
    {
        if (fldName.text != "")
        {
            // create the profile
            string name = ProfileManager.CreateProfile(fldName.text);
            Debug.Log("Created profile " + name + "!");

            // set the profile
            ProfileManager.SetProfile(fldName.text);

            // write last file
            ProfileManager.WriteLastFile();

            // load next scene
            Application.LoadLevel(nextScene);
        } else
        {
            fldName.text = "Please enter a name!";
        }

    }

    public void Quit()
    {
        GameSettings.QuitGame();
    }
}
