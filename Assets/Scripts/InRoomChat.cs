using Photon;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class InRoomChat : Photon.MonoBehaviour
{
	public Rect GuiRect = new Rect(0f, 0f, 250f, 300f);

	public bool IsVisible = true;

	public bool AlignBottom;

	public List<string> messages = new List<string>();

	private string inputLine = string.Empty;

	private Vector2 scrollPos = Vector2.zero;

	public static readonly string ChatRPC = "Chat";

	public void Start()
	{
		if (AlignBottom)
		{
			GuiRect.y = (float)Screen.height - GuiRect.height;
		}
	}

	public void OnGUI()
	{
		if (!IsVisible || !PhotonNetwork.inRoom)
		{
			return;
		}
		if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return))
		{
			if (!string.IsNullOrEmpty(inputLine))
			{
				base.photonView.RPC("Chat", PhotonTargets.All, inputLine);
				inputLine = string.Empty;
				GUI.FocusControl(string.Empty);
				return;
			}
			GUI.FocusControl("ChatInput");
		}
		GUI.SetNextControlName(string.Empty);
		GUILayout.BeginArea(GuiRect);
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		GUILayout.FlexibleSpace();
		for (int num = messages.Count - 1; num >= 0; num--)
		{
			GUILayout.Label(messages[num]);
		}
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("ChatInput");
		inputLine = GUILayout.TextField(inputLine);
		if (GUILayout.Button("Send", GUILayout.ExpandWidth(expand: false)))
		{
			base.photonView.RPC("Chat", PhotonTargets.All, inputLine);
			inputLine = string.Empty;
			GUI.FocusControl(string.Empty);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}

	[PunRPC]
	public void Chat(string newLine, PhotonMessageInfo mi)
	{
		string str = "anonymous";
		if (mi.sender != null)
		{
			str = (string.IsNullOrEmpty(mi.sender.NickName) ? ("player " + mi.sender.ID) : mi.sender.NickName);
		}
		messages.Add(str + ": " + newLine);
	}

	public void AddLine(string newLine)
	{
		messages.Add(newLine);
	}
}
