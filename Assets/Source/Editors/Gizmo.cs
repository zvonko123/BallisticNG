using UnityEngine;
using BnG.Editors;
using System.Collections;

public class Gizmo : MonoBehaviour {

    public E_GIZMOAXIS gizmoAxis;
    public Color colorNormal;
    public Color colorSelected;

    bool movingSelected = false;
    private GameObject selected;
    private Vector3 origin;
    private Vector3 selectedOrigin;

    void Start()
    {
        // setup default color
        GetComponent<MeshRenderer>().material.SetColor("_Color", colorNormal);
    }

    void OnMouseDown()
    {
        // update gizmo color and selected
        GetComponent<MeshRenderer>().material.SetColor("_Color", colorSelected);
        selected = TrackEditorGlobal.currentSelected;
        movingSelected = true;

        // store origin point
        float distance = Camera.main.WorldToScreenPoint(selected.transform.position).z;
        origin = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));
        selectedOrigin = selected.transform.position;
    }

    void OnMouseUp()
    {
        // update gizmo color
        GetComponent<MeshRenderer>().material.SetColor("_Color", colorNormal);
        movingSelected = false;
    }

    void OnMouseDrag()
    {
        // get current world position from mouse
        float distance = Camera.main.WorldToScreenPoint(selected.transform.position).z;
        Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance));

        // get new position on all axis
        Vector3 tempPos = selected.transform.position;
        selected.transform.position = selectedOrigin + (pos - origin);

        // reposition based on axis constraints
        if (gizmoAxis == E_GIZMOAXIS.X)
            selected.transform.position = new Vector3(selected.transform.position.x, tempPos.y, tempPos.z);
        if (gizmoAxis == E_GIZMOAXIS.Y)
            selected.transform.position = new Vector3(tempPos.x, selected.transform.position.y, tempPos.z);
        if (gizmoAxis == E_GIZMOAXIS.Z)
            selected.transform.position = new Vector3(tempPos.x, tempPos.y, selected.transform.position.z);
        if (gizmoAxis == E_GIZMOAXIS.XZ)
            selected.transform.position = new Vector3(selected.transform.position.x, tempPos.y, selected.transform.position.z);
        if (gizmoAxis == E_GIZMOAXIS.XY)
            selected.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y, tempPos.z);
        if (gizmoAxis == E_GIZMOAXIS.YZ)
            selected.transform.position = new Vector3(tempPos.x, selected.transform.position.y, selected.transform.position.z);

    }
}

public enum E_GIZMOAXIS
{
    X,
    Y,
    Z,
    XZ,
    XY,
    YZ,
    ALL
}

public enum E_GIZMOTYPE
{
    MOVE,
    ROTATE,
    SCALE
}
