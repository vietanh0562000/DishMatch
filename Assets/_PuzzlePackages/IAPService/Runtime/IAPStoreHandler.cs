using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public interface IAPStoreHandler
    {
        PurchaseStore Store { get; }
        void SetCanMakePurchase(ConfigurationBuilder builder, ref bool canMakePurchase);
        void OnInitialized(IExtensionProvider extensions);
        bool IsPurchasedDeferred(Product product);
        void RestorePurchase();
        void AppsflyerValidation(Dictionary<string, object> dict, string data, Action<string> success, Action<string> fraud);
        Dictionary<string, string> GetProductDetails();
    }
}   