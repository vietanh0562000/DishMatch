using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Twity.Helpers;
using UnityEngine;
using UnityEngine.Networking;

// Tangled Reality Studios - 10/17/18 - switched to use else instead of two if checks in Unity version checks and switched isNetworkError to !string.IsNullOrEmpty()
namespace Twity
{
    public delegate void TwitterCallback(bool success, string response);
    public delegate void TwitterAuthenticationCallback(bool success);

    public class Client
    {

        public static string screenName;

        #region API Methods

        public static IEnumerator Get(string APIPath, Dictionary<string, string> APIParams, TwitterCallback callback)
        {
            string REQUEST_URL = "https://api.twitter.com/1.1/" + APIPath + ".json";
            SortedDictionary<string, string> parameters = Helper.ConvertToSortedDictionary(APIParams);

            string requestURL = REQUEST_URL + "?" + Helper.GenerateRequestparams(parameters);
            UnityWebRequest request = UnityWebRequest.Get(requestURL);
            request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");

            yield return SendRequest(request, parameters, "GET", REQUEST_URL, callback);
        }

        // Tangled Reality Studios - 10/25/18 - Add separate UploadMedia and MakePost functions
        public static IEnumerator UploadMedia(byte[] mediaData, TwitterCallback callback)
        {
            string REQUEST_URL = "https://upload.twitter.com/1.1/media/upload.json";
            WWWForm form = new WWWForm();
            form.AddBinaryData("media", mediaData);
            UnityWebRequest request = UnityWebRequest.Post(REQUEST_URL, form);
            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();
            yield return SendRequest(request, parameters, "POST", REQUEST_URL, callback);
        }

        public static IEnumerator MakePost(Dictionary<string, string> APIParams, TwitterCallback callback)
        {
            string REQUEST_URL = "https://api.twitter.com/1.1/statuses/update.json";
            SortedDictionary<string, string> parameters = Helper.ConvertToSortedDictionary(APIParams);
            UnityWebRequest request = UnityWebRequestUnlimited.Post(REQUEST_URL, APIParams);
            request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
            yield return SendRequest(request, parameters, "POST", REQUEST_URL, callback);
        }

        public static IEnumerator Post(string APIPath, Dictionary<string, string> APIParams, TwitterCallback callback)
        {
            List<string> endpointForFormdata = new List<string>
            {
                "media/upload",
                "account/update_profile_image",
                "account/update_profile_banner",
                "account/update_profile_background_image"
            };

            string REQUEST_URL = "";
            if (APIPath.Contains("media/"))
            {
                REQUEST_URL = "https://upload.twitter.com/1.1/" + APIPath + ".json";
            }
            else
            {
                REQUEST_URL = "https://api.twitter.com/1.1/" + APIPath + ".json";
            }
            // Tangled Reality Studios - 10/16/18 - Commented out log
            //            Debug.Log(REQUEST_URL);

            WWWForm form = new WWWForm();
            SortedDictionary<string, string> parameters = new SortedDictionary<string, string>();

            if (endpointForFormdata.IndexOf(APIPath) != -1)
            {
                // multipart/form-data

                foreach (KeyValuePair<string, string> parameter in APIParams)
                {
                    if (parameter.Key.Contains("media"))
                    {
                        form.AddBinaryData("media", Convert.FromBase64String(parameter.Value), "", "");
                    }
                    else if (parameter.Key == "image")
                    {
                        form.AddBinaryData("image", Convert.FromBase64String(parameter.Value), "", "");
                    }
                    else if (parameter.Key == "banner")
                    {
                        form.AddBinaryData("banner", Convert.FromBase64String(parameter.Value), "", "");
                    }
                    else
                    {
                        form.AddField(parameter.Key, parameter.Value);
                    }
                }


                UnityWebRequest request = UnityWebRequest.Post(REQUEST_URL, form);
                yield return SendRequest(request, parameters, "POST", REQUEST_URL, callback);
            }
            else if (APIPath == "media/metadata/createa")
            {
                parameters = Helper.ConvertToSortedDictionary(APIParams);
                foreach (KeyValuePair<string, string> parameter in APIParams)
                {
                    form.AddField(parameter.Key, parameter.Value);
                }

                UnityWebRequest request = UnityWebRequest.Post(REQUEST_URL, form);
                request.SetRequestHeader("ContentType", "text/plain; charset=UTF-8");
                yield return SendRequest(request, parameters, "POST", REQUEST_URL, callback);
            }
            else
            {
                // application/x-www-form-urlencoded

                parameters = Helper.ConvertToSortedDictionary(APIParams);
                foreach (KeyValuePair<string, string> parameter in APIParams)
                {
                    form.AddField(parameter.Key, parameter.Value);
                }

                UnityWebRequest request = UnityWebRequest.Post(REQUEST_URL, form);
                request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded");
                yield return SendRequest(request, parameters, "POST", REQUEST_URL, callback);

            }
        }

