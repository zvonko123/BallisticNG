using UnityEngine;
using System.Collections;

public class CrossFadeProExample : MonoBehaviour 
{
	public Camera camera1;
	public Camera camera2;
	public float fadeTime = 2.0f;
	bool inProgress = false;
	bool swap = false;
	
	void Start()
	{
		// StartCoroutine( DoFade() );
	}
	
	void Update()
	{
		if (Input.GetKeyDown("space")) 
		{
			// Debug.Log("Space down!");
			StartCoroutine( DoFade() );
		}
	}

	IEnumerator DoFade()
	{
		if(inProgress == true)
		{
			return false;
		}
		
		// Debug.Log("DoFade here!");
		
		inProgress = true;
		
		swap = !swap;
		
		yield return StartCoroutine( ScreenWipe.use.CrossFadePro (swap? camera1 : camera2, swap? camera2 : camera1, fadeTime) );
		
		// Debug.Log("DoFade done!");
		
		inProgress = false;
	}
}