#if UNITY_EDITOR
using System.Linq;
#endif
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool.Share
{
    [ExecuteInEditMode]
    public class ShareScript : MonoBehaviour
    {
        public enum FileType
        {
            PNG,
            JPG,
            TGA,
            GIF,
            MP4,
            Size
        }

        public readonly static string[] acceptedFileTypes = { "png", "jpg", "jpeg", "gif", "mp4" };
        public readonly static string[] mimeTypeForFileType = { "image/png", "image/jpeg", "image/gif" };

        public CaptureToolConfig config;

        #region MediaToUploadPath
        public string m_mediaToUploadPath = "";
        public string mediaToUploadPath
        {
            get
            {
                return m_mediaToUploadPath;
            }

            set
            {
                if (m_mediaToUploadPath == value) return;
                m_mediaToUploadPath = value;
                mediaPostUrl = "";
                mediaUrl = "";
            }
        }
        public string mediaToUploadDirectory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(mediaToUploadPath);
            }
        }
        public string mediaToUploadFileName
        {
            get
            {
                return System.IO.Path.GetFileName(mediaToUploadPath);
            }
        }
        public string mediaToUploadExtension
        {
            get
            {
                return System.IO.Path.GetExtension(mediaToUploadPath);
            }
        }
        public FileType mediaToUploadFileType
        {
            get
            {
                return FileTypeForExtension(mediaToUploadExtension);
            }
        }

        public void DeleteMediaToUploadPath()
        {
            if (!string.IsNullOrEmpty(mediaToUploadPath))
                System.IO.File.Delete(mediaToUploadPath);
            mediaToUploadPath = "";
        }
        #endregion

        public int mediaWidth;
        public int mediaHeight;

        public string mediaUrl = "";
        public string mediaPostUrl = "";
        public bool useLastMediaPostUrl;
        public bool useLastMediaUrl = true;

        public string defaultTitle = "";
        public string defaultText = "";
        public string defaultUrl = ""; // Ex. https://www.tangledrealitystudios.com/games/ultimate-screenshot-tool-demo/

        public bool useCustomUrl;
        public bool useCustomUrlFunction;

        public Dictionary<string, string> replacements
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();
                result["{media_url}"] = !string.IsNullOrEmpty(mediaUrl) ? mediaUrl : "";
                result["{media_post_url}"] = !string.IsNullOrEmpty(mediaPostUrl) ? mediaPostUrl : "";
                result["{media_width}"] = mediaWidth > 0 ? mediaWidth.ToString() : "";
                result["{media_height}"] = mediaHeight > 0 ? mediaHeight.ToString() : "";
                return result;
            }
        }

        // .NET <= 3.5 only defines 4 parameter System.Func. This a 5 parameter version.
        public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
        public Func<string, string, int, int, bool, string> CustomUrlFunction = CustomUrlExample;
        public string urlToShare
        {
            get
            {
                if (useCustomUrlFunction)
                    return CustomUrlFunction(defaultUrl, mediaUrl, mediaWidth, mediaHeight, false);
                else if (useCustomUrl)
                    return customUrl.FullUrl(replacements);
                else if (useLastMediaPostUrl && mediaPostUrl.Length > 0)
                    return mediaPostUrl;
                else if (useLastMediaUrl && mediaUrl.Length > 0)
                    return mediaUrl;
                return defaultUrl;
            }
        }

        public string serverUser = ""; // Ex. root@jacobhanshaw.com
        public string serverUploadFolder = ""; // Ex. /var/www/tangledrealitystudios.com/games/ultimate-screenshot-tool-demo/
        public string serverUploadUrl = ""; // Ex. https://www.tangledrealitystudios.com/games/ultimate-screenshot-tool-demo/
        public bool includeMediaTypeFolderInServerPath = true;
        public string[] folderForFileType = { "images", "images", "gifs", "videos" };

        public string imgurAlbum;
        public string imgurTitle;
        public string imgurDescription;
        public string imgurTags;
        public string imgurTagsText
        {
            get
            {
                if (imgurTags.Length == 0) return "";
                return "#" + imgurTags.Replace("\n", "").Replace(",", " #");
            }
        }

        public string giphyTags;

        [TextArea]
        public string twitterText;
        public string twitterHashtags;
        public string twitterHashtagsText
        {
            get
            {
                if (twitterHashtags.Length == 0) return "";
                return " #" + twitterHashtags.Replace("\n", "").Replace(",", " #");
            }
        }
        public string twitterFullText
        {
            get
            {
                string fullText = twitterText.Length > 0 ? twitterText : defaultText;
                string hashTagsText = twitterHashtagsText;
                if (hashTagsText.Length > 0) return fullText + " " + hashTagsText;
                return fullText;
            }
        }
        public string twitterViaUsername;
        public string twitterShareUsername;
        public string twitterSharePin;

        public string subreddits = "gamedev";
        public string redditTitle;
        public string redditText;

        public string googlePlusText;

        public string pinterestDescription;
        public string pinterestUrl;

        public string facebookQuote;
        public string facebookHashtag;

        public string emailAddress;
        public string emailSubject;
        public string emailBody;

        public CustomUrl customUrl;

        [SerializeField]
        int _customUrlCount = 1;
        public int customUrlCount
        {
            get
            {
                return _customUrlCount;
            }
            set
            {
                _customUrlCount = value;
#if UNITY_EDITOR
                bool[] newShowCustomUrlSettings = new bool[_customUrlCount];
                for (int i = 0; i < showCustomUrlSettings.Length; ++i)
                    newShowCustomUrlSettings[i] = showCustomUrlSettings[i];
                showCustomUrlSettings = newShowCustomUrlSettings;
#endif

                while (customUrls.Count < _customUrlCount)
                    customUrls.Add(customUrlExample);
                while (customUrls.Count > _customUrlCount)
                    customUrls.RemoveAt(customUrls.Count - 1);
            }
        }
        public List<CustomUrl> customUrls = new List<CustomUrl>
        {
            customUrlExample
        };

        static CustomUrl customUrlExample = new CustomUrl("Custom Url Example", "https://www.example.com/?",
                new List<CustomUrlParameter> {
                    new CustomUrlParameter("Example Text", "text", "hey"),
                    new CustomUrlParameter("Media Url", "media_url", "{media_url}")
        });

        public bool convertingToMP4 { get; private set; }
        public bool uploadingToServer { get; private set; }

        #region Editor variables
