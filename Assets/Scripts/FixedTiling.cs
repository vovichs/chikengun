using System;
using UnityEngine;

[Serializable]
public class FixedTiling : MonoBehaviour
{
	public float tileScale;

	public FixedTiling()
	{
		tileScale = 0.1f;
	}

	public void Start()
	{
		Material sharedMaterial = GetComponent<Renderer>().sharedMaterial;
		Vector3 localScale = transform.localScale;
		float x = localScale.x * tileScale;
		Vector3 localScale2 = transform.localScale;
		sharedMaterial.mainTextureScale = new Vector2(x, localScale2.y * tileScale);
	}

	public void Main()
	{
	}
}
