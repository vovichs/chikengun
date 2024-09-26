using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(AudioListener))]
public class CarCamera : BaseCamera
{
	public CarCamParams camParams;

	public float xInputAxis;

	public float yInputAxis;

	private Transform tr;

	private VehicleParent vp;

	public Transform target;

	private Rigidbody targetBody;

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

	private VehicleParent targetVP;

	private IEnumerator Start()
	{
		tr = base.transform;
		yield return null;
	}

	public void Initialize()
	{
		if (!lookObj)
		{
			GameObject gameObject = new GameObject("Camera Looker");
			lookObj = gameObject.transform;
		}
		castMask = 1;
		if ((bool)target)
		{
			vp = target.GetComponent<VehicleParent>();
			forwardLook = target.forward;
			upLook = target.up;
			targetBody = target.GetComponent<Rigidbody>();
		}
		GetComponent<AudioListener>().velocityUpdateMode = AudioVelocityUpdateMode.Fixed;
	}

	public override void SetTarget(Transform target)
	{
		this.target = target;
		camParams = target.GetComponent<CarController>().camFollowSettings;
		Initialize();
	}

	public override void Enable()
	{
		base.enabled = true;
		base.transform.SetParent(null);
		Camera.main.nearClipPlane = 0.3f;
		Camera.main.fieldOfView = 60f;
	}

	public override void Disable()
	{
		base.enabled = false;
	}

	public void EnterBackViewMode()
	{
		yInputAxis = -1f;
	}

	public void OutFromBackViewMode()
	{
		yInputAxis = 1f;
	}

	public bool IsInBackViewMode()
	{
		return yInputAxis == -1f;
	}

	private void FixedUpdate()
	{
		if (!vp || !vp.norm)
		{
			return;
		}
		SetInput(xInputAxis, yInputAxis);
		if ((bool)target && (bool)targetBody && target.gameObject.activeSelf)
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
			targetUp = ((!stayFlat) ? vp.norm.forward : GlobalControl.worldUpDir);
			lookDir = Vector3.Slerp(lookDir, (xInput != 0f || yInput != 0f) ? new Vector3(xInput, 0f, yInput).normalized : Vector3.forward, 0.1f);
			float a = smoothYRot;
			Vector3 angularVelocity = targetBody.angularVelocity;
			smoothYRot = Mathf.Lerp(a, angularVelocity.y, 0.02f);
			upLook = Vector3.Lerp(upLook, targetUp, 0.05f);
			forwardLook = Vector3.Lerp(forwardLook, targetForward, 0.05f);
			lookObj.rotation = Quaternion.LookRotation(forwardLook, upLook);
			lookObj.position = target.position;
			Vector3 vector2 = lookDir;
			Vector3 forward = lookObj.TransformDirection(vector2);
			Vector3 position = lookObj.TransformPoint(-vector2 * (camParams.diastance + vp.velMag / 100f * camParams.diastance * camParams.speedDistanceK) - vector2 * 0f + new Vector3(0f, camParams.height, 0f));
			tr.position = position;
			tr.rotation = Quaternion.LookRotation(forward, lookObj.up);
			if (stayFlat)
			{
				Transform transform = base.transform;
				float angle = camParams.angle;
				Vector3 localEulerAngles = base.transform.localEulerAngles;
				float y = localEulerAngles.y;
				Vector3 localEulerAngles2 = base.transform.localEulerAngles;
				transform.localEulerAngles = new Vector3(angle, y, localEulerAngles2.z);
			}
		}
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

	private void SetLayerRecursively(GameObject go, int newLayer)
	{
		go.layer = newLayer;
		IEnumerator enumerator = go.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform transform = (Transform)enumerator.Current;
				SetLayerRecursively(transform.gameObject, newLayer);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}
}
