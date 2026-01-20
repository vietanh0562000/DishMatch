using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    using SRF;

    public class IAPAppstoreHandler : IAPStoreHandler
    {
        public PurchaseStore Store => PurchaseStore.Apple;
        private IAppleExtensions _appleExtensions;

        public bool IsPurchasedDeferred(Product product)
        {
            return false;
        }

        public void SetCanMakePurchase(ConfigurationBuilder builder, ref bool canMakePurchase)
        {
            canMakePurchase = builder.Configure<IAppleConfiguration>().canMakePayments;
        }

        public void OnInitialized(IExtensionProvider extensions)
        {
            Debug.Log("IAPAppstoreHandler > In-App Purchasing successfully initialized");

            _appleExtensions = extensions.GetExtension<IAppleExtensions>();
            _appleExtensions.RegisterPurchaseDeferredListener(OnDeferredPurchase);
        }

        public void RestorePurchase()
        {
            _appleExtensions.RestoreTransactions((result, error) =>
            {
                Debug.Log(result
                    ? "IAPAppstoreHandler > the restoration process succeeded"
                    : $"IAPAppstoreHandler > Restoration failed: {error}");
            });
        }

        public void AppsflyerValidation(
            Dictionary<string, object> dictionary, string validationInfo, Action<string> success, Action<string> fraud)
        {
            foreach (var variable in dictionary)
            {
                if (string.IsNullOrEmpty(variable.Key)) continue;

                try
                {
                    var v = variable.Value;
                    var json = Json.Serialize(v);
                    ProductReceiptIOS productReceiptIOS = JsonUtility.FromJson<ProductReceiptIOS>(json);

                    if (productReceiptIOS.result)
                    {
                        success?.Invoke(productReceiptIOS.receipt_data.transaction_id);
                    }
                    else
                    {
                        fraud?.Invoke(productReceiptIOS.receipt_data.transaction_id);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(IAPAppstoreHandler)} > Appsflyer Validation Error: {e.Message}");
                }
            }
        }

        public Dictionary<string, string> GetProductDetails()
        {
            return _appleExtensions?.GetProductDetails();
        }

        private void OnDeferredPurchase(Product product)
        {
            Debug.Log(
                $"IAPAppstoreHandler > OnDeferredPurchase ====== Purchase of {product.definition.id} is deferred");
        }
    }
}