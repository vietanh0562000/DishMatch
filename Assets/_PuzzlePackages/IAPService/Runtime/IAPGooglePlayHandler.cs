using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class IAPGooglePlayHandler : IAPStoreHandler
    {
        public PurchaseStore Store => PurchaseStore.Google;
        private IGooglePlayStoreExtensions _googleExtensions;

        public bool IsPurchasedDeferred(Product product)
        {
            return _googleExtensions != null && _googleExtensions.IsPurchasedProductDeferred(product);
        }
        
        public void SetCanMakePurchase(ConfigurationBuilder builder, ref bool canMakePurchase)
        {
            //Google Play không có tính năng này
        }
        
        public void OnInitialized(IExtensionProvider extensions)
        {
            Debug.Log("IAPGooglePlayHandler > In-App Purchasing successfully initialized");
            _googleExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
        }
        
        public  void RestorePurchase()
        {
            //Không có tác dụng trên Android
        }

        public void AppsflyerValidation(Dictionary<string, object> dict, string data, Action<string> success, Action<string> fraud)
        {
            try
            {
                if (dict.ContainsKey(IAPServiceConstant.PRODUCT_PURCHASE) &&
                    dict[IAPServiceConstant.PRODUCT_PURCHASE] != null)
                {
                    InAppPurchaseValidationResult iapObject =
                        JsonUtility.FromJson<InAppPurchaseValidationResult>(data);
                    success?.Invoke(iapObject.token);
                }
                else if (dict.ContainsKey(IAPServiceConstant.SUBSCRIPTION_PURCHASE) && dict[IAPServiceConstant.SUBSCRIPTION_PURCHASE] != null)
                {
                    SubscriptionValidationResult iapObject =
                        JsonUtility.FromJson<SubscriptionValidationResult>(data);
                    success?.Invoke(iapObject.token);
                }
                else if (dict.ContainsKey(IAPServiceConstant.FAILURE_DATA) && dict[IAPServiceConstant.FAILURE_DATA] != null)
                {
                    InAppPurchaseValidationResult iapObject =
                        JsonUtility.FromJson<InAppPurchaseValidationResult>(data);
                    fraud?.Invoke(iapObject.token);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"{nameof(AppsflyerValidation)} > {e.Message}");
            }
        }
        
        public Dictionary<string, string> GetProductDetails()
        {
            //Return null vì hàm này chỉ có tác dụng cho ios
            return null;
        }
    }
}