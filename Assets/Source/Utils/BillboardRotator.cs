using UnityEngine;
using System.Collections;

public class BillboardRotator : AnimationBase
{

    [Header("[ ROTATION SETTINGS ]")]
    [Range(0.0f, 360.0f)]
	private Vector3 rotStart;
	public Vector3 axis;
	public float rotTime = 4;
    public float waitTime = 4;
	[Range(2, 36)]
	public int numberOfStops;

	public float offset;
    public bool isReverse;

	private float rotation;
	private float oldrotation;
	private float timer;

    void Start()
    {
        // set start rotations
		rotStart = transform.localEulerAngles;
		timer = -offset;
		if (rotTime <= 0) {
			Debug.LogError("BillboardRotator at " + transform + ": rotTime must be greater than zero!");
		}
		if (offset > waitTime) {
			Debug.LogWarning("BillboardRotator at " + transform + ": offset shouldn't exceed waitTime and is currently being clamped.");
		}
    }

    public override void OnUpdate()
    {
		// find rotation
		timer -= Time.deltaTime;
		if (timer < -waitTime) { 
			timer = rotTime;
			oldrotation -= 360/numberOfStops;
		}
			
		rotation = oldrotation + Mathf.SmoothStep(0,360,Mathf.Clamp01(timer/rotTime))/numberOfStops;
      
        // apply rotation
		if (oldrotation > 360) {oldrotation -= 360;}
		transform.localEulerAngles = isReverse ? rotStart - (axis.normalized * rotation) : rotStart + (axis.normalized * rotation);
        
    }
}
