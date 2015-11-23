using UnityEngine;
using UnityEditor;
using BnG.Files;
using System.Collections;
using System.Collections.Generic;

public class Lightmapper : EditorWindow {

    // reference to lightmap settings
    public static LMSettings lms;

    // arrays
    private static List<MeshCollider> sceneObjects = new List<MeshCollider>();
    private static List<Transform> sceneTransforms = new List<Transform>();
    private static List<Transform> trackTransforms = new List<Transform>();
    private static List<Mesh> sceneMeshes = new List<Mesh>();
    private static List<Mesh> trackMeshes = new List<Mesh>();
    private static List<VCMData> vcmData = new List<VCMData>();
    private static List<Light> dLights = new List<Light>();
    private static List<Light> pLights = new List<Light>();

    private static List<GameObject> trackGO = new List<GameObject>();
    private static List<GameObject> sceneGO = new List<GameObject>();

    [MenuItem("BnG/Track Tools/Lightmapper")]
    public static void InitWindow()
    {
        // create new window
        EditorWindow thisWindow = EditorWindow.GetWindow(typeof(Lightmapper));
        thisWindow.position = new Rect(Screen.width / 2, Screen.height / 2, 400, 100);
        thisWindow.minSize = new Vector2(400, 100);
        thisWindow.maxSize = new Vector2(400, 100);
        thisWindow.titleContent = new GUIContent("Lightmapper");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Bake"))
        {
            ProcessLightmap();
        }
    }

    public static void ProcessLightmap()
    {
        CheckForSettings();
        GatherObjects();
        MapLights();
        SaveMap();
        Cleanup();

        // remove progress bar
        EditorUtility.ClearProgressBar();
    }

    public static void CheckForSettings()
    {
        // check for/create lightmap settings
        if (GameObject.Find("LIGHTMAP_SETTINGS").GetComponent<LMSettings>())
        {
            lms = GameObject.Find("LIGHTMAP_SETTINGS").GetComponent<LMSettings>();
        }
        else
        {
            GameObject newLMS = new GameObject("LIGHTMAP_SETTINGS");
            lms = newLMS.AddComponent<LMSettings>();
        }
    }

    public static void GatherObjects()
    {
        // clear lists
        sceneObjects.Clear();
        sceneMeshes.Clear();
        trackMeshes.Clear();
        sceneTransforms.Clear();
        trackTransforms.Clear();
        dLights.Clear();
        pLights.Clear();

        // grab copy of all objects in the scene
        GameObject[] allObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];

        // temp vars
        int i = 0;
        int l1 = LayerMask.NameToLayer("TrackFloor");
        int l2 = LayerMask.NameToLayer("TrackWall");

