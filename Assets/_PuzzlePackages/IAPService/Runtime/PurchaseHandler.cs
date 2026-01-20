using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    using SRF;

    public class PurchaseHandler : IDetailedStoreListener
    {
        private readonly List<BuyPurchaseProcess> _buyProcesses = new List<BuyPurchaseProcess>();
        private readonly List<RestorePurchaseProcess> _restoreProcesses = new List<RestorePurchaseProcess>();
        private readonly IAPStoreHandler _storeHandler;

        private IStoreController _storeController;
        private IAPService _service;

        private bool _canMakePurchase;
        public bool CanMakePurchase => _canMakePurchase;

        public PurchaseHandler()
        {
#if UNITY_EDITOR
            _storeHandler = new IAPFakeStoreHandler();
#elif UNITY_ANDROID
            _storeHandler = new IAPGooglePlayHandler();
#elif UNITY_IOS
            _storeHandler = new IAPAppstoreHandler();
#else
            _storeHandler = new IAPFakeStoreHandler();
#endif
        }

        public void Init(ProductSettings settings, IAPService iapService)
        {
            _service = iapService;

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            _storeHandler.SetCanMakePurchase(builder, ref _canMakePurchase);

            foreach (var product in settings.Products)
            {
                builder.AddProduct(product.productID, product.type);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _storeHandler.OnInitialized(extensions);
            
            Debug.Log("OnInitialized");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            OnInitializeFailed(error, null);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            var errorMessage = $"{_storeHandler.GetType()} > Lỗi init IAP. Nguyên nhân: {error}.";

            if (message != null)
            {
                errorMessage += $" Chi tiết: {message}";
            }

            Debug.LogError(errorMessage);
        }

        public void Purchase(string productID, Action<Product> success, Action<PurchaseFailure> failure)
        {
            if (_storeController == null)
            {
                Debug.LogError($"{typeof(PurchaseHandler)} > Bạn cần phải khởi tạo IAP Service trước.");
                return;
            }

            if (_storeHandler.Store == PurchaseStore.FakeStore)
            {
                success?.Invoke(_storeController.products.WithID(productID));
                return;
            }

            if (_buyProcesses.Any(p => p.HasProductID(productID)))
            {
                Debug.LogError(
                    $"{_storeHandler.GetType()} > Purchase > Đã có 1 quá trình thanh toán khác của productID này đang diễn ra. Đợi thanh toán hoàn tất trước khi thực hiện thanh toán mới");
                return;
            }

            var process = new BuyPurchaseProcess(productID, product =>
            {
                Log(product, "iap");
                success?.Invoke(product);
            }, failure, _service.Timeout);
            _buyProcesses.Add(process);
            _storeController.InitiatePurchase(productID);
        }

        private static void Log(Product product, string where)
        {
            var productID = product.definition.id;
            var metadata  = InAppPurchaser.GetProductMetadata(productID);
            if (metadata == null) return;

            var price         = metadata.localizedPrice;
            var currencyCode  = metadata.isoCurrencyCode;
            var transactionID = product.transactionID;
            var purchaseToken = InAppPurchaser.GetPurchaseToken(product);
            var level         = LevelDataController.instance.Level;

         
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.LogError(
                $"{_storeHandler.GetType()} > Lỗi thanh toán - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");

            OnPurchaseFailed(product, failureReason.ToString());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.LogError($"{_storeHandler.GetType()} > Lỗi thanh toán - Product: '{product.definition.id}'," +
                           $" Lý do: {failureDescription.reason}," +
                           $" Chi tiết: {failureDescription.message}");

            OnPurchaseFailed(product, failureDescription.message);
        }

        private void OnPurchaseFailed(Product product, string msg)
        {
            foreach (var process in _buyProcesses)
            {
                if (!process.HasProductID(product.definition.id)) continue;

                _buyProcesses.Remove(process);
                process.OnPurchaseFailed(msg);
                return;
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log($"{_storeHandler.GetType()} > Processing Purchase: {args.purchasedProduct.definition.id}");

            if (_storeHandler.IsPurchasedDeferred(args.purchasedProduct))
            {
                return PurchaseProcessingResult.Pending;
            }

            ValidatePurchase(args.purchasedProduct);
            return PurchaseProcessingResult.Pending;
        }

        private void ValidatePurchase(Product product)
        {
            if (product == null)
            {
                Debug.LogError("IAPValidationProcess > Không thể xác minh thanh toán cho null product!");
                return;
            }
            
            foreach (var bp in _buyProcesses)
            {
                //Product thuộc Restore Processes có thể có ID trùng với product thuộc Buy Processces
                if (bp.IsStarted) continue;
                if (!bp.HasProductID(product.definition.id)) continue;

                bp.Start(product, (p =>
                {
                    _storeController.ConfirmPendingPurchase(p);
                    _buyProcesses.Remove(bp);
                }));
                return;
            }

            if (_restoreProcesses.Any(rp => rp.HasTransactionID(product.transactionID))) return;

            var rp = new RestorePurchaseProcess();
            rp.Start(product, (p =>
            {
                _storeController.ConfirmPendingPurchase(p);
                _restoreProcesses.Remove(rp);
            }));
            _restoreProcesses.Add(rp);
        }

        public void OnReceiveAppsflyerValidation(string validationInfo)
        {
            Dictionary<string, object> dictionary = Json.Deserialize(validationInfo) as Dictionary<string, object>;
            if (dictionary == null)
            {
                Debug.LogWarning($"{nameof(OnReceiveAppsflyerValidation)} > data is null.");
                return;
            }

            _storeHandler.AppsflyerValidation(dictionary, validationInfo, OnAppsflyerValidationSuccess,
                OnAppsflyerFraudDetected);
        }

        private void OnAppsflyerValidationSuccess(string transactionID)
        {
            foreach (var bp in _buyProcesses)
            {
                if (!bp.HasTransactionID(transactionID)) continue;
                
                Debug.Log($"$[{typeof(PurchaseHandler)}] > Appsflyer Validation [BuyProcess]: Xác minh thanh toán thành công");
                bp.ValidationSucceeded();
                return;
            }

            foreach (var rp in _restoreProcesses)
            {
                if (!rp.HasTransactionID(transactionID)) continue;
                
                Debug.Log($"$[{typeof(PurchaseHandler)}] > Appsflyer Validation [RestoreProcess]: Xác minh thanh toán thành công");
                rp.ValidationSucceeded();
                return;
            }
        }

        private void OnAppsflyerFraudDetected(string transactionID)
        {
            foreach (var bp in _buyProcesses)
            {
                if (!bp.HasTransactionID(transactionID)) continue;
                
                Debug.Log($"$[{typeof(PurchaseHandler)}] > Appsflyer Validation [BuyProcess]:  Phát hiện hack.");
                bp.HackDetected();
                return;
            }

            foreach (var rp in _restoreProcesses)
            {
                if (!rp.HasTransactionID(transactionID)) continue;
                
                Debug.Log($"$[{typeof(PurchaseHandler)}] > Appsflyer Validation [RestoreProcess]:  Phát hiện hack.");
                rp.HackDetected();
                return;
            }
        }


        /// <summary>
        /// Chỉ có tác dụng trên nền tảng ios.
        /// </summary>
        public void RestorePurchase()
        {
            _storeHandler.RestorePurchase();
        }

        /// <summary>
        /// Chỉ có tác dụng với Google Play Store. Các store khác sẽ trả về false.
        /// </summary>
        public bool IsPurchasedProductDeferred(string productID)
        {
            var product = _storeController?.products.WithID(productID);
            return product != null && _storeHandler.IsPurchasedDeferred(product); 
        }

        public ProductMetadata GetProductMetadata(string productID)
        {
            return _storeController?.products.WithID(productID).metadata;
        }

        public void Dispose()
        {
            foreach (var bp in _buyProcesses)
            {
                bp.ReleaseCTS();
            }

            foreach (var rp in _restoreProcesses)
            {
                rp.ReleaseCTS();
            }
        }
    }
}