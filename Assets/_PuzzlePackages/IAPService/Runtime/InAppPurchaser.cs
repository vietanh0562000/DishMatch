using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public static class InAppPurchaser
    {
        private static PurchaseHandler _handler;
        private static PurchaseHandler Handler => _handler ??= new PurchaseHandler();

        /// <summary>
        /// Được gọi khi thanh toán được khôi phục ngầm (Restored Purchase, but not in time (due to a timeout)
        /// </summary>
        public static Action<Product> onPurchaseRestored;

        /// <summary>
        /// Gọi khi một thanh toán được cho là hack
        /// QUAN TRỌNG: Hàm này chỉ được gọi khi hết thời gian chờ để xác minh thanh toán (Validation Timeout). 
        /// Nếu chưa hết thời gian này, hãy sử dụng callback failure (được truyền vào trong hàm Purchase), callback này sẽ được gọi với Type là FailureType.Hack 
        /// </summary>
        public static Action<Product> onHackDetectedInBackground;

        /// <summary>
        /// Gọi khi một thanh toán bị user hủy
        /// </summary>
        public static Action<Product> onUserCancelPurchase;

        /// <summary>
        /// Triggered when server-side purchase validation completes.
        /// True indicates success, False indicates failure.
        /// </summary>
        public static Action<bool> onValidatePurchaseFinish;

        public static void Init(IAPService service)
        {
            var settings = Resources.Load<ProductSettings>(ProductSettings.SETTINGS_NAME);

            if (settings == null)
            {
                Debug.LogError("Bạn cần tạo file Settings trước. Trên thanh menu của Unity, chọn [Puzzle -> IAPService -> IAP Settings]");
                return;
            }

            Handler.Init(settings, service);
        }

        public static void OnReceiveAppsflyerValidation(string validationInfo) { Handler.OnReceiveAppsflyerValidation(validationInfo); }

        //============ Interface =============
        public static void Purchase(string productID, Action<Product> success, Action<PurchaseFailure> failure)
        {
            if (string.IsNullOrEmpty(productID))
            {
                Debug.LogError("IAPService > Product ID không được null hoặc rỗng.");
                return;
            }

#if ACCOUNT_TEST
            success?.Invoke(null);
            return;
#endif

            Handler.Purchase(productID, success, failure);
        }

        public static void RestorePurchase() { Handler.RestorePurchase(); }

        public static ProductMetadata GetProductMetadata(string productID) { return Handler.GetProductMetadata(productID); }

        public static string GetLocalizedPrice(string productID) { return Handler.GetProductMetadata(productID)?.localizedPriceString; }

        public static void Dispose()
        {
            _handler?.Dispose();
            _handler = null;
        }
        public static string GetPurchaseToken(Product product)
        {
            try
            {
                ReceiptData     r  = JsonUtility.FromJson<ReceiptData>(product.receipt);
                Payload         p  = JsonUtility.FromJson<Payload>(r.Payload);
                PayloadJsonData pj = JsonUtility.FromJson<PayloadJsonData>(p.json);
                return pj.purchaseToken;
            }
            catch (Exception e)
            {
                return string.Empty;
            }
        }
    }
}