using UnityEngine;
using BnG.Files;
using BnG.TrackData;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bakes colors into vertices.
/// </summary>

[ExecuteInEditMode]
public class VertexLightMapper : MonoBehaviour {

    public TrData trackData;
    public GameObject sceneParent;
    private List<GameObject> foundObjects = new List<GameObject>();
    private List<GameObject> trackObjects = new List<GameObject>();
    private List<GameObject> sceneObjects = new List<GameObject>();
    [Header("[ LIGHT SETTINGS ] ")]
    public Color ambientLight;
    public Light directionalLight;
    public float shadowDistance;
    public bool reconstructMeshes;
    public bool resetColors;

    public bool bakeMap;
    public string lightmapName;
    public bool foundMap;

    public string progress;
    
    void Start()
    {
        CheckForExistingMap();
        trackData.setTileColors = true;
    }

    void Update()
    {

        if (!bakeMap)
        {
            return;
        }
        else
        {
            bakeMap = false;
        }

        FindObjects();
        LightmapPass();
        RemoveMeshColliders();
        trackData.setTileColors = true;
    }

    private void CheckForExistingMap()
    {
        string path = Application.dataPath + "/Resources/BakedLighting/" + lightmapName + ".vcm";
        if (System.IO.File.Exists(path))
        {
            VCM.Load(lightmapName);
            foundMap = true;
        }
        else
        {
            Debug.Log("Couldn't find " + path + "!");
        }

    }

    private void FindObjects()
    {
        // clear previous lists
        foundObjects.Clear();
        trackObjects.Clear();
        sceneObjects.Clear();

        int foundMeshes = 0;
        foreach(Transform go in sceneParent.transform)
        {
            if (go.gameObject.layer != LayerMask.NameToLayer("TrackFloor") && 
                go.gameObject.layer != LayerMask.NameToLayer("TrackWall"))
            {
                // add object to array
                if (go.GetComponent<MeshFilter>())
                {
                    sceneObjects.Add(go.gameObject);
                    // attach mesh collider (for raycasting)
                    if (!go.gameObject.GetComponent<MeshCollider>())
                    {
                        //go.gameObject.AddComponent<MeshCollider>();
                        //foundObjects.Add(go.gameObject);
                    }

                    if (reconstructMeshes)
                    {
                        Mesh m = go.GetComponent<MeshFilter>().sharedMesh;
                        // re-build mesh so all tris have unique verts
                        Vector3[] newVertices = new Vector3[m.triangles.Length];
                        Vector2[] newUV = new Vector2[m.triangles.Length];
                        Vector3[] newNormals = new Vector3[m.triangles.Length];
                        int[] newTriangles = new int[m.triangles.Length];

                        int triCount = m.triangles.Length;
                        for (int i = 0; i < triCount; ++i)
                        {
                            newVertices[i] = m.vertices[m.triangles[i]];
                            newUV[i] = m.uv[m.triangles[i]];
                            newNormals[i] = m.normals[m.triangles[i]];
                            newTriangles[i] = i;
                        }
                        m.vertices = newVertices;
                        m.uv = newUV;
                        m.normals = newNormals;
                        m.triangles = newTriangles;
                    }

                    foundMeshes++;
                }

            } else
            {
                if (go.GetComponent<MeshFilter>())
                {
                    trackObjects.Add(go.gameObject);
                    foundMeshes++;
                }
            }
        }

        Debug.Log(string.Format("Found {0} meshes!", foundMeshes.ToString()));
    }

    public void LightmapPass()
    {

        progress = "Working";
        // find all lights
        Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];
        AntiLight[] nLights = FindObjectsOfType(typeof(AntiLight)) as AntiLight[];

        #region TEMP VARS

        Color32 newColor;
        Color32 dotColor;
        Vector3 vPos;
        Vector3 toLight;
        float distToL;
        List<VCMData> data = new List<VCMData>();
        int i = 0;
        int j = 0;
        int k = 0;
        int al = 0;
        int e = 0;
        bool excluded = false;
        Mesh m;
        Color32[] newColors;
        float dot;
        Vector3 p1;
        Vector3 p2;
        Vector3 p3;
        int triCount;
        int lightCount = lights.Length;
        int objectCount = trackObjects.Count;

        bool hasDirectionLight = directionalLight != null;
        bool dirNoShadows = directionalLight.shadows == LightShadows.None;

        #endregion

