using System;
using UnityEngine;

public class TPSCamera : BaseCamera
{
	[SerializeField]
	private TPSCamSettings[] SettingsList;

	[SerializeField]
	private TPSCamSettings currentSettings;

	private Transform pivotParent;

	private Transform pivot;

	[SerializeField]
	private LayerMask LayerCast;

	private Vector3 desiredPosition;

	private float prevClosestDistance;

	private float currentClosestDistance;

	private Camera cam;

	public static Action Enabled;

	public bool isInSniperMode;

	private Vector2 smallMouseDelta = Vector2.zero;

	private Vector2 bigMouseDelta = Vector2.zero;

	public float startParentAng;

	private float startPivotAng;

	private float angX;

	private float angY;

	private Vector2 prevBigMouseDelta;

	private Vector2 bufSpeed = Vector2.zero;

	private float smoothTime2 = 0.025f;

	private float sensitivityX = 1.38f;

	private float sensitivityMultiplayer = 1f;

	private float distanceVel;

	private float closestDistance;

	private Vector3 relVel;

	private Vector3 pivotRelCenter;

	public float CameraPivotShoulderLength
	{
		get
		{
			return GameController.instance.OurPlayer.playerWeaponManager.CurrentWeapon.pivotShoulderLengthFPS;
		}
		set
		{
			Vector3 position = pivotParent.position;
			float x = position.x;
			Vector3 position2 = pivotParent.position;
			float y = position2.y;
			Vector3 localPosition = pivot.localPosition;
			float y2 = y + localPosition.y;
			Vector3 position3 = pivotParent.position;
			Vector3 a = new Vector3(x, y2, position3.z);
			pivot.position = a + pivot.right * currentSettings.pivotShoulderLength;
		}
	}

	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

	private void Start()
	{
		GameController.instance.cameraMode = CameraMode.TPS;
		pivotParent = new GameObject().transform;
		pivotParent.name = "TPSCameraPivots";
		pivot = new GameObject().transform;
		pivot.name = "pivot";
		pivot.transform.position = pivotParent.transform.position + pivotParent.transform.right * currentSettings.pivotShoulderLength + pivotParent.transform.up * currentSettings.pivotHeight;
		pivot.localEulerAngles = new Vector3(currentSettings.pivotXAngle, 0f, 0f);
		Vector3 localEulerAngles = pivot.localEulerAngles;
		startPivotAng = localEulerAngles.x;
		pivot.transform.SetParent(pivotParent);
		prevClosestDistance = currentSettings.normalDistanceFromPivotToCamera;
		currentClosestDistance = currentSettings.normalDistanceFromPivotToCamera;
		base.transform.SetParent(pivot);
		pivotParent.SetParent(null);
		pivotParent.localScale = new Vector3(1f, 1f, 1f);
		base.transform.position = pivot.position - pivot.forward * currentSettings.normalDistanceFromPivotToCamera;
	}

	public void SetSettingsList(TPSCamSettings[] list)
	{
		SettingsList = list;
		currentSettings = Array.Find(list, (TPSCamSettings st) => st.mode == CamMode.TPSnear);
		if ((bool)pivot)
		{
			CameraPivotShoulderLength = currentSettings.pivotShoulderLength;
			pivot.SetLocalPositionY(currentSettings.pivotHeight);
		}
	}

	public void ToggleModes()
	{
		if (currentSettings.mode == CamMode.TPSnear)
		{
			SwitchMode(CamMode.TPSnormal);
		}
		else
		{
			SwitchMode(CamMode.TPSnear);
		}
	}

	public void SwitchMode(CamMode mode)
	{
		if (currentSettings.normalDistanceFromPivotToCamera != 0f)
		{
			int num = Array.FindIndex(SettingsList, (TPSCamSettings sets) => sets.mode == mode);
			currentSettings = SettingsList[num];
			CameraPivotShoulderLength = currentSettings.pivotShoulderLength;
			pivot.SetLocalPositionY(currentSettings.pivotHeight);
		}
	}

	private void OnEnable()
	{
		cam.nearClipPlane = 0.3f;
		if ((bool)GameController.instance)
		{
			GameController.instance.cameraMode = CameraMode.TPS;
		}
		base.transform.SetParent(pivot);
		if ((bool)GameWindow.instance)
		{
			GameWindow.instance.OnEnterTPSCameraMode();
		}
	}

