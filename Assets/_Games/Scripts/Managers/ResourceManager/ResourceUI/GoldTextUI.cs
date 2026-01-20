namespace PuzzleGames
{
    using System;
    using com.ootii.Messages;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages.Navigator;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class GoldTextUI : AResourceUI
    {
        public TextMeshProUGUI txtGold;
        public Button          btnGetMoreGold;
        public ParticleSystem  fx;

        private void UpdateUI_GoldAndStar() { txtGold.SetText(FormatNumber.FormatKMBNumber(UserResourceController.instance.UserResource.gold)); }

        public override ResourceType Type => ResourceType.Gold;

        protected override void Start()
        {
            base.Start();

            MessageDispatcher.AddListener(EventID.PURCHASE_GOLD, message => UpdateUI(), true);
            
            btnGetMoreGold.onClick.RemoveAllListeners();

            btnGetMoreGold.onClick.AddListener(() =>
            {
                if (Navigator.Instance)
                {
                    Navigator.Instance?.MoveToTab(0);
                }
                else
                {
                    WindowManager.Instance.OpenWindow<ShopInGamePanel>();
                }
            });
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener(EventID.PURCHASE_GOLD, message => UpdateUI(), true);
        }

        public override void OnReachUI(bool isLast)
        {
            if (fx)
                fx.Play();

            HapticController.instance.Play();
            
            if (isLast)
            {
                txtGold.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3).OnComplete(UpdateUI);
                MessageDispatcher.SendMessage(EventID.PURCHASE_GOLD, 0);
            }
            
            AudioController.PlaySound(SoundKind.UIRecivedGold);
        }
        public override void UpdateUI() { UpdateUI_GoldAndStar(); }
    }
}