using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunTeams : MonoBehaviour
{
	public enum Team : byte
	{
		none,
		red,
		blue
	}

	public static Dictionary<Team, List<PhotonPlayer>> PlayersPerTeam;

	public const string TeamPlayerProp = "team";

	public void Start()
	{
		PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
		Array values = Enum.GetValues(typeof(Team));
		IEnumerator enumerator = values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				PlayersPerTeam[(Team)current] = new List<PhotonPlayer>();
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public void OnDisable()
	{
		PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
	}

	public void OnJoinedRoom()
	{
		UpdateTeams();
	}

	public void OnLeftRoom()
	{
		Start();
	}

	public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
	{
		UpdateTeams();
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		UpdateTeams();
	}

	public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		UpdateTeams();
	}

	public void UpdateTeams()
	{
		Array values = Enum.GetValues(typeof(Team));
		IEnumerator enumerator = values.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				PlayersPerTeam[(Team)current].Clear();
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
		{
			PhotonPlayer photonPlayer = PhotonNetwork.playerList[i];
			Team team = photonPlayer.GetTeam();
			PlayersPerTeam[team].Add(photonPlayer);
		}
	}
}
