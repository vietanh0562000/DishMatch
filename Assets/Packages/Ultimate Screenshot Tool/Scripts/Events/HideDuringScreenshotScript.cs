using UnityEngine;

namespace TRS.CaptureTool
{
    public class HideDuringScreenshotScript : MonoBehaviour
    {
        public bool disableAfterAwake;

        void Awake()
        {
            ScreenshotScript.WillTakeScreenshot += Disable;
            ScreenshotScript.ScreenshotTaken += Enable;
            if (disableAfterAwake) gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            ScreenshotScript.WillTakeScreenshot -= Disable;
            ScreenshotScript.ScreenshotTaken -= Enable;
        }

        void Enable(ScreenshotScript screenshotScript, Texture2D screenshotTexture)
        {
            gameObject.SetActive(true);
        }

        void Disable(ScreenshotScript screenshotScript, int width, int height, int scale)
        {
            gameObject.SetActive(false);
        }
    }
}