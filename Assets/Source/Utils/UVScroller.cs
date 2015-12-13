using UnityEngine;
using System.Collections;

public class UVScroller : MonoBehaviour {

    public Vector2 ScrollAmount;
	public Vector2 startOffset;
    private Vector2 UVs;
    public bool waterSimulation;

    private float waterSimX;
    private float waterSimY;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    void FixedUpdate()
    {
        if (waterSimulation)
        {
            waterSimX += Time.deltaTime;
            waterSimY += Time.deltaTime * 2;

            float wavesX = Mathf.Sin(waterSimX);
            float wavesY = Mathf.Sin(waterSimY);

            UVs.x += (ScrollAmount.x * Time.deltaTime) * wavesX;
            UVs.y += (ScrollAmount.y * Time.deltaTime) * wavesY;
        } else
        {
			UVs += (ScrollAmount) * Time.deltaTime;
        }

		UVs += (ScrollAmount) * Time.deltaTime;
		mat.SetTextureOffset("_MainTex", UVs+startOffset);
    }
}
