using UnityEngine;
using System.Collections;

public class RectWipeExample : MonoBehaviour 
{
	public Camera camera1;
	public Camera camera2;
	public float wipeTime = 2.0f;
	public AnimationCurve curve;
	private bool inProgress = false;
	private bool swap = false;

	void Update ()
	{
		if (Input.GetKeyDown("up")) 
		{
			StartCoroutine( DoWipe( ScreenWipe.ZoomType.Grow ) );
		}
		else if (Input.GetKeyDown("down")) 
		{
			StartCoroutine( DoWipe( ScreenWipe.ZoomType.Shrink ) );
		}
	}

	IEnumerator DoWipe ( ScreenWipe.ZoomType zoom  )
	{
		if( inProgress == true )
		{
			return false;
		}
		
		// Debug.Log("DoWipe here!");
		
		inProgress = true;
		
		swap = !swap;
		
		yield return StartCoroutine( ScreenWipe.use.RectWipe(swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom, curve ) );
		
		inProgress = false;
	}
}