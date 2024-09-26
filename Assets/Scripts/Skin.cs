using UnityEngine;

public class Skin : MonoBehaviour
{
	public enum MeshType
	{
		Ball,
		Cube,
		Disk,
		Mesh
	}

	public Texture texture;

	public bool canWearEyes;

	public bool canWearSmile;

	public bool canWearHat;

	public MeshType meshType;

	public GameObject MeshSkin;
}
