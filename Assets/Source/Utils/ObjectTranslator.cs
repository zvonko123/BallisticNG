using UnityEngine;
using System.Collections;

public class ObjectTranslator : MonoBehaviour {

	public Vector3 speed;

	void LateUpdate()
	{
		transform.Translate(speed * Time.deltaTime);
	}
}
