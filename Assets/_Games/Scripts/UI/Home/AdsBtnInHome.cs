namespace PuzzleGames
{
    using System;
    using com.ootii.Messages;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using UnityEngine;
    using UnityEngine.UI;

    public class AdsBtnInHome : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Awake() { MessageDispatcher.AddListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private void OnDestroy() { MessageDispatcher.RemoveListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private void OnBuyNoAds(IMessage rmessage) { gameObject.SetActive(false); }

        private void Start()
        {
            gameObject.SetActive(LevelDataController.instance.GetLevelJustPassed() >=
                                 ServerConfig.Instance<ValueRemoteConfig>().numLevelToShowInterstitial
                                 && !UserResourceController.instance.HasRemoveAds());

            _button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton() { WindowManager.Instance.OpenWindow<NoAdsPanel>(); }
    }
}