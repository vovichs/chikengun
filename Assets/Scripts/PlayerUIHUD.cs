using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHUD : MonoBehaviour
{
	public int id;

	[SerializeField]
	private Text playerName;

	[SerializeField]
	private Image hpLine;

	public CharacterMotor player;

	public RectTransform rectTransform;

	public Transform target;

	public void SetTeam(TeamID team)
	{
		playerName.color = GameController.instance.GetTeamColor(team);
		hpLine.color = GameController.instance.GetTeamColor(team);
	}

	public void SetPlayerName(string plName)
	{
		playerName.text = plName;
	}

	public void UpdateHP_K(float k)
	{
		RectTransform obj = hpLine.rectTransform;
		float num = 0f - (1f - k);
		Vector2 sizeDelta = hpLine.rectTransform.sizeDelta;
		float x = num * sizeDelta.x;
		Vector2 anchoredPosition = hpLine.rectTransform.anchoredPosition;
		obj.anchoredPosition = new Vector2(x, anchoredPosition.y);
	}

	public void ShowHPLine(bool show)
	{
		hpLine.transform.parent.gameObject.SetActive(show);
	}
}
