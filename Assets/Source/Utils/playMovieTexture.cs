using UnityEngine;
using System.Collections;
public class playMovieTexture : MonoBehaviour {
	
	public int matIndex;
	void Start () {
		((MovieTexture)GetComponent<Renderer>().materials[matIndex].mainTexture).loop = true;
		((MovieTexture)GetComponent<Renderer>().materials[matIndex].mainTexture).Play();
	}
}
