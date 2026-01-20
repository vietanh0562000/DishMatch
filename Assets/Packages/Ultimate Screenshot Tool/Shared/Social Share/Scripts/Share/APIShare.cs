using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using TRS.CaptureTool.Extras;
using Twity.DataModels.Core;
using UnityEngine;
using UnityEngine.Networking;
namespace TRS.CaptureTool.Share
{
    public static class APIShare
    {
        public static void NativeShare(string filePath, string text = "", string url = "")
        {
            Debug.Log("NativeShare filePath: " + filePath);
        }

        public static void UploadToImgur(CaptureToolConfig config, string filePath, string description = "", string title = "", string album = "", System.Action<string, string> onComplete = null)
        {
            CoroutineBehaviour.StaticStartCoroutine(UploadToImgurAsync(config, filePath, description, title, album, onComplete));
        }

        public static void UploadToGiphy(CaptureToolConfig config, string filePath, string tags = "", string rehostUrl = "", System.Action<string, string> onComplete = null)
        {
            CoroutineBehaviour.StaticStartCoroutine(UploadToGiphyAsync(config, filePath, tags, rehostUrl, onComplete));
        }

        public static void UploadToTwitter(CaptureToolConfig config, string filePath, string username, string text = "", System.Action<Tweet, string, string> onComplete = null)
        {
            CoroutineBehaviour.StaticStartCoroutine(UploadToTwitterAsync(config, filePath, username, text, onComplete));
        }

        public static void UploadToFileIO(string text, string expires = "1w", System.Action<string> onComplete = null)
        {
            CoroutineBehaviour.StaticStartCoroutine(UploadToFileIOAsync(text, expires, onComplete));
        }

        public static IEnumerator UploadToImgurAsync(CaptureToolConfig config, string filePath, string description = "", string title = "", string album = "", System.Action<string, string> onComplete = null)
        {
            if (config != null)
            {
                bool hasError = false;
                if (string.IsNullOrEmpty(config.imgurAccessToken))
                {
                    if (string.IsNullOrEmpty(config.imgurClientId))
                    {
                        Debug.LogError("Imgur Error: Upload attempted without Imgur Client Id set in config.");
                        hasError = true;
                    }
                    if (string.IsNullOrEmpty(config.imgurClientSecret))
                    {
                        Debug.LogError("Imgur Error: Upload attempted without Imgur Client Secret set in config.");
                        hasError = true;
                    }
                    if (string.IsNullOrEmpty(config.imgurRefreshToken))
                    {
                        Debug.LogError("Imgur Error: Upload attempted without Imgur Refresh Token set in config.");
                        hasError = true;
                    }
                }

                if (hasError)
                    yield break;
            }
            else
            {
                Debug.LogError("Imgur Error: Upload attempted without config file.");
                yield break;
            }

            if (CheckForFileSizeError("Imgur", filePath, 200000000, 20000000))
                yield break;

            string baseUrl;
            UnityWebRequest www;
            WWWForm form = new WWWForm();
            string accessToken = config.imgurAccessToken;
#pragma warning disable 0429, 0162
            if (!config.imgurAnonymousMode && string.IsNullOrEmpty(accessToken))
            {
                form.AddField("client_id", config.imgurClientId);
                form.AddField("client_secret", config.imgurClientSecret);
                form.AddField("refresh_token", config.imgurRefreshToken);
                form.AddField("grant_type", "refresh_token");

                www = UnityWebRequest.Post("https://api.imgur.com/oauth2/token", form);
                using (www)
                {
                    yield return www.SendWebRequest();

                    if (!string.IsNullOrEmpty(www.error)) //www.isNetworkError || www.isHttpError
                    {
                        Debug.LogError("Imgur authentication failed with error: " + www.error + " response: " + www.downloadHandler.text);
                        yield break;
                    }

                    var resultJSON = JSON.Parse(www.downloadHandler.text);
                    string expiresIn = resultJSON["expires_in"].Value;
                    accessToken = resultJSON["access_token"].Value;
                    SavedCaptureToolKeys.SaveImgurAccessToken(accessToken, expiresIn);
                }
            }
#pragma warning restore 0429, 0162

            string fileName = System.IO.Path.GetFileName(filePath);
            string mimeType = PathExtensions.MimeTypeForFilePath(filePath);
            form.AddBinaryData("image", System.IO.File.ReadAllBytes(filePath), fileName, mimeType);

            if (!string.IsNullOrEmpty(title))
                form.AddField("title", title);
            if (!string.IsNullOrEmpty(description))
                form.AddField("description", description);
            if (!string.IsNullOrEmpty(album))
                form.AddField("album", album);
#pragma warning disable 0162
            baseUrl = config.imgurFreeMode ? "https://api.imgur.com/3/image" : "https://imgur-apiv3.p.rapidapi.com/3/image";
            www = UnityWebRequest.Post(baseUrl, form);
            if (config.imgurAnonymousMode)
                www.SetRequestHeader("Authorization", "Client-ID " + config.imgurClientId);
            else
                www.SetRequestHeader("Authorization", "Bearer " + accessToken);
            if (!config.imgurFreeMode)
            {
                www.SetRequestHeader("X-Rapidapi-Host", "imgur-apiv3.p.rapidapi.com");
                www.SetRequestHeader("X-Rapidapi-Key", config.imgurXRapidAPIKey);
            }
#pragma warning restore 0162

            string mediaUrl = null;
            string mediaPostUrl = null;
            using (www)
            {
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error)) //www.isNetworkError || www.isHttpError
                {
                    Debug.LogError("Imgur upload failed with error: " + www.error + " response: " + www.downloadHandler.text);
                    yield break;
                }

