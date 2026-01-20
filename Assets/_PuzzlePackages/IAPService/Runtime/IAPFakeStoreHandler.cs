using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class IAPFakeStoreHandler : IAPStoreHandler
    {
        public PurchaseStore Store => PurchaseStore.FakeStore;

        public bool IsPurchasedDeferred(Product product)
        {
            return false;
        }

        public IAPFakeStoreHandler()
        {
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
        }

        public void SetCanMakePurchase(ConfigurationBuilder builder, ref bool canMakePurchase)
        {
        }

        public void OnInitialized(IExtensionProvider extensions)
        {
        }

        public void RestorePurchase()
        {
        }

        public void AppsflyerValidation(Dictionary<string, object> dict, string data, Action<string> success, Action<string> fraud)
        {
        }

        public Dictionary<string, string> GetProductDetails()
        {
            return null;
        }
    }
}