using UnityEngine;

namespace TRS.CaptureTool.Share
{
    public static class SavedCaptureToolKeys
    {
        const string IMGUR_ACCESS_TOKEN_KEY = "TRS_IMGUR_ACCESS_TOKEN_KEY";
        const string IMGUR_EXPIRATION_DATE_KEY = "TRS_IMGUR_EXPIRATION_DATE_KEY";

        const string TWITTER_USERNAME_KEY = "TRS_TWITTER_USERNAME_KEY";
        const string TWITTER_ACCESS_TOKEN_KEY = "TRS_TWITTER_ACCESS_TOKEN_KEY";
        const string TWITTER_ACCESS_TOKEN_SECRET_KEY = "TRS_TWITTER_ACCESS_TOKEN_SECRET_KEY";

        #region Imgur Access Token
        public static string ImgurAccessToken()
        {
            if (PlayerPrefs.HasKey(IMGUR_EXPIRATION_DATE_KEY))
            {
                string expirationDate = PlayerPrefs.GetString(IMGUR_EXPIRATION_DATE_KEY);
                if (string.IsNullOrEmpty(expirationDate))
                    return "";

                if (System.Convert.ToDateTime(expirationDate).CompareTo(System.DateTime.Now) > 0)
                    return PlayerPrefs.GetString(IMGUR_ACCESS_TOKEN_KEY);
                else
                    ClearImgurAccessToken();
            }

            return "";
        }

        public static void ClearImgurAccessToken()
        {
            PlayerPrefs.SetString(IMGUR_ACCESS_TOKEN_KEY, "");
            PlayerPrefs.SetString(IMGUR_EXPIRATION_DATE_KEY, "");
            PlayerPrefs.Save();
        }

        public static void SaveImgurAccessToken(string accessToken, string expiresIn)
        {
            System.DateTime dateTime = System.DateTime.Now.AddSeconds(System.Convert.ToInt64(expiresIn));

            PlayerPrefs.SetString(IMGUR_ACCESS_TOKEN_KEY, accessToken);
            PlayerPrefs.SetString(IMGUR_EXPIRATION_DATE_KEY, string.Format("{0:F}", dateTime));
            PlayerPrefs.Save();
        }
        #endregion

        #region Twitter Access Tokens
        public static string TwitterGeneratedTokenUsername()
        {
            if (PlayerPrefs.HasKey(TWITTER_USERNAME_KEY))
                return PlayerPrefs.GetString(TWITTER_USERNAME_KEY);

            return "";
        }

        public static string TwitterGeneratedAccessToken()
        {
            if (PlayerPrefs.HasKey(TWITTER_ACCESS_TOKEN_KEY))
                return PlayerPrefs.GetString(TWITTER_ACCESS_TOKEN_KEY);

            return "";
        }

        public static string TwitterGeneratedAccessTokenSecret()
        {
            if (PlayerPrefs.HasKey(TWITTER_ACCESS_TOKEN_SECRET_KEY))
                return PlayerPrefs.GetString(TWITTER_ACCESS_TOKEN_SECRET_KEY);

            return "";
        }

        public static void ClearTwitterGeneratedAccessToken()
        {
            PlayerPrefs.SetString(TWITTER_USERNAME_KEY, "");
            PlayerPrefs.SetString(TWITTER_ACCESS_TOKEN_KEY, "");
            PlayerPrefs.SetString(TWITTER_ACCESS_TOKEN_SECRET_KEY, "");
            PlayerPrefs.Save();
        }

        public static void SaveTwitterGeneratedAccessToken(string username, string accessToken, string accessTokenSecret)
        {
            PlayerPrefs.SetString(TWITTER_USERNAME_KEY, username);
            PlayerPrefs.SetString(TWITTER_ACCESS_TOKEN_KEY, accessToken);
            PlayerPrefs.SetString(TWITTER_ACCESS_TOKEN_SECRET_KEY, accessTokenSecret);
            PlayerPrefs.Save();
        }
        #endregion
    }
}