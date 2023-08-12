using UnityEngine;
using UnityEditor;
using BnG.TrackData;
using BnG.Helpers;
using System;
using System.Collections;

[Serializable]
[CustomEditor(typeof(TrSectionEditorMono))]
public class TrSectionEditor : Editor {

    [SerializeField]
    private static TrSectionEditorMono tClass;
    [SerializeField]
    private static TrData managerData;
    [SerializeField]
    private static bool isActive = false;
    [SerializeField]
    private static E_TRANSFORM transformMode;
    [SerializeField]
    private static TrSection selectedSection;
    private static bool settingNextSection;

    [MenuItem("BnG/Track Tools/Section Editor")]
    public static void Init()
    {
        // only create window if the manager data class is found
        managerData = GameObject.Find("MANAGER_DATA").GetComponent<TrData>();
        if (managerData == null)
        {
            isActive = false;
            return;
        }

        // isActive switch
        if (isActive)
        {
            if (tClass != null)
                DestroyImmediate(tClass.gameObject);

            isActive = false;
        }
        else if (!isActive)
        {
            isActive = true;
            tClass = new GameObject("SECTION EDITOR").AddComponent<TrSectionEditorMono>();
            tClass.hideFlags = HideFlags.HideInHierarchy;

            // select editor
            GameObject[] gm = new GameObject[1];
            gm[0] = tClass.gameObject;
            Selection.objects = gm;

        }

    }

    void OnSceneGUI()
    {
        if (!isActive)
            return;

        Inputs();
        UpdateSection();

        // do not allow selection
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        if (selectedSection != null)
        {
            selectedSection.SECTION_POSITION = Handles.PositionHandle(selectedSection.SECTION_POSITION, TrackDataHelper.SectionGetRotation(selectedSection));
        }
    }

    private static void Inputs()
    {
        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.W)
        {
            transformMode = E_TRANSFORM.MOVE;
        }

        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.E)
        {
            transformMode = E_TRANSFORM.ROTATE;
        }

        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Space)
        {
            settingNextSection = true;
        }

        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.S)
        {
            managerData.cacheSectionPosition = true;
        }
    }

    private static void UpdateSection()
    {
        if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.LeftControl)
        {
            HandleUtility.Repaint();

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1 << LayerMask.NameToLayer("TrackFloor")))
            {
                Mesh m = hit.transform.GetComponent<MeshFilter>().sharedMesh;
                TrTile tile = TrackDataHelper.TileFromTriangleIndex(hit.triangleIndex, E_TRACKMESH.FLOOR, managerData.TRACK_DATA);
                if (settingNextSection && selectedSection != null)
                {
                    selectedSection.SECTION_NEXT = tile.TILE_SECTION;
                    settingNextSection = false;
                } else
                {
                    selectedSection = tile.TILE_SECTION;
                }
                managerData.SECTION_CURRENT = selectedSection.SECTION_INDEX;
                managerData.SECTION_NEXT = selectedSection.SECTION_NEXT.SECTION_INDEX;

            }
        }
    }

    public enum E_TRANSFORM
    {
        MOVE    = 0,
        ROTATE  = 1
    }

}
