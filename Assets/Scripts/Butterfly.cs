using UnityEngine;

public class Butterfly : MonoBehaviour
{
	public Vector3 zoneSize = Vector3.one;

	public GameObject[] butterflyObjects;

	public int butterflyCount;

	public float maxSpeed = 1f;

	public float arrivalRadius = 0.2f;

	private Vector3[] targets;

	private Transform[] flies;

	private Vector3[] velocities;

	private void Start()
	{
		targets = new Vector3[butterflyCount];
		flies = new Transform[butterflyCount];
		velocities = new Vector3[butterflyCount];
		for (int i = 0; i < butterflyCount; i++)
		{
			GameObject original = butterflyObjects[Random.Range(0, butterflyObjects.Length - 1)];
			Vector3 position = base.transform.position;
			float x = position.x + UnityEngine.Random.Range(0f - zoneSize.x, zoneSize.x) / 2f;
			Vector3 position2 = base.transform.position;
			float y = position2.y + UnityEngine.Random.Range(0f - zoneSize.y, zoneSize.y) / 2f;
			Vector3 position3 = base.transform.position;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(x, y, position3.z + UnityEngine.Random.Range(0f - zoneSize.z, zoneSize.z) / 2f), Quaternion.identity);
			flies[i] = gameObject.transform;
			targets[i] = GetRandomTarget(flies[i].position);
		}
	}

	private void Update()
	{
		for (int i = 0; i < butterflyCount; i++)
		{
			flies[i].LookAt(targets[i]);
			if (Seek(i))
			{
				targets[i] = GetRandomTarget(flies[i].position);
			}
		}
	}

	private Vector3 GetRandomTarget(Vector3 position)
	{
		Vector3 position2 = base.transform.position;
		float x = position2.x + UnityEngine.Random.Range(0f - zoneSize.x, zoneSize.x) / 2f;
		Vector3 position3 = base.transform.position;
		float y = position3.y + UnityEngine.Random.Range(0f - zoneSize.y, zoneSize.y) / 2f;
		Vector3 position4 = base.transform.position;
		return new Vector3(x, y, position4.z + UnityEngine.Random.Range(0f - zoneSize.z, zoneSize.z) / 2f);
	}

	private bool Seek(int index)
	{
		flies[index].position += velocities[index];
		Vector3 a = targets[index] - flies[index].position;
		if (a.magnitude > arrivalRadius)
		{
			a.Normalize();
			a *= maxSpeed * Time.deltaTime;
			velocities[index] = a;
			return false;
		}
		return true;
	}
}
