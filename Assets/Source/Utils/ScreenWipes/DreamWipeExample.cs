using UnityEngine;
using System.Collections;

public class DreamWipeExample : MonoBehaviour 
{
	public Camera camera1;
	public Camera camera2;
	public float fadeTime = 4.0f;
	public float waveScale = .07f;				// Higher numbers make the effect more exaggerated. Can be negative, .5/-.5 is the max
	public float waveFrequency = 25.0f;			// Higher numbers make more waves in the effect
	private bool inProgress = false;
	private bool swap = false;

	void Start ()
	{
		ScreenWipe.use.InitializeDreamWipe();
	}

	void Update ()
	{
		if (Input.GetKeyDown("space")) 
		{
			StartCoroutine( DoFade() );
		}
	}

	IEnumerator DoFade ()
	{
		// Debug.Log("Pressed space!");
		if( inProgress == true )
		{
			return false;
		}
		
		// Debug.Log("DoFade here!");
		
		inProgress = true;
		
		swap = !swap;
		
		yield return StartCoroutine( ScreenWipe.use.DreamWipe( swap? camera1 : camera2, swap? camera2 : camera1, fadeTime, waveScale, waveFrequency ) );
		
		inProgress = false;
		
		// Debug.Log("DoFade done!");
	}
}