	private void LateUpdate()
	{
		if (currentSettings == null || GameController.instance.OurPlayer == null)
		{
			return;
		}
		smallMouseDelta = GameInputController.instance.mouseDelta * 0.55f;
		pivotParent.position = GameController.instance.OurPlayer.transform.position;
		bigMouseDelta.x += smallMouseDelta.x;
		Vector3 forward = pivot.forward;
		if (forward.y <= 0f)
		{
			Vector3 localEulerAngles = pivot.localEulerAngles;
			if (!(localEulerAngles.x >= currentSettings.maxTopAng) || !(smallMouseDelta.y < 0f))
			{
				bigMouseDelta.y += smallMouseDelta.y;
			}
		}
		else
		{
			Vector3 localEulerAngles2 = pivot.localEulerAngles;
			if (!(localEulerAngles2.x <= 360f + currentSettings.maxBottompAng) || !(smallMouseDelta.y > 0f))
			{
				bigMouseDelta.y += smallMouseDelta.y;
			}
		}
		prevBigMouseDelta = Vector2.SmoothDamp(prevBigMouseDelta, bigMouseDelta, ref bufSpeed, smoothTime2, float.PositiveInfinity, Time.deltaTime) * sensitivityMultiplayer;
		angX = prevBigMouseDelta.x / (float)Screen.width * 360f * sensitivityX;
		Transform transform = pivotParent;
		Vector3 eulerAngles = pivotParent.eulerAngles;
		float x = eulerAngles.x;
		float y = startParentAng + angX;
		Vector3 eulerAngles2 = pivotParent.eulerAngles;
		transform.eulerAngles = new Vector3(x, y, eulerAngles2.z);
		angY = (0f - prevBigMouseDelta.y) / (float)Screen.width * 360f * sensitivityX;
		angY = Mathf.Clamp(angY, currentSettings.maxBottompAng - startPivotAng, currentSettings.maxTopAng - startPivotAng);
		float num = startPivotAng + angY;
		Transform transform2 = pivot;
		float x2 = num + currentSettings.pivotXAngle;
		Vector3 localEulerAngles3 = pivot.localEulerAngles;
		float y2 = localEulerAngles3.y;
		Vector3 localEulerAngles4 = pivot.localEulerAngles;
		transform2.localEulerAngles = new Vector3(x2, y2, localEulerAngles4.z);
		base.transform.LookAt(pivot);
		RaycastPivotPosition();
		Raycast();
		FindTargetPoint();
	}

