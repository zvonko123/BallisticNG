using UnityEngine;
using System.Collections;

public class ShipConstructor : MonoBehaviour {

    public void SpawnShip(bool isAI)
    {
        // load ship prefab and get settings from it
        GameObject prefab = LoadShip(RaceSettings.playerShip);
        ShipSettings settings = prefab.GetComponent<ShipSettings>();
        if (settings == null)
        {
            Debug.LogError("The loaded prefab does not contain a settings class!");
            return;
        }

        // create axis containers
        GameObject axis = new GameObject("_axis");
        GameObject anim = new GameObject("_anim");
        anim.transform.parent = transform;
        anim.transform.localPosition = Vector3.zero;
        anim.transform.localRotation = Quaternion.identity;

        axis.transform.parent = anim.transform;
        axis.transform.localPosition = Vector3.zero;
        axis.transform.localRotation = Quaternion.identity;

        // parent prefab to axis
        prefab.transform.parent = axis.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        settings.REF_RECHARGEFX.transform.parent = transform;
        settings.REF_SHIELD.transform.parent = anim.transform;
        settings.REF_DROID.transform.parent = transform;

        // create rigibody
        Rigidbody body = gameObject.AddComponent<Rigidbody>();
        body.useGravity = false;
        body.constraints = RigidbodyConstraints.FreezeRotation;
        body.drag = 1;

        // create classses
        ShipRefs r = gameObject.AddComponent<ShipRefs>();
        r.settings = settings;
        r.position = gameObject.AddComponent<ShipPosition>();
        r.position.r = r;

        r.effects = gameObject.AddComponent<ShipEffects>();
        r.effects.r = r;

        r.input = gameObject.AddComponent<ShipInput>();
        r.input.r = r;

        r.sim = gameObject.AddComponent<ShipSim>();
        r.sim.r = r;

        r.body = body;
        r.mesh = settings.REF_MESH;
        r.cam = CreateNewCamera(isAI, r);
        r.shield = settings.DAMAGE_SHIELD;
        r.axis = axis;
        r.anim = anim;

        // attach mesh collider to mesh
        MeshCollider mc = r.mesh.AddComponent<MeshCollider>();
        mc.convex = true;
        gameObject.tag = "Ship";

        // add frictionless physics manterial to collider
        PhysicMaterial shipMat = new PhysicMaterial();

        shipMat.bounceCombine = PhysicMaterialCombine.Minimum;
        shipMat.frictionCombine = PhysicMaterialCombine.Minimum;
        shipMat.bounciness = 0;
        shipMat.dynamicFriction = 0;
        shipMat.staticFriction = 0;
        mc.material = shipMat;

        r.isAI = isAI;

        // destroy this class (it's no longer needed)
        RaceSettings.SHIPS.Add(r);
        if (RaceSettings.isNetworked)
            RaceSettings.serverReferences.players.Add(r);

        Destroy(this);

    }

    private Camera CreateNewCamera(bool isAi, ShipRefs r)
    {
        // create empty gameobject
        GameObject newCamera = new GameObject("Ship Camera");
        newCamera.transform.parent = transform;

        // attach all components needed for cameras
        Camera c = newCamera.AddComponent<Camera>();
        c.backgroundColor = Color.black;
        newCamera.AddComponent<GUILayer>();
        newCamera.AddComponent<FlareLayer>();
        newCamera.AddComponent<AudioListener>();

        // render planes
        c.nearClipPlane = 0.05f;
        c.farClipPlane = 1500.0f;

        // ai switch
        if (isAi)
        {
            newCamera.SetActive(false);
        } else
        {
            RaceSettings.currentCamera = c;
            StartCamera sc = newCamera.AddComponent<StartCamera>();
            sc.r = r;
        }
        return c;
    }

    private GameObject LoadShip(E_SHIPS ship)
    {
        return Instantiate(Resources.Load("Ships/" + ship.ToString()) as GameObject) as GameObject;
    }
}
