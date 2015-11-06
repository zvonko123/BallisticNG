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
    [Header("[ LIGHT SETTINGS ] ")]
    public Color ambientLight;
    public Light directionalLight;

    public bool bakeMap;
    public string lightmapName;
    public bool foundMap;
    
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

        int foundMeshes = 0;
        foreach(Transform go in sceneParent.transform)
        {
            if (go.gameObject.layer != LayerMask.NameToLayer("TrackFloor") && 
                go.gameObject.layer != LayerMask.NameToLayer("TrackWall"))
            {
                // add object to array
                if (go.GetComponent<MeshFilter>())
                {
                    foundObjects.Add(go.gameObject);

                    // attach mesh collider (for raycasting)
                    go.gameObject.AddComponent<MeshCollider>();

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

    private void LightmapPass()
    {

        // find all lights
        Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];

        Color32 newColor;
        Color32 dotColor;
        Color32 attenColor;
        Vector3 vPos;
        Vector3 toLight;
        List<VCMData> data = new List<VCMData>();
        for (int i = 0; i < foundObjects.Count; i++)
        {
            Mesh m = foundObjects[i].GetComponent<MeshFilter>().sharedMesh;

            Color32[] newColors = new Color32[m.vertices.Length];

            for (int j = 0; j < m.vertices.Length; j++)
            {
                newColors[j] = ambientLight;
            }
            m.colors32 = newColors;
        }

        for (int i = 0; i < trackObjects.Count; i++)
        {
            Mesh m = trackObjects[i].GetComponent<MeshFilter>().sharedMesh;

            Color32[] newColors = new Color32[m.vertices.Length];
            for (int j = 0; j < m.triangles.Length; j += 3)
            {
                newColor = ambientLight;
                if (directionalLight != null)
                {
                    // check that there isn't anything obscuring the vertex from the light direction
                    vPos = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                    int ignoreTrack = ~(1 << LayerMask.NameToLayer("TrackFloor") | 1 << LayerMask.NameToLayer("TrackWall"));
                    if (!Physics.Raycast(vPos, -directionalLight.transform.forward, Mathf.Infinity, ignoreTrack) 
                        || directionalLight.GetComponent<Light>().shadows == LightShadows.None)
                    {
                        float dot = Vector3.Dot(m.normals[m.triangles[j]], directionalLight.transform.up);
                        dotColor = new Color(dot, dot, dot, 1.0f);

                        newColor += dotColor * directionalLight.GetComponent<Light>().color;
                    }

                }

                // calculate attenuation for each point light in the scene
                for (int k = 0; k < lights.Length; k++)
                {
                    if (lights[k].type == LightType.Point)
                    {
                        vPos = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                        toLight = lights[k].transform.position;

                        // linecast to light (shadows)
                        int ignoreTrack = ~(1 << LayerMask.NameToLayer("TrackFloor") | 1 << LayerMask.NameToLayer("TrackWall"));
                        if (!Physics.Linecast(vPos, toLight, ignoreTrack) || lights[k].shadows == LightShadows.None)
                        {
                            float dist = Vector3.Distance(toLight, vPos) / lights[k].range;
                            dist *= dist;

                            float atten = 1.0f / (1.0f + 25.0f * dist);
                            attenColor = new Color(atten, atten, atten, 1.0f);

                            newColor += attenColor * lights[k].color * (lights[k].intensity);
                        }
                    }
                }
                newColors[m.triangles[j]] = newColor;
                newColors[m.triangles[j + 1]] = newColor;
                newColors[m.triangles[j + 2]] = newColor;
            }
            m.colors32 = newColors;

            data.Add(new VCMData());
            if (trackObjects[i].GetComponent<VCMID>())
                    DestroyImmediate(trackObjects[i].GetComponent<VCMID>());

            trackObjects[i].AddComponent<VCMID>();
            trackObjects[i].GetComponent<VCMID>().ID = i;
            data[i].ID = i;
            for (int j = 0; j < newColors.Length; j++)
            {
                data[i].colors.Add(newColors[j]);
            }

        }
        VCM.Save(lightmapName, data.ToArray());
    }

    private void SaveVCM()
    {

    }

    private void RemoveMeshColliders()
    {
        for (int i = 0; i < foundObjects.Count; i++)
            DestroyImmediate(foundObjects[i].GetComponent<MeshCollider>());
    }
}
