namespace PuzzleGames
{
    using System.Collections.Generic;
    using com.ootii.Messages;
    using Sirenix.OdinInspector;
    using UnityEngine.UI;
    using System.Collections;
    using System.Threading.Tasks;
    using BasePuzzle.PuzzlePackages.Navigator;
    using UnityEngine;
    using UnityEngine.Purchasing;

    public class ShopUI : MonoBehaviour
    {
        [SerializeField] ShopPackSO shopPack;

        [SerializeField] private ShopWhere         _where;
        [SerializeField] private ContentSizeFitter _contentSizeFitter;

        [Title("Gold Pack")] [SerializeField] private PurchaseGoldPackage _goldPack;
        [SerializeField]                      private Transform           _goldPackContent;

        [Title("Bundle Pack")] [SerializeField]
        private Transform _bundleContent;

        [Title("Ads Pack")] [SerializeField] private Transform _adsContent;


        private enum ShopWhere
        {
            Home,
            GameScene
        }

        private void Awake()
        {
            SetupGoldPacks();
            SetupBundlePacks();
            RefreshShop(null);
        }
        private void SetupBundlePacks()
        {
            foreach (Transform child in _bundleContent)
            {
                Destroy(child.gameObject);
            }

            foreach (Transform child in _adsContent)
            {
                Destroy(child.gameObject);
            }

            var bundles = shopPack.StandardPackBundles;
            if (bundles == null || bundles.Count == 0) return;
            foreach (var bundle in bundles)
            {
                var pack = Instantiate(bundle.Prefab, bundle.RemoveAds ? _adsContent : _bundleContent);
                pack.SetPack(bundle);
                
                pack.OnSuccess(() =>
                {
                    WindowManager.Instance.OpenWindow<RewardsBundlePanel>(onLoaded: panel =>
                    {
                        panel.OnClosed(() =>
                        {
                            if (_where == ShopWhere.Home)
                            {
                                Navigator.Instance.MoveToTab(1);
                            }
                            else
                            {
                                WindowManager.Instance.CloseCurrentWindow();
                            }
                        });
                        panel.SetPurchasePackage(pack._bundle);
                    });
                });
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_bundleContent as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_adsContent as RectTransform);
        }

        private void SetupGoldPacks()
        {
            foreach (Transform child in _goldPackContent)
            {
                Destroy(child.gameObject);
            }

            List<GoldPack> goldPacks = shopPack.GoldPacks;
            if (goldPacks == null || goldPacks.Count == 0) return;

            foreach (var goldPack in goldPacks)
            {
                var newGoldPack = Instantiate(_goldPack, _goldPackContent);
                newGoldPack.SetPack(goldPack);
                if (_where == ShopWhere.Home)
                {
                    newGoldPack.OnSuccess(() => Navigator.Instance.MoveToTab(1));
                }
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_goldPackContent as RectTransform);
        }

        private void OnEnable() { MessageDispatcher.AddListener(EventID.BUY_NO_ADS, RefreshShop, true); }

        private void OnDisable() { MessageDispatcher.RemoveListener(EventID.BUY_NO_ADS, RefreshShop, true); }

        private void RefreshShop(IMessage rmessage)
        {
            _contentSizeFitter.enabled = false;

            StartCoroutine(CoWait());
            return;

            IEnumerator CoWait()
            {
                yield return null;
                yield return null;
                _contentSizeFitter.enabled = true;
            }
        }
    }
}