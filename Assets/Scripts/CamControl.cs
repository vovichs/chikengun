using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour
{
	public Camera cam;

	public Transform pivot;

	public Transform lookPos;

	public Slider lookPosOffsetX;

	public Slider lookPosOffsetY;

	public Transform target;

	public Transform ground;

	public float rotateSpeed = 10f;

	public float tiltMax = 40f;

	public float tiltMin = 30f;

	private bool rotateEnable = true;

	private bool UIArea;

	public bool AutoRotate;

	private Vector3 rotation;

	public float[] zoom = new float[3];

	public float[] lookPosOffset = new float[3];

	public float smooth = 5f;

	private int zoomIdx;

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			rotation.y = UnityEngine.Input.GetAxis("Mouse X") * rotateSpeed;
			rotation.x = UnityEngine.Input.GetAxis("Mouse Y") * rotateSpeed;
		}
		else
		{
			rotation = Vector3.zero;
		}
		if (rotateEnable && !UIArea)
		{
			CamRotate(rotation);
		}
		if (AutoRotate)
		{
			CamRotate(new Vector3(0f, rotateSpeed * 3f * Time.deltaTime, 0f));
		}
		if (Input.GetMouseButtonDown(1))
		{
			CamZoom();
		}
		cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom[zoomIdx], Time.deltaTime * smooth);
		float num;
		float num2;
		if ((bool)lookPosOffsetX)
		{
			Vector3 localPosition = lookPos.localPosition;
			num = Mathf.Lerp(localPosition.x, lookPosOffsetX.value, Time.deltaTime * smooth);
			Vector3 localPosition2 = lookPos.localPosition;
			num2 = Mathf.Lerp(localPosition2.y, lookPosOffset[zoomIdx] + lookPosOffsetY.value, Time.deltaTime * smooth);
		}
		else
		{
			num = 0f;
			Vector3 localPosition3 = lookPos.localPosition;
			num2 = Mathf.Lerp(localPosition3.y, lookPosOffset[zoomIdx], Time.deltaTime * smooth);
		}
		Transform transform = lookPos;
		float x = num;
		float y = num2;
		Vector3 localPosition4 = lookPos.localPosition;
		transform.localPosition = new Vector3(x, y, localPosition4.z);
		cam.transform.LookAt(lookPos);
	}

	private void LateUpdate()
	{
		Vector3 position = ground.position;
		Vector3 position2 = target.position;
		float x = position2.x;
		Vector3 position3 = ground.position;
		if (x - position3.x >= 5f)
		{
			position.x += 5f;
			ground.position = position;
		}
		else
		{
			Vector3 position4 = target.position;
			float x2 = position4.x;
			Vector3 position5 = ground.position;
			if (x2 - position5.x <= -5f)
			{
				position.x -= 5f;
				ground.position = position;
			}
		}
		Vector3 position6 = target.position;
		float z = position6.z;
		Vector3 position7 = ground.position;
		if (z - position7.z >= 5f)
		{
			position.z += 5f;
			ground.position = position;
		}
		else
		{
			Vector3 position8 = target.position;
			float z2 = position8.z;
			Vector3 position9 = ground.position;
			if (z2 - position9.z <= -5f)
			{
				position.z -= 5f;
				ground.position = position;
			}
		}
		base.transform.position = target.position;
	}

	private void CamRotate(Vector3 rot)
	{
		Vector3 eulerAngles = base.transform.rotation.eulerAngles;
		float y = eulerAngles.y + rot.y;
		base.transform.rotation = Quaternion.Euler(0f, y, 0f);
		Vector3 eulers = new Vector3(rot.x * 3f, 0f, 0f);
		pivot.Rotate(eulers, Space.Self);
		Vector3 eulerAngles2 = pivot.localRotation.eulerAngles;
		float x = eulerAngles2.x;
		if (x > 180f)
		{
			x -= 360f;
			if (x < 0f - tiltMin)
			{
				pivot.localRotation = Quaternion.Euler(0f - tiltMin, 0f, 0f);
			}
		}
		else if (x > tiltMax)
		{
			pivot.localRotation = Quaternion.Euler(tiltMax, 0f, 0f);
		}
	}

	public void CamZoom()
	{
		zoomIdx++;
		zoomIdx = (int)Mathf.Repeat(zoomIdx, zoom.Length);
	}

	public void RotateOption(bool enable)
	{
		rotateEnable = enable;
	}

	public void isUIArea(bool param)
	{
		UIArea = param;
	}
}
