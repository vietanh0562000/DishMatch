using UnityEngine;
using UnityEngine.UI;

// Twitter does not support CORS, so shares must happen indirectly in the browser.
// https://stackoverflow.com/questions/35879943/twitter-api-authorization-fails-cors-preflight-in-browser
// https://twittercommunity.com/t/will-twitter-api-support-cors-headers-soon/28276/8
namespace TRS.CaptureTool.Share
{
    public class DemoShareScript : MonoBehaviour
    {
        public ShareScript shareScript;
        public InstantTwitterShareScript instantTwitterShareScript;
        public string noMediaToShareText = "No Media to Share";

        Button button;
        Text text;

        void Awake()
        {
            button = GetComponent<Button>();
            text = GetComponentInChildren<Text>();
        }

        void Update()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            text.text = "Share";
            button.interactable = !string.IsNullOrEmpty(shareScript.mediaToUploadPath) && !shareScript.uploadingToServer;
#elif !UNITY_WEBGL
            button.interactable = !string.IsNullOrEmpty(shareScript.mediaToUploadPath);
            if(!button.interactable)
                text.text = noMediaToShareText;
            else
                text.text = "Share to Twitter";
#else
            // Twitter does not support CORS, so shares must happen indirectly in the browser.
            // https://stackoverflow.com/questions/35879943/twitter-api-authorization-fails-cors-preflight-in-browser
            // https://twittercommunity.com/t/will-twitter-api-support-cors-headers-soon/28276/8
            button.interactable = !string.IsNullOrEmpty(shareScript.urlToShare);
            if (!button.interactable)
                text.text = noMediaToShareText;
            else
                text.text = "Share to Twitter";
#endif
        }

        public void DemoShare()
        {
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
            APIShare.NativeShare(shareScript.mediaToUploadPath, shareScript.defaultText, shareScript.defaultUrl);
#elif !UNITY_WEBGL
            System.Action<string> onComplete = (statusUrl) => { if (string.IsNullOrEmpty(statusUrl)) { return; } Application.OpenURL(statusUrl); };

            instantTwitterShareScript.config = shareScript.config;
            instantTwitterShareScript.filePath = shareScript.mediaToUploadPath;
            instantTwitterShareScript.onComplete = onComplete;
            instantTwitterShareScript.gameObject.SetActive(true);
#else
            // Twitter does not support CORS, so shares must happen indirectly in the browser.
            // https://stackoverflow.com/questions/35879943/twitter-api-authorization-fails-cors-preflight-in-browser
            // https://twittercommunity.com/t/will-twitter-api-support-cors-headers-soon/28276/8
            WebShare.ShareToTwitter(shareScript.urlToShare, "@tangled_reality " + shareScript.twitterText, shareScript.twitterHashtags);
#endif
        }
    }
}