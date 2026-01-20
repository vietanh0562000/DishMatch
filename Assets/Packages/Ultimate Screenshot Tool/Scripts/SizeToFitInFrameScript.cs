using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    public class SizeToFitInFrameScript : MonoBehaviour
    {
        public Graphic frameGraphic;
        Texture2D texture;

        void Awake()
        {
            if (frameGraphic == null)
                Debug.LogError("Frame graphic (such as RawImage) must be set to size this object to.");
            texture = frameGraphic.mainTexture as Texture2D;
            if (texture == null)
                Debug.LogError("Graphic must have a Texture2D set as the texture.");
            if (!texture.isReadable)
                Debug.LogError("Texture must be readable.");
            RectTransform rectTransform = transform as RectTransform;
            if (rectTransform == null)
                Debug.LogError("This object must have a rect transform in order to size to fit the frame.");

            Color32[] pixels = texture.GetPixels32();
            int center = texture.width / 2 + (texture.height / 2) * texture.width;
            if (pixels[center].a > 0)
            {
                Debug.LogError("It's expected that the empty region of the frame be in the center of the image.");
                return;
            }

            float scaleX = frameGraphic.rectTransform.rect.width / texture.width * (frameGraphic.transform.lossyScale.x / transform.lossyScale.x);
            float scaleY = frameGraphic.rectTransform.rect.height / texture.height * (frameGraphic.transform.lossyScale.y / transform.lossyScale.y);

            // Use off center values to account for notches.
            int verticallyOffCenter = texture.width / 2 + (texture.height / 4) * texture.width;
            int leftEdgeRaw = pixels.FindNextSolidPixelIndex(verticallyOffCenter, -1);
            // Find the x-coordinate of the solid pixel and add 1 to find the first transparent location.
            int leftEdge = leftEdgeRaw % texture.width + 1;

            int rightEdgeRaw = pixels.FindNextSolidPixelIndex(verticallyOffCenter, 1);
            // Also need to convert the value to an offset from the right edge.
            int rightEdge = texture.width - (rightEdgeRaw % texture.width);

            int horizontallyOffCenter = texture.width / 4 + (texture.height / 2) * texture.width;
            int topEdgeRaw = pixels.FindNextSolidPixelIndex(horizontallyOffCenter, texture.width);
            int topEdge = texture.height - (topEdgeRaw / texture.width) - 1;

            int bottomEdgeRaw = pixels.FindNextSolidPixelIndex(horizontallyOffCenter, -texture.width);
            int bottomEdge = bottomEdgeRaw / texture.width + 1;

            rectTransform.offsetMin = new Vector2(leftEdge * scaleX, bottomEdge * scaleY);
            rectTransform.offsetMax = new Vector2(-rightEdge * scaleX, -topEdge * scaleY);
        }
    }
}