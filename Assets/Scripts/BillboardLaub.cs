using UnityEngine;

public class BillboardLaub : MonoBehaviour
{
	private Transform mainCamTransform;

	private Transform cachedTransform;

	private void Start()
	{
		mainCamTransform = Camera.main.transform;
		cachedTransform = base.transform;
	}

	private void Update()
	{
		Vector3 vector = mainCamTransform.InverseTransformPoint(cachedTransform.position);
		if (vector.z >= 0f)
		{
			Vector3 b = mainCamTransform.position - cachedTransform.position;
			b.x = (b.z = 0f);
			cachedTransform.LookAt(mainCamTransform.position - b);
			GetComponent<Renderer>().enabled = true;
		}
		else
		{
			GetComponent<Renderer>().enabled = false;
		}
	}
}
