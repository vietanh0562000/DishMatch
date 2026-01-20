using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public class CaptureToolConfig : ScriptableObject
    {
        public bool imgurFreeMode = false;
        public bool imgurAnonymousMode = false;
        public string imgurClientId = ""; // Short alphanumeric value
        public string imgurClientSecret = ""; // Long alphanumeric value
        public string imgurRefreshToken = ""; // Long alphanumeric value
        [Tooltip("This variable is obsolete and has been replaced with the imgurXRapidAPIKey.")]
        public string imgurXMashapeKey = ""; // Long alphanumeric value
        public string imgurXRapidAPIKey = ""; // Long alphanumeric value

        [HideInInspector]
        public string imgurAccessToken = "";

        public string giphyApiKey = ""; // Long alphanumeric value
        public string giphyUsername = ""; // Required for approved apps only. Otherwise leave empty

        // App information to get approval to tweet on behalf of other users.
        public string twitterConsumerKey = ""; // Short alphanumeric value
        [UnityEngine.Serialization.FormerlySerializedAs("twitterconsumerSecret")]
        public string twitterConsumerSecret = ""; // Long alphanumeric value

        bool useDeveloperTwitter { get { return (Application.isEditor || useDeveloperTwitterInProduction) && !forcePlayerLogIn; } }
        public bool forcePlayerLogIn;
        public bool useDeveloperTwitterInProduction;
        public string twitterUsername { get { return useDeveloperTwitter ? twitterDeveloperUsername : twitterPlayerUsername; } }
        public string twitterAccessToken { get { return useDeveloperTwitter ? twitterDeveloperAccessToken : twitterPlayerAccessToken; } }
        public string twitterAccessTokenSecret { get { return useDeveloperTwitter ? twitterDeveloperAccessTokenSecret : twitterPlayerAccessTokenSecret; } }

        public string altTwitterUsername { get { return useDeveloperTwitter ? twitterPlayerUsername : twitterDeveloperUsername; } }
        public string altTwitterAccessToken { get { return useDeveloperTwitter ? twitterPlayerAccessToken : twitterDeveloperAccessToken; } }
        public string altTwitterAccessTokenSecret { get { return useDeveloperTwitter ? twitterPlayerAccessTokenSecret : twitterDeveloperAccessTokenSecret; } }


        public string twitterPlayerUsername = "";
        public string twitterPlayerAccessToken = "";
        public string twitterPlayerAccessTokenSecret = "";

        // App information to tweet on the app owner's acccount without need for pin authentication.
        // Either fill out all (username, token, and secret) or none. Code does not check if you filled out each piece.
        public string twitterDeveloperUsername = ""; // Twitter Username - No @
        public string twitterDeveloperAccessToken = ""; // Long alphanumeric value
        public string twitterDeveloperAccessTokenSecret = ""; // Long alphanumeric value

        public string facebookAppId = ""; // 16-digit number

        public void LoadKeys()
        {
            imgurAccessToken = SavedCaptureToolKeys.ImgurAccessToken();

            if (useDeveloperTwitter)
            {
                if (string.IsNullOrEmpty(twitterDeveloperUsername))
                    twitterDeveloperUsername = SavedCaptureToolKeys.TwitterGeneratedTokenUsername();
                if (string.IsNullOrEmpty(twitterDeveloperAccessToken))
                    twitterDeveloperAccessToken = SavedCaptureToolKeys.TwitterGeneratedAccessToken();
                if (string.IsNullOrEmpty(twitterDeveloperAccessTokenSecret))
                    twitterDeveloperAccessTokenSecret = SavedCaptureToolKeys.TwitterGeneratedAccessTokenSecret();
            }
            else
            {
                if (string.IsNullOrEmpty(twitterPlayerUsername))
                    twitterPlayerUsername = SavedCaptureToolKeys.TwitterGeneratedTokenUsername();
                if (string.IsNullOrEmpty(twitterPlayerAccessToken))
                    twitterPlayerAccessToken = SavedCaptureToolKeys.TwitterGeneratedAccessToken();
                if (string.IsNullOrEmpty(twitterPlayerAccessTokenSecret))
                    twitterPlayerAccessTokenSecret = SavedCaptureToolKeys.TwitterGeneratedAccessTokenSecret();
            }

            Twity.Oauth.consumerKey = twitterConsumerKey;
            Twity.Oauth.consumerSecret = twitterConsumerSecret;

            LoadTwitterAuthKeys();
        }

        public bool LoadTwitterAuthKeys(string selectedUsername = "")
        {
            bool hasUsername = !string.IsNullOrEmpty(selectedUsername);
            if ((hasUsername && twitterUsername == selectedUsername) || (!hasUsername && twitterUsername.Length > 0))
            {
                Twity.Client.screenName = twitterUsername;
                Twity.Oauth.accessToken = twitterAccessToken;
                Twity.Oauth.accessTokenSecret = twitterAccessTokenSecret;
                return true;
            }

            if ((hasUsername && altTwitterUsername == selectedUsername) || (!hasUsername && altTwitterUsername.Length > 0))
            {
                Twity.Client.screenName = altTwitterUsername;
                Twity.Oauth.accessToken = altTwitterAccessToken;
                Twity.Oauth.accessTokenSecret = altTwitterAccessTokenSecret;
                return true;
            }

            return false;
        }

        public void SetTwitterAuthKeys(string username, string accessToken, string accessTokenSecret)
        {
            if (useDeveloperTwitter)
            {
                twitterDeveloperUsername = username;
                twitterDeveloperAccessToken = accessToken;
                twitterDeveloperAccessTokenSecret = accessTokenSecret;
            }
            else
            {
                twitterPlayerUsername = username;
                twitterPlayerAccessToken = accessToken;
                twitterPlayerAccessTokenSecret = accessTokenSecret;
            }

            SavedCaptureToolKeys.SaveTwitterGeneratedAccessToken(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
        }
    }
}