        public static IEnumerator GetOauth2BearerToken(TwitterAuthenticationCallback callback)
        {
            string url = "https://api.twitter.com/oauth2/token";

            string credential = Helper.UrlEncode(Oauth.consumerKey) + ":" + Helper.UrlEncode(Oauth.consumerSecret);
            credential = Convert.ToBase64String(Encoding.UTF8.GetBytes(credential));

            WWWForm form = new WWWForm();
            form.AddField("grant_type", "client_credentials");

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("ContentType", "application/x-www-form-urlencoded;charset=UTF-8");
            request.SetRequestHeader("Authorization", "Basic " + credential);

            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error)) //if (request.isNetworkError)
                callback(false);

            if (request.responseCode == 200 || request.responseCode == 201)
            {
                Twity.Oauth.bearerToken = JsonUtility.FromJson<Twity.DataModels.Oauth.BearerToken>(request.downloadHandler.text).access_token;
                callback(true);
            }
            else
            {
                callback(false);
            }
        }

        public static IEnumerator GenerateRequestToken(TwitterAuthenticationCallback callback)
        {
            yield return GenerateRequestToken(callback, "oob");
        }

        public static IEnumerator GenerateRequestToken(TwitterAuthenticationCallback callback, string callbackURL)
        {
            string url = "https://api.twitter.com/oauth/request_token";
            ClearTokens();

            SortedDictionary<string, string> p = new SortedDictionary<string, string>();
            p.Add("oauth_callback", callbackURL);

            // Tangled Reality Studios - 10/19/18 - Extra hassle to get around empty post body in older versions of Unity - changed until next comment
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes("hi");
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", Oauth.GenerateHeaderWithAccessToken(p, "POST", url));

            // Tangled Reality Studios - 10/17/18 - switched to use else instead of two if checks
            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error)) //if (request.isNetworkError)
            {
                callback(false);
            }
            else
            {
                if (request.responseCode == 200 || request.responseCode == 201)
                {
                    string[] arr = request.downloadHandler.text.Split("&"[0]);
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string s in arr)
                    {
                        string k = s.Split("="[0])[0];
                        string v = s.Split("="[0])[1];
                        d[k] = v;
                    }
                    Oauth.requestToken = d["oauth_token"];
                    Oauth.requestTokenSecret = d["oauth_token_secret"];
                    Oauth.authorizeURL = "https://api.twitter.com/oauth/authorize?oauth_token=" + Oauth.requestToken;
                    callback(true);
                }
                else
                {
                    callback(false);
                }
            }
        }

        public static IEnumerator GenerateAccessToken(string pin, TwitterAuthenticationCallback callback)
        {
            string url = "https://api.twitter.com/oauth/access_token";

            SortedDictionary<string, string> p = new SortedDictionary<string, string>();
            p.Add("oauth_verifier", pin);

            WWWForm form = new WWWForm();
            form.AddField("oauth_verifier", pin);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            request.SetRequestHeader("Authorization", Oauth.GenerateHeaderWithAccessToken(p, "POST", url));

            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error)) // if (request.isNetworkError)
            {
                callback(false);
            }
            else
            {
                if (request.responseCode == 200 || request.responseCode == 201)
                {
                    string[] arr = request.downloadHandler.text.Split("&"[0]);
                    Dictionary<string, string> d = new Dictionary<string, string>();
                    foreach (string s in arr)
                    {
                        string k = s.Split("="[0])[0];
                        string v = s.Split("="[0])[1];
                        d[k] = v;
                    }
                    Oauth.accessToken = d["oauth_token"];
                    Oauth.accessTokenSecret = d["oauth_token_secret"];
                    screenName = d["screen_name"];
                    callback(true);
                }
                else
                {
                    Debug.Log(request.responseCode);
                    callback(false);
                }
            }
        }

        #endregion

        #region RequestHelperMethods

        private static IEnumerator SendRequest(UnityWebRequest request, SortedDictionary<string, string> parameters, string method, string requestURL, TwitterCallback callback)
        {
            if (!string.IsNullOrEmpty(Oauth.accessToken))
            {
                request.SetRequestHeader("Authorization", Oauth.GenerateHeaderWithAccessToken(parameters, method, requestURL));
            }
            else if (!string.IsNullOrEmpty(Oauth.bearerToken))
            {
                request.SetRequestHeader("Authorization", "Bearer " + Oauth.bearerToken);
            }
            else if (method == "POST")
            {
                Debug.LogError("Twitter Error: You must be authenticated to complete this request. (Set keys in config or use in game UI.)");
                yield break;
            }

            yield return request.SendWebRequest();

            if (!string.IsNullOrEmpty(request.error)) // if (request.isNetworkError)
            {
                callback(false, JsonHelper.ArrayToObject(request.error));
            }
            else
            {
                if (request.responseCode == 200 || request.responseCode == 201)
                {
                    callback(true, JsonHelper.ArrayToObject(request.downloadHandler.text));
                }
                else
                {
                    Debug.Log(request.responseCode);
                    callback(false, JsonHelper.ArrayToObject(request.downloadHandler.text));
                }
            }
        }

        private static void ClearTokens()
        {
            Oauth.requestToken = String.Empty;
            Oauth.requestTokenSecret = String.Empty;
            Oauth.accessToken = String.Empty;
            Oauth.accessTokenSecret = String.Empty;
        }

        #endregion

    }

    // Tangled Reality Studios - 11/5/18 - added class as post wasn't encoding properly
    public class UnityWebRequestUnlimited : MonoBehaviour
    {
        private static string EscapeLongDataString(string stringToEscape)
        {
            StringBuilder sb = new StringBuilder();
            var length = stringToEscape.Length;

            var limit = 32767 - 1;
            for (int i = 0; i < length; i += limit)
                sb.Append(Helper.UrlEncode(stringToEscape.Substring(i, Math.Min(limit, length - i))));

            return sb.ToString();
        }

        public static byte[] SerializeSimpleForm(Dictionary<string, string> formFields)
        {
            string text = "";
            foreach (KeyValuePair<string, string> current in formFields)
            {
                if (text.Length > 0)
                    text += "&";
                text = text + Helper.UrlEncode(current.Key) + "=" + EscapeLongDataString(current.Value);
            }
            return Encoding.UTF8.GetBytes(text);
        }

        public static UnityWebRequest Post(string uri, Dictionary<string, string> formFields)
        {
            UnityWebRequest unityWebRequest = new UnityWebRequest(uri, "POST");
            byte[] data = null;
            if (formFields != null && formFields.Count != 0)
                data = UnityWebRequestUnlimited.SerializeSimpleForm(formFields);

            unityWebRequest.uploadHandler = new UploadHandlerRaw(data)
            {
                contentType = "application/x-www-form-urlencoded"
            };
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
            return unityWebRequest;
        }
    }

}

