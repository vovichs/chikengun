using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FreeMoneyScreen : BaseScreen
{
	public int rewardCoins = 5;

	public GameObject mainWindow;

	public GameObject RewardWindow;

	public GameObject FailedLoadWindow;

	public Text balanceLabel;

	public Text rewardLabel;

	public Text rewardLabelSuccess;

	private IEnumerator Start()
	{
		yield return null;
		AdsController.RewardedVideoFinished = (Action)Delegate.Combine(AdsController.RewardedVideoFinished, new Action(HandleRewardBasedVideoRewarded));
		AdsController.RewardedVideoFailed = (Action)Delegate.Combine(AdsController.RewardedVideoFailed, new Action(HandleRewardBasedVideoFailedToLoad));
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Combine(LocalStore.CurrencyBalanceChanged, new Action<int>(UpdateBalance));
		UpdateBalance(LocalStore.currencyBalance);
		rewardLabel.text = rewardCoins.ToString();
		rewardLabelSuccess.text = rewardCoins.ToString();
	}

	public void OnWatchVideoBtnClick()
	{
		AdsController.instance.ShowRewardedVideo();
	}

	private void HandleRewardBasedVideoRewarded()
	{
		StartCoroutine(VideoSuccess());
	}

	private IEnumerator VideoSuccess()
	{
		MonoBehaviour.print("give coins");
		RewardWindow.SetActive(value: true);
		LocalStore.GiveMoney(rewardCoins);
		mainWindow.SetActive(value: false);
		yield return new WaitForSeconds(1.2f);
		RewardWindow.SetActive(value: false);
		mainWindow.SetActive(value: true);
	}

	private void HandleRewardBasedVideoFailedToLoad()
	{
		StartCoroutine(VideoFail());
	}

	private IEnumerator VideoFail()
	{
		UnityEngine.Debug.Log("VideoFail");
		mainWindow.SetActive(value: false);
		FailedLoadWindow.SetActive(value: true);
		yield return new WaitForSeconds(1.2f);
		FailedLoadWindow.SetActive(value: false);
		mainWindow.SetActive(value: true);
	}

	private void OnDestroy()
	{
		LocalStore.CurrencyBalanceChanged = (Action<int>)Delegate.Remove(LocalStore.CurrencyBalanceChanged, new Action<int>(UpdateBalance));
		AdsController.RewardedVideoFinished = (Action)Delegate.Remove(AdsController.RewardedVideoFinished, new Action(HandleRewardBasedVideoRewarded));
		AdsController.RewardedVideoFailed = (Action)Delegate.Remove(AdsController.RewardedVideoFailed, new Action(HandleRewardBasedVideoFailedToLoad));
	}

	private void UpdateBalance(int balance)
	{
		balanceLabel.text = balance.ToString();
	}

	public void ShowMainWindow()
	{
		mainWindow.SetActive(value: true);
	}
}
