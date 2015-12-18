using UnityEngine;
using System.Collections;

public class sineTranslator : MonoBehaviour {

	public Vector3 speed;
	public Vector3 offset;
	public Vector3 range;
	private Vector3 start;

	void Start()
	{
		start = transform.localPosition;
	}
	void LateUpdate()
	{
		offset.x += speed.x * Time.deltaTime;
		offset.y += speed.y * Time.deltaTime;
		offset.z += speed.z * Time.deltaTime;
		transform.localPosition = 
			new Vector3(start.x + Mathf.Cos(offset.x) * range.x,
				start.y + Mathf.Cos(offset.y) * range.y,
				start.z + Mathf.Cos(offset.z) * range.z);
		if (offset.magnitude > 1000) { offset = -offset; } //limiter
	}
}
