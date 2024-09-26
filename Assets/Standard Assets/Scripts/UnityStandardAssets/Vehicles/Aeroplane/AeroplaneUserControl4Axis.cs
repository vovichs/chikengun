using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
	[RequireComponent(typeof(AeroplaneController))]
	public class AeroplaneUserControl4Axis : MonoBehaviour
	{
		public float maxRollAngle = 80f;

		public float maxPitchAngle = 80f;

		private AeroplaneController m_Aeroplane;

		private float m_Throttle;

		private bool m_AirBrakes;

		private float m_Yaw;

		private void Awake()
		{
			m_Aeroplane = GetComponent<AeroplaneController>();
		}

		private void FixedUpdate()
		{
			float roll = CrossPlatformInputManager.GetAxis("Mouse X");
			float pitch = CrossPlatformInputManager.GetAxis("Mouse Y");
			m_AirBrakes = CrossPlatformInputManager.GetButton("Fire1");
			m_Yaw = CrossPlatformInputManager.GetAxis("Horizontal");
			m_Throttle = CrossPlatformInputManager.GetAxis("Vertical");
			AdjustInputForMobileControls(ref roll, ref pitch, ref m_Throttle);
			m_Aeroplane.Move(roll, pitch, m_Yaw, m_Throttle, m_AirBrakes);
		}

		private void AdjustInputForMobileControls(ref float roll, ref float pitch, ref float throttle)
		{
			float num = roll * maxRollAngle * ((float)Math.PI / 180f);
			float num2 = pitch * maxPitchAngle * ((float)Math.PI / 180f);
			roll = Mathf.Clamp(num - m_Aeroplane.RollAngle, -1f, 1f);
			pitch = Mathf.Clamp(num2 - m_Aeroplane.PitchAngle, -1f, 1f);
		}
	}
}
