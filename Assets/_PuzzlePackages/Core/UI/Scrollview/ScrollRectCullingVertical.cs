using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class ScrollRectCullingVertical : ScrollRectCulling
    {
        [Serializable]
        private struct SpacingProperties
        {
            public int paddingTop, paddingBottom, space, offsetX;
        }

        [SerializeField] private SpacingProperties spacing;

        private const int _MIN_HEIGHT = 100; //Ngăn việc gọi vòng lặp vô tận lúc đưa item vào scrollview.


        private enum Alignment
        {
            Left,
            Center,
            Right
        }

        [SerializeField] private Alignment _alignment = Alignment.Center;

        private ScrollRectCullingItem _firstItem, _lastItem;
        
        protected override void OnInitialize()
        {
            var item = GetItem(0);
            if (!item) return;

            if (-spacing.paddingTop <= -Viewport.rect.height) return;

            items.AddLast(item);
            SetBottomItemPosition(item, -spacing.paddingTop);

            _firstItem = _lastItem = item;
            AddItemToBottom();
        }

        protected override bool CheckVisibilityAndToggle(Vector2 deltaPos)
        {
            switch (deltaPos.y)
            {
                case < 0:
                    if (-deltaPos.y < updateFactor)
                        return false;

                    OnScrollDown();
                    return true;

                case > 0:
                    if (deltaPos.y < updateFactor)
                        return false;

                    OnScrollUp();
                    return true;

                default:
                    return false;
            }
        }

        private void OnScrollDown()
        {
            AddItemToTop();
            RemoveItemFromBottom();
        }

        private void OnScrollUp()
        {
            AddItemToBottom();
            RemoveItemFromTop();
        }

        private void AddItemToBottom()
        {
            if (!_lastItem) return;

            var lrt = _lastItem.RectTransform;
            var bottomY = lrt.anchoredPosition.y - HeightOfRect(lrt) / 2 - spacing.space;

            if (bottomY + Content.anchoredPosition.y <= -Viewport.rect.height) return;

            var item = GetItem(_lastItem.Index + 1);
            if (!item) return;

            items.AddLast(item);
            _lastItem = item;

            SetBottomItemPosition(item, bottomY);
            AddItemToBottom();
        }

        private void AddItemToTop()
        {
            if (!_firstItem) return;
            if (_firstItem.Index == 0) return;

            var frt = _firstItem.RectTransform;
            var topY = frt.anchoredPosition.y + HeightOfRect(frt) / 2 + spacing.space;

            if (topY + Content.anchoredPosition.y >= 0) return;

            var item = GetItem(_firstItem.Index - 1);
            if (!item) return;

            items.AddFirst(item);
            _firstItem = item;

            SetTopItemPosition(item, topY);
            AddItemToTop();
        }

        private void RemoveItemFromBottom()
        {
            if (!_lastItem) return;

            var lrt = _lastItem.RectTransform;
            var bottomY = lrt.anchoredPosition.y + HeightOfRect(lrt) / 2;

            if (bottomY + Content.anchoredPosition.y >= -Viewport.rect.height) return;

            removeItemAction(_lastItem);
            items.RemoveLast();
            _lastItem = items.Last.Value;
            RemoveItemFromBottom();
        }

        private void RemoveItemFromTop()
        {
            if (!_firstItem) return;

            var frt = _firstItem.RectTransform;
            var topY = frt.anchoredPosition.y - HeightOfRect(frt) / 2;

            // Debug.LogError($"bottomY: {bottomY}, contentY: {Content.anchoredPosition.y}");
            if (topY + Content.anchoredPosition.y <= 0) return;

            removeItemAction(_firstItem);
            items.RemoveFirst();
            _firstItem = items.First.Value;
            RemoveItemFromTop();
        }

        private float HeightOfRect(RectTransform rTransform)
        {
            if (rTransform.rect.height > 0) return rTransform.rect.height;

            // Debug.LogWarning($"Gameobject [{rTransform.name}] có height <= 0");
            return _MIN_HEIGHT;
        }

        /// <param name="item">Item được set vị trí</param>
        /// <param name="bottomY">TopY của item được set vị trí</param>
        private void SetBottomItemPosition(ScrollRectCullingItem item, float bottomY)
        {
            var rect = item.RectTransform.rect;
            var height = HeightOfRect(item.RectTransform);
            var posY = bottomY - height / 2;

            item.RectTransform.anchoredPosition = _alignment switch
            {
                Alignment.Left => new Vector2(spacing.offsetX + rect.width / 2, posY),
                Alignment.Right => new Vector2(-(spacing.offsetX + rect.width / 2), posY),
                _ => new Vector2(spacing.offsetX, posY)
            };

            Content.sizeDelta = new Vector2(Content.sizeDelta.x, height + spacing.paddingBottom - bottomY);
        }

        private void SetTopItemPosition(ScrollRectCullingItem item, float topY)
        {
            var rect = item.RectTransform.rect;
            var height = HeightOfRect(item.RectTransform);
            var posY = topY + height / 2;

            item.RectTransform.anchoredPosition = _alignment switch
            {
                Alignment.Left => new Vector2(spacing.offsetX + rect.width / 2, posY),
                Alignment.Right => new Vector2(-(spacing.offsetX + rect.width / 2), posY),
                _ => new Vector2(spacing.offsetX, posY)
            };
        }

        private ScrollRectCullingItem GetItem(int index)
        {
            var item = getItemAction(index);
            if (!item) return null;

            item.RectTransform.anchorMin = item.RectTransform.anchorMax = _alignment switch
            {
                Alignment.Left => new Vector2(0f, 1f),
                Alignment.Right => new Vector2(1f, 1f),
                _ => new Vector2(0.5f, 1f)
            };

            item.SetIndex(index);
            return item;
        }
        
        #if UNITY_EDITOR
        public void Reset()
        {
            Content.anchorMin = Content.anchorMax = Content.pivot = new Vector2(0.5f, 1f);
            
            var image = GetComponent<Image>();
            if(image) DestroyImmediate(image);

            var scrollRect = GetComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            
            var horizontalBar = scrollRect.horizontalScrollbar;
            if(horizontalBar) DestroyImmediate(horizontalBar.gameObject);
            scrollRect.horizontalScrollbar = null;

            var viewport = scrollRect.viewport;
            var viewportImg = viewport.GetComponent<Image>();
            if(viewportImg) DestroyImmediate(viewportImg);

            var mask = viewport.GetComponent<Mask>();
            if(mask) DestroyImmediate(mask);

            viewport.AddComponent<RectMask2D>();
        }
#endif
    }
}