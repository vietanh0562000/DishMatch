using System;
using System.Collections.Generic;

namespace BasePuzzle.PuzzlePackages.IAPService
{
    [Serializable]

    class InAppPurchaseValidationResult : EventArgs
    {
        public bool success;
        public ProductPurchase? productPurchase;
        public ValidationFailureData? failureData;
        public string token;
    }

    [Serializable]
    class ProductPurchase
    {
        public string kind;
        public string purchaseTimeMillis;
        public int purchaseState;
        public int consumptionState;
        public string developerPayload;
        public string orderId;
        public int purchaseType;
        public int acknowledgementState;
        public string purchaseToken;
        public string productId;
        public int quantity;
        public string obfuscatedExternalAccountId;
        public string obfuscatedExternalProfil;
        public string regionCode;
    }

    [Serializable]
    class ValidationFailureData
    {
        public int status;
        public string description;
    }

    [Serializable]
    class SubscriptionValidationResult
    {
        public bool success;
        public SubscriptionPurchase? subscriptionPurchase;
        public ValidationFailureData? failureData;
        public string token;
    }

    [Serializable]
    class SubscriptionPurchase
    {
        public string acknowledgementState;
        public CanceledStateContext? canceledStateContext;
        public ExternalAccountIdentifiers? externalAccountIdentifiers;
        public string kind;
        public string latestOrderId;
        public List<SubscriptionPurchaseLineItem> lineItems;
        public string? linkedPurchaseToken;
        public PausedStateContext? pausedStateContext;
        public string regionCode;
        public string startTime;
        public SubscribeWithGoogleInfo? subscribeWithGoogleInfo;
        public string subscriptionState;
        public TestPurchase? testPurchase;
    }

    [Serializable]
    class CanceledStateContext
    {
        public DeveloperInitiatedCancellation? developerInitiatedCancellation;
        public ReplacementCancellation? replacementCancellation;
        public SystemInitiatedCancellation? systemInitiatedCancellation;
        public UserInitiatedCancellation? userInitiatedCancellation;

    }

    [Serializable]
    class ExternalAccountIdentifiers
    {
        public string externalAccountId;
        public string obfuscatedExternalAccountId;
        public string obfuscatedExternalProfileId;
    }

    [Serializable]
    class SubscriptionPurchaseLineItem
    {
        public AutoRenewingPlan? autoRenewingPlan;
        public DeferredItemReplacement? deferredItemReplacement;
        public string expiryTime;
        public OfferDetails? offerDetails;
        public PrepaidPlan? prepaidPlan;
        public string productId;
    }

    [Serializable]
    class PausedStateContext
    {
        public string autoResumeTime;
    }

    [Serializable]
    class SubscribeWithGoogleInfo
    {
        public string emailAddress;
        public string familyName;
        public string givenName;
        public string profileId;
        public string profileName;
    }

    [Serializable]
    class TestPurchase
    {
    }

    [Serializable]
    class DeveloperInitiatedCancellation
    {
    }

    [Serializable]
    class ReplacementCancellation
    {
    }

    [Serializable]
    class SystemInitiatedCancellation
    {
    }

    [Serializable]
    class UserInitiatedCancellation
    {
        public CancelSurveyResult? cancelSurveyResult;
        public string cancelTime;
    }

    [Serializable]
    class AutoRenewingPlan
    {
        public string? autoRenewEnabled;
        public SubscriptionItemPriceChangeDetails? priceChangeDetails;
    }

    [Serializable]
    class DeferredItemReplacement
    {
        public string productId;
    }

    [Serializable]
    class OfferDetails
    {
        public List<string>? offerTags;
        public string basePlanId;
        public string? offerId;
    }

    [Serializable]
    class PrepaidPlan
    {
        public string? allowExtendAfterTime;
    }

    [Serializable]
    class CancelSurveyResult
    {
        public string reason;
        public string reasonUserInput;
    }

    [Serializable]
    class SubscriptionItemPriceChangeDetails
    {
        public string expectedNewPriceChargeTime;
        public Money? newPrice;
        public string priceChangeMode;
        public string priceChangeState;
    }

    [Serializable]
    class Money
    {
        public string currencyCode;
        public long nanos;
        public long units;
    }
}
   