                //Debug.Log("Imgur Download Handler Text: " + www.downloadHandler.text);
                var resultJSON = JSON.Parse(www.downloadHandler.text);
                string mediaId = resultJSON["data"]["id"].Value;
                //Debug.Log("Media successfully uploaded to imgur with id: " + mediaId);

                mediaUrl = resultJSON["data"]["link"].Value;
                mediaPostUrl = "https://imgur.com/" + mediaId;
            }

            onComplete(mediaUrl, mediaPostUrl);
        }

        public static IEnumerator UploadToGiphyAsync(CaptureToolConfig config, string filePath, string tags = "", string rehostUrl = "", System.Action<string, string> onComplete = null)
        {
            if (config != null)
            {
                if (string.IsNullOrEmpty(config.giphyApiKey))
                {
                    Debug.LogError("Giphy Error: Upload attempted without Giphy API key set in config.");
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Giphy Error: Upload attempted without config file.");
                yield break;
            }

            if (CheckForFileSizeError("Giphy", filePath, 100000000, 100000000))
                yield break;

            WWWForm form = new WWWForm();
            form.AddField("api_key", config.giphyApiKey);
            if (!string.IsNullOrEmpty(config.giphyUsername))
                form.AddField("username", config.giphyUsername);
            if (!string.IsNullOrEmpty(rehostUrl))
                form.AddField("source_image_url", rehostUrl);
            else
            {
                string fileName = System.IO.Path.GetFileName(filePath);
                string mimeType = PathExtensions.MimeTypeForFilePath(filePath);
                form.AddBinaryData("file", System.IO.File.ReadAllBytes(filePath), fileName, mimeType);
            }
            if (!string.IsNullOrEmpty(tags))
                form.AddField("tags", tags);

            UnityWebRequest www = UnityWebRequest.Post("https://upload.giphy.com/v1/gifs", form);
            www.SetRequestHeader("api_key", config.giphyApiKey);

            string resultUrl = null;
            using (www)
            {
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error)) //www.isNetworkError || www.isHttpError
                {
                    Debug.LogError("Giphy upload failed with error: " + www.error + " response: " + www.downloadHandler.text);
                    yield break;
                }

                //Debug.Log("Giphy Download Handler Text: " + www.downloadHandler.text);
                var resultJSON = JSON.Parse(www.downloadHandler.text);
                string mediaId = resultJSON["data"]["id"].Value;
                //Debug.Log("Media successfully uploaded to giphy with id: " + mediaId);
                resultUrl = "https://media.giphy.com/media/" + mediaId + "/giphy.gif";
            }

            onComplete(resultUrl, resultUrl);
        }

        public static IEnumerator UploadToTwitterAsync(CaptureToolConfig config, string filePath, string username, string text = "", System.Action<Tweet, string, string> onComplete = null)
        {
            if (username != Twity.Client.screenName)
            {
                if (config != null)
                {
                    bool success = config.LoadTwitterAuthKeys(username);
                    if (!success)
                    {
                        Debug.LogError("Twitter Error: Invalid authentication information for @" + username);
                        yield break;
                    }
                }
                else if (string.IsNullOrEmpty(Twity.Oauth.accessToken))
                {
                    Debug.LogError("Twitter Error: Tweet attempted without authentication or config file.");
                    yield break;
                }
            }

            if (CheckForFileSizeError("Twitter", filePath, 15000000, 5000000))
                yield break;

            Twity.TwitterCallback StatusUpdateCallback = delegate (bool success, string response)
            {
                Tweet tweet = null;
                string tweetUrl = null;
                if (success)
                {
                    tweet = JsonUtility.FromJson<Tweet>(response);
                    string tweetUsername = string.IsNullOrEmpty(username) ? config.twitterUsername : username;
                    if (string.IsNullOrEmpty(tweetUsername)) tweetUsername = "AnyUser";

                    tweetUrl = "https://twitter.com/" + tweetUsername + "/status/" + tweet.id_str;
                    Debug.Log("Tweet shared: " + tweetUrl);
                }
                else
                    Debug.Log("Tweet failed: " + response);

                if (onComplete != null)
                    onComplete(tweet, tweetUrl, response);
            };

            Twity.TwitterCallback MediaUploadCallback = delegate (bool success, string response)
            {
                if (success)
                {
                    UploadMedia media = JsonUtility.FromJson<UploadMedia>(response);

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters["status"] = text;
                    parameters["media_ids"] = media.media_id.ToString();
                    CoroutineBehaviour.StaticStartCoroutine(Twity.Client.MakePost(parameters, StatusUpdateCallback));
                }
                else
                    Debug.Log("Twitter media upload failed: " + response);
            };

            if (filePath.Length > 0)
            {
                byte[] imgBinary = System.IO.File.ReadAllBytes(filePath);
                CoroutineBehaviour.StaticStartCoroutine(Twity.Client.UploadMedia(imgBinary, MediaUploadCallback));
            }
            else
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>() { { "status", text } };
                CoroutineBehaviour.StaticStartCoroutine(Twity.Client.MakePost(parameters, StatusUpdateCallback));
            }

            yield break;
        }

        public static IEnumerator UploadToFileIOAsync(string text, string expires = "1w", System.Action<string> onComplete = null)
        {
            WWWForm form = new WWWForm();
            form.AddField("text", text);

            string resultUrl = null;
            using (UnityWebRequest www = UnityWebRequest.Post("https://file.io/?expires=" + expires, form))
            {
                yield return www.SendWebRequest();

                if (!string.IsNullOrEmpty(www.error)) //www.isNetworkError || www.isHttpError
                {
                    Debug.LogError("File IO upload failed with error: " + www.error + " response: " + www.downloadHandler.text);
                    yield break;
                }

                //Debug.Log("File IO Download Handler Text: " + www.downloadHandler.text);
                var resultJSON = JSON.Parse(www.downloadHandler.text);
                resultUrl = resultJSON["link"].Value;
            }

            onComplete(resultUrl);
        }

        static bool CheckForFileSizeError(string service, string filePath, int maxGifSize, int maxImageSize)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;

            bool isGif = ShareScript.FileTypeForExtension(System.IO.Path.GetExtension(filePath)) == ShareScript.FileType.GIF;
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
            if ((isGif && fileInfo.Length > maxGifSize) || (isGif && fileInfo.Length > maxImageSize))
            {
                float bytesInAMegabyte = 1000000;
                if (isGif)
                    Debug.LogError(service + " Error: Cannot upload a gif larger than " + (float)maxGifSize/ bytesInAMegabyte + " MB");
                else
                    Debug.LogError(service + " Error: Cannot upload an image larger than " + (float)maxImageSize / bytesInAMegabyte + " MB");
                return true;
            }

            return false;
        }
    }
}