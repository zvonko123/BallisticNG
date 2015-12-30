using UnityEngine;
using BnG.Editors;
using System.Collections;
using System.Collections.Generic;

public class ShipEditorManager : MonoBehaviour {

    [Header("[ REFERENCES ]")]
    public GameObject cam;
    public GizmoManager gizmoManager;

    [Header("[ PREFABS ]")]
    public ShipSettings shipPrefab;
    public GameObject GizmoTransform;
    public GameObject GizmoRotate;
    public GameObject GizmoScale;

    public GameObject currentSelected;
    public MeshFilter shipMeshFilter;

    private List<MeshCollider> raycastColliders = new List<MeshCollider>();


    void Start()
    {
        // setup gizmos
        if (GizmoTransform != null)
            gizmoManager.GizmoTransform = Instantiate(GizmoTransform) as GameObject;

        // setup camera manager
        cam.GetComponent<ShipEditorCamera>().editorManager = this;

        // create colliders for raycasting
        foreach(Transform t in shipPrefab.transform)
        {
            if (t.gameObject.GetComponent<MeshFilter>())
                raycastColliders.Add(t.gameObject.AddComponent<MeshCollider>());
        }
    }

    void Update()
    {
        // update gizmos
        gizmoManager.UpdateGizmosSize(cam.transform);

        if (currentSelected != null)
            gizmoManager.UpdateGizmoPosition(currentSelected.transform);

        gizmoManager.UpdateGizmoActive();

        // update current selected
        TrackEditorGlobal.currentSelected = currentSelected;
    }
}
