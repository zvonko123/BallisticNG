using UnityEngine;
using System.Collections;

public class SquishWipeExample : MonoBehaviour 
{
	Camera camera1;
	Camera camera2;
	float wipeTime = 2.0f;
	AnimationCurve curve;
	private bool inProgress = false;
	private bool swap = false;

	void Update ()
	{
		if (Input.GetKeyDown("up")) 
		{
			DoWipe(ScreenWipe.TransitionType.Up);
		}
		else if (Input.GetKeyDown("down")) 
		{
			DoWipe(ScreenWipe.TransitionType.Down);
		}
		else if (Input.GetKeyDown("left")) 
		{
			DoWipe(ScreenWipe.TransitionType.Left);
		}
		else if (Input.GetKeyDown("right")) 
		{
			DoWipe(ScreenWipe.TransitionType.Right);
		}
	}

	void DoWipe ( ScreenWipe.TransitionType transitionType  )
	{
		if (inProgress) return;
		inProgress = true;
		
		swap = !swap;
		StartCoroutine( ScreenWipe.use.SquishWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, transitionType, curve) );
		
		inProgress = false;
	}
}