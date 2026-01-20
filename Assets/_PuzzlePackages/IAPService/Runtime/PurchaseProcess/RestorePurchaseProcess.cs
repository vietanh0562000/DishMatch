using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class RestorePurchaseProcess : PurchaseProcess
    {
        //Restore Purchase không có timeout nên bắn ra sự kiện InAppPurchaser.onPurchaseRestored luôn
        protected override void OnValidationSucceeded()
        {
            InAppPurchaser.onPurchaseRestored?.Invoke(PurchasedProduct);
        }

        //Restore Purchase không có timeout nên bắn ra sự kiện InAppPurchaser.onHackDetectedInBackground luôn
        protected override void OnHackDetected()
        {
            InAppPurchaser.onHackDetectedInBackground?.Invoke(PurchasedProduct);
        }

        //Restore Purchase không có timeout nên bắn ra sự kiện InAppPurchaser.onUserCancelPurchase luôn
        protected override void OnUserCancelPurchase()
        {
            InAppPurchaser.onUserCancelPurchase?.Invoke(PurchasedProduct);
        }
    }
}