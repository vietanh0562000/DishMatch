namespace PuzzleGames
{
    using System;
    using System.Collections;
    using ChuongCustom;
    using BasePuzzle.PuzzlePackages.Navigator;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;

    [Popup("RefillPanel", closeWhenClickOnBackdrop = false)]
    public class RefillPanel : BasePopup
    {
        public HeartTextUI     HeartTextUI;
        public TextMeshProUGUI txtPrice;
        public RectTransform buttonGroup;

        [FoldoutGroup("Text Material")] [SerializeField]
        private Material _canBuy, _cantBuy;
        
        private int  refillPrice = 900;
        private bool inHome;

        public override void Init()
        {
            base.Init();
            HeartTextUI.Push();
            buttonGroup.gameObject.SetActive(true);

            UserResourceController.onAddGold -= i => UpdatePrice();
            UserResourceController.onAddGold += i => UpdatePrice();
        }

        public void SetInHome(bool b)
        {
            inHome = b;
            SetRefillPrice(UserResourceController.instance.GOLD_TO_BUY_HEART);

            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (UserResourceController.instance.UserResource.gold < refillPrice)
            {
                txtPrice.fontMaterial = _cantBuy;
                txtPrice.text         = $" <sprite=1> <color=\"red\"> {refillPrice}</color>";
            }
            else
            {
                txtPrice.fontMaterial = _canBuy;
                txtPrice.text         = $" <sprite=1> {refillPrice}";
            }
        }

        public void SetRefillPrice(int price) { refillPrice = price; }
        
        public void ButtonClosePanel()
        {
            CloseView();

            if (inHome) return;
            if (UserResourceController.instance.UserResource.heart <= 0)
            {
                LoadSceneManager.Instance.LoadScene("Home");
            }
        }

        public void ButtonBuy()
        {
            var isMaxHearth   = UserResourceController.instance.IsMaxHeart();
            var isCanBuyHeart = UserResourceController.instance.CanBuyHeart();
            if (isMaxHearth)
            {
                Debug.LogError("live is full");
            }
            else
            {
                if (isCanBuyHeart)
                {
                    ShowFly(UserResourceController.instance.GetMaxHeart() -
                            UserResourceController.instance.UserResource.heart);
                    UserResourceController.instance.BuyHeart();
                }
                else
                {
                    if (inHome)
                    {
                        CloseView();
                        Navigator.Instance.MoveToTab(0);
                    }
                    else
                        WindowManager.Instance.OpenWindow<ShopInGamePanel>();
                }
            }
        }

        public void ButtonWatchAds()
        {
            var isMaxHearth = UserResourceController.instance.IsMaxHeart();
            if (isMaxHearth)
            {
                Debug.LogError("live is full");
            }
            else
            {
                UserResourceController.instance.WatchAds_FreeHeart(() =>
                {
                    ShowFly(1);
                });
            }
        }

        private void ShowFly(int amount)
        {
            buttonGroup.gameObject.SetActive(false);
            HeartTextUI.onLastUpdate = DelayClosePanel;
            FlyManager.Instance.ShowFly(ResourceType.Heart, amount, buttonGroup);
        }

        private void DelayClosePanel()
        {
            StartCoroutine(COClose());
            
            IEnumerator COClose()
            {
                yield return new WaitForSecondsRealtime(0.5f);
                ButtonClosePanel();
            }
        }

        public override void DidPopExit(Memory<object> args)
        {
            HeartTextUI.Pop();
            UserResourceController.onAddGold -= i => UpdatePrice();
            
            base.DidPopExit(args);
        }
    }
}