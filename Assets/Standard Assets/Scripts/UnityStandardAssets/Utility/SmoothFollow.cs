using UnityEngine;

namespace UnityStandardAssets.Utility
{
	public class SmoothFollow : MonoBehaviour
	{
		[SerializeField]
		private Transform target;

		[SerializeField]
		private float distance = 10f;

		[SerializeField]
		private float height = 5f;

		[SerializeField]
		private float rotationDamping;

		[SerializeField]
		private float heightDamping;

		private void Start()
		{
		}

		private void LateUpdate()
		{
			if ((bool)target)
			{
				Vector3 eulerAngles = target.eulerAngles;
				float y = eulerAngles.y;
				Vector3 position = target.position;
				float b = position.y + height;
				Vector3 eulerAngles2 = base.transform.eulerAngles;
				float y2 = eulerAngles2.y;
				Vector3 position2 = base.transform.position;
				float y3 = position2.y;
				y2 = Mathf.LerpAngle(y2, y, rotationDamping * Time.deltaTime);
				y3 = Mathf.Lerp(y3, b, heightDamping * Time.deltaTime);
				Quaternion rotation = Quaternion.Euler(0f, y2, 0f);
				base.transform.position = target.position;
				base.transform.position -= rotation * Vector3.forward * distance;
				Transform transform = base.transform;
				Vector3 position3 = base.transform.position;
				float x = position3.x;
				float y4 = y3;
				Vector3 position4 = base.transform.position;
				transform.position = new Vector3(x, y4, position4.z);
				base.transform.LookAt(target);
			}
		}
	}
}
