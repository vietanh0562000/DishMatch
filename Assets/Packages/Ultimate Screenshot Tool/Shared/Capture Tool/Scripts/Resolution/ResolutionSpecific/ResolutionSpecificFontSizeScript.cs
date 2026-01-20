using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ResolutionFontSizePair
    {
        public Vector2 resolution;
        public int fontSize;

        public ResolutionFontSizePair(Vector2 resolution, int fontSize)
        {
            this.resolution = resolution;
            this.fontSize = fontSize;
        }
    }

    [ExecuteInEditMode]
    public class ResolutionSpecificFontSizeScript : ResolutionSpecificScript
    {
        public ResolutionFontSizePair[] resolutionFontSizePairs;

        Text text;

        public override void Start()
        {
            base.Start();

            text = GetComponent<Text>();
        }

        public override void UpdateForResolution(int width, int height)
        {
            if (!ValidObject()) return;

            if (resolutionFontSizePairs == null) return;

            foreach (ResolutionFontSizePair resolutionFontSizePair in resolutionFontSizePairs)
            {
                if (resolutionFontSizePair.resolution.x == width && resolutionFontSizePair.resolution.y == height)
                {
                    text.fontSize = resolutionFontSizePair.fontSize;
                    break;
                }
            }
        }
    }
}