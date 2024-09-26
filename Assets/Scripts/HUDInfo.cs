using UnityEngine;
using UnityEngine.UI;

public class HUDInfo : MonoBehaviour
{
	public Text item_name;

	public RawImage armorCountImage;

	public void UpdateView(float curHP, float maxHP)
	{
		Vector2 sizeDelta = armorCountImage.rectTransform.sizeDelta;
		float x = sizeDelta.x;
		float num = curHP / maxHP;
		float num2 = 0f - x + x * num;
		RectTransform rectTransform = armorCountImage.rectTransform;
		float x2 = num2;
		Vector2 anchoredPosition = armorCountImage.rectTransform.anchoredPosition;
		rectTransform.anchoredPosition = new Vector2(x2, anchoredPosition.y);
		if (num < 0.25f)
		{
			armorCountImage.color = new Color(0.9f, 0f, 0f);
		}
		else if (num < 0.5f)
		{
			armorCountImage.color = new Color(0.9f, 0.6f, 0f);
		}
		else
		{
			armorCountImage.color = new Color(0.1f, 0.9f, 0f);
		}
	}

	public void SetName(string str)
	{
		item_name.text = str;
	}

	private void Update()
	{
	}

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}
}
