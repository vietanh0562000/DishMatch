using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public class InstantScreenshotShareButtonScript : MonoBehaviour
    {
        [Tooltip("ShareScript with the share settings.")]
        public ShareScript shareScript;
        [Tooltip("ScreenshotScript that will take the screenshot.")]
        public ScreenshotScript screenshotScript;
        [Tooltip("InstantTwitterShareScript that will display the UI for sharing the screenshot.")]
        public InstantTwitterShareScript instantTwitterShareScript;

        [UnityEngine.Serialization.FormerlySerializedAs("username")]
        [Tooltip("Optional username to override the value from the ShareScript's config's twitterUsername property.")]
        public string usernameOverride;
        [Tooltip("Comma-separated list of additional hashtags. (Ex. screenshotsaturday,indiedevhour)")]
        public string extraHashtags;

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

            string fullHashtagString = shareScript.twitterHashtagsText;
            foreach (string hashtag in extraHashtags.Split(','))
                fullHashtagString += " #" + hashtag;

            instantTwitterShareScript.filePath = filePath;
            if (!string.IsNullOrEmpty(usernameOverride))
                instantTwitterShareScript.username = usernameOverride;
            instantTwitterShareScript.defaultText = shareScript.twitterText + fullHashtagString;
            instantTwitterShareScript.config = shareScript.config;
            instantTwitterShareScript.gameObject.SetActive(true);
        }
    }
}