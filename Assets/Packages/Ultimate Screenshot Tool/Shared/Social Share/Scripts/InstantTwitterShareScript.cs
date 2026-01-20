using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Share
{
    public class InstantTwitterShareScript : MonoBehaviour
    {
        public GameObject twitterPinPanel;
        public InputField tweetInputField;
        public InputField pinInputField;
        public Text pinErrorText;

        [UnityEngine.Serialization.FormerlySerializedAs("overrideConfig")]
        [Tooltip("Optional CaptureToolConfig. By default, the tokens will be fetched for the input username (or the current access token will be used if neither are provided).")]
        public CaptureToolConfig config;
        [Tooltip("Optional Twitter username to use when sharing. Forces a specific username rather than using the current access token.")]
        public string username;
        [Tooltip("Optional text to put in the tweet input field.")]
        public string defaultText;
        [HideInInspector]
        public string filePath;
        [HideInInspector]
        public System.Action<string> onComplete;

        void OnEnable()
        {
            bool hasUsername = !string.IsNullOrEmpty(username);
            if (config != null && (!hasUsername || username != Twity.Client.screenName))
                config.LoadTwitterAuthKeys(username);

            if (string.IsNullOrEmpty(Twity.Oauth.accessToken) || (hasUsername && username != Twity.Client.screenName))
            {
                if (string.IsNullOrEmpty(Twity.Oauth.consumerKey) || string.IsNullOrEmpty(Twity.Oauth.consumerSecret))
                    Debug.LogError("Twitter Error: Twitter consumer key and consumer secret must be set in the config in the share tab of the tool or in an override config on this script.");
                else
                    StartCoroutine(Twity.Client.GenerateRequestToken(RequestTokenCallback));
            }

            tweetInputField.text = defaultText;
        }

        public void SubmitPin()
        {
            pinErrorText.text = "";
            GenerateAccessToken(pinInputField.text);
        }

        public void Share()
        {
            System.Action<Twity.DataModels.Core.Tweet, string, string> fullOnComplete = (tweet, statusUrl, response) =>
            {
                if (onComplete != null)
                    onComplete(statusUrl);
            };

            if (config != null)
                APIShare.UploadToTwitter(config, filePath, username, tweetInputField.text, fullOnComplete);
            else
                Debug.LogError("No config set. Provide a CaptureToolConfig with the authentication details to get started.");
            gameObject.SetActive(false);
        }

        void RequestTokenCallback(bool success)
        {
            if (!success)
            {
                Debug.LogError("Request for Twitter token failed");
                return;
            }
            // When request successes, you can display `Twity.Oauth.authorizeURL` to user so that they may use a web browser to access Twitter.
            Application.OpenURL(Twity.Oauth.authorizeURL);
            twitterPinPanel.SetActive(true);
        }

        void GenerateAccessToken(string pin)
        {
            // pin is numbers displayed on web browser when user complete authorization.
            StartCoroutine(Twity.Client.GenerateAccessToken(pin, AccessTokenCallback));
        }

        void AccessTokenCallback(bool success)
        {
            if (!success)
            {
                pinErrorText.text = "Pin Failed";
                return;
            }
            // When success, authorization is completed. You can make request to other endpoint.
            // User's screen_name is in '`Twity.Client.screenName`.

            if (config != null)
                config.SetTwitterAuthKeys(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
            else
            {
                Debug.LogError("No config set. Provide a CaptureToolConfig with the authentication details to get started.");
                SavedCaptureToolKeys.SaveTwitterGeneratedAccessToken(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
            }

            twitterPinPanel.SetActive(false);
        }
    }
}