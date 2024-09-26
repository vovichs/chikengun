using System.Collections;
using UnityEngine;

public class HipPointsWidget : MonoBehaviour
{
	public static HipPointsWidget instance;

	public RectTransform darkBack;

	public RectTransform line;

	private float inversMaxHp = -1f;

	private void Awake()
	{
		instance = this;
	}

	private IEnumerator Start()
	{
		while (GameController.instance == null || GameController.instance.OurPlayer == null)
		{
			yield return null;
		}
		UpdateHP(GameController.instance.OurPlayer.playerInfo.max_hp);
	}

	public void UpdateHP(float value)
	{
		if (!(darkBack == null))
		{
			if (inversMaxHp < 0f)
			{
				inversMaxHp = 1f / (float)GameController.instance.OurPlayer.playerInfo.max_hp;
			}
			Vector2 sizeDelta = darkBack.sizeDelta;
			float x = sizeDelta.x;
			float num = x * value * inversMaxHp;
			RectTransform rectTransform = line;
			float x2 = num;
			Vector2 sizeDelta2 = line.sizeDelta;
			rectTransform.sizeDelta = new Vector2(x2, sizeDelta2.y);
		}
	}
}
