using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public class InstantScreenshotNativeShareButtonScript : MonoBehaviour
    {
        [Tooltip("ShareScript with the share settings.")]
        public ShareScript shareScript;
        [Tooltip("ScreenshotScript that will take the screenshot.")]
        public ScreenshotScript screenshotScript;

        // Used to differentiate this button causing a save event from any other screenshot save event.
        bool wasClicked;

        // Awake/OnDestroy used over OnEnable/OnDisable as this button is hidden during captures.
        // If using OnEnable/OnDisable, it would unsubscribe before the save event and re-subscribe after it.
        void Awake()
        {
            ScreenshotScript.ScreenshotSaved += ScreenshotSaved;
        }

        void OnDestroy()
        {
            ScreenshotScript.ScreenshotSaved -= ScreenshotSaved;
        }

        public void OnClick()
        {
            wasClicked = true;
            screenshotScript.TakeSingleScreenshot();
        }

        void ScreenshotSaved(ScreenshotScript screenshotScript, string filePath)
        {
            if (!wasClicked)
                return;
            wasClicked = false;

            APIShare.NativeShare(filePath, shareScript.defaultText, shareScript.defaultUrl);
        }
    }
}