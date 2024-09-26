using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	public AnimationCurve animationCurve;

	public float duration;

	private void OnEnable()
	{
		StartCoroutine(Shrink());
	}

	private IEnumerator Shrink()
	{
		base.transform.localScale = Vector3.one;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float scale = 1f - animationCurve.Evaluate(elapsed / duration);
			base.transform.localScale = new Vector3(scale, scale, scale);
			elapsed += Time.deltaTime;
			yield return 0;
		}
		base.gameObject.Recycle();
	}
}
