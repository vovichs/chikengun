using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
	public Text playerName;

	public RectTransform HPLine;

	public Text teamName;

	public Image teamIcon;

	public CharacterMotor characterMotor;

	private float initWidth;

	private void Start()
	{
		Vector2 sizeDelta = HPLine.sizeDelta;
		initWidth = sizeDelta.x;
	}

	private void Update()
	{
		if (Camera.main != null)
		{
			Transform transform = base.transform;
			Vector3 eulerAngles = Camera.main.transform.eulerAngles;
			transform.rotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
		}
	}

	public void SetPlayerName(string name)
	{
		playerName.text = name;
	}

	public void SetTeamName(TeamID t)
	{
		if (t == TeamID.None)
		{
			teamName.gameObject.SetActive(value: false);
			teamIcon.gameObject.SetActive(value: false);
		}
		else
		{
			teamName.text = MultiplayerController.instance.GetTeamCustomName(t);
			teamIcon.sprite = DataModel.instance.TeamIcon(t);
		}
		teamName.color = DataModel.instance.TeamColor(t);
	}

	public void UpdateHP()
	{
		if (!(characterMotor == null))
		{
			Vector2 sizeDelta = HPLine.parent.GetComponent<RectTransform>().sizeDelta;
			initWidth = sizeDelta.x;
			RectTransform hPLine = HPLine;
			float x = initWidth * characterMotor.HP / (float)characterMotor.playerInfo.max_hp;
			Vector2 sizeDelta2 = HPLine.sizeDelta;
			hPLine.sizeDelta = new Vector2(x, sizeDelta2.y);
		}
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
