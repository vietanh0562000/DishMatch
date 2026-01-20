using UnityEngine;
using UnityEngine.UI;

// Displays the most recent screenshot.
using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    // Finalizers/Deconstructors (~ deinit methods) are not called consistently.
    // Therefore, the ScreenshotTaken method may get called multiple times.
    [RequireComponent(typeof(RawImage))]
    public class LastTakenScreenshotScript : MonoBehaviour
    {
        public Texture2D defaultTexture;
        [Tooltip("Whether to make a copy of the texture in this script, so the texture is still available here even if destroyed elsewhere.")]
        public bool copyTexture;

        bool textureIsCopy;

        RawImage rawImage;
        AspectFitScript aspectFitScript;

        public LastTakenScreenshotScript()
        {
            ScreenshotScript.ScreenshotTaken += ScreenshotTaken;
        }


        ~LastTakenScreenshotScript()
        {
            ScreenshotScript.ScreenshotTaken -= ScreenshotTaken;
        }

        void Awake()
        {

            rawImage = GetComponent<RawImage>();
            aspectFitScript = GetComponent<AspectFitScript>();
            if (defaultTexture != null)
                UpdateImage(defaultTexture);
        }

        void OnDestroy()
        {
            CleanUp();
        }

        void ScreenshotTaken(ScreenshotScript screenshotScript, Texture2D screenshotTexture)
        {
            if (rawImage == null)
            {
                defaultTexture = screenshotTexture;
                return;
            }

            UpdateImage(screenshotTexture);
        }

        void UpdateImage(Texture2D texture)
        {
            if (aspectFitScript != null)
                aspectFitScript.SetTexture(texture);
            else
            {
                CleanUp();

                Texture2D textureToUse = texture;
                if(copyTexture)
                {
                    textureIsCopy = true;
                    textureToUse = new Texture2D(texture.width, texture.height, texture.format, texture.mipmapCount > 1);
                    Graphics.CopyTexture(texture, textureToUse);
                    textureToUse.Apply(false);
                }

                rawImage.texture = textureToUse;
            }
        }

        void CleanUp() {
            if (textureIsCopy)
                MonoBehaviourExtended.FlexibleDestroy(rawImage.texture);
        }
    }
}