using UnityEngine;

namespace TRS.CaptureTool
{
    public class ShowDuringScreenshotScript : MonoBehaviour
    {
        public bool disableAfterAwake;

        void Awake()
        {
            ScreenshotScript.WillTakeScreenshot += Enable;
            ScreenshotScript.ScreenshotTaken += Disable;
            if (disableAfterAwake) gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            ScreenshotScript.WillTakeScreenshot -= Enable;
            ScreenshotScript.ScreenshotTaken -= Disable;
        }

        void Enable(ScreenshotScript screenshotScript, int width, int height, int scale)
        {
            gameObject.SetActive(true);
        }

        void Disable(ScreenshotScript screenshotScript, Texture2D screenshotTexture)
        {
            gameObject.SetActive(false);
        }
    }
}