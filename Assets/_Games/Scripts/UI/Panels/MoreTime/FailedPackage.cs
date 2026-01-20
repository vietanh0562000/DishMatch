using UnityEngine;

namespace PuzzleGames
{
    using System;
    using BasePuzzle.PuzzlePackages;

    public class FailedPackage : MonoBehaviour
    {
        [SerializeField] private RectTransform      _target;
        [SerializeField] private StandardPackBundle _failedBundle;

        private PurchasePackage _failedPackage;
        private void Awake()
        {
            _failedPackage = Instantiate(_failedBundle.Prefab, _target);
            _failedPackage.SetPack(_failedBundle);
            _failedPackage.gameObject.SetActive(false);
        }

        public void Show(Action onSuccess)
        {
            _failedPackage.gameObject.SetActive(true);
            
            Action action = onSuccess + (() =>
            {
                _ = FlyManager.Instance.ShowFly(_failedBundle.Resources);
                _ = FlyManager.Instance.ShowFly(_failedBundle.InfiResources);
            });
            _failedPackage.OnSuccess(action);
        }

        public void Hide()
        {
            _failedPackage.gameObject.SetActive(false);
        }
    }
}
