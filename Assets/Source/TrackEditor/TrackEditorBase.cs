using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using BnG.Editors;

public class TrackEditorBase : MonoBehaviour {

    // ui elements
    [Header(" [ UI REFERENCES ]")]
    public RectTransform selectMarquee;

    private EditorTrackData trackData = new EditorTrackData();

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
        NodeEditing();
    }

    private void SelectObjects()
    {

    }

    private void HoverSelection()
    {

    }

    private void NodeEditing()
    {
        if (Input.GetKeyDown(KeyCode.A))
            CreateNewNode(Vector3.zero, 0.0f);
    }

    private void UpdateNodeSpheres()
    {
        // delete and clear current nodes
        for (int i = 0; i < trackData.nodeSpheres.Count; ++i)
            Destroy(trackData.nodeSpheres[i]);

        trackData.nodeSpheres.Clear();

        // create new nodes
        for (int i = 0; i < trackData.points.Count; ++i)
        {
            GameObject newSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newSphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
    }

    private void CreateNewNode(Vector3 positon, float roll)
    {
        trackData.points.Add(positon);
        trackData.rolls.Add(roll);
        UpdateNodeSpheres();
    }
}

public class EditorTrackData
{
    // location data
    public List<Vector3> points = new List<Vector3>();
    public List<float> rolls = new List<float>();

    // visual data
    public List<GameObject> nodeSpheres = new List<GameObject>();
}
