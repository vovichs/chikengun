using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEventSystemHandler
	{
		public enum AxisOption
		{
			Both,
			OnlyHorizontal,
			OnlyVertical
		}

		public int MovementRange = 100;

		public AxisOption axesToUse;

		public string horizontalAxisName = "Horizontal";

		public string verticalAxisName = "Vertical";

		private Vector3 m_StartPos;

		private bool m_UseX;

		private bool m_UseY;

		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;

		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;

		private void OnEnable()
		{
			CreateVirtualAxes();
		}

		private void Start()
		{
			m_StartPos = base.transform.position;
		}

		private void UpdateVirtualAxes(Vector3 value)
		{
			Vector3 a = m_StartPos - value;
			a.y = 0f - a.y;
			a /= MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(0f - a.x);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(a.y);
			}
		}

		private void CreateVirtualAxes()
		{
			m_UseX = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal);
			m_UseY = (axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical);
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}

		public void OnDrag(PointerEventData data)
		{
			Vector3 zero = Vector3.zero;
			if (m_UseX)
			{
				Vector2 position = data.position;
				int value = (int)(position.x - m_StartPos.x);
				value = Mathf.Clamp(value, -MovementRange, MovementRange);
				zero.x = value;
			}
			if (m_UseY)
			{
				Vector2 position2 = data.position;
				int value2 = (int)(position2.y - m_StartPos.y);
				value2 = Mathf.Clamp(value2, -MovementRange, MovementRange);
				zero.y = value2;
			}
			base.transform.position = new Vector3(m_StartPos.x + zero.x, m_StartPos.y + zero.y, m_StartPos.z + zero.z);
			UpdateVirtualAxes(base.transform.position);
		}

		public void OnPointerUp(PointerEventData data)
		{
			base.transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}

		public void OnPointerDown(PointerEventData data)
		{
		}

		private void OnDisable()
		{
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}
			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}
	}
}
