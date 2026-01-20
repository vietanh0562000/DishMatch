namespace PuzzleGames
{
    using System;
    using com.ootii.Messages;
    using DG.Tweening;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class InGameGoldTextUI : AResourceUI
    {
        public TextMeshProUGUI txtGold;
        public Button          btnGetMoreGold;
        public Canvas          canvasOrder;
        public ParticleSystem  fx;

        private void UpdateUI_GoldAndStar() { txtGold.SetText(FormatNumber.FormatKMBNumber(UserResourceController.instance.UserResource.gold)); }

        public override ResourceType Type => ResourceType.Gold;

        protected override void Start()
        {
            base.Start();

            btnGetMoreGold.onClick.RemoveAllListeners();

            btnGetMoreGold.onClick.AddListener(() => { WindowManager.Instance.OpenWindow<ShopInGamePanel>(); });

            MessageDispatcher.AddListener(EventID.DISABLE_BTN_SHOP,
                _ => { btnGetMoreGold.onClick.RemoveAllListeners(); }, true);
        }

        private void OnDisable()
        {
            MessageDispatcher.RemoveListener(EventID.DISABLE_BTN_SHOP,
                _ => { btnGetMoreGold.onClick.RemoveAllListeners(); }, true);
        }

        public override void OnReachUI(bool isLast)
        {
            if (fx)
                fx.Play();
            
            HapticController.instance.Play();
            
            if (isLast)
            {
                txtGold.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3).OnComplete(UpdateUI);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    if (canvasOrder.overrideSorting)
                    {
                        canvasOrder.overrideSorting = false;
                    }
                });
            }
            
            AudioController.PlaySound(SoundKind.UIRecivedGold);
        }

        public override void EnableCanvas()
        {
            if (!canvasOrder.overrideSorting)
            {
                canvasOrder.overrideSorting = true;
            }
        }

        public override void UpdateUI() { UpdateUI_GoldAndStar(); }
    }
}