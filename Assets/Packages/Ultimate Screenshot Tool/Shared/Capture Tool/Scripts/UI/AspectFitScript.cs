using UnityEngine;
using UnityEngine.UI;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RawImage))]
    public class AspectFitScript : MonoBehaviour
    {
        RawImage rawImage;

        Vector2 originalSize;
        Vector2 originalAnchorMin;
        Vector2 originalAnchorMax;

        bool setup;
        bool textureIsCopy;

        void Start()
        {
            Setup();
        }

        void Setup()
        {
            if (setup)
                return;

            setup = true;
            rawImage = GetComponent<RawImage>();
            originalSize = new Vector2(rawImage.rectTransform.rect.width, rawImage.rectTransform.rect.height);
            originalAnchorMin = rawImage.rectTransform.anchorMin;
            originalAnchorMax = rawImage.rectTransform.anchorMax;
        }

        public void SetTexture(Texture2D texture, bool copy = true)
        {
            if (!setup)
                Setup();

            if (textureIsCopy)
                Destroy(rawImage.texture);

            if (copy)
            {
                Texture2D textureCopy = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
                Graphics.CopyTexture(texture, textureCopy);
                textureCopy.Apply(false);

                texture = textureCopy;
            }

            textureIsCopy = copy;
            AdjustSizeToTexture(texture);
            rawImage.texture = texture;
        }

        public Texture2D GetTexture()
        {
            return (Texture2D)rawImage.texture;
        }

        public void AdjustSizeToTexture(Texture2D texture)
        {
            // Return to original size to measure. This allows handling non-constant size views.
            rawImage.rectTransform.anchorMin = originalAnchorMin;
            rawImage.rectTransform.anchorMax = originalAnchorMax;


            Vector2 adjustedSizeDelta = originalSize;
            if (originalAnchorMin.x != originalAnchorMax.x)
                adjustedSizeDelta.x = 0;
            if (originalAnchorMin.y != originalAnchorMax.y)
                adjustedSizeDelta.y = 0;

            rawImage.rectTransform.sizeDelta = adjustedSizeDelta;
            rawImage.rectTransform.ForceUpdateRectTransforms();

            Vector2 updatedSize = rawImage.rectTransform.rect.size;
            float xScale = updatedSize.x / (float)texture.width;
            float yScale = updatedSize.y / (float)texture.height;
            float finalScale = Mathf.Min(xScale, yScale);

            float xCenter = originalAnchorMin.x;
            if (originalAnchorMin.x != originalAnchorMax.x)
                xCenter = (originalAnchorMax.x + originalAnchorMin.x) / 2f;
            float yCenter = originalAnchorMin.y;
            if (originalAnchorMin.y != originalAnchorMax.y)
                yCenter = (originalAnchorMax.y + originalAnchorMin.y) / 2f;

            rawImage.rectTransform.anchorMin = new Vector2(xCenter, yCenter);
            rawImage.rectTransform.anchorMax = new Vector2(xCenter, yCenter);
            rawImage.rectTransform.sizeDelta = new Vector2(texture.width * finalScale, texture.height * finalScale);
        }

        void OnDestroy()
        {
            if (textureIsCopy)
                MonoBehaviourExtended.FlexibleDestroy(rawImage.texture);
        }
    }
}