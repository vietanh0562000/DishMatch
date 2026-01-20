using UnityEngine;
using UnityEngine.UI;

namespace ScrollSnapExtension
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectEnsureVisible : MonoBehaviour
    {
        private RectTransform _maskTransform;
        private ScrollRect _mScrollRect;
        private RectTransform _mScrollTransform;
        private RectTransform _mContent;

        private void InitScrollRectEnsureVisible()
        {
            if (_mScrollRect == null)
            {
                _mScrollRect = GetComponent<ScrollRect>();
                _mScrollTransform = _mScrollRect.transform as RectTransform;
                _mContent = _mScrollRect.content;
                _maskTransform = _mScrollRect.viewport;
            }
        }

        public void ForceScrollNormalizedToTarget(RectTransform target)
        {
            InitScrollRectEnsureVisible();

            var itemCenterPositionInScroll = GetWorldPointInWidget(_mScrollTransform, GetWidgetWorldPoint(target));
            var targetPositionInScroll = GetWorldPointInWidget(_mScrollTransform, GetWidgetWorldPoint(_maskTransform));
            var difference = targetPositionInScroll - itemCenterPositionInScroll;
            difference.z = 0f;

            if (!_mScrollRect.horizontal)
            {
                difference.x = 0f;
            }
            if (!_mScrollRect.vertical)
            {
                difference.y = 0f;
            }

            var normalizedDifference = new Vector2(difference.x / (_mContent.rect.size.x - _mScrollTransform.rect.size.x), difference.y / (_mContent.rect.size.y - _mScrollTransform.rect.size.y));
            var newNormalizedPosition = _mScrollRect.normalizedPosition - normalizedDifference;

            if (_mScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            if (_mScrollRect.horizontal)
            {
                _mScrollRect.horizontalNormalizedPosition = newNormalizedPosition.x;
            }

            if (_mScrollRect.vertical)
            {
                _mScrollRect.verticalNormalizedPosition = newNormalizedPosition.y;
            }
        }

        public Vector2 GetNormalizedPosition(RectTransform target)
        {
            InitScrollRectEnsureVisible();

            var itemCenterPositionInScroll = GetWorldPointInWidget(_mScrollTransform, GetWidgetWorldPoint(target));
            var targetPositionInScroll = GetWorldPointInWidget(_mScrollTransform, GetWidgetWorldPoint(_maskTransform));
            var difference = targetPositionInScroll - itemCenterPositionInScroll;
            difference.z = 0f;

            if (!_mScrollRect.horizontal)
            {
                difference.x = 0f;
            }
            if (!_mScrollRect.vertical)
            {
                difference.y = 0f;
            }

            var normalizedDifference = new Vector2(difference.x / (_mContent.rect.size.x - _mScrollTransform.rect.size.x), difference.y / (_mContent.rect.size.y - _mScrollTransform.rect.size.y));
            var newNormalizedPosition = _mScrollRect.normalizedPosition - normalizedDifference;

            if (_mScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            return newNormalizedPosition;
        }

        private Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            var pivotOffset = new Vector3((0.5f - target.pivot.x) * target.rect.size.x, (0.5f - target.pivot.y) * target.rect.size.y, 0f);
            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        private Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }
    }
}