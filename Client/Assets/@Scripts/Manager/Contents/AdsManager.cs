using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager
{
	public void Init(Action onAdsInit)
    {
		MobileAds.Initialize(initStatus =>
		{
			var isInitSuccess = true;
			var statusMap = initStatus.getAdapterStatusMap();
			foreach (var status in statusMap)
			{
				var className = status.Key;
				var adapterStatus = status.Value;
				Debug.Log($"Adapter: {className}, State: {adapterStatus.InitializationState}, Description: {adapterStatus.Description}");
				if (adapterStatus.InitializationState != AdapterState.Ready)
				{
					isInitSuccess = false;
				}
			}

			if (isInitSuccess)
			{
				onAdsInit?.Invoke();
				Debug.Log($"Google Ads �ʱ�ȭ ����");
			}
			else
			{
				Debug.LogError($"Google Ads �ʱ�ȭ ����");
			}
		});

		LoadDailyFreeGemRewardedAd();
	}

	private RewardedAd _dailFreeDiamondRewardAd;
	//private string _dailFreeDiamondRewardAdUnitId = "ca-app-pub-2745919206689978/7526431956"; // ���� ����
	private string _dailFreeDiamondRewardAdUnitId = "ca-app-pub-3940256099942544/5224354917";	// �׽�Ʈ

	private void LoadDailyFreeGemRewardedAd()
	{
		var adRequest = new AdRequest();

		RewardedAd.Load(_dailFreeDiamondRewardAdUnitId, adRequest,
			(RewardedAd ad, LoadAdError error) =>
			{
				if (error != null || ad == null)
				{
					Debug.LogError($"������ ���� �ε� ���� : {error}");
					return;
				}

				Debug.Log($"������ ���� �ε� ���� : {ad.GetResponseInfo()}");
				_dailFreeDiamondRewardAd = ad;
				ListenToDailyFreeGemRewardedAdEvents();
			});
	}

	private void ListenToDailyFreeGemRewardedAdEvents()
	{
		if (_dailFreeDiamondRewardAd == null)
		{
			return;
		}

		_dailFreeDiamondRewardAd.OnAdPaid += (AdValue adValue) =>
		{
		};

		_dailFreeDiamondRewardAd.OnAdImpressionRecorded += () =>
		{
		};

		_dailFreeDiamondRewardAd.OnAdClicked += () =>
		{
		};

		_dailFreeDiamondRewardAd.OnAdFullScreenContentOpened += () =>
		{
		};

		_dailFreeDiamondRewardAd.OnAdFullScreenContentClosed += () =>
		{
			LoadDailyFreeGemRewardedAd();
		};

		_dailFreeDiamondRewardAd.OnAdFullScreenContentFailed += (AdError error) =>
		{
			LoadDailyFreeGemRewardedAd();
		};
	}

	public void ShowDailyFreeGemRewardedAd(Action onRewardDailyFreeGemAd = null)
	{
		if (_dailFreeDiamondRewardAd != null && _dailFreeDiamondRewardAd.CanShowAd())
		{
			_dailFreeDiamondRewardAd.Show((GoogleMobileAds.Api.Reward reward) =>
			{
				onRewardDailyFreeGemAd?.Invoke();
			});
		}
		else
		{
			Debug.LogError($"������ ���� �غ� �ȵ�.");
		}
	}

	public void Close()
	{
		if (_dailFreeDiamondRewardAd != null)
		{
			_dailFreeDiamondRewardAd.Destroy();
			_dailFreeDiamondRewardAd = null;
		}
	}
}
