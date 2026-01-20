using UnityEngine;

namespace TRS.CaptureTool
{
    // These methods are more efficient in memory and speed, but slightly less precise than ColorExtensions. +/- 1 per component.
    public static class Color32Extensions
    {
        public static Color32[] AlphaBlend(this Color32[] background, Color32[] foreground, bool replace = false)
        {
            if (background.Length != foreground.Length)
                throw new System.InvalidOperationException("AlphaBlend only works with two equal sized images");

            Color32[] resultPixels = background;
            if (!replace)
                resultPixels = new Color32[background.Length];
            for (int i = 0; i < background.Length; i++)
                resultPixels[i] = background[i].AlphaBlend(foreground[i]);
            return resultPixels;
        }

        public static Color32 AlphaBlend(this Color32 backgroundColor, Color32 foregroundColor)
        {
            if (foregroundColor.a >= 255) return foregroundColor;

            float backgroundAlpha = (float)backgroundColor.a / 255.0f;

            float sourceAlpha = (float)foregroundColor.a / 255.0f;
            float destAlpha = 1f - sourceAlpha;
            float resultAlpha = sourceAlpha + destAlpha * backgroundAlpha;
            Color32 resultColor = new Color32(
                (byte)(((float)foregroundColor.r * sourceAlpha + (float)backgroundColor.r * backgroundAlpha * destAlpha) / resultAlpha),
                (byte)(((float)foregroundColor.g * sourceAlpha + (float)backgroundColor.g * backgroundAlpha * destAlpha) / resultAlpha),
                (byte)(((float)foregroundColor.b * sourceAlpha + (float)backgroundColor.b * backgroundAlpha * destAlpha) / resultAlpha),
                (byte)(resultAlpha * 255.0f));
            return resultColor;
        }

        public static Color32[] SLBlend(this Color32[] background, Color32[] foreground, bool replace = false)
        {
            if (background.Length != foreground.Length)
                throw new System.InvalidOperationException("SLBlend only works with two equal sized images");

            Color32[] resultPixels = background;
            if (!replace)
                resultPixels = new Color32[background.Length];
            for (int i = 0; i < background.Length; i++)
                resultPixels[i] = background[i].SLBlend(foreground[i]);
            return resultPixels;
        }

        public static Color32 SLBlend(this Color32 backgroundColor, Color32 foregroundColor)
        {
            if (foregroundColor.a >= 255) return foregroundColor;
            float foregroundBlend = (float)foregroundColor.a / 255.0f;
            float backgroundBlend = 1.0f - foregroundBlend;
            return new Color32(
                (byte)((float)foregroundColor.r * foregroundBlend + (float)backgroundColor.r * backgroundBlend),
                (byte)((float)foregroundColor.g * foregroundBlend + (float)backgroundColor.g * backgroundBlend),
                (byte)((float)foregroundColor.b * foregroundBlend + (float)backgroundColor.b * backgroundBlend),
                (byte)((float)foregroundColor.a * foregroundBlend + (float)backgroundColor.a * backgroundBlend));
        }

        public static int FindNextSolidPixelIndex(this Color32[] pixels, int startIndex, int offset)
        {
            int currentIndex = startIndex;
            do
            {
                currentIndex += offset;
            } while (pixels[currentIndex].a != 255);

            return currentIndex;
        }

        public static Color32 FromByteArray(byte[] bytes)
        {
            return new Color32(bytes[0], bytes[1], bytes[2], (bytes.Length >= 4 ? bytes[3] : (byte)255));
        }

        public static byte[] ToByteArrayWithAlpha(this Color32 color, bool withAlpha)
        {
            byte[] result = new byte[withAlpha ? 4 : 3];
            result[0] = color.r;
            result[1] = color.g;
            result[2] = color.b;

            if (!withAlpha) return result;
            result[3] = color.a;
            return result;
        }

        public static long SizeInBytes(this Color32[] colors)
        {
            return colors.LongLength * 4;
        }

        public static long EstimatedSizeInBytes(int width, int height)
        {
            return width * height * 4;
        }
    }
}