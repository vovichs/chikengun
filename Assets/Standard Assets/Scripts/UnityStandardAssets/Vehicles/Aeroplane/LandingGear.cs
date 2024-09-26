using UnityEngine;

namespace UnityStandardAssets.Vehicles.Aeroplane
{
	public class LandingGear : MonoBehaviour
	{
		private enum GearState
		{
			Raised = -1,
			Lowered = 1
		}

		public float raiseAtAltitude = 40f;

		public float lowerAtAltitude = 40f;

		private GearState m_State = GearState.Lowered;

		private Animator m_Animator;

		private Rigidbody m_Rigidbody;

		private AeroplaneController m_Plane;

		private void Start()
		{
			m_Plane = GetComponent<AeroplaneController>();
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if (m_State == GearState.Lowered && m_Plane.Altitude > raiseAtAltitude)
			{
				Vector3 velocity = m_Rigidbody.velocity;
				if (velocity.y > 0f)
				{
					m_State = GearState.Raised;
				}
			}
			if (m_State == GearState.Raised && m_Plane.Altitude < lowerAtAltitude)
			{
				Vector3 velocity2 = m_Rigidbody.velocity;
				if (velocity2.y < 0f)
				{
					m_State = GearState.Lowered;
				}
			}
			m_Animator.SetInteger("GearState", (int)m_State);
		}
	}
}
