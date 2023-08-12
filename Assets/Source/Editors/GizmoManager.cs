using UnityEngine;
using BnG.Editors;
using System.Collections;

public class GizmoManager : MonoBehaviour {

    public GameObject GizmoTransform;
    public GameObject GizmoRotate;
    public GameObject GizmoScale;

    public void UpdateGizmosSize(Transform camera)
    {
        if (GizmoTransform != null)
        {
            float size = Camera.main.WorldToScreenPoint(GizmoTransform.transform.position).z / 2;
            GizmoTransform.transform.localScale = new Vector3(size, size, size);
        }
    }

    public void UpdateGizmoPosition(Transform target)
    {
        if (GizmoTransform != null)
        {
            GizmoTransform.transform.position = target.position;
        }
    }

    public void UpdateGizmoActive()
    {
        if (GizmoTransform != null)
            GizmoTransform.SetActive(TrackEditorGlobal.currentSelected != null);
    }
}
