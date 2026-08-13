using System;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace GameLecture.Framework.Samples
{
    [CreateAssetMenu(fileName = "AdsConfig", menuName = "GameLecture/Framework/Ads Config")]
    public sealed class AdsConfig : ScriptableObject
    {
        [Header("Android")]
        [SerializeField] private string androidAppKey;
        [SerializeField] private string androidInterstitialAdUnitId;
        [SerializeField] private string androidRewardedAdUnitId;

        [Header("iOS")]
        [SerializeField] private string iosAppKey;
        [SerializeField] private string iosInterstitialAdUnitId;
        [SerializeField] private string iosRewardedAdUnitId;

        public string AppKey
        {
            get
            {
#if UNITY_ANDROID
                return androidAppKey;
#elif UNITY_IOS
                return iosAppKey;
#else
                return string.Empty;
#endif
            }
        }

        public string InterstitialAdUnitId
        {
            get
            {
#if UNITY_ANDROID
                return androidInterstitialAdUnitId;
#elif UNITY_IOS
                return iosInterstitialAdUnitId;
#else
                return string.Empty;
#endif
            }
        }

        public string RewardedAdUnitId
        {
            get
            {
#if UNITY_ANDROID
                return androidRewardedAdUnitId;
#elif UNITY_IOS
                return iosRewardedAdUnitId;
#else
                return string.Empty;
#endif
            }
        }
    }

    public sealed class LevelPlayAdsManager : Singleton<LevelPlayAdsManager>
    {
        private LevelPlayInterstitialAd _interstitial;
        private LevelPlayRewardedAd _rewarded;
        private Action _rewardCallback;

        public void Initialize(AdsConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.AppKey))
            {
                Debug.LogWarning("LevelPlay is not initialized on this Editor platform. Build for Android or iOS.");
                return;
            }

            LevelPlay.OnInitSuccess += OnInitializationSucceeded;
            LevelPlay.OnInitFailed += OnInitializationFailed;
            LevelPlay.Init(config.AppKey);

            _interstitial = new LevelPlayInterstitialAd(config.InterstitialAdUnitId);
            _interstitial.OnAdLoaded += OnInterstitialLoaded;
            _interstitial.OnAdLoadFailed += OnInterstitialLoadFailed;
            _interstitial.OnAdDisplayFailed += OnInterstitialDisplayFailed;

            _rewarded = new LevelPlayRewardedAd(config.RewardedAdUnitId);
            _rewarded.OnAdLoaded += OnRewardedLoaded;
            _rewarded.OnAdLoadFailed += OnRewardedLoadFailed;
            _rewarded.OnAdDisplayFailed += OnRewardedDisplayFailed;
            _rewarded.OnAdRewarded += OnRewarded;
        }

        public void ShowInterstitial()
        {
            if (_interstitial == null)
                throw new InvalidOperationException("Call Initialize before showing an ad.");
            if (_interstitial.IsAdReady())
                _interstitial.ShowAd();
            else
                _interstitial.LoadAd();
        }

        public void ShowRewarded(Action onRewarded)
        {
            if (_rewarded == null)
                throw new InvalidOperationException("Call Initialize before showing an ad.");
            _rewardCallback = onRewarded;
            if (_rewarded.IsAdReady())
                _rewarded.ShowAd();
            else
                _rewarded.LoadAd();
        }

        private void OnInterstitialLoaded(LevelPlayAdInfo info) => _interstitial.ShowAd();
        private static void OnInterstitialLoadFailed(LevelPlayAdError error) =>
            Debug.LogError($"LevelPlay interstitial load failed: {error}");
        private static void OnInterstitialDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error) =>
            Debug.LogError($"LevelPlay interstitial display failed: {error}");

        private void OnRewardedLoaded(LevelPlayAdInfo info) => _rewarded.ShowAd();
        private static void OnRewardedLoadFailed(LevelPlayAdError error) =>
            Debug.LogError($"LevelPlay rewarded load failed: {error}");
        private static void OnRewardedDisplayFailed(LevelPlayAdInfo info, LevelPlayAdError error) =>
            Debug.LogError($"LevelPlay rewarded display failed: {error}");

        private void OnRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            _rewardCallback?.Invoke();
            _rewardCallback = null;
        }

        private static void OnInitializationSucceeded(LevelPlayConfiguration configuration) =>
            Debug.Log("LevelPlay initialized.");
        private static void OnInitializationFailed(LevelPlayInitError error) =>
            Debug.LogError($"LevelPlay initialization failed: {error}");
    }
}
