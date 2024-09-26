using System;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMove : MonoBehaviour
{
	[Serializable]
	public struct BufferRecord
	{
		public Vector3 pos;

		public Quaternion rot;

		public double time;

		public BufferRecord(Vector3 pos, Quaternion rot, double time)
		{
			this.pos = pos;
			this.rot = rot;
			this.time = time;
		}
	}

	private List<BufferRecord> BufferedPositions = new List<BufferRecord>();

	private PhotonView photonView;

	public BufferRecord first;

	public int count;

	private void Start()
	{
		photonView = GetComponent<PhotonView>();
	}

	private void Update()
	{
		count = BufferedPositions.Count;
		if (!photonView.isMine)
		{
			UpdateClonePosition();
		}
	}

	public void AddStreamPack(Vector3 recivedPlayerPos, Quaternion recivedPlayerRot)
	{
		BufferRecord item = new BufferRecord(recivedPlayerPos, recivedPlayerRot, PhotonNetwork.time);
		BufferedPositions.Add(item);
		if (BufferedPositions.Count > 4)
		{
			BufferedPositions.RemoveAt(0);
		}
		if (first.time == 0.0)
		{
			first = item;
		}
	}

	private void UpdateClonePosition()
	{
		if (BufferedPositions.Count >= 2)
		{
			Predicate<BufferRecord> match = lastBuf;
			int num = BufferedPositions.FindLastIndex(match);
			if (num != -1)
			{
				if (num == BufferedPositions.Count - 1)
				{
					Transform transform = base.transform;
					BufferRecord bufferRecord = BufferedPositions[num];
					transform.position = bufferRecord.pos;
					Transform transform2 = base.transform;
					BufferRecord bufferRecord2 = BufferedPositions[num];
					transform2.rotation = bufferRecord2.rot;
					return;
				}
				double num2 = myPhotonTime();
				BufferRecord bufferRecord3 = BufferedPositions[num];
				double num3 = num2 - bufferRecord3.time;
				BufferRecord bufferRecord4 = BufferedPositions[num + 1];
				double time = bufferRecord4.time;
				BufferRecord bufferRecord5 = BufferedPositions[num];
				float t = (float)(num3 / (time - bufferRecord5.time));
				Transform transform3 = base.transform;
				BufferRecord bufferRecord6 = BufferedPositions[num];
				Vector3 pos = bufferRecord6.pos;
				BufferRecord bufferRecord7 = BufferedPositions[num + 1];
				transform3.position = Vector3.Lerp(pos, bufferRecord7.pos, t);
				Transform transform4 = base.transform;
				BufferRecord bufferRecord8 = BufferedPositions[num];
				Quaternion rot = bufferRecord8.rot;
				BufferRecord bufferRecord9 = BufferedPositions[num + 1];
				transform4.rotation = Quaternion.Lerp(rot, bufferRecord9.rot, t);
			}
			else
			{
				Transform transform5 = base.transform;
				BufferRecord bufferRecord10 = BufferedPositions[0];
				transform5.position = bufferRecord10.pos;
				Transform transform6 = base.transform;
				BufferRecord bufferRecord11 = BufferedPositions[0];
				transform6.rotation = bufferRecord11.rot;
			}
		}
		else if (BufferedPositions.Count == 1)
		{
			Transform transform7 = base.transform;
			BufferRecord bufferRecord12 = BufferedPositions[0];
			transform7.position = bufferRecord12.pos;
			Transform transform8 = base.transform;
			BufferRecord bufferRecord13 = BufferedPositions[0];
			transform8.rotation = bufferRecord13.rot;
		}
	}

	private double myPhotonTime()
	{
		return PhotonNetwork.time - 0.20000000298023224;
	}

	private bool lastBuf(BufferRecord arg)
	{
		return arg.time < myPhotonTime();
	}
}
