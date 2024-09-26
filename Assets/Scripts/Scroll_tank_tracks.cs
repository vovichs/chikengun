using System;
using UnityEngine;

[Serializable]
public class Scroll_tank_tracks : MonoBehaviour
{
	public float scrollSpeed;

	public Scroll_tank_tracks()
	{
		scrollSpeed = 10f;
	}

	public void Update()
	{
		float y = Time.time * scrollSpeed * Time.deltaTime % 1f;
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(0f, y));
	}

	public void Main()
	{
	}
}
