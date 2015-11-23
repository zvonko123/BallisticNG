using UnityEngine;
using System.Collections;

public class ObjectRotator : MonoBehaviour {

    public Vector3 rotSpeed;

    void LateUpdate()
    {
        transform.Rotate(rotSpeed * Time.deltaTime);
    }
}
