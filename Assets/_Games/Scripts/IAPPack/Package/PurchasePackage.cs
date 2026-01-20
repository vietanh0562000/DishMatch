using BasePuzzle.PuzzlePackages.IAPService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

namespace PuzzleGames
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using com.ootii.Messages;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using Sirenix.OdinInspector;
    using TMPro;

    public class PurchasePackage : MonoBehaviour
    {
        [SerializeField] private bool     _isFailedPackage;
        [SerializeField] private TMP_Text _txtPrice, txtPackName;

        [SerializeField, ReadOnly] public StandardPackBundle _bundle;

        [SerializeField] private UnityEvent<PurchaseFailure> _onFalure;

        [SerializeField] private List<RewardInfo> _resourceUis = new();

        private PurchaseID _purchaseID => _bundle.ID;

        private bool _isNoAdsPack => _bundle.RemoveAds;

        private Action _onSuccess;

#if UNITY_EDITOR
        private void OnValidate() { _resourceUis = GetComponentsInChildren<RewardInfo>(true).ToList(); }
#endif

        public void OnSuccess(Action onSuccess) { _onSuccess = onSuccess; }

        public void SetPack(StandardPackBundle bundle)
        {
            _bundle = bundle;
            BindData();
        }

        protected virtual void BindData()
        {
            if (_isNoAdsPack && LevelDataController.instance.Level <
                ServerConfig.Instance<ValueRemoteConfig>().numLevelToShowInterstitial)
            {
                gameObject.SetActive(false);
                return;
            }

            txtPackName.SetText(_bundle.bundleName);

            var localizedPrice = InAppPurchaser.GetLocalizedPrice(_purchaseID.GetPurchasePKG());
            if (!string.IsNullOrEmpty(localizedPrice))
                _txtPrice.text = localizedPrice;

            foreach (var resourceUI in _resourceUis)
            {
                foreach (var resource in _bundle.Resources)
                {
                    if (resource.type != resourceUI.Type)
                    {
                        continue;
                    }

                    if (resource.value <= 0) continue;
                    resourceUI.SetValue(resource);
                }

                foreach (var resource in _bundle.InfiResources)
                {
                    if (resource.type != resourceUI.Type)
                    {
                        continue;
                    }

                    if (resource.value <= 0) continue;
                    resourceUI.SetValue(resource);
                }
            }
        }

        private void OnEnable() { MessageDispatcher.AddListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private IEnumerator Start()
        {
            if (!_isNoAdsPack) yield break;

            while (UserResourceController.instance.UserResource == null)
                yield return new WaitForSeconds(2f);

            var userResource = UserResourceController.instance.UserResource;
            gameObject.SetActive(!userResource.removeAds && LevelDataController.instance.Level >=
                ServerConfig.Instance<ValueRemoteConfig>().numLevelToShowInterstitial);
        }

        private void OnDestroy() { MessageDispatcher.RemoveListener(EventID.BUY_NO_ADS, OnBuyNoAds, true); }

        private void OnBuyNoAds(IMessage rmessage)
        {
            if (!_isNoAdsPack) return;

            StartCoroutine(CoDisableGo());

            IEnumerator CoDisableGo()
            {
                yield return null;
                gameObject.SetActive(false);
            }
        }

        public void ClickBtnPurchase()
        {
            AudioController.PlaySound(SoundKind.UIClickButton);
            InAppPurchaser.Purchase(_purchaseID.GetPurchasePKG(), OnSuccess, OnFailure);
        }

        private void OnSuccess(Product product)
        {
            ResourceManager.Instance.Add(_bundle.Resources);
            ResourceManager.Instance.AddFreeTime(_bundle.InfiResources);

            if (_isNoAdsPack)
            {
                UserResourceController.instance.SetRemoveAds(true);

                UIToastManager.Instance.Show("Buy succeed!");

                MessageDispatcher.SendMessage(EventID.BUY_NO_ADS, 0);
            }

            _onSuccess?.Invoke();
        }

        private void OnFailure(PurchaseFailure purchaseFailure)
        {
            Debug.LogError($"{gameObject.name} > purchaseFailure: {purchaseFailure.Message}");
            _onFalure?.Invoke(purchaseFailure);
        }
    }
}