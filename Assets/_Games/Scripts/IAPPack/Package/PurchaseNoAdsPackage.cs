namespace PuzzleGames
{
    using System.Collections;
    using com.ootii.Messages;
    using UnityEngine;
    using UnityEngine.Purchasing;
    using BasePuzzle.PuzzlePackages.IAPService;
    using UnityEngine.Events;

    public class PurchaseNoAdsPackage : MonoBehaviour
    {
        private PurchaseID _purchaseID => PurchaseID.no_ads_pack;

        [SerializeField] private UnityEvent<PurchaseFailure> _onFalure;

        private void OnEnable() { MessageDispatcher.AddListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private IEnumerator Start()
        {
            while (UserResourceController.instance.UserResource == null)
                yield return new WaitForSeconds(2f);

            var userResource = UserResourceController.instance.UserResource;
            gameObject.SetActive(!userResource.removeAds);
        }

        private void OnDestroy() { MessageDispatcher.RemoveListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private void OnBuyNoAds(IMessage rmessage) { gameObject.SetActive(false); }

        public void ClickBtnPurchase()
        {
            AudioController.PlaySound(SoundKind.UIClickButton);
            InAppPurchaser.Purchase(_purchaseID.GetPurchasePKG(), OnSuccess, OnFailure);
        }

        private void OnSuccess(Product product)
        {
            UserResourceController.instance.SetRemoveAds(true);
            UIToastManager.Instance.Show("Buy succeed!");
            MessageDispatcher.SendMessage(EventID.BUY_NO_ADS, 0);
        }

        private void OnFailure(PurchaseFailure purchaseFailure)
        {
            Debug.LogError($"{gameObject.name} > purchaseFailure: {purchaseFailure.Message}");
            _onFalure?.Invoke(purchaseFailure);
        }
    }
}