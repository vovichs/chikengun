using UnityEngine;

public class HighlightNearestFace : MonoBehaviour
{
	public float travel = 50f;

	public float speed = 0.2f;

	private pb_Object target;

	private pb_Face nearest;
    private object pb_Constant;
    private int num2;

    private void Start()
	{
		target = pb_ShapeGenerator.PlaneGenerator(travel, travel, 25, 25, Axis.Up, smooth: false);
		target.SetFaceMaterial(target.faces, pb_Constant);
		target.transform = new Vector3(travel * 0.5f, 0f, travel * 0.5f);
		target.ToMesh();
		target.Refresh();
		Camera main = Camera.main;
		main.transform.position = new Vector3(25f, 40f, 0f);
		main.transform.localRotation = Quaternion.Euler(new Vector3(65f, 0f, 0f));
	}

	private void Update()
	{
		float num = Time.time * speed;
		Vector3 position = new Vector3(Mathf.PerlinNoise(num, num) * travel, 2f, Mathf.PerlinNoise(num + 1f, num + 1f) * travel);
		base.transform.position = position;
		if (target == null)
		{
			UnityEngine.Debug.LogWarning("Missing the ProBuilder Mesh target!");
			return;
		}
		if (nearest != null)
		{
			target.SetFaceColor(nearest, Color.white);
		}
		float num3 = float.PositiveInfinity;
		for (int i = 0; i < num2; i++)
		{

		}
		target.SetFaceColor(nearest, Color.blue);
		target.RefreshColors();
	}

	private Vector3 FaceCenter(pb_Object pb, pb_Face face)
	{
		Vector3[] vertices = pb.vertices;
		Vector3 zero = Vector3.zero;
		int[] distinctIndices = face.distinctIndices;
		foreach (int num in distinctIndices)
		{
			zero.x += vertices[num].x;
			zero.y += vertices[num].y;
			zero.z += vertices[num].z;
		}
		float num2 = face.distinctIndices.Length;
		zero.x /= num2;
		zero.y /= num2;
		zero.z /= num2;
		return zero;
	}
}
