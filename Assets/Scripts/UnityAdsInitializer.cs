using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsInitializer : MonoBehaviour
{
	[SerializeField]
	private string androidGameId = "18658";

	[SerializeField]
	private string iosGameId = "18660";

	[SerializeField]
	private bool testMode;

	private void Start()
	{
		string gameId = null;
		if (DataModel.isAndroid)
		{
			gameId = androidGameId;
		}
		if (DataModel.isIOS)
		{
			gameId = iosGameId;
		}
	}
}
