using MirraGames.SDK.Common;
using System;
using Unity.Services.LevelPlay;
using Logger = MirraGames.SDK.Common.Logger;
using LevelPlaySDK = Unity.Services.LevelPlay.LevelPlay;

namespace MirraGames.SDK.LevelPlay
{
    [Provider(typeof(IAds))]
    public class LevelPlayAds : CommonAds
    {
        private readonly LevelPlayAds_Configuration Configuration;

        private LevelPlayBannerAd BannerAd;
        private LevelPlayInterstitialAd InterstitialAd;
        private LevelPlayRewardedAd RewardedAd;

        private Action OnInterstitialOpen;
        private Action<bool> OnInterstitialClose;

        private Action OnRewardedOpen;
        private Action<bool> OnRewardedClose;
        private bool IsRewardedSuccess = false;

        public LevelPlayAds(LevelPlayAds_Configuration configuration, IEventAggregator eventAggregator) : base(eventAggregator)
        {
            Configuration = configuration;

            LevelPlaySDK.OnInitSuccess += OnInitializationSuccess;
            LevelPlaySDK.OnInitFailed += OnInitializationFailed;

            LevelPlaySDK.Init(Configuration.AppKey);
        }

        // Initialization callbacks.

        private void OnInitializationSuccess(LevelPlayConfiguration configuration)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnInitializationSuccess.");

            // Create and load banner ad.
            if (!string.IsNullOrEmpty(Configuration.BannerAdUnitId))
            {
                BannerAd = new LevelPlayBannerAd(Configuration.BannerAdUnitId);
                BannerAd.OnAdLoaded += OnBannerLoaded;
                BannerAd.OnAdLoadFailed += OnBannerFailedToLoad;
                BannerAd.OnAdDisplayed += OnBannerShown;
                BannerAd.OnAdDisplayFailed += OnBannerShowFailed;
                BannerAd.OnAdClicked += OnBannerClicked;
                BannerAd.OnAdCollapsed += OnBannerCollapsed;
                BannerAd.OnAdLeftApplication += OnBannerLeftApplication;
                BannerAd.OnAdExpanded += OnBannerExpanded;
                BannerAd.LoadAd();
            }

            // Create and load interstitial ad.
            if (!string.IsNullOrEmpty(Configuration.InterstitialAdUnitId))
            {
                InterstitialAd = new LevelPlayInterstitialAd(Configuration.InterstitialAdUnitId);
                InterstitialAd.OnAdLoaded += OnInterstitialLoaded;
                InterstitialAd.OnAdLoadFailed += OnInterstitialFailedToLoad;
                InterstitialAd.OnAdDisplayed += OnInterstitialShown;
                InterstitialAd.OnAdDisplayFailed += OnInterstitialShowFailed;
                InterstitialAd.OnAdClosed += OnInterstitialClosed;
                InterstitialAd.OnAdClicked += OnInterstitialClicked;
                InterstitialAd.LoadAd();
            }

            // Create and load rewarded ad.
            if (!string.IsNullOrEmpty(Configuration.RewardedAdUnitId))
            {
                RewardedAd = new LevelPlayRewardedAd(Configuration.RewardedAdUnitId);
                RewardedAd.OnAdLoaded += OnRewardedVideoLoaded;
                RewardedAd.OnAdLoadFailed += OnRewardedVideoFailedToLoad;
                RewardedAd.OnAdDisplayed += OnRewardedVideoShown;
                RewardedAd.OnAdDisplayFailed += OnRewardedVideoShowFailed;
                RewardedAd.OnAdClosed += OnRewardedVideoClosed;
                RewardedAd.OnAdRewarded += OnRewardedVideoRewarded;
                RewardedAd.OnAdClicked += OnRewardedVideoClicked;
                RewardedAd.LoadAd();
            }
        }

