namespace BasePuzzle.PuzzlePackages.IAPService
{
    public static class IAPServiceConstant
    {
        public const string PRODUCT_PURCHASE = "productPurchase";
        public const string SUBSCRIPTION_PURCHASE = "subscriptionPurchase";
        public const string FAILURE_DATA = "failureData";
        public const string VALIDATION_TIMEOUT_MESSAGE = "Hết thời gian chờ xác nhận thanh toán. Thanh toán này sẽ được xác nhận lại khi mở lại app." +
                                                         "Nếu thành công sự kiện InAppPurchaser.onPurchaseRestored sẽ được gọi, nếu thất bại sẽ gọi sự kiện: InAppPurchaser.onHackDetectedInBackground." +
                                                         "Nếu vẫn không thành công, việc xác nhận sẽ tiếp tục được thực hiện lại ở lần mở app sau.";
    }
}