using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    public static class AppShare
    {
        /* Deep Link URLs
    What's App:
    whatsapp://send?
    Facebook:
    https://developers.facebook.com/docs/sharing/reference/share-dialog
    Twitter:
    https://developer.twitter.com/en/docs/twitter-for-websites/tweet-button/overview.html
    Reddit:
    https://www.reddit.com/dev/api/#POST_api_submit
    Google Plus:
    https://developers.google.com/+/web/share/
    Pinterest:
    https://developers.pinterest.com/docs/widgets/save/?
*/

        public static void Share(string url)
        {
            // Debug.Log("Share: " + url);
#if !UNITY_EDITOR && UNITY_WEBGL
            OpenWindow(url);
            // Application.ExternalEval("window.open('" + url + "', '_blank')");
#else
            Application.OpenURL(url);
#endif
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        public static void OpenWindow(string url)
        {
            openWindow(url);
        }

        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void openWindow(string url);
#endif

        public static void ShareToWhatsApp(string text)
        {
            Share(WhatsAppUrl(text));
        }

        public static string WhatsAppUrl(string text)
        {
                const string BASE_URL = "whatsapp://send?";

#pragma warning disable 0618
            string arguments = "&text=" + WWW.EscapeURL(text);
#pragma warning restore 0618
            return WWWExtensions.CombinedUrl(BASE_URL, arguments);
        }
    }
}
