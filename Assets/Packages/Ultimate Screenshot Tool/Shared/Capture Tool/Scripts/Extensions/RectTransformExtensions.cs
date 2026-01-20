using UnityEngine;

namespace TRS.CaptureTool
{
    public static class RectTransformExtensions
    {
        public static Rect RectForResolution(this RectTransform transform, Resolution resolution)
        {
            return transform.RectForResolution(resolution, Vector2.zero);
        }

        public static Rect RectForResolution(this RectTransform transform, Resolution resolution, Vector2 scale)
        {
            if (scale == Vector2.zero)
                scale = transform.lossyScale;

            Vector2 size = Vector2.Scale(transform.rect.size, scale);
            Vector2 position = new Vector2(transform.position.x - transform.pivot.x * size.x,
                                           transform.position.y - transform.pivot.y * size.y);
            Rect resultRect = new Rect(position, size);

            Vector2 scaledAnchoredPosition = Vector2.Scale(transform.anchoredPosition, scale);
            Vector2 scaledOffsetMin = Vector2.Scale(transform.offsetMin, scale);
            Vector2 scaledOffsetMax = Vector2.Scale(transform.offsetMax, scale);


            bool widthRelative = transform.anchorMin.x != transform.anchorMax.x;
            if (widthRelative)
                resultRect.width = resolution.width * (transform.anchorMax.x - transform.anchorMin.x) - scaledOffsetMin.x + scaledOffsetMax.x;
            bool heightRelative = transform.anchorMin.y != transform.anchorMax.y;
            if (heightRelative)
                resultRect.height = resolution.height * (transform.anchorMax.y - transform.anchorMin.y) - scaledOffsetMin.y + scaledOffsetMax.y;

            if (resultRect.width > resolution.width)
            {
                Debug.LogWarning("Cutout warning:\nRect width is greater than resolution width.");
                resultRect.width = resolution.width;
            }
            if (resultRect.height > resolution.height)
            {
                Debug.LogWarning("Cutout warning:\nRect height is greater than resolution height.");
                resultRect.height = resolution.height;
            }

            if (widthRelative)
                resultRect.x = resolution.width * transform.anchorMin.x + scaledOffsetMin.x;
            else
                resultRect.x = resolution.width * transform.anchorMin.x - resultRect.width * transform.pivot.x + scaledAnchoredPosition.x;

            if (heightRelative)
                resultRect.y = resolution.height * transform.anchorMin.y + scaledOffsetMin.y;
            else
                resultRect.y = resolution.height * transform.anchorMin.y - resultRect.height * transform.pivot.y + scaledAnchoredPosition.y;

            resultRect = ErrorCheckRectForResolution(resultRect, resolution);
            return resultRect;
        }

        public static Rect RectForCurrentResolution(this RectTransform transform)
        {
#if UNITY_2017_3_OR_NEWER
            ((RectTransform)transform).ForceUpdateRectTransforms();
#endif
            Rect resultRect = transform.Rect();
            Resolution resolution = ScreenExtensions.CurrentResolution();
            resultRect = ErrorCheckRectForResolution(resultRect, resolution);

            return resultRect;
        }

        public static Rect Rect(this RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Vector2 position = new Vector2(transform.position.x - transform.pivot.x * size.x,
                                           transform.position.y - transform.pivot.y * size.y);
            return new Rect(position, size);
        }

        public static Rect ErrorCheckRectForResolution(Rect resultRect, Resolution resolution)
        {
            string warningMessage = "";
            string errorMessage = "";
            if (resultRect.x > resolution.width)
                errorMessage += "\nCutout x position is too large and off screen for resolution: " + resolution;
            if (resultRect.y > resolution.height)
                errorMessage += "\nCutout y position is too large and off screen for resolution: " + resolution;
            if (resultRect.x < 0)
            {
                if (resultRect.width - resultRect.x <= 0)
                    errorMessage += "\nCutout x position is too small and off screen for resolution: " + resolution;
                else
                {
                    warningMessage += "\nCutout left edge of cutout is off screen for resolution: " + resolution;

                    resultRect.width += resultRect.x;
                    resultRect.x = 0;
                }
            }
            if (resultRect.y < 0)
            {
                if (resultRect.width - resultRect.x <= 0)
                    errorMessage += "\nCutout y position is too small and off screen for resolution: " + resolution;
                else
                {
                    warningMessage += !string.IsNullOrEmpty(errorMessage) ? "\n" : "";
                    warningMessage += "\nCutout bottom edge of cutout is off screen for resolution: " + resolution;

                    resultRect.height += resultRect.y;
                    resultRect.y = 0;
                }
            }
            if (resultRect.x + resultRect.width > resolution.width)
            {
                warningMessage += "\nCutout is too wide to fit on screen for resolution: " + resolution;

                resultRect.width = resolution.width - resultRect.x;
            }
            if (resultRect.y + resultRect.height > resolution.height)
            {
                warningMessage += !string.IsNullOrEmpty(errorMessage) ? "\n" : "";
                warningMessage += "\nCutout is too tall to fit on screen for resolution: " + resolution;

                resultRect.height = resolution.height - resultRect.y;
            }

            if (warningMessage.Length > 0)
                Debug.LogWarning("Cutout Warning:" + warningMessage);
            if (errorMessage.Length > 0)
                Debug.LogError("Cutout Error:" + errorMessage);

            return resultRect;
        }
    }
}
