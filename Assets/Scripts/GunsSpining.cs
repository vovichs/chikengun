using System;
using UnityEngine;

[Serializable]
public class GunsSpining : MonoBehaviour
{
	public void Update()
	{
		transform.Rotate(Time.deltaTime * 250f, 0f, 0f);
	}

	public void Main()
	{
	}
}
