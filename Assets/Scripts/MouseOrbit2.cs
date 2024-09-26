using UnityEngine;

public class MouseOrbit2 : MonoBehaviour
{
	public Transform target;

	public float distance = 5f;

	public float xSpeed = 120f;

	public float ySpeed = 120f;

	public float yMinLimit = -20f;

	public float yMaxLimit = 80f;

	public float distanceMin = 0.5f;

	public float distanceMax = 15f;

	private Rigidbody rb;

	private float x;

	private float y;

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.freezeRotation = true;
		}
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			x += UnityEngine.Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
			y -= UnityEngine.Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			Quaternion rotation = Quaternion.Euler(y, x, 0f);
			distance = Mathf.Clamp(distance - UnityEngine.Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMin, distanceMax);
			if (Physics.Linecast(target.position, base.transform.position, out RaycastHit hitInfo))
			{
				distance -= hitInfo.distance;
			}
			Vector3 point = new Vector3(0f, 0f, 0f - distance);
			Vector3 position = rotation * point + target.position;
			base.transform.rotation = rotation;
			base.transform.position = position;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
