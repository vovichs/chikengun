using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
	public Transform target;

	public float height = 15f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			Transform transform = base.transform;
			Vector3 position = target.position;
			float x = position.x;
			Vector3 position2 = target.position;
			float y = position2.y + height;
			Vector3 position3 = target.position;
			transform.position = new Vector3(x, y, position3.z);
			base.transform.LookAt(target);
		}
	}
}