        private void OnInitializationFailed(LevelPlayInitError error)
        {
            Logger.CreateError(this, $"[LevelPlay] [Callback] OnInitializationFailed(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
        }

        // Banner implementation.

        protected override void InvokeBannerImpl()
        {
            BannerAd?.ShowAd();
        }

        protected override void RefreshBannerImpl()
        {
            BannerAd?.LoadAd();
        }

        protected override void DisableBannerImpl()
        {
            BannerAd?.HideAd();
        }

        // Interstitial implementation.

        protected override void InvokeInterstitialImpl(InterstitialParameters parameters, Action onOpen, Action<bool> onClose)
        {
            OnInterstitialOpen = onOpen;
            OnInterstitialClose = onClose;
            if (InterstitialAd != null && InterstitialAd.IsAdReady())
            {
                InterstitialAd.ShowAd();
            }
        }

        // Rewarded implementation.

        protected override void InvokeRewardedImpl(RewardedParameters parameters, Action onOpen, Action<bool> onClose)
        {
            OnRewardedOpen = onOpen;
            OnRewardedClose = onClose;
            IsRewardedSuccess = false;
            if (RewardedAd != null && RewardedAd.IsAdReady())
            {
                RewardedAd.ShowAd();
            }
        }

        // Banner callbacks.

        private void OnBannerLoaded(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnBannerLoaded(adUnitId:{adInfo.AdUnitId}).");
        }

        private void OnBannerFailedToLoad(LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnBannerFailedToLoad(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
        }

        private void OnBannerShown(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnBannerShown.");
            IsBannerVisible = true;
        }

        private void OnBannerShowFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnBannerShowFailed(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
            IsBannerVisible = false;
        }

        private void OnBannerClicked(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnBannerClicked.");
        }

        private void OnBannerCollapsed(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnBannerCollapsed.");
        }

        private void OnBannerLeftApplication(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnBannerLeftApplication.");
        }

        private void OnBannerExpanded(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnBannerExpanded.");
        }

        // Interstitial callbacks.

        private void OnInterstitialLoaded(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnInterstitialLoaded(adUnitId:{adInfo.AdUnitId}).");
        }

        private void OnInterstitialFailedToLoad(LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnInterstitialFailedToLoad(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
        }

        private void OnInterstitialShown(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnInterstitialShown.");
            OnInterstitialOpen?.Invoke();
            IsInterstitialVisible = true;
        }

        private void OnInterstitialShowFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnInterstitialShowFailed(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
            OnInterstitialClose?.Invoke(false);
            IsInterstitialVisible = false;
        }

        private void OnInterstitialClosed(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnInterstitialClosed.");
            OnInterstitialClose?.Invoke(true);
            IsInterstitialVisible = false;
            InterstitialAd?.LoadAd();
        }

        private void OnInterstitialClicked(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnInterstitialClicked.");
        }

        // Rewarded callbacks.

        private void OnRewardedVideoLoaded(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnRewardedVideoLoaded(adUnitId:{adInfo.AdUnitId}).");
        }

        private void OnRewardedVideoFailedToLoad(LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnRewardedVideoFailedToLoad(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
        }

        private void OnRewardedVideoShown(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnRewardedVideoShown.");
            OnRewardedOpen?.Invoke();
            IsRewardedVisible = true;
        }

        private void OnRewardedVideoShowFailed(LevelPlayAdInfo adInfo, LevelPlayAdError error)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnRewardedVideoShowFailed(errorCode:{error.ErrorCode}, errorMessage:{error.ErrorMessage}).");
            OnRewardedClose?.Invoke(false);
            IsRewardedVisible = false;
        }

        private void OnRewardedVideoClosed(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnRewardedVideoClosed.");
            OnRewardedClose?.Invoke(IsRewardedSuccess);
            IsRewardedVisible = false;
            RewardedAd?.LoadAd();
        }

        private void OnRewardedVideoRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            Logger.CreateText(this, $"[LevelPlay] [Callback] OnRewardedVideoRewarded(name:{reward.Name}, amount:{reward.Amount}).");
            IsRewardedSuccess = true;
        }

        private void OnRewardedVideoClicked(LevelPlayAdInfo adInfo)
        {
            Logger.CreateText(this, "[LevelPlay] [Callback] OnRewardedVideoClicked.");
        }
    }
}