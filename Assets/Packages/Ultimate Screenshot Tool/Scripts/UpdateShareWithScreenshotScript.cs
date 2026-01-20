using UnityEngine;

using TRS.CaptureTool.Share;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ShareScript))]
    public class UpdateShareWithScreenshotScript : MonoBehaviour
    {
        int cachedWidth;
        int cachedHeight;

        ShareScript shareScript;

        void OnEnable()
        {
            if (shareScript == null)
                shareScript = GetComponent<ShareScript>();
            SubscribeToMediaEvents();
        }

        void OnDisable()
        {
            UnsubscribeFromMediaEvents();
        }

        void SubscribeToMediaEvents()
        {
            ScreenshotScript.WillTakeScreenshot += UpdateScaledMediaSize;
            ScreenshotScript.ScreenshotSaved += UpdateScreenshotMediaPath;
        }

        void UnsubscribeFromMediaEvents()
        {
            ScreenshotScript.WillTakeScreenshot -= UpdateScaledMediaSize;
            ScreenshotScript.ScreenshotSaved -= UpdateScreenshotMediaPath;
        }

        void UpdateScaledMediaSize(ScreenshotScript screenshotScript, int width, int height, int scale)
        {
            cachedWidth = width * scale;
            cachedHeight = height * scale;
        }

        void UpdateScreenshotMediaPath(ScreenshotScript screenshotScript, string filePath)
        {
            UpdateShareScript(filePath);
        }

        void UpdateShareScript(string filePath)
        {
            shareScript.mediaWidth = cachedWidth;
            shareScript.mediaHeight = cachedHeight;
            shareScript.mediaToUploadPath = filePath;
        }
    }
}