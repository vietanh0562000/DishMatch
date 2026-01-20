using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class Notifier : MonoBehaviour
    {
        [SerializeField] private TMP_Text _lNotification;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] public RectTransform _notifyRect;
        [SerializeField] private float _distance, _showTime;

        private static Notifier _notifier;
        private bool _showing = false;

        private void Awake()
        {
            if (_notifier == null)
            {
                _notifier = this;
                return;
            }
            
            Debug.LogError("Falcon Puzzle Packages Notifier > You've already have another Notifier in this scene." +
                           "So, this GameObject is not necessary and will be destroyed!");
            
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _notifier = null;
        }

        public static void Show(ref string message)
        {
            _notifier.ShowNotification(message);
        }

        private void ShowNotification(string text)
        {
            if (_showing) return;
            _showing = true;

            _lNotification.text = text;

            _notifyRect.anchoredPosition = Vector2.zero;
            _notifyRect.gameObject.SetActive(true);

            _notifyRect.DOAnchorPosY(_distance, _showTime, true)
                .SetEase(_ease)
                .OnComplete(OnCompleted);
        }

        private void OnCompleted()
        {
            _notifyRect.gameObject.SetActive(false);
            _showing = false;
        }
    }
}
