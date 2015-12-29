using UnityEngine;
using BnG.Editors;
using System.Collections;

public class ShipEditorCamera : MonoBehaviour {

    private Vector2 orbitRotation;
    private float orbitZoom;
    private Vector3 target;
    public ShipEditorManager editorManager;

    void Update()
    {
        orbitZoom -= Input.GetAxis("Mouse ScrollWheel") * 5;
        orbitZoom = Mathf.Clamp(orbitZoom, 0.5f, 10.0f);

        if (Input.GetMouseButton(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            orbitRotation += new Vector2(Input.GetAxis("Mouse Y") * 4, -Input.GetAxis("Mouse X") * 4);
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // update target
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (TrackEditorGlobal.currentSelected != null)
                target = TrackEditorGlobal.currentSelected.transform.position;
            else
                target = Vector3.zero;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit))
            {
                    editorManager.currentSelected = null;
            }
            else
            {
                if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Gizmo"))
                    editorManager.currentSelected = hit.transform.gameObject;
            }

        }

        Quaternion rot = Quaternion.Euler(orbitRotation.x, orbitRotation.y, 0.0f);
        Vector3 pos = rot * new Vector3(0.0f, 0.0f, -orbitZoom) + target;

        transform.position = pos;
        transform.rotation = rot;
    }
}
