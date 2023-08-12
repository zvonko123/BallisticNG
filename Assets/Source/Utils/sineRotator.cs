using UnityEngine;
using System.Collections;

public class sineRotator : AnimationBase {

	public Vector3 speed;
	public Vector3 offset;
	public Vector3 range;
	public Vector3 center;
	private Vector3 start;

	void Start()
	{
		start = transform.localEulerAngles;
	}

    public override void OnUpdate()
    {
        offset += speed * Time.deltaTime;
		transform.localEulerAngles = 
			new Vector3(start.x + center.x + Mathf.Sin(offset.x) * range.x,
						start.y + center.y + Mathf.Sin(offset.y) * range.y,
						start.z + center.z + Mathf.Sin(offset.z) * range.z);
		if (offset.magnitude > 10000) { offset = -offset; } //limiter
	}
}
