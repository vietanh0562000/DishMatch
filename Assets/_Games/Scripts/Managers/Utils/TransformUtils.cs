namespace PuzzleGames
{
    using UnityEngine;

    public static class TransformUtils
    {
        public static void MatchRectTransform(this Component source, Component target, Vector2 deltaPos = new())
        {
            var sourceRect = source.GetComponent<RectTransform>();
            var targetRect = target.GetComponent<RectTransform>();

            if (sourceRect == null || targetRect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            var cachedParent = targetRect.parent;
            source.transform.SetParent(targetRect);
            sourceRect.anchoredPosition = Vector2.zero + deltaPos;
            sourceRect.SetParent(cachedParent);
        }

        public static void SetDeltaYRectTransform(this Component source, Component target, float deltaY)
        {
            var sourceRect = source.GetComponent<RectTransform>();
            var targetRect = target.GetComponent<RectTransform>();

            if (sourceRect == null || targetRect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            sourceRect.SetPivot(Vector2.one * 0.5f);

            var cachedParent = targetRect.parent;
            source.transform.SetParent(targetRect);
            var vector2 = sourceRect.anchoredPosition;
            vector2.y                   = 0;
            sourceRect.anchoredPosition = vector2;
            source.transform.SetParent(cachedParent);
            vector2                     =  sourceRect.anchoredPosition;
            vector2.y                   += deltaY;
            sourceRect.anchoredPosition =  vector2;
        }

        public static void RotateRectTrans(this Component target, float angle)
        {
            var rect = target.GetComponent<RectTransform>();

            if (rect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            rect.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }

        public static void SetPos(this Component target, Vector2 pos)
        {
            var rect = target.GetComponent<RectTransform>();

            if (rect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            rect.anchoredPosition = pos;
        }
        
        public static void SetPosX(this Component target, float x)
        {
            var rect = target.GetComponent<RectTransform>();

            if (rect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            var vector2 = rect.anchoredPosition;
            vector2.x             = x;
            rect.anchoredPosition = vector2;
        }
        
        public static void SetPivot(this Component target, Vector2 pivot)
        {
            var rect = target.GetComponent<RectTransform>();

            if (rect == null)
            {
                Debug.LogError("Target RectTransform must be not null.");
                return;
            }

            rect.pivot = pivot;
        }
    }
}