#if UNITY_EDITOR
        public bool hiddenMode;
        public bool subWindowMode;
        public bool gifsAvailable;
#pragma warning disable 0414
        [SerializeField]
        bool showSelectFileSettings;
        [SerializeField]
        bool showSelectUrlSettings;
        [SerializeField]
        bool showShareSettings;
        [SerializeField]
        bool showDisplaySettings;
        [SerializeField]
        public bool[] showCustomUrlSettings = { true };

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showCopyGameUrlButton")]
        bool showCopyDefaultUrlButton = true;
        [SerializeField]
        bool showCopyMediaUrlButton = true;
        [SerializeField]
        bool showCopyUrlToShareButton = true;
        [SerializeField]
        bool showOpenMediaPostUrlButton = true;

        public bool[] showSharedMethod = Enumerable.Repeat(true, (int)Share.Platform.Size).ToArray();
        public bool[] showSharedMethodSettings = new bool[(int)Share.Platform.Size];
        [SerializeField]
        bool showGiphyRehostButton = true;
        [SerializeField]
        bool showCopyHashtagsButton = true;
        [SerializeField]
        bool showTweetDeckButton = true;
#pragma warning restore 0414
#endif
        #endregion

        void Awake()
        {
            m_mediaToUploadPath = "";
            mediaUrl = "";
            mediaPostUrl = "";

            convertingToMP4 = false;
            uploadingToServer = false;
            if (config != null)
                config.LoadKeys();
            else
                Debug.LogWarning("ShareScript: Many share features will not function without a config file.");
        }