	private void Raycast()
	{
		float d = 0.3f;
		float d2 = 0.18f;
		Vector3 a = pivot.position - pivot.forward * currentSettings.normalDistanceFromPivotToCamera;
		Vector3 end = a - base.transform.right * d + base.transform.up * d2;
		Vector3 end2 = a - base.transform.right * d - base.transform.up * d2;
		Vector3 end3 = a + base.transform.right * d - base.transform.up * d2;
		Vector3 end4 = a + base.transform.right * d + base.transform.up * d2;
		desiredPosition = pivot.position - pivot.forward * currentSettings.normalDistanceFromPivotToCamera;
		currentClosestDistance = currentSettings.normalDistanceFromPivotToCamera;
		if (Physics.Linecast(pivot.position, end, out RaycastHit hitInfo, LayerCast))
		{
			float magnitude = Vector3.Project(pivot.position - hitInfo.point, desiredPosition).magnitude;
			magnitude = Vector3.Project(hitInfo.point - pivot.position, base.transform.position - pivot.position).magnitude;
			if (magnitude < currentClosestDistance)
			{
				currentClosestDistance = magnitude;
			}
		}
		if (Physics.Linecast(pivot.position, end4, out hitInfo, LayerCast))
		{
			float magnitude2 = Vector3.Project(pivot.position - hitInfo.point, desiredPosition).magnitude;
			magnitude2 = Vector3.Project(hitInfo.point - pivot.position, base.transform.position - pivot.position).magnitude;
			if (magnitude2 < currentClosestDistance)
			{
				currentClosestDistance = magnitude2;
			}
		}
		if (Physics.Linecast(pivot.position, end2, out hitInfo, LayerCast))
		{
			float magnitude3 = Vector3.Project(pivot.position - hitInfo.point, desiredPosition).magnitude;
			magnitude3 = Vector3.Project(hitInfo.point - pivot.position, base.transform.position - pivot.position).magnitude;
			if (magnitude3 < currentClosestDistance)
			{
				currentClosestDistance = magnitude3;
			}
		}
		if (Physics.Linecast(pivot.position, end3, out hitInfo, LayerCast))
		{
			float magnitude4 = Vector3.Project(pivot.position - hitInfo.point, desiredPosition).magnitude;
			magnitude4 = Vector3.Project(hitInfo.point - pivot.position, base.transform.position - pivot.position).magnitude;
			if (magnitude4 < currentClosestDistance)
			{
				currentClosestDistance = magnitude4;
			}
		}
		closestDistance = Mathf.SmoothDamp(prevClosestDistance, currentClosestDistance, ref distanceVel, currentSettings.camDistanceSpeed);
		prevClosestDistance = currentClosestDistance;
		Vector3 vector = pivot.position - pivot.forward * closestDistance;
		float smoothTime = 0.1f;
		if ((base.transform.position - pivot.position).sqrMagnitude < (vector - pivot.position).sqrMagnitude)
		{
			smoothTime = 0.3f;
		}
		base.transform.position = Vector3.SmoothDamp(base.transform.position, vector, ref relVel, smoothTime);
		float nearClipPlane = Mathf.Clamp(0.3f * closestDistance / currentSettings.normalDistanceFromPivotToCamera, 0.05f, 0.3f);
		Camera.main.nearClipPlane = nearClipPlane;
		base.transform.LookAt(pivot.position);
	}

	private void RaycastPivotPosition()
	{
		Vector3 position = pivotParent.position;
		float x = position.x;
		Vector3 position2 = pivotParent.position;
		float y = position2.y;
		Vector3 localPosition = pivot.localPosition;
		float y2 = y + localPosition.y;
		Vector3 position3 = pivotParent.position;
		pivotRelCenter = new Vector3(x, y2, position3.z);
		float d = currentSettings.pivotShoulderLength;
		Vector3 b = pivot.right * 0.41f;
		if (Physics.Raycast(pivotRelCenter + b, pivot.right, out RaycastHit hitInfo, currentSettings.pivotShoulderLength, LayerCast))
		{
			d = (hitInfo.point - pivotRelCenter).magnitude * 0.8f;
		}
		pivot.position = pivotRelCenter + pivot.right * d;
	}

	private void OnDrawGizmos()
	{
	}

	private void FindTargetPoint()
	{
		GameController.instance.OurPlayer.SetTargetAndLookPoint(base.transform.position + base.transform.forward * 10f, currentSettings.mode);
		Ray ray = cam.ScreenPointToRay(GameWindow.instance.AimSprite.rectTransform.position);
		Vector3 bulletTargetPoint = ray.origin + ray.direction * 300f;
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 300f, LayerCast))
		{
			GameController.instance.OurPlayer.SetGunTargetPoint(hitInfo.point);
			bulletTargetPoint = hitInfo.point;
		}
		else
		{
			GameController.instance.OurPlayer.SetGunTargetPoint(ray.origin + ray.direction * 1000f);
		}
		GameController.instance.OurPlayer.playerWeaponManager.bulletTargetPoint = bulletTargetPoint;
	}

	public override void Enable()
	{
		base.Enable();
		base.enabled = true;
		cam.fieldOfView = 60f;
		cam.nearClipPlane = 0.3f;
		if (Enabled != null)
		{
			Enabled();
		}
	}

	public override void Disable()
	{
		base.Disable();
		base.enabled = false;
	}

	public void ToggleSniperMode()
	{
		if (isInSniperMode)
		{
			ExitSniperMode();
		}
		else
		{
			EnterSniperMode();
		}
	}

	public override void EnterSniperMode()
	{
		isInSniperMode = true;
		cam.fieldOfView = 18f;
	}

	public void ExitSniperMode()
	{
		isInSniperMode = false;
		cam.fieldOfView = 60f;
	}
}
