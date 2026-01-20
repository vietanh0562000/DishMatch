using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BasePuzzle.PuzzlePackages.Core
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class ScrollRectCulling : MonoBehaviour
    {
        [SerializeField, Tooltip("Tự động kích hoạt trong hàm OnEnable.")] private bool _autoActivation;
        [SerializeField, Tooltip("Tự động hủy kích hoạt trong hàm OnDisable.")] private bool _autoDeactivation;

        protected readonly LinkedList<ScrollRectCullingItem> items = new LinkedList<ScrollRectCullingItem>();
        protected Func<int, ScrollRectCullingItem> getItemAction;
        protected Action<ScrollRectCullingItem> removeItemAction;
        protected Action<ScrollRectCullingItem> updateItemAction;

        private ScrollRect _scrollRect;
        private ScrollRect ScrollRect
        {
            get
            {
                if (_scrollRect == null)
                {
                    _scrollRect = GetComponent<ScrollRect>();
                }

                return _scrollRect;
            }
        }

        protected RectTransform Viewport => ScrollRect.viewport;
        public RectTransform Content => ScrollRect.content;

        private bool _isActivating, _isActivated;
        private Vector2 _prevContentPos, _contentStartPos;

        //Chênh lệch khoảng cách giữa 2 frame của scrollview vượt qua con số này mới thực hiện tính toán (tối ưu hiệu năng)
        protected readonly float updateFactor = 10f;

        public bool IsActivated => _isActivated;

        protected abstract void OnInitialize();
        protected abstract bool CheckVisibilityAndToggle(Vector2 deltaPos);

        private void OnEnable()
        {
            if (_autoActivation) Activate();
        }

        private void OnDisable()
        {
            if (_autoDeactivation) Deactivate();
        }

        public void SetGetItemAction(Func<int, ScrollRectCullingItem> action)
        {
            getItemAction = action;
        }

        public void SetRemoveItemAction(Action<ScrollRectCullingItem> action)
        {
            removeItemAction = action;
        }

        public void SetUpdateItemAction(Action<ScrollRectCullingItem> action)
        {
            updateItemAction = action;
        }

        /// <summary>
        /// Gọi hàm này scrollview mới bắt đầu họat động
        /// </summary>
        public void ActivateManually()
        {
            if (_autoActivation)
            {
                Debug.LogWarning("Bạn đang bật chế độ AutoActivation nên không thể activate thủ công được.");
                return;
            }

            Activate();
        }

        private void Activate()
        {
            if (_isActivating || _isActivated) return;
            _isActivating = true;

            StartCoroutine(CoActivate());
        }

        public void DeactivateManually()
        {
            if (_autoDeactivation)
            {
                Debug.LogWarning("Bạn đang bật chế độ AutoDeactivation nên không thể deactivate thủ công được.");
                return;
            }
            
            Deactivate();
        }

        private void Deactivate()
        {
            _isActivated = false;
            _isActivated = false;
            StopCoroutine(CoActivate());
            ClearItemsAndResetPosition();
        }

        private IEnumerator CoActivate()
        {
            yield return new WaitForEndOfFrame();

            _prevContentPos = _contentStartPos = Content.anchoredPosition;

            if (Viewport.rect.height <= 0)
            {
                Debug.LogError($"{typeof(ScrollRectCulling)} > chiều cao của Viewport phải lớn hơn 0.");
                yield break;
            }

            var type = typeof(ScrollRectCulling).ToString();
            if (getItemAction == null)
            {
                Debug.LogError(
                    $"{type} > Gọi {type}.SetGetItemAction trước khi gọi {type}.Activate để {type} hoạt động đúng");
                yield break;
            }

            if (removeItemAction == null)
            {
                Debug.LogError(
                    $"{type} > Gọi {type}.SetRemoveItemAction trước khi gọi {type}.Activate để {type} hoạt động đúng");
                yield break;
            }

            OnInitialize();
            _isActivated = true;
            _isActivating = false;
        }

        private void Update()
        {
            if (!_isActivated) return;

            if (!CheckVisibilityAndToggle(Content.anchoredPosition - _prevContentPos)) return;
            _prevContentPos = Content.anchoredPosition;
        }


        /// <summary>
        /// Update lại data cho scrollview.
        /// </summary>
        /// <param name="reactivate">
        /// True: dùng trong trường hơp số lượng item có sự thay đổi. Scrollview sẽ reset về vị trí ban đầu.
        /// False: dùng trong trường hợp số lượng item không thay đổi. Scrollview sễ giữ nguyên vị trí hiện tại.</param>
        public void UpdateData(bool reactivate)
        {
            if (!_isActivated) return;

            var type = typeof(ScrollRectCulling).ToString();

            if (!reactivate)
            {
                if (updateItemAction == null)
                {
                    Debug.LogError(
                        $"{type} > Gọi {type}.SetUpdateItemAction trước khi gọi {type}.UpdateData để {type} hoạt động đúng");
                    return;
                }

                foreach (var item in items)
                {
                    updateItemAction.Invoke(item);
                }

                return;
            }

            ClearItemsAndResetPosition();
            OnInitialize();
        }

        private void ClearItemsAndResetPosition()
        {
            foreach (var item in items)
            {
                removeItemAction?.Invoke(item);
            }

            items.Clear();
            Content.anchoredPosition = _contentStartPos;
        }
    }
}