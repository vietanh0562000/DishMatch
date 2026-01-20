namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using Cysharp.Threading.Tasks;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    [Popup("BuyBoosterPanel", closeWhenClickOnBackdrop = false)]
    public class BuyBoosterPanel : BasePopup
    {
        [Space] [SerializeField] private TMP_Text priceTMP;
        [SerializeField]         private TMP_Text nameTMP;
        [SerializeField]         private TMP_Text detailTMP;
        [SerializeField]         private TMP_Text amountTMP;
        [SerializeField]         private Image    iconImage;

        [Space] [SerializeField] private Button button;
        
        [FoldoutGroup("Text Material")] [SerializeField]
        private Material _canBuy, _cantBuy;

        private PowerUpResource _powerUpManager;

        protected override void Awake()
        {
            base.Awake();
            button.onClick.AddListener(OnClickButton);
        }

        public override void Init()
        {
            GameManager.Instance.Stop();
            UserResourceController.onAddGold -= i => UpdateGold();
            UserResourceController.onAddGold += i => UpdateGold();
        }

        public override UniTask WillPopExit(Memory<object> args)
        {
            GameManager.Instance.Continue();
            UserResourceController.onAddGold -= i => UpdateGold();
            return base.WillPopExit(args);
        }

        private void OnClickButton()
        {
            if (_powerUpManager.EnoughResourceToBuy)
            {
                _powerUpManager.ExchangeResource();
                FlyManager.Instance.ShowFly(_powerUpManager.Type, _powerUpManager.Amount);
                ResourceType.Gold.Manager()?.UI.UpdateUI();
                CloseView();
            }
            else
            {
                WindowManager.Instance.OpenWindow<ShopInGamePanel>();
            }
        }

        public void BuyBooster(PowerupKind type)
        {
            _powerUpManager = type.ToResourceType().Manager() as PowerUpResource;

            if (!_powerUpManager)
            {
                return;
            }

            nameTMP.SetText(_powerUpManager.Kind.GetDescription());
            detailTMP.SetText(_powerUpManager.Detail);
            amountTMP.SetText($"x{_powerUpManager.Amount}");
            iconImage.sprite = _powerUpManager.GetIcon();
            iconImage.SetNativeSize();

            UpdateGold();
        }

        private void UpdateGold()
        {
            var price = _powerUpManager.Price;
            if (_powerUpManager.EnoughResourceToBuy)
            {
                priceTMP.fontMaterial = _canBuy;
                priceTMP.text         = "<sprite name=\"coin\">" + price;
            }
            else
            {
                priceTMP.fontMaterial = _cantBuy;
                priceTMP.text         = "<sprite name=\"coin\"> <color=\"red\">" + price + "</color>";
            }
        }
    }
}