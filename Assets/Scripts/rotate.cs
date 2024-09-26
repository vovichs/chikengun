using System;
using UnityEngine;

[Serializable]
public class rotate : MonoBehaviour
{
	public float rotationspeed;

	public rotate()
	{
		rotationspeed = 30f;
	}

	public void Update()
	{
		transform.Rotate(Vector3.up * Time.deltaTime * rotationspeed);
	}

	public void Main()
	{
	}
}
