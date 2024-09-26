using UnityEngine;

namespace UnityStandardAssets.SceneUtils
{
	public class PlaceTargetWithMouse : MonoBehaviour
	{
		public float surfaceOffset = 1.5f;

		public GameObject setTargetOn;

		private void Update()
		{
			if (!Input.GetMouseButtonDown(0))
			{
				return;
			}
			Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
			if (Physics.Raycast(ray, out RaycastHit hitInfo))
			{
				base.transform.position = hitInfo.point + hitInfo.normal * surfaceOffset;
				if (setTargetOn != null)
				{
					setTargetOn.SendMessage("SetTarget", base.transform);
				}
			}
		}
	}
}
