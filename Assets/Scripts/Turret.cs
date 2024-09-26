using UnityEngine;

public class Turret : MonoBehaviour
{
	public Bullet bulletPrefab;

	public Transform gun;

	private void Update()
	{
		Plane plane = new Plane(Vector3.up, base.transform.position);
		Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (plane.Raycast(ray, out float enter))
		{
			Vector3 forward = Vector3.Normalize(ray.GetPoint(enter) - base.transform.position);
			Quaternion to = Quaternion.LookRotation(forward);
			base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, to, 360f * Time.deltaTime);
			if (Input.GetMouseButtonDown(0))
			{
				bulletPrefab.Spawn(gun.position, gun.rotation);
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			bulletPrefab.DestroyPooled();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
		{
			bulletPrefab.DestroyAll();
		}
	}
}
