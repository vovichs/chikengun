using UnityEngine;

public class HorseCamera : BaseCamera
{
	private Transform target;

	private CarCamParams camFollowSettings;

	private byte typeOfCameraView;

	public float initNearDistance = 8f;

	private float distance;

	private float rotationDamping = 5.5f;

	private float distanceDamping = 2f;

	private float wantedDistance;

	private float deltaTime;

	private Vector3 fixedCarPos;

	private float FixTime;

	public GameObject car;

	public override void Enable()
	{
		base.enabled = true;
		Camera.main.transform.SetParent(null);
	}

	public override void Disable()
	{
		base.enabled = false;
	}

	public void SeTarget(Transform target)
	{
		this.target = target;
		camFollowSettings = target.GetComponent<HorseController>().camFollowSettings;
	}

	private void Start()
	{
		rotationDamping = 7f;
		distance = initNearDistance;
		wantedDistance = initNearDistance;
	}

	public void SetType(byte type)
	{
		typeOfCameraView = type;
		if (typeOfCameraView == 0)
		{
			initNearDistance = 8f;
		}
		else if (typeOfCameraView == 1)
		{
			initNearDistance = 15f;
		}
		else if (typeOfCameraView == 2)
		{
			fixedCarPos = base.transform.position;
			FixTime = Time.time;
		}
	}

	private void LateUpdate()
	{
		deltaTime = Time.fixedDeltaTime;
		if (typeOfCameraView == 0)
		{
			NearToCarSmoothFollow();
		}
		if (typeOfCameraView == 1)
		{
			NearToCarSmoothFollow();
		}
		if (typeOfCameraView == 2)
		{
			FixedToArenaSmoothFollow();
		}
	}

	public void updateDistance(float carSpeedCoefficient)
	{
		wantedDistance = initNearDistance * (1f + carSpeedCoefficient);
	}

	private void NearToCarSmoothFollow()
	{
		if ((bool)target)
		{
			Vector3 eulerAngles = target.eulerAngles;
			float y = eulerAngles.y;
			Vector3 position = target.position;
			float b = position.y + camFollowSettings.height;
			Vector3 eulerAngles2 = base.transform.eulerAngles;
			float y2 = eulerAngles2.y;
			Vector3 position2 = base.transform.position;
			float y3 = position2.y;
			y2 = Mathf.LerpAngle(y2, y, rotationDamping * deltaTime);
			distance = Mathf.Lerp(camFollowSettings.diastance, wantedDistance, distanceDamping * deltaTime);
			y3 = Mathf.Lerp(y3, b, 7f * deltaTime);
			Quaternion rotation = Quaternion.Euler(0f, y2, 0f);
			base.transform.position = target.position;
			base.transform.position -= rotation * Vector3.forward * distance;
			Transform transform = base.transform;
			Vector3 position3 = base.transform.position;
			float x = position3.x;
			float y4 = y3;
			Vector3 position4 = base.transform.position;
			transform.position = new Vector3(x, y4, position4.z);
			Transform transform2 = base.transform;
			Vector3 position5 = target.position;
			float x2 = position5.x;
			Vector3 position6 = target.position;
			float y5 = position6.y + camFollowSettings.height;
			Vector3 position7 = target.position;
			transform2.LookAt(new Vector3(x2, y5, position7.z));
			base.transform.SetLocalEulerX(camFollowSettings.angle);
		}
	}

	private void AwayFromCarSmoothFollow()
	{
	}

	private void FixedToArenaSmoothFollow()
	{
		base.transform.position = Vector3.Lerp(fixedCarPos, new Vector3(0f, 15f, 0f), Time.time - FixTime);
		base.transform.LookAt(new Vector3(0f, 5f, 50f));
	}
}
