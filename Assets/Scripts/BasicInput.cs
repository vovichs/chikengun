using UnityEngine;

[RequireComponent(typeof(VehicleParent))]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Input/Basic Input", 0)]
public class BasicInput : MonoBehaviour
{
	private VehicleParent vp;

	public string accelAxis;

	public string brakeAxis;

	public string steerAxis;

	public string ebrakeAxis;

	public string boostButton;

	public string upshiftButton;

	public string downshiftButton;

	public string pitchAxis;

	public string yawAxis;

	public string rollAxis;

	private void Start()
	{
		vp = GetComponent<VehicleParent>();
	}

	private void Update()
	{
		if (!string.IsNullOrEmpty(upshiftButton) && Input.GetButtonDown(upshiftButton))
		{
			vp.PressUpshift();
		}
		if (!string.IsNullOrEmpty(downshiftButton) && Input.GetButtonDown(downshiftButton))
		{
			vp.PressDownshift();
		}
	}

	private void FixedUpdate()
	{
		if (!string.IsNullOrEmpty(accelAxis))
		{
			vp.SetAccel(UnityEngine.Input.GetAxis(accelAxis));
		}
		if (!string.IsNullOrEmpty(brakeAxis))
		{
			vp.SetBrake(UnityEngine.Input.GetAxis(brakeAxis));
		}
		if (!string.IsNullOrEmpty(steerAxis))
		{
			vp.SetSteer(UnityEngine.Input.GetAxis(steerAxis));
		}
		if (!string.IsNullOrEmpty(ebrakeAxis))
		{
			vp.SetEbrake(UnityEngine.Input.GetAxis(ebrakeAxis));
		}
		if (!string.IsNullOrEmpty(boostButton))
		{
			vp.SetBoost(Input.GetButton(boostButton));
		}
		if (!string.IsNullOrEmpty(pitchAxis))
		{
			vp.SetPitch(UnityEngine.Input.GetAxis(pitchAxis));
		}
		if (!string.IsNullOrEmpty(yawAxis))
		{
			vp.SetYaw(UnityEngine.Input.GetAxis(yawAxis));
		}
		if (!string.IsNullOrEmpty(rollAxis))
		{
			vp.SetRoll(UnityEngine.Input.GetAxis(rollAxis));
		}
		if (!string.IsNullOrEmpty(upshiftButton))
		{
			vp.SetUpshift(UnityEngine.Input.GetAxis(upshiftButton));
		}
		if (!string.IsNullOrEmpty(downshiftButton))
		{
			vp.SetDownshift(UnityEngine.Input.GetAxis(downshiftButton));
		}
	}
}
