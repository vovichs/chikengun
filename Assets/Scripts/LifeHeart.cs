using System.Collections;
using UnityEngine;

public class LifeHeart : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(WakeUp());
	}

	private void Update()
	{
	}

	private IEnumerator WakeUp()
	{
		Vector3 localScale = base.transform.localScale;
		float scaleX = localScale.x;
		Vector3 scale = Vector3.zero;
		base.transform.localScale = scale;
		float T = 2f;
		for (float t = 0f; t < T; t += Time.deltaTime)
		{
			scale.x = t / T * scaleX;
			scale.y = t / T * scaleX;
			scale.z = t / T * scaleX;
			base.transform.localScale = scale;
			yield return null;
		}
	}
}
