using SimpleJSON;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AdsWidget : MonoBehaviour
{
	[SerializeField]
	private GameObject AdsBtnPrefab;

	[SerializeField]
	private Transform container;

	[SerializeField]
	private GameObject window;

	private static bool dontShowOnThisSession;

	private void Awake()
	{
		if (dontShowOnThisSession)
		{
			base.gameObject.SetActive(value: false);
		}
		else
		{
			NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Combine(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
		}
	}

	private void OnDestroy()
	{
		NetworkManager.ConfigsLoaded = (Action<string>)Delegate.Remove(NetworkManager.ConfigsLoaded, new Action<string>(OnConfigsLoaded));
	}

	private void OnConfigsLoaded(string jsonAppData)
	{
		JSONNode jSONNode = JSON.Parse(jsonAppData);
		if (jSONNode != null)
		{
			IEnumerator enumerator = jSONNode["Apps"].AsArray.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					JSONNode jSONNode2 = (JSONNode)enumerator.Current;
					if (!Application.identifier.Equals(jSONNode2["Id"].Value))
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(AdsBtnPrefab);
						gameObject.transform.SetParent(container);
						gameObject.transform.localScale = Vector3.one;
						StartCoroutine(HandleApp(jSONNode2, gameObject));
					}
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
			window.SetActive(value: true);
		}
	}

	private IEnumerator HandleApp(JSONNode appNode, GameObject button)
	{
		button.GetComponent<Button>().onClick.AddListener(delegate
		{
			string value = appNode["UrlAndroid"].Value;
			if (DataModel.isIOS)
			{
				value = appNode["UrlIOS"].Value;
			}
			if (value != string.Empty)
			{
				Application.OpenURL(value);
			}
		});
		button.GetComponentInChildren<Text>().text = appNode["Name"];
		WWW www = new WWW(appNode["Icon"]);
		yield return www;
		Sprite sprite = Sprite.Create(www.texture, new Rect(0f, 0f, www.texture.width, www.texture.height), new Vector2(0f, 0f), 100f);
		button.GetComponentInChildren<Image>().sprite = sprite;
	}

	public void Close()
	{
		dontShowOnThisSession = true;
		base.gameObject.SetActive(value: false);
	}
}
