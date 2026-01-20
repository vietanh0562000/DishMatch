using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public static class RectExtensions
    {
        public static Rect ReverseNormalize(this Rect rect, Vector2 size)
        {
            if (rect.x <= 1f)
                rect.x *= size.x;
            if (rect.y <= 1f)
                rect.y *= size.y;
            if (rect.width <= 1f)
                rect.width *= size.x;
            if (rect.height <= 1f)
                rect.height *= size.y;
            return rect;
        }

        public static Rect ReverseNormalizePosition(this Rect rect, Vector2 size)
        {
            if (rect.x <= 1f)
                rect.x *= size.x;
            if (rect.y <= 1f)
                rect.y *= size.y;
            return rect;
        }

        public static Rect ReverseNormalizeSize(this Rect rect, Vector2 size)
        {
            if (rect.width <= 1f)
                rect.width *= size.x;
            if (rect.height <= 1f)
                rect.height *= size.y;
            return rect;
        }
    }
}