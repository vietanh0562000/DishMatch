using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    public static class WebShare
    {
        /* Share URL Apis
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

        public static void ShareToTwitter(string url = "", string text = "", string hashtags = "", string username = "")
        {
            Share(TwitterShareUrl(url, text, hashtags, username));
        }

        public static void ShareToTweetDeck()
        {
            Share(TweetDeckUrl());
        }

        public static void ShareToReddit(string subreddit, string url = "", string text = "", string title = "")
        {
            Share(RedditShareUrl(subreddit, url, text, title));
        }

        public static void ShareToGooglePlus(string url = "", string text = "")
        {
            Share(GooglePlusShareUrl(url, text));
        }

        public static void ShareToPinterest(string media = "", string description = "", string url = "")
        {
            Share(PinterestShareUrl(media, description, url));
        }

        public static void ShareToFacebook(CaptureToolConfig config, string url = "", string quote = "", string hashtag = "")
        {
            Share(FacebookShareUrl(config, url, quote, hashtag));
        }

        public static void ShareByEmail(string url = "", string body = "", string email = "", string title = "")
        {
            Share(EmailShareUrl(url, body, email, title));
        }

        public static string TwitterShareUrl(string url = "", string text = "", string hashtags = "", string username = "")
        {
            const string BASE_URL = "https://twitter.com/intent/tweet?";

#pragma warning disable 0618
            string arguments = "";
            if (!string.IsNullOrEmpty(text))
                arguments += "&text=" + WWW.EscapeURL(text);
            if (!string.IsNullOrEmpty(hashtags))
                arguments += "&hashtags=" + hashtags;
            if (!string.IsNullOrEmpty(url))
                arguments += "&url=" + WWW.EscapeURL(url);
            if (!string.IsNullOrEmpty(username))
                arguments += "&via=" + username;
#pragma warning restore 0618

            return WWWExtensions.CombinedUrl(BASE_URL, arguments);
        }

        public static string TweetDeckUrl()
        {
            return "https://tweetdeck.twitter.com/";
        }

        public static string RedditShareUrl(string subreddit, string url = "", string text = "", string title = "")
        {
            string baseUrl = "https://www.reddit.com/r/" + subreddit + "/submit?";

#pragma warning disable 0618
            string arguments = "";
            if (!string.IsNullOrEmpty(title))
                arguments += "&title=" + WWW.EscapeURL(title);
            if (!string.IsNullOrEmpty(text))
                arguments += "&text=" + WWW.EscapeURL(text);
            if (!string.IsNullOrEmpty(url))
                arguments += "&url=" + WWW.EscapeURL(url);
#pragma warning restore 0618

            return WWWExtensions.CombinedUrl(baseUrl, arguments);
        }

        public static string GooglePlusShareUrl(string url = "", string text = "")
        {
            const string BASE_URL = "https://plus.google.com/share?";

#pragma warning disable 0618
            string arguments = "";
            if (!string.IsNullOrEmpty(text))
                arguments += "&text=" + WWW.EscapeURL(text);
            if (!string.IsNullOrEmpty(url))
                arguments += "&url=" + WWW.EscapeURL(url);
#pragma warning restore 0618

            return WWWExtensions.CombinedUrl(BASE_URL, arguments);
        }

        public static string PinterestShareUrl(string media = "", string description = "", string url = "")
        {
            const string BASE_URL = "https://www.pinterest.com/pin/create/button/?";

#pragma warning disable 0618
            string arguments = "";
            if (!string.IsNullOrEmpty(media))
                arguments += "&media=" + WWW.EscapeURL(media);
            if (!string.IsNullOrEmpty(description))
                arguments += "&description=" + WWW.EscapeURL(description);
            if (!string.IsNullOrEmpty(url))
                arguments += "&url=" + WWW.EscapeURL(url);
#pragma warning restore 0618

            return WWWExtensions.CombinedUrl(BASE_URL, arguments);
        }

        public static string FacebookShareUrl(CaptureToolConfig config, string url = "", string quote = "", string hashtag = "")
        {
            if (config != null)
            {
                if (string.IsNullOrEmpty(config.facebookAppId))
                {
                    Debug.LogError("Facebook Error: Cannot create url without required Facebook app id set in config.");
                    return "";
                }
            }
            else
            {
                Debug.LogError("Facebook Error: Cannot create url without valid config with Facebook app id set.");
                return "";
            }

            const string BASE_URL = "https://www.facebook.com/dialog/share?";

#pragma warning disable 0618
            string arguments = "&app_id=" + config.facebookAppId;
            if (!string.IsNullOrEmpty(quote))
                arguments += "&quote=" + WWW.EscapeURL(quote);
            if (!string.IsNullOrEmpty(hashtag))
                arguments += "&hashtag=" + hashtag;
            if (!string.IsNullOrEmpty(url))
                arguments += "&href=" + WWW.EscapeURL(url);
#pragma warning disable 0618

            return WWWExtensions.CombinedUrl(BASE_URL, arguments);
        }

        public static string EmailShareUrl(string url = "", string body = "", string email = "", string subject = "")
        {
            string baseUrl = "mailto:";
            if (!string.IsNullOrEmpty(email))
                baseUrl += email;
            baseUrl += "?";

            string arguments = "";
            if (!string.IsNullOrEmpty(subject))
                arguments += "&subject=" + WWWExtensions.PlusEscapeUrl(subject);
            if (!string.IsNullOrEmpty(url))
                body += "\n\n" + url;
            if (!string.IsNullOrEmpty(body))
                arguments += "&body=" + WWWExtensions.PlusEscapeUrl(body);

            return WWWExtensions.CombinedUrl(baseUrl, arguments);
        }
    }
}