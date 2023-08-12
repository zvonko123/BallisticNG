using UnityEngine;
using System.Collections;

public class freezeFade : MonoBehaviour 
{
	public Camera mainCamera;
	public Camera stillCamera;
	public float fadeTime = 2.0f;
	bool inProgress = false;
	[Tooltip("this button activates the fade. It will attempt to turn itself off, however animations override this, so in animations remember to disable it manually immidiately after pushing it.")]
	public bool go = false;

	void Start()
	{
		// StartCoroutine( DoFade() );
	}

	void Update()
	{
		if ( go ) 
		{
			go = false;
			stillCamera.transform.position = mainCamera.transform.position;
			stillCamera.transform.rotation = mainCamera.transform.rotation;
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

		yield return StartCoroutine( ScreenWipe.use.CrossFadePro (stillCamera, mainCamera, fadeTime) );

		// Debug.Log("DoFade done!");

		inProgress = false;
	}
}