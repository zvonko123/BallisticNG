using UnityEngine;
using System.Collections;

public class ShapeWipeExample : MonoBehaviour 
{
	Camera camera1;
	Camera camera2;
	float wipeTime = 2.0f;
	float rotateAmount = 360.0f;
	Mesh[] shapeMesh;
	AnimationCurve curve;
	private bool inProgress = false;
	private bool swap = false;
	private int useShape = 0;

	void Update ()
	{
		if (Input.GetKeyDown("up")) 
		{
			DoWipe( ScreenWipe.ZoomType.Grow );
		}
		else if (Input.GetKeyDown("down")) 
		{
			DoWipe( ScreenWipe.ZoomType.Shrink );
		}

		if (Input.GetKeyDown("1")) {useShape = 0;}
		if (Input.GetKeyDown("2")) {useShape = 1;}
		if (Input.GetKeyDown("3")) {useShape = 2;}
	}

	void DoWipe ( ScreenWipe.ZoomType zoom  )
	{
		if (inProgress) return;
		inProgress = true;
		
		swap = !swap;
		StartCoroutine( ScreenWipe.use.ShapeWipe (swap? camera1 : camera2, swap? camera2 : camera1, wipeTime, zoom, shapeMesh[useShape], rotateAmount, curve) );
		
		inProgress = false;
	}
}