#if UNITY_EDITOR
        public void ConvertToMP4()
        {
            Shell.ShellEvents shellEvents = new Shell.ShellEvents();
            //shellEvents.LogEvent += (eventString) => { Debug.Log("MP4 Convert Log: " + eventString); };
            shellEvents.ErrorEvent += () => { Debug.LogError("MP4 Convert failed. Log events for details."); };
            shellEvents.DoneEvent += () =>
            {
                mediaToUploadPath = System.IO.Path.Combine(mediaToUploadDirectory, mediaToUploadFileName.Replace("gif", "mp4"));
                Debug.Log("Updated mediaFileDirectory: " + mediaToUploadDirectory);
                convertingToMP4 = false;
            };

            //Debug.Log("mediaFileDirectory: " + mediaToUploadDirectory);
            //Debug.Log("mp4Command: " + mp4Command);
            convertingToMP4 = true;
            ShellCommands.ConvertToMP4(mediaToUploadPath, shellEvents);
        }

        public void UploadToServer()
        {
            string fullServerUploadFolder = serverUploadFolder;
            string newMediaUrl = serverUploadUrl;
            if (includeMediaTypeFolderInServerPath)
            {
                FileType fileType = mediaToUploadFileType;
                fullServerUploadFolder += folderForFileType[(int)fileType];
                newMediaUrl += folderForFileType[(int)fileType] + '/';
            }
            newMediaUrl += mediaToUploadFileName;

            Shell.ShellEvents shellEvents = new Shell.ShellEvents();
            //shellEvents.LogEvent += (eventString) => { Debug.Log("Scp Log: " + eventString); };
            shellEvents.ErrorEvent += () => { Debug.LogError("SerialCopy failed. Log events for details."); };
            shellEvents.DoneEvent += () => { SetNewMediaUrls(newMediaUrl, newMediaUrl); };

            uploadingToServer = true;
            // Debug.Log("Uploading: " + mediaToUploadFileName);
            ShellCommands.SerialCopy(mediaToUploadPath, serverUser, fullServerUploadFolder, shellEvents);
        }
#endif

        public void UploadToImgur(System.Action<string, string> onComplete = null)
        {
            System.Action<string, string> fullOnComplete = SetNewMediaUrls;
            if(onComplete != null)
            {
                fullOnComplete = (mediaUrl, mediaPostUrl) => {
                    SetNewMediaUrls(mediaUrl, mediaPostUrl);
                    onComplete(mediaUrl, mediaPostUrl);
                };
            }

            uploadingToServer = true;
             //Debug.Log("Uploading: " + mediaToUploadFileName);

            string title = imgurTitle.Length > 0 ? imgurTitle : defaultTitle;
            string text = imgurDescription.Length > 0 ? imgurDescription : defaultText;
            if (imgurTags.Length > 0)
                text += " " + imgurTagsText;
            APIShare.UploadToImgur(config, mediaToUploadPath, text, title, imgurAlbum, fullOnComplete);
        }

        public void UploadToGiphy(System.Action<string, string> onComplete = null)
        {
            System.Action<string, string> fullOnComplete = SetNewMediaUrls;
            if (onComplete != null)
            {
                fullOnComplete = (mediaUrl, mediaPostUrl) => {
                    SetNewMediaUrls(mediaUrl, mediaPostUrl);
                    onComplete(mediaUrl, mediaPostUrl);
                };
            }

            uploadingToServer = true;
            // Debug.Log("Uploading: " + mediaToUploadFileName);

            APIShare.UploadToGiphy(config, mediaToUploadPath, giphyTags, "", SetNewMediaUrls);
        }

        public void RehostOnGiphy(System.Action<string, string> onComplete = null)
        {
            System.Action<string, string> fullOnComplete = SetNewMediaUrls;
            if (onComplete != null)
            {
                fullOnComplete = (mediaUrl, mediaPostUrl) => {
                    SetNewMediaUrls(mediaUrl, mediaPostUrl);
                    onComplete(mediaUrl, mediaPostUrl);
                };
            }

            uploadingToServer = true;
            // Debug.Log("Uploading: " + mediaToUploadFileName);

            APIShare.UploadToGiphy(config, "", giphyTags, mediaUrl, SetNewMediaUrls);
        }

        public void InstantShareToTwitter(System.Action<string> onComplete = null)
        {
            System.Action<Twity.DataModels.Core.Tweet, string, string> fullOnComplete = (tweet, statusUrl, response) =>
            {
                if (onComplete != null)
                    onComplete(statusUrl);
            };

            string text = twitterText.Length > 0 ? twitterText : defaultText;
            if (twitterHashtags.Length > 0)
                text += " " + twitterHashtagsText;
            APIShare.UploadToTwitter(config, mediaToUploadPath, twitterShareUsername, text);
        }

        public void ShareToTwitter()
        {
            string text = twitterText.Length > 0 ? twitterText : defaultText;
            WebShare.ShareToTwitter(urlToShare, text, twitterHashtags, twitterViaUsername);
        }

        public void ShareToTweetDeck()
        {
            WebShare.ShareToTweetDeck();
        }

        public void ShareToReddit()
        {
            string title = redditTitle.Length > 0 ? redditTitle : defaultTitle;
            string text = redditText.Length > 0 ? redditText : defaultText;

            string[] subredditsArray = subreddits.Split(',');
            foreach (string subreddit in subredditsArray)
                WebShare.ShareToReddit(subreddit.Trim(), urlToShare, text, title);
        }

        public void ShareToGooglePlus()
        {
            string text = googlePlusText.Length > 0 ? googlePlusText : defaultText;
            WebShare.ShareToGooglePlus(urlToShare, text);
        }

        public void ShareToPinterest()
        {
            // We don't want to pass the media url as the connected url to pinterest
            string url = pinterestUrl.Length > 0 ? pinterestUrl : useCustomUrlFunction ? CustomUrlFunction(defaultUrl, mediaUrl, mediaWidth, mediaHeight, true) : useCustomUrl ? customUrl.FullUrl(replacements) : defaultUrl;
            string description = pinterestDescription.Length > 0 ? pinterestDescription : defaultText;
            WebShare.ShareToPinterest(mediaUrl, description, url);
        }

        public void ShareToFacebook()
        {
            string text = facebookQuote.Length > 0 ? facebookQuote : defaultText;
            WebShare.ShareToFacebook(config, urlToShare, text, facebookHashtag);
        }

        public void ShareByEmail()
        {
            string body = emailBody.Length > 0 ? emailBody : defaultText;
            string address = emailAddress.Length > 0 ? emailAddress : defaultText;
            string subject = emailSubject.Length > 0 ? emailSubject : defaultText;
            WebShare.ShareByEmail(urlToShare, body, address, subject);
        }

        public void ShareToCustomUrl(CustomUrl customUrl)
        {
            WebShare.Share(customUrl.FullUrl(replacements));
        }

