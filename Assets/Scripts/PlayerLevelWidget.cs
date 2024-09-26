using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevelWidget : MonoBehaviour
{
	public RectTransform progressLine;

	public Text playerLevelLabel;

	private void Start()
	{
		StatisticsManager.ExpChanged = (Action)Delegate.Combine(StatisticsManager.ExpChanged, new Action(OnExpChanged));
		OnExpChanged();
	}

	private void OnExpChanged()
	{
		playerLevelLabel.text = DataModel.instance.PlayerLevelIndex.ToString();
		RectTransform t = progressLine;
		Vector2 sizeDelta = progressLine.sizeDelta;
		float num = 0f - sizeDelta.x;
		float num2 = (float)DataModel.PlayerExp * 1f / (float)DataModel.instance.CurrentLevelMaxExp();
		Vector2 sizeDelta2 = progressLine.sizeDelta;
		t.SetLocalPositionX(num + num2 * sizeDelta2.x);
	}

	private void OnDestroy()
	{
		StatisticsManager.ExpChanged = (Action)Delegate.Remove(StatisticsManager.ExpChanged, new Action(OnExpChanged));
	}
}
