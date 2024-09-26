using System;
using UnityEngine;

[Serializable]
public class WaterScroller : MonoBehaviour
{
	public float scrollSpeed;

	public WaterScroller()
	{
		scrollSpeed = 0.1f;
	}

	public void Update()
	{
		if (GetComponent<Renderer>().material.shader.isSupported)
		{
			Camera.main.depthTextureMode = (Camera.main.depthTextureMode | DepthTextureMode.Depth);
		}
		float num = Time.time * scrollSpeed;
		GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", new Vector2(num / 10f, num));
	}

	public void Main()
	{
	}
}
