using UnityEngine;
using System.Collections;

public class CrossFadeExample : MonoBehaviour 
{
	public Camera camera1;
	public Camera camera2;
	public float fadeTime = 2.0f;
	public AnimationCurve curve;
	private bool inProgress = false;
	private bool swap = false;

	void Update ()
	{
		if( Input.GetKeyDown("space") ) 
		{
			DoFade();
		}
	}

	void DoFade ()
	{
		if( inProgress )
		{
			return;
		}
		
		inProgress = true;
		
		swap = !swap;
		
		
		Camera tmpCam1;
		Camera tmpCam2;
		
		if( swap )
		{
			tmpCam1 = camera1;
			tmpCam2 = camera2;
		}
		else
		{
			tmpCam1 = camera2;
			tmpCam2 = camera1;
		}
		
		
		StartCoroutine( ScreenWipe.use.CrossFade( tmpCam1, tmpCam2, fadeTime, curve ) ); 
		
		inProgress = false;
	}
}