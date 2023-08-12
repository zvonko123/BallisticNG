using UnityEngine;
using System.Collections;
using BnG.Editors;

public class TrackEditorCamera {

    // camera position
    private static Vector3 curentFocus;
    private static Vector3 targetFocus;
    private static float targetDistance;

    private static Vector2 cameraRotation;
    private static Vector3 cameraPan;
    private static float cameraZoom = 10.0f;

    // camera references
    public static Camera cam;

    public static void InitCamera()
    {
        cam = Camera.main;
    }

    public static void UpdateFocus(Vector3 focus) { targetFocus = focus; cameraPan = Vector3.zero; cameraZoom = 1.0f; }

    public static void UpdateCamera()
    {
        GetCameraInput();
        UpdateTransform();
    }

    private static void GetCameraInput()
    {
        // move camera
        if (CameraControls.move.GetInput())
            cameraPan += cam.transform.TransformDirection(Input.GetAxis("Mouse X") * (targetDistance * 0.1f), Input.GetAxis("Mouse Y") * (targetDistance * 0.1f), 0.0f);

        // rotate camera
        if (CameraControls.rotation.GetInput())
            cameraRotation += new Vector2(Input.GetAxis("Mouse X") * (targetDistance * 2), -Input.GetAxis("Mouse Y") * (targetDistance * 2));

        // zoom camera
        if (CameraControls.zoom.GetInput())
            cameraZoom += Input.GetAxis("Mouse X") * targetDistance;

        // reset focus
        if (CameraControls.focus.GetInput())
        {
            if (TrackEditorGlobal.currentSelection.Length == 0)
            {
                UpdateFocus(Vector3.zero);
            } else
            {
                int i = 0;
                Vector3 selectionMid = Vector3.zero;
                for (i = 0; i < TrackEditorGlobal.currentSelection.Length; ++i)
                    selectionMid += TrackEditorGlobal.currentSelection[i].position;

                UpdateFocus(selectionMid / TrackEditorGlobal.currentSelection.Length);
            }

            
        }
        
    }

    private static void UpdateTransform()
    {
        // update focus
        curentFocus = Vector3.Lerp(curentFocus, targetFocus, Time.deltaTime * 5);

        // update distance to focus
        targetDistance = Vector3.Distance(cam.transform.position, curentFocus);
        targetDistance = Mathf.Clamp(targetDistance, 0.01f, 1.0f);

        // apply orbit pan
        Quaternion rot = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0.0f);
        Vector3 pos = rot * new Vector3(0.0f, 0.0f, -cameraZoom) + curentFocus + cameraPan;

        // update transform
        cam.transform.position = pos;
        cam.transform.rotation = rot;
    }
}
