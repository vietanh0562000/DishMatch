namespace PuzzleGames
{
    using System;
    using UnityEngine;
    using UnityEngine.Purchasing;
    using BasePuzzle.PuzzlePackages.IAPService;
    using Sirenix.OdinInspector;
    using TMPro;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class PurchaseGoldPackage : MonoBehaviour
    {
        [SerializeField, ReadOnly] private GoldPack pack;

        [FoldoutGroup("BindData"), SerializeField]
        private TMP_Text _txtPrice;

        [FoldoutGroup("BindData"), SerializeField]
        private TMP_Text _txtGold;
        
        [FoldoutGroup("BindData"), SerializeField]
        private TMP_Text _txtBonus;

        [FoldoutGroup("BindData"), SerializeField]
        private Image _iconImg;
        
        [SerializeField] private UnityEvent<PurchaseFailure> _onFalure;

        private Action _onSuccess;

        public void OnSuccess(Action onSuccess) { _onSuccess = onSuccess; }
        
        public void SetPack(GoldPack goldPack)
        {
            pack            = goldPack;
            _iconImg.sprite = pack.Icon;
            //_iconImg.SetNativeSize();

            string gold = pack.BonusValue > 0
                ? $"+{pack.BonusValue}"
                : "";
            _txtBonus.SetText(gold);
            _txtGold.SetText($"{pack.GoldValue}");
        }

        private void OnEnable()
        {
            var localizedPrice = InAppPurchaser.GetLocalizedPrice(pack.ID.GetPurchasePKG());
            if (!string.IsNullOrEmpty(localizedPrice))
                _txtPrice.text = localizedPrice;
        }

        public void ClickBtnPurchase()
        {
            AudioController.PlaySound(SoundKind.UIClickButton);
            InAppPurchaser.Purchase(pack.ID.GetPurchasePKG(), OnSuccess, OnFailure);
        }

        private void OnSuccess(Product product)
        {
            _onSuccess?.Invoke();
            var finalGold = pack.GoldValue + pack.BonusValue;

            ResourceType.Gold.Manager().Add(finalGold);
            FlyManager.Instance.ShowFly(ResourceType.Gold, finalGold);
        }

        private void OnFailure(PurchaseFailure purchaseFailure)
        {
            Debug.LogError($"{gameObject.name} > purchaseFailure: {purchaseFailure.Message}");
            _onFalure?.Invoke(purchaseFailure);
        }
    }
}