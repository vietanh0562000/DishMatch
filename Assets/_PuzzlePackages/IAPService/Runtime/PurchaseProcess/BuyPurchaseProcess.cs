using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public class BuyPurchaseProcess : PurchaseProcess
    {
        private readonly string _productID;
        private readonly Action<Product> _onSuccess;
        private readonly Action<PurchaseFailure> _onFail;
        private readonly float _timeout;
        private bool _isTimeout;

        public bool IsStarted { get; private set; }

        public BuyPurchaseProcess(
            string productID, Action<Product> success, Action<PurchaseFailure> failure, float timeout)
        {
            _productID = productID;
            _onSuccess = success;
            _onFail = failure;
            _timeout = timeout;
        }

        //Nếu xác minh hợp lệ trước khi timeout thì gọi callback success để trả thưởng luôn.
        //Sau khi timeout thì bắn ra sự kiện InAppPurchaser.onPurchaseRestored
        protected override void OnValidationSucceeded()
        {
            if (_isTimeout)
            {
                InAppPurchaser.onPurchaseRestored?.Invoke(PurchasedProduct);
                return;
            }

            _onSuccess?.Invoke(PurchasedProduct);
        }

        //Nếu xác minh là hack trước khi timeout thì gọi callback failure truyền vào FailureType.Hack.
        //Sau khi timeout thì bắn ra sự kiện InAppPurchaser.onHackDetectedInBackground
        protected override void OnHackDetected()
        {
            if (_isTimeout)
            {
                InAppPurchaser.onHackDetectedInBackground?.Invoke(PurchasedProduct);
                return;
            }

            _onFail?.Invoke(new PurchaseFailure(FailureType.Hack, $"ProductID: {PurchasedProduct.definition.id}, Payout: {PurchasedProduct.definition.payout}"));
        }

        protected override void OnUserCancelPurchase()
        {
            if (_isTimeout)
            {
                InAppPurchaser.onUserCancelPurchase?.Invoke(PurchasedProduct);
                return;
            }
            
            _onFail?.Invoke(new PurchaseFailure(FailureType.UserCancel, $"ProductID: {PurchasedProduct.definition.id}, Payout: {PurchasedProduct.definition.payout}"));
        }

        //Nếu timeout trước khi xác minh thành công (hoặc hack) thì gọi callback falure truyền vào FailureType.Timeout
        private void OnTimeout()
        {
            _isTimeout = true;
            _onFail?.Invoke(new PurchaseFailure(FailureType.Timeout, IAPServiceConstant.VALIDATION_TIMEOUT_MESSAGE));
        }

        public override void Start(Product product, Action<Product> onFinish)
        {
            base.Start(product, onFinish);
            Timeout(_timeout, Cts.Token);
            IsStarted = true;
        }

        private async void Timeout(float time, CancellationToken ct)
        {
            try
            {
                await Task.Delay((int)(time * 1000), ct);
                OnTimeout();
            }
            catch (OperationCanceledException e)
            {
                Debug.Log($"[{GetType()}] > Timeout task cancelled: {e.Message}");
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(PurchaseProcess)} > {e.Message}");
            }
        }

        public bool HasProductID(string productID)
        {
            return _productID == productID;
        }

        public void OnPurchaseFailed(string reason)
        {
            if (_onFail == null) return;

            var failure = new PurchaseFailure(FailureType.PurchaseFailed, reason);
            _onFail.Invoke(failure);
        }
    }
}