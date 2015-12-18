using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CivCarSystem : MonoBehaviour {

    [Header("[ CAR DATA ]")]
    public GameObject[] carObjects;
    public Color[] carColors;

    [Header("[ EMISSION SETTINGS]")]
    public Vector3 emissionSize;
    public float carEmissionSpeed;
    public float carLifetime;
    public float carMinSpeed;
    public float carMaxSpeed;

	[Header("[ ANIMATION SETTINGS]")]
	public AnimationCurve extraYaw;
	public AnimationCurve extraPitch;
	public AnimationCurve extraZ;
	public AnimationCurve extraSpeed;

    private List<CivCar> cars = new List<CivCar>();
    private float spawnTimer;

	/* Rotation is set up such that cars will follow the same paths regardless 
	 * of the initial speed chosen for them. This is true unless speed is also 
	 * being changed with the curve. Keep in mind that these variables don't 
	 * define a path, they just define offsets over time, and there can be  bad 
	 * combinations. In particular, yawing and pitching at the same time will 
	 * induce a roll, so try to avoid doing that. */

    void Update()
    {
        // spawn timer
        spawnTimer += Time.deltaTime;
        if (spawnTimer > carEmissionSpeed)
            SpawnCar();

        // update cars
        UpdateCars();
    }

    void SpawnCar()
    {
        // reset spawn timer
        spawnTimer = 0;

        // get random local spawn position
        Vector3 newSpawn = Vector3.zero;
        newSpawn.x = Random.Range(-emissionSize.x, emissionSize.x);
        newSpawn.y = Random.Range(-emissionSize.y, emissionSize.y);
        newSpawn.z = Random.Range(-emissionSize.z, emissionSize.z);

        // create new car
        int toSpawn = Random.Range(0, carObjects.Length);
        int carColor = Random.Range(0, carColors.Length);
        float carSpeed = Random.Range(carMinSpeed, carMaxSpeed);
        GameObject newCar = Instantiate(carObjects[toSpawn]) as GameObject;
        newCar.GetComponent<Renderer>().material.SetColor("_CarColor", carColors[carColor]);

        // update car list
        cars.Add(new CivCar(newCar, carSpeed, carLifetime));

        // position car
        newCar.transform.parent = transform;
        newCar.transform.localPosition = newSpawn;
        newCar.transform.localRotation = Quaternion.identity;
    }

    void UpdateCars()
    {
        for (int i = 0; i < cars.Count; ++i)
        {
            // update position
			float eval = (cars[i].lifespan / carLifetime);
			cars[i].car.transform.Translate((transform.forward + new Vector3(0,0,extraZ.Evaluate(eval))) * (cars[i].speed + extraSpeed.Evaluate(eval)) * Time.deltaTime);
			cars[i].car.transform.Rotate(new Vector3(extraPitch.Evaluate(eval),0,extraYaw.Evaluate(eval)) * (cars[i].speed + extraSpeed.Evaluate(eval)) * Time.deltaTime);

            // update lifespan
            cars[i].lifespan += Time.deltaTime;

            // destroy if over lifetime
            if (cars[i].lifespan > carLifetime)
            {
                Destroy(cars[i].car);
                cars.RemoveAt(i);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.4f);

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(Vector3.zero, emissionSize);
		Gizmos.DrawRay(Vector3.zero, transform.forward * 2);
    }
}

/// <summary>
/// Represents a car in the civcar system
/// </summary>
public class CivCar
{
    public CivCar(GameObject go, float spd, float life)
    {
        car = go;
        speed = spd;
        lifetime = life;
    }

    public GameObject car;
    public float speed;
    public float lifetime;
    public float lifespan;
}
