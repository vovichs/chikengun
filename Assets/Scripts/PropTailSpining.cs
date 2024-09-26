using System;
using UnityEngine;

[Serializable]
public class PropTailSpining : MonoBehaviour
{
	public void Update()
	{
		transform.Rotate(Time.deltaTime * 500f, 0f, 0f);
	}

	public void Main()
	{
	}
}
