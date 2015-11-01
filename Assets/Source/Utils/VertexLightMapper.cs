using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Bakes colors into vertices.
/// </summary>

[ExecuteInEditMode]
public class VertexLightMapper : MonoBehaviour {

    public GameObject sceneParent;
    private List<GameObject> foundObjects = new List<GameObject>();
    private List<GameObject> trackObjects = new List<GameObject>();
    [Header("[ LIGHT SETTINGS ] ")]
    public Color ambientLight;
    public Light directionalLight;

    public bool bakeMap;
    public bool realtimeUpdate;
    private float realtimeTimer;
    
    void Update()
    {
        if (!realtimeUpdate)
        {
            if (!bakeMap)
                return;
            else
                bakeMap = false;
        } else
        {
            realtimeTimer += 0.1f;
            if (realtimeTimer > 2)
            {
                realtimeTimer = 0;
            } else
            {
                return;
            }
        }

        

        FindObjects();
        LightmapPass();
        RemoveMeshColliders();
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
    }

    private void LightmapPass()
    {
        // find all lights
        Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];


        for (int i = 0; i < foundObjects.Count; i++)
        {
            Mesh m = foundObjects[i].GetComponent<MeshFilter>().sharedMesh;

            Color32[] newColors = new Color32[m.vertices.Length];
            Color32 newColor;
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
            Color32 newColor;
            for (int j = 0; j < m.triangles.Length - 3; j += 6)
            {
                newColor = ambientLight;
                if (directionalLight != null)
                {
                    // check that there isn't anything obscuring the vertex from the light direction
                    Vector3 vPos = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                    if (!Physics.Raycast(vPos, -directionalLight.transform.forward))
                    {
                        float dot = Mathf.Abs(Vector3.Dot(m.normals[m.triangles[j]], -directionalLight.transform.forward.normalized));
                        Color dotColor = new Color();
                        dotColor.r = dot;
                        dotColor.g = dot;
                        dotColor.b = dot;

                        newColor += dotColor * directionalLight.GetComponent<Light>().color;
                    }

                }

                // calculate attenuation for each point light in the scene
                for (int k = 0; k < lights.Length; k++)
                {
                    if (lights[k].type == LightType.Point)
                    {
                        Vector3 vPos = trackObjects[i].transform.TransformPoint(m.vertices[m.triangles[j]]);
                        Vector3 toLight = lights[k].transform.position;

                        // linecast to light (shadows)
                        if (!Physics.Raycast(vPos, toLight) || lights[k].shadows == LightShadows.None)
                        {
                            float dist = Vector3.Distance(toLight, vPos);
                            //dist *= dist;
                            //dist = Mathf.Clamp(dist, 0.0f, 1.0f);

                            float atten = lights[k].range / (lights[k].range + 25.0f * dist);
                            Color attenColor = new Color();
                            attenColor.r = atten;
                            attenColor.g = atten;
                            attenColor.b = atten;

                            newColor += attenColor * lights[k].color * (lights[k].intensity);
                        }
                    }
                }
                newColors[m.triangles[j]] = newColor;
                newColors[m.triangles[j + 1]] = newColor;
                newColors[m.triangles[j + 2]] = newColor;
                newColors[m.triangles[j + 3]] = newColor;
                newColors[m.triangles[j + 4]] = newColor;
                newColors[m.triangles[j + 5]] = newColor;
            }
            m.colors32 = newColors;
        }
    }

    private void RemoveMeshColliders()
    {
        for (int i = 0; i < foundObjects.Count; i++)
            DestroyImmediate(foundObjects[i].GetComponent<MeshCollider>());
    }
}
