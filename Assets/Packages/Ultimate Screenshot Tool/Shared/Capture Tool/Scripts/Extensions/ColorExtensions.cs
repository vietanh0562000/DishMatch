using UnityEngine;

namespace TRS.CaptureTool
{
    // These methods are less efficient in memory and speed, but slightly more precise than ColorExtensions. +/- 1 per component.
    public static class ColorExtensions
    {
        public static Color[] AlphaBlend(this Color[] background, Color[] foreground, bool replace = false)
        {
            if (background.Length != foreground.Length)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");

            Color[] resultPixels = background;
            if (!replace)
                resultPixels = new Color[background.Length];
            for (int i = 0; i < background.Length; i++)
                resultPixels[i] = background[i].AlphaBlend(foreground[i]);
            return resultPixels;
        }

        public static Color AlphaBlend(this Color backgroundColor, Color foregroundColor)
        {
            if (foregroundColor.a >= 1f) return foregroundColor;

            float sourceAlpha = foregroundColor.a;
            float destAlpha = 1f - foregroundColor.a;
            float resultAlpha = sourceAlpha + destAlpha * backgroundColor.a;
            Color resultColor = (foregroundColor * sourceAlpha + backgroundColor * backgroundColor.a * destAlpha) / resultAlpha;
            resultColor.a = resultAlpha;
            return resultColor;
        }

        public static Color[] SLBlend(this Color[] background, Color[] foreground, bool replace = false)
        {
            if (background.Length != foreground.Length)
                throw new System.InvalidOperationException("SLBlend only works with two equal sized images");

            Color[] resultPixels = background;
            if (!replace)
                resultPixels = new Color[background.Length];
            for (int i = 0; i < background.Length; i++)
                resultPixels[i] = background[i].SLBlend(foreground[i]);
            return resultPixels;
        }

        public static Color SLBlend(this Color backgroundColor, Color foregroundColor)
        {
            if (foregroundColor.a >= 1f) return foregroundColor;
            return foregroundColor * foregroundColor.a + backgroundColor * (1f - foregroundColor.a);
        }
    }
}