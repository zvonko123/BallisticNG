using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using BnG.Editors;

public class TrackEditorBase : MonoBehaviour {

    // ui elements
    [Header(" [ UI REFERENCES ]")]
    public RectTransform selectMarquee;


    private Vector2 marqueeStart;

    public void Start()
    {
        // init classes
        TrackEditorCamera.InitCamera();
    }

    public void Update()
    {
        // update classes
        TrackEditorCamera.UpdateCamera();

        SelectObjects();
    }

    private void SelectObjects()
    {

    }
}
