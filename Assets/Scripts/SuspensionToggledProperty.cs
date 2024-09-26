using System;

[Serializable]
public class SuspensionToggledProperty
{
	public enum Properties
	{
		steerEnable,
		steerInvert,
		driveEnable,
		driveInvert,
		ebrakeEnable,
		skidSteerBrake
	}

	public Properties property;

	public bool toggled;
}
