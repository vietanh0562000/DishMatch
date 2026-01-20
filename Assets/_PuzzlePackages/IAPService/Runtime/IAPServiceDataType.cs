using System;
using UnityEngine.Purchasing;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    #region Structs
    
    public struct ReceiptData
    {
        public string Payload;
        public string Store;
        public string TransactionID;
    }

    public struct Payload
    {
        public string json;
        public string signature;
        public string skuDetails;
    }

    public struct PayloadJsonData
    {
        public string orderId;
        public string packageName;
        public string productId;
        public float purchaseTime;
        public int purchaseState;
        public string purchaseToken;
        public int quantity;
        public bool acknowledged;
    }

    public struct ProductDetail
    {
        public Product product;
        public string orderId;
        public string where;
    }
    
    public struct ResponseFromServer
    {
        public string message;
        public int code;
        public int data;
    }
    
    public struct PurchaseFailure
    {
        public FailureType Type { get; }
        public string Message { get; }
        
        public PurchaseFailure(FailureType type, string message = "")
        {
            Type = type;
            Message = message;
        }

        public override string ToString()
        {
            return $"PurchaseFailure > Type: {Type}, Message: {Message}";
        }
    }
    
    public struct ProductReceiptIOS
    {
        public bool result;
        public ProductReceiptDataIOS receipt_data;
    }
    
    public struct ProductReceiptDataIOS
    {
        public string quantity;
        public string purchase_date_ms;
        public string transaction_id;
        public string is_trial_period;
        public string original_transaction_id;
        public string purchase_date;
        public string product_id;
        public string original_purchase_date_pst;
        public string in_app_ownership_type;
        public string original_purchase_date_ms;
        public string purchase_date_pst;
        public string original_purchase_date;
    }

    #endregion

    #region enum

    [Flags]
    public enum ValidationMethod
    {
        None = 0,
        Server = 1 << 0,
        Appsflyer = 1 << 1,
        ServerAndAppsflyer = ~0
    }
    
    public enum FailureType
    {
        PurchaseFailed,
        Timeout,
        Hack,
        UserCancel
    }
    
    public enum PurchaseStore
    {
        Google,
        Apple,
        FakeStore
    }

    #endregion
}