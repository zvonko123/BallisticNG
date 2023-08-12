using UnityEngine;
using System.Collections;

public class ObjectRotator : AnimationBase {

    public Vector3 rotSpeed;

    public override void OnUpdate()
    {
        transform.Rotate(rotSpeed * Time.deltaTime);
    }
}
