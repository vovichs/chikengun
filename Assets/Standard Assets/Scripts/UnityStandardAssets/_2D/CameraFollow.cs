using UnityEngine;

namespace UnityStandardAssets._2D
{
	public class CameraFollow : MonoBehaviour
	{
		public float xMargin = 1f;

		public float yMargin = 1f;

		public float xSmooth = 8f;

		public float ySmooth = 8f;

		public Vector2 maxXAndY;

		public Vector2 minXAndY;

		private Transform m_Player;

		private void Awake()
		{
			m_Player = GameObject.FindGameObjectWithTag("Player").transform;
		}

		private bool CheckXMargin()
		{
			Vector3 position = base.transform.position;
			float x = position.x;
			Vector3 position2 = m_Player.position;
			return Mathf.Abs(x - position2.x) > xMargin;
		}

		private bool CheckYMargin()
		{
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = m_Player.position;
			return Mathf.Abs(y - position2.y) > yMargin;
		}

		private void Update()
		{
			TrackPlayer();
		}

		private void TrackPlayer()
		{
			Vector3 position = base.transform.position;
			float value = position.x;
			Vector3 position2 = base.transform.position;
			float value2 = position2.y;
			if (CheckXMargin())
			{
				Vector3 position3 = base.transform.position;
				float x = position3.x;
				Vector3 position4 = m_Player.position;
				value = Mathf.Lerp(x, position4.x, xSmooth * Time.deltaTime);
			}
			if (CheckYMargin())
			{
				Vector3 position5 = base.transform.position;
				float y = position5.y;
				Vector3 position6 = m_Player.position;
				value2 = Mathf.Lerp(y, position6.y, ySmooth * Time.deltaTime);
			}
			value = Mathf.Clamp(value, minXAndY.x, maxXAndY.x);
			value2 = Mathf.Clamp(value2, minXAndY.y, maxXAndY.y);
			Transform transform = base.transform;
			float x2 = value;
			float y2 = value2;
			Vector3 position7 = base.transform.position;
			transform.position = new Vector3(x2, y2, position7.z);
		}
	}
}
