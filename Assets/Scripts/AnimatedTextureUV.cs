using System;
using UnityEngine;

[Serializable]
public class AnimatedTextureUV : MonoBehaviour
{
	public int uvAnimationTileX;

	public int uvAnimationTileY;

	public int frameMax;

	public float framesPerSecond;

	public AnimatedTextureUV()
	{
		uvAnimationTileX = 8;
		uvAnimationTileY = 4;
		frameMax = 32;
		framesPerSecond = 10f;
	}

	public void Update()
	{
		int num = (int)(Time.time * framesPerSecond);
		num %= frameMax;
		Vector2 value = new Vector2(1f / (float)uvAnimationTileX, 1f / (float)uvAnimationTileY);
		int num2 = num % uvAnimationTileX;
		int num3 = num / uvAnimationTileX;
		Vector2 value2 = new Vector2((float)num2 * value.x, 1f - value.y - (float)num3 * value.y);
		GetComponent<Renderer>().material.SetTextureOffset("_MainTex", value2);
		GetComponent<Renderer>().material.SetTextureScale("_MainTex", value);
	}

	public void Main()
	{
	}
}
