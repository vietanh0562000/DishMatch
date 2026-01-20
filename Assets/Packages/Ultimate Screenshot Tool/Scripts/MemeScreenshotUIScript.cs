using UnityEngine;
using UnityEngine.UI;

// Note: This script does not attempt to copy and store the screenshot from the callback.
// If this is a behavior you need, please let me know: jacob@tangledrealitystudios.com
namespace TRS.CaptureTool
{
    public class MemeScreenshotUIScript : SaveScreenshotUIScript
    {
        public Text[] placeholderTexts;

        [Tooltip("Script used to capture the canvas with the text.")]
        public CanvasCaptureScript canvasCaptureScript;

        [Tooltip("Captures the canvas exactly as it appears on screen. Slightly slower.\n\nThe alternative is a method which adjusts layout slightly, but will capture more quickly and at the right scale.")]
        public bool useExactCaptureMethod = true;

        public override void SaveScreenshot()
        {
            if (screenshot == null)
            {
                Debug.LogError("No screenshot available to preview or save.");
                gameObject.SetActive(false);
                return;
            }

            screenshot = screenshot.EditableCopy();

            for (int i = 0; i < placeholderTexts.Length; ++i)
                placeholderTexts[i].text = "";

            Texture2D overlayTexture;
            if (useExactCaptureMethod)
            {
                overlayTexture = canvasCaptureScript.ExactCapture();
                overlayTexture.Resize(new Resolution { width = screenshot.width, height = screenshot.height });
            }
            else
            {
                Resolution captureResolution = new Resolution { width = screenshot.width, height = screenshot.height };
                overlayTexture = canvasCaptureScript.Capture(captureResolution);
            }

            LayerInFrontTextureTransformation textureTransformation = ScriptableObject.CreateInstance<LayerInFrontTextureTransformation>();
            textureTransformation.Init(overlayTexture, Vector2.zero, TextureTransformation.LayerPositionPoint.Center, true, true, Color.black, true);
            screenshot = screenshot.ApplyTransformation(textureTransformation, true);

            base.SaveScreenshot();
        }
    }
}