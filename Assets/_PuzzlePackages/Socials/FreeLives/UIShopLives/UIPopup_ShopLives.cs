namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using BasePuzzle.PuzzlePackages.Core;
    using BasePuzzle.PuzzlePackages.Navigator;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [Popup("UIPopup_ShopLives", closeWhenClickOnBackdrop: true)]
    public class UIPopup_ShopLives : BasePopup
    {
        public HeartTextUI HeartTextUI;

        [Header("Full Vitality")] public Button          btnBuy;
        public                           TextMeshProUGUI txtPrice;

        [Header("Watch Ads")] public RectTransform rectBtnWatchAds;
        public                       Button        btnWatchAds;

        private Action _onClosePopup;

        private void Start() { }

        public override void Init()
        {
            _onClosePopup = null;
            HeartTextUI.Pop();
            UpdateUI();
        }
        public override void DidPopExit(Memory<object> args)
        {
            _onClosePopup?.Invoke();
            UserResourceController.instance.GetFreeLive();
            base.DidPopExit(args);
        }

        private void UpdateUI()
        {
            btnBuy.onClick.RemoveAllListeners();
            btnBuy.onClick.AddListener(() =>
            {
                var isMaxHearth   = UserResourceController.instance.IsMaxHeart();
                var isCanBuyHearh = UserResourceController.instance.CanBuyHeart();
                if (isMaxHearth)
                {
                    UIToastManager.Instance.Show(LocalizationHelper.GetTranslation("live_notify_full"));
                }
                else
                {
                    if (isCanBuyHearh)
                    {
                        var amountHeart = 5 - UserResourceController.instance.UserResource.heart;
                        UserResourceController.instance.BuyHeart();

                        _onClosePopup = () => { FlyManager.Instance.ShowFly(ResourceType.Heart, amountHeart); };
                    }
                    else
                    {
                        _onClosePopup = () => { Navigator.Instance.MoveToTab(0); };
                    }

                    CloseView();
                }
            });

            if (UserResourceController.instance.UserResource.gold < UserResourceController.instance.GOLD_TO_BUY_HEART)
            {
                txtPrice.text = "Refill<br>" +
                                $" <sprite=1> <color=\"red\"> {UserResourceController.instance.GOLD_TO_BUY_HEART}</color>";
            }
            else
            {
                txtPrice.text = "Refill<br>" + $" <sprite=1> {UserResourceController.instance.GOLD_TO_BUY_HEART}";
            }

            btnWatchAds.onClick.RemoveAllListeners();
            btnWatchAds.onClick.AddListener(() =>
            {
                var isMaxHearth = UserResourceController.instance.IsMaxHeart();
                if (isMaxHearth)
                {
                    UIToastManager.Instance.Show(LocalizationHelper.GetTranslation("live_notify_full"));
                }
                else
                {
                    UserResourceController.instance.WatchAds_FreeHeart(() =>
                    {
                        _onClosePopup = () => { FlyManager.Instance.ShowFly(ResourceType.Heart, 1); };

                        CloseView();
                    });
                }
            });
            rectBtnWatchAds.gameObject.SetActive(true);
        }
    }
}