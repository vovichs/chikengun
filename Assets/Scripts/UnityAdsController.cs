using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UnityAdsController : MonoBehaviour
{
	public static Action RewardedVideoFinished;

	public static Action RewardedVideoFailed;

	[CompilerGenerated]
	private static Action<ShowResult> _003C_003Ef__mg_0024cache0;

	public static void ShowAd()
	{
		UnityEngine.Debug.Log("UnityAds call  isReady = " + Advertisement.IsReady());
	}

	public static void ShowRewardedAd()
	{
		
	}

	private static void HandleShowResult(ShowResult result)
	{
		AdsController.instance.isShowingAdsNow = false;
	}
}
