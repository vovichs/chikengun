using UnityEngine;

public static class GizmosExtra
{
	public static void DrawWireCylinder(Vector3 pos, Vector3 dir, float radius, float height)
	{
		float num = height * 0.5f;
		Quaternion rotation = Quaternion.LookRotation(dir, new Vector3(0f - dir.y, dir.x, 0f));
		Gizmos.DrawLine(pos + rotation * new Vector3(radius, 0f, num), pos + rotation * new Vector3(radius, 0f, 0f - num));
		Gizmos.DrawLine(pos + rotation * new Vector3(0f - radius, 0f, num), pos + rotation * new Vector3(0f - radius, 0f, 0f - num));
		Gizmos.DrawLine(pos + rotation * new Vector3(0f, radius, num), pos + rotation * new Vector3(0f, radius, 0f - num));
		Gizmos.DrawLine(pos + rotation * new Vector3(0f, 0f - radius, num), pos + rotation * new Vector3(0f, 0f - radius, 0f - num));
		for (float num2 = 0f; num2 < 6.28f; num2 += 0.1f)
		{
			Vector3 from = pos + rotation * new Vector3(Mathf.Sin(num2) * radius, Mathf.Cos(num2) * radius, num);
			Vector3 to = pos + rotation * new Vector3(Mathf.Sin(num2 + 0.1f) * radius, Mathf.Cos(num2 + 0.1f) * radius, num);
			Gizmos.DrawLine(from, to);
			Vector3 from2 = pos + rotation * new Vector3(Mathf.Sin(num2) * radius, Mathf.Cos(num2) * radius, 0f - num);
			Vector3 to2 = pos + rotation * new Vector3(Mathf.Sin(num2 + 0.1f) * radius, Mathf.Cos(num2 + 0.1f) * radius, 0f - num);
			Gizmos.DrawLine(from2, to2);
		}
	}
}
