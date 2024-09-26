using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Camera/Camera Control", 0)]
public class CameraControl : MonoBehaviour
{
	private Transform tr;

	private Camera cam;

	private VehicleParent vp;

	public Transform target;

	private Rigidbody targetBody;

	public float height;

	public float distance;

	private float xInput;

	private float yInput;

	private Vector3 lookDir;

	private float smoothYRot;

	private Transform lookObj;

	private Vector3 forwardLook;

	private Vector3 upLook;

	private Vector3 targetForward;

	private Vector3 targetUp;

	[Tooltip("Should the camera stay flat? (Local y-axis always points up)")]
	public bool stayFlat;

	[Tooltip("Mask for which objects will be checked in between the camera and target vehicle")]
	public LayerMask castMask;

	private void Start()
	{
		tr = base.transform;
		cam = GetComponent<Camera>();
		Initialize();
	}

	public void Initialize()
	{
		if (!lookObj)
		{
			GameObject gameObject = new GameObject("Camera Looker");
			lookObj = gameObject.transform;
		}
		if ((bool)target)
		{
			vp = target.GetComponent<VehicleParent>();
			distance += vp.cameraDistanceChange;
			height += vp.cameraHeightChange;
			forwardLook = target.forward;
			upLook = target.up;
			targetBody = target.GetComponent<Rigidbody>();
		}
		GetComponent<AudioListener>().velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
	}

	private void FixedUpdate()
	{
		if (!target || !targetBody || !target.gameObject.activeSelf)
		{
			return;
		}
		if (vp.groundedWheels > 0)
		{
			Vector3 vector;
			if (stayFlat)
			{
				Vector3 up = vp.norm.up;
				float x = up.x;
				Vector3 up2 = vp.norm.up;
				vector = new Vector3(x, 0f, up2.z);
			}
			else
			{
				vector = vp.norm.up;
			}
			targetForward = vector;
		}
		targetUp = ((!stayFlat) ? vp.norm.forward : GlobalControl.worldUpDir);
		lookDir = Vector3.Slerp(lookDir, (xInput != 0f || yInput != 0f) ? new Vector3(xInput, 0f, yInput).normalized : Vector3.forward, 0.1f * TimeMaster.inverseFixedTimeFactor);
		float a = smoothYRot;
		Vector3 angularVelocity = targetBody.angularVelocity;
		smoothYRot = Mathf.Lerp(a, angularVelocity.y, 0.02f * TimeMaster.inverseFixedTimeFactor);
		if (Physics.Raycast(target.position, -targetUp, out RaycastHit hitInfo, 1f) && !stayFlat)
		{
			upLook = Vector3.Lerp(upLook, (!((double)Vector3.Dot(hitInfo.normal, targetUp) > 0.5)) ? targetUp : hitInfo.normal, 0.05f * TimeMaster.inverseFixedTimeFactor);
		}
		else
		{
			upLook = Vector3.Lerp(upLook, targetUp, 0.05f * TimeMaster.inverseFixedTimeFactor);
		}
		forwardLook = Vector3.Lerp(forwardLook, targetForward, 0.05f * TimeMaster.inverseFixedTimeFactor);
		lookObj.rotation = Quaternion.LookRotation(forwardLook, upLook);
		lookObj.position = target.position;
		Vector3 normalized = (lookDir - new Vector3(Mathf.Sin(smoothYRot), 0f, Mathf.Cos(smoothYRot)) * Mathf.Abs(smoothYRot) * 0.2f).normalized;
		Vector3 forward = lookObj.TransformDirection(normalized);
		Vector3 vector2 = lookObj.TransformPoint(-normalized * distance - normalized * Mathf.Min(targetBody.velocity.magnitude * 0.05f, 2f) + new Vector3(0f, height, 0f));
		if (Physics.Linecast(target.position, vector2, out hitInfo, castMask))
		{
			tr.position = hitInfo.point + (target.position - vector2).normalized * (cam.nearClipPlane + 0.1f);
		}
		else
		{
			tr.position = vector2;
		}
		tr.rotation = Quaternion.LookRotation(forward, lookObj.up);
	}

	public void SetInput(float x, float y)
	{
		xInput = x;
		yInput = y;
	}

	private void OnDestroy()
	{
		if ((bool)lookObj)
		{
			UnityEngine.Object.Destroy(lookObj.gameObject);
		}
	}
}
