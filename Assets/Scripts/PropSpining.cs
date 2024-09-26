using System;
using UnityEngine;

[Serializable]
public class PropSpining : MonoBehaviour
{
	public void Update()
	{
		transform.Rotate(0f, Time.deltaTime * 500f, 0f);
	}

	public void Main()
	{
	}
}