        #region TRACK OBJECTS
        int totalIndex = 0;
        // go through each objects
        for (i = 0; i < objectCount; ++i)
        {
            totalIndex++;
            m = trackObjects[i].GetComponent<MeshFilter>().sharedMesh;
            newColors = new Color32[m.vertices.Length];
            triCount = m.triangles.Length;

            // go through each triangle
            if (resetColors)
            {
                for (j = 0; j < triCount; j += 3)
                {
                    newColors[m.triangles[j]] = Color.white;
                    newColors[m.triangles[j + 1]] = Color.white;
                    newColors[m.triangles[j + 2]] = Color.white;
                }
            }
            else
            {
                for (j = 0; j < triCount; j += 3)
                {
                    newColor = ambientLight;
                    if (hasDirectionLight)
                    {
                        if (dirNoShadows)
                        {
                            dot = Vector3.Dot(m.normals[m.triangles[j]], directionalLight.transform.up);
                            dotColor = new Color(dot, dot, dot, 1.0f);

                            newColor += dotColor * directionalLight.color * directionalLight.intensity;
                        }
                        else
                        {
                            p1 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                            p2 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 1]]);
                            p3 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 2]]);
                            vPos = (p1 + p2 + p3) / 3;

                            if (!Physics.Raycast(vPos, Vector3.up, shadowDistance))
                            {
                                dot = Vector3.Dot(m.normals[m.triangles[j]], directionalLight.transform.up);
                                dotColor = new Color(dot, dot, dot, 1.0f);

                                newColor += dotColor * directionalLight.color * directionalLight.intensity;
                            }
                        }
                    }

                    // calculate attenuation for each point light in the scene
                    for (k = 0; k < lightCount; ++k)
                    {
                        if (lights[k].type == LightType.Point)
                        {
                            p1 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                            p2 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 1]]);
                            p3 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 2]]);
                            vPos = (p1 + p2 + p3) / 3;
                            toLight = lights[k].transform.position;

                            distToL = Vector3.Distance(vPos, toLight);

                            if (distToL < lights[k].range * 1.2f)
                            {
                                if (lights[k].shadows == LightShadows.None)
                                {
                                    newColor += CalculateAtten(toLight, vPos, lights[k].range) * lights[k].color * (lights[k].intensity);
                                }
                                else if (lights[k].shadows != LightShadows.None)
                                {
                                    if (!Physics.Linecast(vPos, toLight))
                                        newColor += CalculateAtten(toLight, vPos, lights[k].range) * lights[k].color * (lights[k].intensity);
                                }
                            }
                        }
                    }

                    // anti-lights
                    for (al = 0; al < nLights.Length; ++al)
                    {
                        excluded = false;
                        for (e = 0; e < nLights[al].exclusions.Length; e++)
                        {
                            if (trackObjects[i] == nLights[al].exclusions[e])
                                excluded = true;
                        }

                        if (!excluded)
                        {
                            p1 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                            p2 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 1]]);
                            p3 = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j + 2]]);
                            vPos = (p1 + p2 + p3) / 3;
                            toLight = nLights[al].transform.position;

                            distToL = Vector3.Distance(vPos, toLight);

                            if (nLights[al].isArea)
                            {
                                if (Mathf.Abs(vPos.x - nLights[al].transform.position.x) < nLights[al].area.x &&
                                    Mathf.Abs(vPos.y - nLights[al].transform.position.y) < nLights[al].area.y &&
                                    Mathf.Abs(vPos.z - nLights[al].transform.position.z) < nLights[al].area.z)
                                {
                                    if (nLights[al].allowAmbient)
                                    {
                                        newColor = ambientLight;
                                    }
                                    else
                                    {
                                        newColor = Color.black;
                                    }
                                }
                            }
                            else
                            {
                                if (distToL < nLights[al].range)
                                {
                                    if (nLights[al].allowAmbient)
                                    {
                                        newColor = ambientLight;
                                    }
                                    else
                                    {
                                        newColor = Color.black;
                                    }
                                }
                            }
                        }
                    }

                    // set vert colors
                    newColors[m.triangles[j]] = newColor;
                    newColors[m.triangles[j + 1]] = newColor;
                    newColors[m.triangles[j + 2]] = newColor;
                }
            }

            m.colors32 = newColors;

            data.Add(new VCMData());
            if (trackObjects[i].GetComponent<VCMID>())
                    DestroyImmediate(trackObjects[i].GetComponent<VCMID>());

            trackObjects[i].AddComponent<VCMID>();
            trackObjects[i].GetComponent<VCMID>().ID = i;
            data[i].ID = i;
            for (j = 0; j < newColors.Length; j++)
            {
                data[i].colors.Add(newColors[j]);
            }

        }
        #endregion

        #region TRACK OBJECTS
        objectCount = sceneObjects.Count;
        int vertCount = 0;
        // go through each objects
        for (i = 0; i < objectCount; ++i)
        {
            m = sceneObjects[i].GetComponent<MeshFilter>().sharedMesh;
            newColors = new Color32[m.vertices.Length];
            vertCount = m.vertices.Length;

            // go through each triangle
            if (resetColors)
            {
                for (j = 0; j < vertCount; ++j)
                {
                    newColors[j] = Color.white;
                }
            }
            else
            {
                for (j = 0; j < vertCount; ++j)
                {
                    newColor = ambientLight;
                    if (hasDirectionLight)
                    {
                        if (dirNoShadows)
                        {
                            dot = Vector3.Dot(m.normals[j], directionalLight.transform.up);
                            dotColor = new Color(dot, dot, dot, 1.0f);

                            newColor += dotColor * directionalLight.color * directionalLight.intensity;
                        }
                        else
                        {
                            vPos = sceneObjects[i].transform.TransformPoint(m.vertices[j]);

                            if (!Physics.Raycast(vPos, Vector3.up, shadowDistance))
                            {
                                dot = Vector3.Dot(m.normals[j], directionalLight.transform.up);
                                dotColor = new Color(dot, dot, dot, 1.0f);

                                newColor += dotColor * directionalLight.color * directionalLight.intensity;
                            }
                        }
                    }

                    // calculate attenuation for each point light in the scene
                    for (k = 0; k < lightCount; ++k)
                    {
                        if (lights[k].type == LightType.Point)
                        {
                            vPos = sceneObjects[i].transform.TransformPoint(m.vertices[j]);
                            toLight = lights[k].transform.position;

                            distToL = Vector3.Distance(vPos, toLight);

                            if (distToL < lights[k].range * 1.2f)
                            {
                                if (lights[k].shadows == LightShadows.None)
                                {
                                    newColor += CalculateAtten(toLight, vPos, lights[k].range) * lights[k].color * (lights[k].intensity);
                                }
                                else if (lights[k].shadows != LightShadows.None)
                                {
                                    if (!Physics.Linecast(vPos, toLight))
                                        newColor += CalculateAtten(toLight, vPos, lights[k].range) * lights[k].color * (lights[k].intensity);
                                }
                            }
                        }
                    }

                    // anti-lights
                    for (al = 0; al < nLights.Length; ++al)
                    {
                        excluded = false;
                        for (e = 0; e < nLights[al].exclusions.Length; e++)
                        {
                            if (sceneObjects[i] == nLights[al].exclusions[e])
                                excluded = true;
                        }

                        if (!excluded)
                        {
                            vPos = sceneObjects[i].transform.TransformPoint(m.vertices[j]);
                            toLight = nLights[al].transform.position;

                            distToL = Vector3.Distance(vPos, toLight);

                            if (nLights[al].isArea)
                            {
                                if (Mathf.Abs(vPos.x - nLights[al].transform.position.x) < nLights[al].area.x &&
                                    Mathf.Abs(vPos.y - nLights[al].transform.position.y) < nLights[al].area.y &&
                                    Mathf.Abs(vPos.z - nLights[al].transform.position.z) < nLights[al].area.z)
                                {
                                    if (nLights[al].allowAmbient)
                                    {
                                        newColor = ambientLight;
                                    }
                                    else
                                    {
                                        newColor = Color.black;
                                    }
                                }
                            }
                            else
                            {
                                if (distToL < nLights[al].range)
                                {
                                    if (nLights[al].allowAmbient)
                                    {
                                        newColor = ambientLight;
                                    }
                                    else
                                    {
                                        newColor = Color.black;
                                    }
                                }
                            }
                        }
                    }

                    // set vert colors
                    newColors[j] = newColor;
                }
            }

            m.colors32 = newColors;

            data.Add(new VCMData());
            if (sceneObjects[i].GetComponent<VCMID>())
                DestroyImmediate(sceneObjects[i].GetComponent<VCMID>());

            sceneObjects[i].AddComponent<VCMID>();
            sceneObjects[i].GetComponent<VCMID>().ID = totalIndex + i;
            data[totalIndex + i].ID = totalIndex + i;
            for (j = 0; j < newColors.Length; j++)
            {
                data[totalIndex + i].colors.Add(newColors[j]);
            }

        }
        #endregion
        VCM.Save(lightmapName, data.ToArray());

        progress = "Finished";
    }

    private void SaveVCM()
    {

    }

    private Color CalculateAtten(Vector3 to, Vector3 from, float range)
    {
        float dist = Vector3.Distance(to, from) / range;
        dist *= dist;

        float atten = 1.0f / (1.0f + 25.0f * dist);
        return new Color(atten, atten, atten, 1.0f);
    }

    private void RemoveMeshColliders()
    {
        for (int i = 0; i < foundObjects.Count; i++)
            DestroyImmediate(foundObjects[i].GetComponent<MeshCollider>());
    }
}
