using UnityEngine;

namespace GameLecture.Framework.Samples
{
    public sealed class FrameworkBootstrap : MonoBehaviour
    {
        [SerializeField] private AdsConfig adsConfig;

        private void Start()
        {
            ResourceManager.Instance.LoadAll(onComplete: OnResourcesLoaded);
        }

        private void OnResourcesLoaded()
        {
            GameDataManager.Instance.Load();
            GameSaveManager.Instance.Load();
            if (adsConfig != null)
                LevelPlayAdsManager.Instance.Initialize(adsConfig);
        }
    }
}
