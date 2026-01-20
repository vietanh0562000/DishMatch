using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    public abstract class PurchaseProcess
    {
        public enum PurchaseState
        {
            Purchased = 0,
            Canceled = 1,
            Pending = 2,
            FreeTrial = 3,
            PendingDeferred = 4,
            Hack = 5
        }

        private Action<Product> _onValidationFinished;

        protected Product PurchasedProduct { get; private set; }
        protected CancellationTokenSource Cts { get; private set; }

        protected abstract void OnValidationSucceeded();
        protected abstract void OnHackDetected();
        protected abstract void OnUserCancelPurchase();

        public virtual void Start(Product product, Action<Product> onFinish)
        {
            PurchasedProduct = product;
            _onValidationFinished = onFinish;
        }


        public void ValidationSucceeded()
        {
            OnValidationSucceeded();
            ReleaseCTS();
            _onValidationFinished?.Invoke(PurchasedProduct);
        }

        public void CancelPurchaseByUser()
        {
            OnUserCancelPurchase();
            ReleaseCTS();
            _onValidationFinished?.Invoke(PurchasedProduct);
        }

        public void HackDetected()
        {
            OnHackDetected();
            ReleaseCTS();
            _onValidationFinished?.Invoke(PurchasedProduct);
        }

        // ReSharper disable once InconsistentNaming
        public void ReleaseCTS()
        {
            if (Cts == null) return;
            Cts.Cancel();
            Cts.Dispose();
            Cts = null;
        }

        public bool HasTransactionID(string id)
        {
            if (PurchasedProduct == null) return false;
            return PurchasedProduct.transactionID == id;
        }
    }
}