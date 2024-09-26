using System;
using UnityEngine;

[RequireComponent(typeof(Terrain))]
[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("RVP/C#/Ground Surface/Terrain Surface", 2)]
public class TerrainSurface : MonoBehaviour
{
	private Transform tr;

	private TerrainData terDat;

	private float[,,] terrainAlphamap;

	public int[] surfaceTypes = new int[0];

	[NonSerialized]
	public float[] frictions;

	private void Start()
	{
		tr = base.transform;
		if (!GetComponent<Terrain>().terrainData)
		{
			return;
		}
		terDat = GetComponent<Terrain>().terrainData;
		if (!Application.isPlaying)
		{
			return;
		}
		UpdateAlphamaps();
		frictions = new float[surfaceTypes.Length];
		for (int i = 0; i < frictions.Length; i++)
		{
			if (GroundSurfaceMaster.surfaceTypesStatic[surfaceTypes[i]].useColliderFriction)
			{
				frictions[i] = GetComponent<Collider>().material.dynamicFriction * 2f;
			}
			else
			{
				frictions[i] = GroundSurfaceMaster.surfaceTypesStatic[surfaceTypes[i]].friction;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying && (bool)terDat && surfaceTypes.Length != terDat.alphamapLayers)
		{
			ChangeSurfaceTypesLength();
		}
	}

	public void UpdateAlphamaps()
	{
		terrainAlphamap = terDat.GetAlphamaps(0, 0, terDat.alphamapWidth, terDat.alphamapHeight);
	}

	private void ChangeSurfaceTypesLength()
	{
		int[] array = surfaceTypes;
		surfaceTypes = new int[terDat.alphamapLayers];
		for (int i = 0; i < surfaceTypes.Length && i < array.Length; i++)
		{
			surfaceTypes[i] = array[i];
		}
	}

	public int GetDominantSurfaceTypeAtPoint(Vector3 pos)
	{
		float z = pos.z;
		Vector3 position = tr.position;
		float num = z - position.z;
		Vector3 size = terDat.size;
		float x = Mathf.Clamp01(num / size.z);
		float x2 = pos.x;
		Vector3 position2 = tr.position;
		float num2 = x2 - position2.x;
		Vector3 size2 = terDat.size;
		Vector2 vector = new Vector2(x, Mathf.Clamp01(num2 / size2.x));
		float num3 = 0f;
		int num4 = 0;
		float num5 = 0f;
		for (int i = 0; i < terrainAlphamap.GetLength(2); i++)
		{
			num5 = terrainAlphamap[Mathf.FloorToInt(vector.x * (float)(terDat.alphamapWidth - 1)), Mathf.FloorToInt(vector.y * (float)(terDat.alphamapHeight - 1)), i];
			if (num5 > num3)
			{
				num3 = num5;
				num4 = i;
			}
		}
		return surfaceTypes[num4];
	}

	public float GetFriction(int sType)
	{
		float result = 1f;
		for (int i = 0; i < surfaceTypes.Length; i++)
		{
			if (sType == surfaceTypes[i])
			{
				result = frictions[i];
				break;
			}
		}
		return result;
	}
}