        // iterate through all objectgs
        for (i = 0; i < allObjects.Length; ++i)
        {
            if (allObjects[i].layer != l1 && allObjects[i].layer != l2)
            {
                // found scene mesh
                if (allObjects[i].GetComponent<MeshFilter>())
                {
                    if (allObjects[i].transform.root == lms.sceneParent.transform)
                    {
                        sceneMeshes.Add(allObjects[i].GetComponent<MeshFilter>().sharedMesh);
                        sceneTransforms.Add(allObjects[i].transform);
                        sceneGO.Add(allObjects[i]);

                        // if there isn't already a collider then create one and cache it to remove it later
                        if (!allObjects[i].GetComponent<MeshCollider>())
                        {
                            MeshCollider mc = allObjects[i].AddComponent<MeshCollider>();
                            sceneObjects.Add(mc);
                        }
                    }
                }
            } else
            {
                // found track mesh
                if (allObjects[i].GetComponent<MeshFilter>())
                {
                    trackMeshes.Add(allObjects[i].GetComponent<MeshFilter>().sharedMesh);
                    trackTransforms.Add(allObjects[i].transform);
                    trackGO.Add(allObjects[i]);
                }
            }

            // lights
            if (allObjects[i].GetComponent<Light>())
            {
                // directional
                if (allObjects[i].GetComponent<Light>().type == LightType.Directional)
                    dLights.Add(allObjects[i].GetComponent<Light>());

                // spot
                if (allObjects[i].GetComponent<Light>().type == LightType.Point)
                    pLights.Add(allObjects[i].GetComponent<Light>());
            }

            // progress report
            EditorUtility.DisplayProgressBar("Lightmapping", string.Format("Gathering Objects {0} / {1}", i, allObjects.Length - 1), i / (allObjects.Length - 1));
        }
        
    }

    public static void MapLights()
    {
        // temp vars
        int i = 0;
        int j = 0;
        int k = 0;
        Color32[] cols;
        float dot;
        Color dotColor;
        Vector3 vPos;
        Vector3 lPos;
        float vtol;
        Mesh m;
        Vector3[] verts;
        Vector3[] normals;
        int[] tris;
        int vcmID = 0;

        // scene meshes
        for (i = 0; i < sceneMeshes.Count; ++i)
        {
            // create new colors array
            cols = new Color32[sceneMeshes[i].vertices.Length];
            m = sceneMeshes[i];
            verts = m.vertices;
            normals = m.normals;

            // lighting
            for (j = 0; j < cols.Length; ++j)
            {
                // apply ambient
                cols[j] = lms.ambientLight;

                // apply directional light
                for (k = 0; k < dLights.Count; ++k)
                {
                    if (dLights[k].shadows != LightShadows.None)
                    {
                        vPos = sceneTransforms[i].TransformPoint(verts[j]);

                        if (!Physics.Raycast(vPos, Vector3.up))
                        {
                            dot = Vector3.Dot(normals[j], dLights[k].transform.up);
                            dot = Mathf.Clamp(dot, 0.0f, 1.0f);
                            dotColor.r = dot;
                            dotColor.g = dot;
                            dotColor.b = dot;
                            dotColor.a = 1.0f;

                            cols[j] += dotColor * dLights[k].color * dLights[k].intensity;
                        }
                    } else
                    {
                        dot = Vector3.Dot(normals[j], dLights[k].transform.up);
                        dot = Mathf.Clamp(dot, 0.0f, 1.0f);
                        dotColor.r = dot;
                        dotColor.g = dot;
                        dotColor.b = dot;
                        dotColor.a = 1.0f;

                        cols[j] += dotColor * dLights[k].color * dLights[k].intensity;
                    }
                }

                // apply point lights
                for (k = 0; k < pLights.Count; ++k)
                {
                    vPos = sceneTransforms[i].TransformPoint(verts[j]);
                    lPos = pLights[k].transform.position;

                    vtol = Vector3.Distance(vPos, lPos);

                    if (pLights[k].shadows != LightShadows.None)
                    {
                        cols[j] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                    } else
                    {
                        if (!Physics.Linecast(vPos, lPos))
                            cols[j] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                    }
                }
            }

            // apply colors
            sceneMeshes[i].colors32 = cols;

            // progress report
            EditorUtility.DisplayProgressBar("Lightmapping", string.Format("Applying Scene Lighting {0} / {1}", i, sceneMeshes.Count), i / (sceneMeshes.Count));

            // VCM data
            vcmData.Add(new VCMData());
            if (!sceneGO[i].GetComponent<VCMID>())
                sceneGO[i].AddComponent<VCMID>();

            sceneGO[i].GetComponent<VCMID>().ID = vcmID;
            vcmData[i].ID = vcmID;
            for (j = 0; j < cols.Length; ++j)
            {
                vcmData[i].colors.Add(cols[j]);
            }

            vcmID++;
        }

        // temp vars
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;

        // track meshes
        for (i = 0; i < trackMeshes.Count; ++i)
        {
            // create new colors array
            cols = new Color32[trackMeshes[i].vertices.Length];
            m = trackMeshes[i];
            verts = m.vertices;
            tris = m.triangles;
            normals = m.normals;

            // lighting
            for (j = 0; j < tris.Length; j += 3)
            {
                // apply ambient
                cols[j] = lms.ambientLight;
                cols[j + 1] = lms.ambientLight;
                cols[j + 2] = lms.ambientLight;

                // apply directional lights
                for (k = 0; k < dLights.Count; ++k)
                {
                    p1 = trackTransforms[i].TransformPoint(verts[tris[j]]);
                    p2 = trackTransforms[i].TransformPoint(verts[tris[j + 1]]);
                    p3 = trackTransforms[i].TransformPoint(verts[tris[j + 2]]);
                    vPos = (p1 + p2 + p3) / 3;

                    if (dLights[k].shadows != LightShadows.None)
                    {
                        if (!Physics.Raycast(vPos, Vector3.up))
                        {
                            dot = Vector3.Dot(normals[tris[j]], dLights[k].transform.up);
                            dot = Mathf.Clamp(dot, 0.0f, 1.0f);
                            dotColor.r = dot;
                            dotColor.g = dot;
                            dotColor.b = dot;
                            dotColor.a = 1.0f;

                            cols[tris[j]] += dotColor * dLights[k].color * dLights[k].intensity;
                            cols[tris[j + 1]] += dotColor * dLights[k].color * dLights[k].intensity;
                            cols[tris[j + 2]] += dotColor * dLights[k].color * dLights[k].intensity;
                        }
                    } else
                    {
                        dot = Vector3.Dot(normals[tris[j]], dLights[k].transform.up);
                        dot = Mathf.Clamp(dot, 0.0f, 1.0f);
                        dotColor.r = dot;
                        dotColor.g = dot;
                        dotColor.b = dot;
                        dotColor.a = 1.0f;

                        cols[tris[j]] += dotColor * dLights[k].color * dLights[k].intensity;
                        cols[tris[j + 1]] += dotColor * dLights[k].color * dLights[k].intensity;
                        cols[tris[j + 2]] += dotColor * dLights[k].color * dLights[k].intensity;
                    }
                }

                // apply point lights
                for (k = 0; k < pLights.Count; ++k)
                {
                    p1 = trackTransforms[i].TransformPoint(verts[tris[j]]);
                    p2 = trackTransforms[i].TransformPoint(verts[tris[j + 1]]);
                    p3 = trackTransforms[i].TransformPoint(verts[tris[j + 2]]);
                    vPos = (p1 + p2 + p3) / 3;

                    lPos = pLights[k].transform.position;

                    vtol = Vector3.Distance(vPos, lPos);

                    if (pLights[k].shadows != LightShadows.None)
                    {
                        cols[j] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                        cols[j + 1] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                        cols[j + 2] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                    }
                    else
                    {
                        if (!Physics.Linecast(vPos, lPos))
                        {
                            cols[j] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                            cols[j + 1] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                            cols[j + 2] += CalculateAtten(lPos, vPos, pLights[k].range) * pLights[k].color * pLights[k].intensity;
                        }
                    }
                }

            }

            // apply colors
            trackMeshes[i].colors32 = cols;

            // progress report
            EditorUtility.DisplayProgressBar("Lightmapping", string.Format("Applying Track Ambient {0} / {1}", i, trackMeshes.Count - 1), i / (trackMeshes.Count - 1));

            // VCM data
            vcmData.Add(new VCMData());
            if (!trackGO[i].GetComponent<VCMID>())
                trackGO[i].AddComponent<VCMID>();

            trackGO[i].GetComponent<VCMID>().ID = vcmID;
            vcmData[i].ID = vcmID;
            for (j = 0; j < cols.Length; ++j)
            {
                vcmData[i].colors.Add(cols[j]);
            }

            vcmID++;
        }
    }

    public static void SaveMap()
    {
        VCM.Save(lms.lightmapName, vcmData.ToArray());
    }

    public static void Cleanup()
    {
        // remove colliders
        int i = 0;
        for (i = 0; i < sceneObjects.Count; ++i)
            DestroyImmediate(sceneObjects[i]);
    }

    private static Color CalculateAtten(Vector3 to, Vector3 from, float range)
    {
        float dist = Vector3.Distance(to, from) / range;
        dist *= dist;

        float atten = 1.0f / (1.0f + 25.0f * dist);
        return new Color(atten, atten, atten, 1.0f);
    }
}