#if UNITY_IOS || UNITY_ANDROID
        public void NativeShare(bool withUrl)
        {
            APIShare.NativeShare(mediaToUploadPath, defaultText, withUrl ? defaultUrl : "");
        }
#endif

        #region Helpers
        // If you're sharing this link directly, you do want to escape the parameters.
        // If you're sharing this url in another url, you don't want to double escape the parameters.
        static string CustomUrlExample(string baseUrl, string mediaUrl, int mediaWidth, int mediaHeight, bool escapeParameters)
        {
#pragma warning disable 0618
            string mediaUrlToUse = escapeParameters ? WWW.EscapeURL(mediaUrl) : mediaUrl;
#pragma warning restore 0618
            return baseUrl + "?media_url=" + mediaUrlToUse + "&media_width=" + mediaWidth + "&media_height=" + mediaHeight;
        }

        void SetNewMediaUrls(string medialUrl, string mediaPostUrl)
        {
            this.mediaUrl = medialUrl;
            this.mediaPostUrl = mediaPostUrl;
            Debug.Log("Uploaded to: " + medialUrl + " edit at: " + mediaPostUrl);

            uploadingToServer = false;
        }

        public static FileType FileTypeForExtension(string extension)
        {
            switch (extension)
            {
                case ".png":
                    return FileType.PNG;
                case ".jpeg":
                case ".jpg":
                    return FileType.JPG;
                case ".tga":
                    return FileType.TGA;
                case ".gif":
                    return FileType.GIF;
                case ".mp4":
                    return FileType.MP4;
                default:
                    return FileType.Size;
            }
        }

        static string MimeTypeForPath(string filePath)
        {
            string extension = System.IO.Path.GetExtension(filePath);
            return MimeTypeForFileType(FileTypeForExtension(extension));
        }

        static string MimeTypeForFileType(FileType fileType)
        {
            return mimeTypeForFileType[(int)fileType];
        }
        #endregion
    }
}
