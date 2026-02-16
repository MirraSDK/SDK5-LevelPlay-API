using MirraGames.SDK.Common;
using UnityEngine;

namespace MirraGames.SDK.LevelPlay
{
    [ProviderConfiguration(typeof(LevelPlayAds))]
    public class LevelPlayAds_Configuration : PropertyGroup
    {
        public override string Name => nameof(LevelPlayAds);

        [field: SerializeField] public string AppKey { get; private set; } = "";
        [field: SerializeField] public string BannerAdUnitId { get; private set; } = "";
        [field: SerializeField] public string InterstitialAdUnitId { get; private set; } = "";
        [field: SerializeField] public string RewardedAdUnitId { get; private set; } = "";

        public override StringProperty[] GetStringProperties()
        {
            return new StringProperty[] {
                new(
                    "App Key",
                    getter: () => { return AppKey; },
                    setter: (value) => { AppKey = value; }
                ),
                new(
                    "Banner Ad Unit Id",
                    getter: () => { return BannerAdUnitId; },
                    setter: (value) => { BannerAdUnitId = value; }
                ),
                new(
                    "Interstitial Ad Unit Id",
                    getter: () => { return InterstitialAdUnitId; },
                    setter: (value) => { InterstitialAdUnitId = value; }
                ),
                new(
                    "Rewarded Ad Unit Id",
                    getter: () => { return RewardedAdUnitId; },
                    setter: (value) => { RewardedAdUnitId = value; }
                ),
            };
        }

    }
}
