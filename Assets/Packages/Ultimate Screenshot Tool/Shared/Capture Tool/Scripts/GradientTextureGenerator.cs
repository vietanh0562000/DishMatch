using UnityEngine;

namespace TRS.CaptureTool
{
    public class GradientTextureGenerator : MonoBehaviour
    {
        public enum GradientDirection
        {
            LeftToRight,
            BottomToTop,
            RightToLeft,
            TopToBottom,
            BottomLeftToTopRight,
            TopLeftToBottomRight,
            BottomRightToTopLeft,
            TopRightToBottomLeft
        };

        public Gradient gradient;
        public GradientColorKey[] colorKey;
        public GradientAlphaKey[] alphaKey;

        public GradientDirection direction;

        public int width;
        public int height;

        public bool saveInBackground = true;
        public ScreenshotFileSettings fileSettings;
        public string lastSaveFilePath;

        #region Editor variables
#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField]
        bool showSaveSettings = true;
#pragma warning restore 0414
#endif
        #endregion

        public Texture2D Texture()
        {
            if(colorKey != null && alphaKey != null)
                gradient.SetKeys(colorKey, alphaKey);

            if (width == 0 || height == 0) return null;

            Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture2D.filterMode = FilterMode.Bilinear;
            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    float t = TimeForDirectionAndPosition(direction, (float)x / (float)(width - 1), (float)y / (float)(height - 1));
                    Color color = gradient.Evaluate(t);
                    texture2D.SetPixel(x, y, color);
                }
            }
            texture2D.Apply(false);
            return texture2D;
        }

        public void Save()
        {
            string fullFilePath = fileSettings.FullFilePath("", fileSettings.FileNameWithCaptureDetails("", width + "x" + height));
            SaveAccordingToFileSettings(Texture(), fileSettings, fullFilePath);
        }

        public void SaveAccordingToFileSettings(Texture2D textureToSave, ScreenshotFileSettings fileSettings, string filePath)
        {
            System.Action<string, bool> completionBlock = (savedFilePath, savedSuccessfully) =>
            {
#if UNITY_EDITOR
                if (savedSuccessfully)
                    Debug.Log("Saved combined texture to: " + savedFilePath);
#endif
                lastSaveFilePath = savedFilePath;
            };

            textureToSave.SaveAccordingToFileSettings(fileSettings, filePath, completionBlock);
            fileSettings.IncrementCount();
            fileSettings.SaveCount();
        }

        float TimeForDirectionAndPosition(GradientDirection direction, float x, float y)
        {
            switch (direction)
            {
                case GradientDirection.LeftToRight:
                    return x;
                case GradientDirection.BottomToTop:
                    return y;
                case GradientDirection.RightToLeft:
                    return 1.0f - x;
                case GradientDirection.TopToBottom:
                    return 1.0f - y;
                case GradientDirection.BottomLeftToTopRight:
                    return (x + y) / 2.0f;
                case GradientDirection.TopLeftToBottomRight:
                    return (x + (1.0f - y)) / 2.0f;
                case GradientDirection.BottomRightToTopLeft:
                    return ((1.0f - x) + y) / 2.0f;
                case GradientDirection.TopRightToBottomLeft:
                    return ((1.0f - x) + (1.0f - y)) / 2.0f;
                default:
                    throw new UnityException("Missing case statement for GradientDirection: " + direction);
            }
        }
    }
}