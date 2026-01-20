using UnityEngine;
using UnityEditor;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool.Share
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(ShareScript))]
    public class ShareScriptEditor : Editor
    {
        bool hasFileToShare;
        bool hasUrlToShare;
        bool hasMediaUrl;
        ShareScript.FileType mediaFileType;
        bool isGif;
        bool isAnimated;
        bool processing;

        static Share.Platform[] configListOrder = { Share.Platform.Website, Share.Platform.Imgur, Share.Platform.Giphy, Share.Platform.Twitter,
            Share.Platform.Reddit, Share.Platform.GooglePlus, Share.Platform.Pinterest, Share.Platform.Facebook };

        public override void OnInspectorGUI()
        {
            if (((ShareScript)target).hiddenMode)
                return;

            FullShareUI();
        }

        public void FullShareUI()
        {
            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled = !EditorApplication.isCompiling && !EditorApplication.isUpdating;

            serializedObject.Update();

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("config"));
            if (GUILayout.Button("Create New Config"))
                serializedObject.FindProperty("config").objectReferenceValue = CreateCaptureToolConfig.Create();
            EditorGUILayout.HelpBox("Config files are created in " + CreateCaptureToolConfig.AssetDatabaseDirectory() + " to avoid overwrites when tool is updated.", MessageType.Info);

            hasFileToShare = ((ShareScript)target).mediaToUploadFileName.Length > 0;
            mediaFileType = ((ShareScript)target).mediaToUploadFileType;
            isGif = mediaFileType == ShareScript.FileType.GIF;
            isAnimated = isGif || mediaFileType == ShareScript.FileType.MP4;
            processing = ((ShareScript)target).convertingToMP4 || ((ShareScript)target).uploadingToServer;

            DisplaySettings();

            ShareSettings();

            string fileToShare = hasFileToShare ? ((ShareScript)target).mediaToUploadFileName : "No File Selected";
            if (((ShareScript)target).convertingToMP4)
                fileToShare = "Converting to MP4...";
            EditorGUILayout.LabelField("File to upload: " + fileToShare);

            SelectFileSettings();

            hasUrlToShare = ((ShareScript)target).urlToShare.Length > 0;
            hasMediaUrl = !string.IsNullOrEmpty(((ShareScript)target).mediaUrl);
            string urlToShare = hasUrlToShare ? ((ShareScript)target).urlToShare : "No Url Set";
            if (((ShareScript)target).uploadingToServer)
                fileToShare = "Uploading...";
            EditorGUILayout.LabelField("Url to share: " + urlToShare);
            SelectUrlSettings();

            bool showCopyDefaultUrlButton = serializedObject.FindProperty("showCopyDefaultUrlButton").boolValue;
            showCopyDefaultUrlButton &= !string.IsNullOrEmpty(((ShareScript)target).defaultUrl);
            if (showCopyDefaultUrlButton && GUILayout.Button("Copy Default Url", GUILayout.MinHeight(40)))
                GUIUtility.systemCopyBuffer = ((ShareScript)target).defaultUrl;

            bool showCopyMediaUrlButton = serializedObject.FindProperty("showCopyMediaUrlButton").boolValue;
            showCopyMediaUrlButton &= !string.IsNullOrEmpty(((ShareScript)target).mediaUrl);
            if (showCopyMediaUrlButton && GUILayout.Button("Copy Media Url", GUILayout.MinHeight(40)))
                GUIUtility.systemCopyBuffer = ((ShareScript)target).mediaUrl;

            bool showCopyUrlToShareButton = serializedObject.FindProperty("showCopyUrlToShareButton").boolValue;
            showCopyUrlToShareButton &= !string.IsNullOrEmpty(((ShareScript)target).urlToShare);
            if (showCopyUrlToShareButton && GUILayout.Button("Copy Url to Share", GUILayout.MinHeight(40)))
                GUIUtility.systemCopyBuffer = ((ShareScript)target).urlToShare;

            bool showOpenMediaPostUrlButton = serializedObject.FindProperty("showOpenMediaPostUrlButton").boolValue;
            showOpenMediaPostUrlButton &= !string.IsNullOrEmpty(((ShareScript)target).mediaPostUrl);
            if (showOpenMediaPostUrlButton && GUILayout.Button("Open Media Post Url", GUILayout.MinHeight(40)))
                Application.OpenURL(((ShareScript)target).mediaPostUrl);

            EditorGUILayout.HelpBox("Media must be uploaded before it can be shared (with exception to Twitter Instant Share which uploads automatically and Tweetdeck which is not auto-populated).", MessageType.Info);

            Server();

            Imgur();

            if (((ShareScript)target).gifsAvailable)
                Giphy();

            Twitter();

            Reddit();

            GooglePlus();

            Pinterest();

            Facebook();

            Email();

            CustomUrls();

            // Properties set on the target directly (like ((ShareScript)target).showSharedMethod[(int)platform]) are not saved if the target is not set as dirty
            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            GUI.enabled = originalGUIEnabled;
        }

        void DisplaySettings()
        {
            bool showDisplaySettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showDisplaySettings", "Display Settings");
            if (showDisplaySettings)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showCopyDefaultUrlButton"), new GUIContent("Show Copy Default Url"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showCopyMediaUrlButton"), new GUIContent("Show Copy Media Url"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showCopyUrlToShareButton"), new GUIContent("Show Copy Url to Share"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showOpenMediaPostUrlButton"), new GUIContent("Show Open Media Post Url"));

                foreach (Share.Platform platform in configListOrder)
                {
                    string platformName = platform.ToString();
                    if (platform == Share.Platform.Website)
                        platformName = "Server Upload";
                    ((ShareScript)target).showSharedMethod[(int)platform] = EditorGUILayout.Toggle("Show " + platformName, ((ShareScript)target).showSharedMethod[(int)platform]);
                    if (((ShareScript)target).showSharedMethod[(int)platform])
                    {
                        if (platform == Share.Platform.Giphy)
                            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showGiphyRehostButton"), new GUIContent("Show Giphy Rehost"));
                        if (platform == Share.Platform.Twitter)
                        {
                            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showCopyHashtagsButton"), new GUIContent("Show Copy Hashtags"));
                            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("showTweetDeckButton"), new GUIContent("Show TweetDeck"));
                        }
                    }
                }

                ((ShareScript)target).customUrlCount = EditorGUILayout.IntField("Custom Url Count", ((ShareScript)target).customUrlCount);
            }
        }

        void ShareSettings()
        {
            bool showShareSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showShareSettings", "Share Settings");
            if (showShareSettings)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTitle"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("defaultText"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("defaultUrl"));
                EditorGUILayout.HelpBox("Default values will be used if nothing else is specified.", MessageType.Info);
            }
        }

        void SelectFileSettings()
        {
            bool showSelectFileSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSelectFileSettings", "Select File to Upload");
            if (showSelectFileSettings)
            {
                EditorGUILayout.LabelField("File To Upload Path");
                EditorGUILayout.BeginHorizontal();
                string currentMediaPath = ((ShareScript)target).mediaToUploadPath;
                ((ShareScript)target).mediaToUploadPath = EditorGUILayout.TextField(currentMediaPath, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                    OpenFilePanelForMediaPath();
                EditorGUILayout.EndHorizontal();

                if (hasFileToShare && isGif)
                {
                    bool originalGUIEnabled = GUI.enabled;
                    GUI.enabled &= !processing;
                    if (isGif && GUILayout.Button("Convert to MP4", GUILayout.MinHeight(40)))
                        ((ShareScript)target).ConvertToMP4();
                    GUI.enabled = originalGUIEnabled;

                    EditorGUILayout.HelpBox("Convert to MP4 button uses ffmpeg to convert a gif to MP4.", MessageType.Info);
                }
            }
        }

        void SelectUrlSettings()
        {
            bool showSelectUrlSettings = CustomEditorGUILayout.BoldFoldoutForProperty(serializedObject, "showSelectUrlSettings", "Select Url to Share");
            if (showSelectUrlSettings)
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomUrl"));

                if (serializedObject.FindProperty("useCustomUrl").boolValue)
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useCustomUrlFunction"));
                    if (!serializedObject.FindProperty("useCustomUrlFunction").boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("customUrl"));
                        EditorGUILayout.HelpBox("The parameter values {media_url}, {media_post_url}, {media_width}, or {media_height} will be replaced with the proper values.", MessageType.Info);
                    }
                    else
                        EditorGUILayout.HelpBox("Use this option to share a customized url by overriding the CustomCodeFunction delegate. (Keep your code in a separate file, so you can update the asset without losing it.)", MessageType.Info);
                }
                else
                {
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useLastMediaPostUrl"));
                    CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("useLastMediaUrl"));
                    if (serializedObject.FindProperty("useLastMediaUrl").boolValue)
                        EditorGUILayout.HelpBox("The last upload url created for this file is used.", MessageType.Info);
                    else
                        EditorGUILayout.HelpBox("The default url is used.", MessageType.Info);
                }
            }
        }

        void Server()
        {
            Share.Platform platform = Share.Platform.Website;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Server");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("serverUser"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("serverUploadFolder"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("serverUploadUrl"));
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("includeMediaTypeFolderInServerPath"));
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= hasFileToShare && !processing;
            if (GUILayout.Button("Upload to Server", GUILayout.MinHeight(40)))
                ((ShareScript)target).UploadToServer();
            GUI.enabled = originalGUIEnabled;
        }

        void Imgur()
        {
            Share.Platform platform = Share.Platform.Imgur;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Imgur");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Album");
                string oldImgurAlbum = serializedObject.FindProperty("imgurAlbum").stringValue;
                serializedObject.FindProperty("imgurAlbum").stringValue = EditorGUILayout.TextArea(oldImgurAlbum, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Title");
                string oldImgurTitle = serializedObject.FindProperty("imgurTitle").stringValue;
                serializedObject.FindProperty("imgurTitle").stringValue = EditorGUILayout.TextArea(oldImgurTitle, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));


                EditorGUILayout.LabelField("Description");
                string oldImgurDescription = serializedObject.FindProperty("imgurDescription").stringValue;
                serializedObject.FindProperty("imgurDescription").stringValue = EditorGUILayout.TextArea(oldImgurDescription, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Tags");
                string oldImgurTags = serializedObject.FindProperty("imgurTags").stringValue;
                serializedObject.FindProperty("imgurTags").stringValue = EditorGUILayout.TextArea(oldImgurTags, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.HelpBox("Comma-separated list of hashtags. (gamedev,indiedev,gaming)", MessageType.Info);
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= hasFileToShare && !processing;
            if (GUILayout.Button("Upload to Imgur", GUILayout.MinHeight(40)))
                ((ShareScript)target).UploadToImgur();
            GUI.enabled = originalGUIEnabled;
        }

        void Giphy()
        {
            Share.Platform platform = Share.Platform.Giphy;
            if (!((ShareScript)target).showSharedMethod[(int)platform] || (!isAnimated && hasFileToShare))
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Giphy");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Tags");
                string oldGiphyTags = serializedObject.FindProperty("giphyTags").stringValue;
                serializedObject.FindProperty("giphyTags").stringValue = EditorGUILayout.TextArea(oldGiphyTags, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.HelpBox("Comma separated without the hashtags. (Ex. unity,gaming,indie,indedev,gamedev,indiegame,gif)", MessageType.Info);
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= hasFileToShare && !processing;
            if (GUILayout.Button("Upload to Giphy", GUILayout.MinHeight(40)))
                ((ShareScript)target).UploadToGiphy();

            GUI.enabled &= hasMediaUrl;
            bool showGiphyRehostButton = serializedObject.FindProperty("showGiphyRehostButton").boolValue;
            if (showGiphyRehostButton && GUILayout.Button("Rehost on Giphy", GUILayout.MinHeight(40)))
                ((ShareScript)target).RehostOnGiphy();
            GUI.enabled = originalGUIEnabled;
        }

        void Twitter()
        {
            Share.Platform platform = Share.Platform.Twitter;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Twitter");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Text");
                string oldTwitterText = serializedObject.FindProperty("twitterText").stringValue;
                serializedObject.FindProperty("twitterText").stringValue = EditorGUILayout.TextArea(oldTwitterText, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Hashtags");
                string oldTwitterHashtags = serializedObject.FindProperty("twitterHashtags").stringValue;
                serializedObject.FindProperty("twitterHashtags").stringValue = EditorGUILayout.TextArea(oldTwitterHashtags, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.HelpBox("Comma-separated list without the hashtags. (Ex. gamedev,indiedev,indiegame,madewithunity)", MessageType.Info);
                bool showCopyHashtagsButton = serializedObject.FindProperty("showCopyHashtagsButton").boolValue;
                if (showCopyHashtagsButton && GUILayout.Button("Copy Hashtags", GUILayout.MinHeight(40)))
                    GUIUtility.systemCopyBuffer = ((ShareScript)target).twitterHashtagsText;
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            bool showTweetDeckButton = serializedObject.FindProperty("showTweetDeckButton").boolValue;
            if (showTweetDeckButton && GUILayout.Button("Share to TweetDeck", GUILayout.MinHeight(40)))
            {
                Application.OpenURL("file:///" + System.Uri.EscapeUriString(((ShareScript)target).mediaToUploadPath));
                ((ShareScript)target).ShareToTweetDeck();
            }
            if (GUILayout.Button("Share to Twitter", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareToTwitter();

            CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("twitterShareUsername"), new GUIContent("Instant Share Username"));
            if (string.IsNullOrEmpty(Twity.Oauth.accessToken))
                CustomEditorGUILayout.PropertyField(serializedObject.FindProperty("twitterSharePin"), new GUIContent("Instant Share Pin"));

            string shareUsername = serializedObject.FindProperty("twitterShareUsername").stringValue;
            if (string.IsNullOrEmpty(Twity.Oauth.accessToken) || shareUsername != Twity.Client.screenName)
            {
                CaptureToolConfig config = ((ShareScript)target).config;
                if (config == null)
                {
                    EditorGUILayout.HelpBox("Config file must be created before using Twitter Instant Share.", MessageType.Warning);
                    GUI.enabled = false;
                }
                else
                {
                    bool authSucceeded = config.LoadTwitterAuthKeys(shareUsername);
                    if (authSucceeded)
                    {
                        if (shareUsername.Length <= 0)
                            serializedObject.FindProperty("twitterShareUsername").stringValue = Twity.Client.screenName;
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No Twitter user authenticated. Set username, access token, and access token secret in config or set cosumer key and consumer secret and use pin authentication.", MessageType.Info);
                        if (!string.IsNullOrEmpty(config.twitterUsername) || !string.IsNullOrEmpty(config.altTwitterUsername))
                            EditorGUILayout.HelpBox("Authenticated Twitter user does not match entered username. Ensure username matches the username in your config, you're requesting a pin from the proper account, or leave username blank to auto-fill with existing auth data.", MessageType.Info);

                        if (GUILayout.Button("Request Pin"))
                        {
                            CoroutineBehaviour.StaticStartCoroutine(Twity.Client.GenerateRequestToken((success) =>
                            {
                                if (!success)
                                {
                                    Debug.LogError("Request for Twitter token failed");
                                    return;
                                }
                                Application.OpenURL(Twity.Oauth.authorizeURL);
                            }));
                        }

                        if (GUILayout.Button("Submit Pin"))
                        {
                            string pin = serializedObject.FindProperty("twitterSharePin").stringValue;
                            CoroutineBehaviour.StaticStartCoroutine(Twity.Client.GenerateAccessToken(pin, (success) =>
                            {
                                if (!success)
                                {
                                    Debug.LogError("Twitter pin failed");
                                    return;
                                }

                                config.SetTwitterAuthKeys(Twity.Client.screenName, Twity.Oauth.accessToken, Twity.Oauth.accessTokenSecret);
                            }));
                        }

                        GUI.enabled = false;
                    }
                }
            }

            if (GUILayout.Button("Instant Share to Twitter", GUILayout.MinHeight(40)))
                ((ShareScript)target).InstantShareToTwitter();
            GUI.enabled = originalGUIEnabled;
        }

        void Reddit()
        {
            Share.Platform platform = Share.Platform.Reddit;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Reddit");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {

                EditorGUILayout.LabelField("Subreddits");
                string oldSubreddits = serializedObject.FindProperty("subreddits").stringValue;
                serializedObject.FindProperty("subreddits").stringValue = EditorGUILayout.TextArea(oldSubreddits, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Title");
                string oldRedditTitle = serializedObject.FindProperty("redditTitle").stringValue;
                serializedObject.FindProperty("redditTitle").stringValue = EditorGUILayout.TextArea(oldRedditTitle, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Text");
                string oldRedditText = serializedObject.FindProperty("redditText").stringValue;
                serializedObject.FindProperty("redditText").stringValue = EditorGUILayout.TextArea(oldRedditText, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.HelpBox("Enter multiple comma separated subreddits to open multiple tabs.", MessageType.Info);
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            if (GUILayout.Button("Share to Reddit", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareToReddit();
            GUI.enabled = originalGUIEnabled;
        }

        void GooglePlus()
        {
            Share.Platform platform = Share.Platform.GooglePlus;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Google Plus");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Text");
                string oldGooglePlusText = serializedObject.FindProperty("googlePlusText").stringValue;
                serializedObject.FindProperty("googlePlusText").stringValue = EditorGUILayout.TextArea(oldGooglePlusText, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            if (GUILayout.Button("Share to Google Plus", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareToGooglePlus();
            GUI.enabled = originalGUIEnabled;
        }

        void Pinterest()
        {
            Share.Platform platform = Share.Platform.Pinterest;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Pinterest");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Description");
                string oldPinterestDescription = serializedObject.FindProperty("pinterestDescription").stringValue;
                serializedObject.FindProperty("pinterestDescription").stringValue = EditorGUILayout.TextArea(oldPinterestDescription, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Url to Share");
                string oldPinterestUrl = serializedObject.FindProperty("pinterestUrl").stringValue;
                serializedObject.FindProperty("pinterestUrl").stringValue = EditorGUILayout.TextArea(oldPinterestUrl, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.HelpBox("Pinterest can connect a url with your pin. Entering a custom value here will override behaviour. Otherwise, url falls through the CustomUrlFunction url, urlToShareOverride, and finally to the defaultUrl.", MessageType.Info);
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            if (GUILayout.Button("Share to Pinterest", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareToPinterest();
            GUI.enabled = originalGUIEnabled;
        }

        void Facebook()
        {
            Share.Platform platform = Share.Platform.Facebook;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Facebook");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Quote");
                string oldFacebookQuote = serializedObject.FindProperty("facebookQuote").stringValue;
                serializedObject.FindProperty("facebookQuote").stringValue = EditorGUILayout.TextArea(oldFacebookQuote, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Hashtag");
                string oldFacebookHashtag = serializedObject.FindProperty("facebookHashtag").stringValue;
                serializedObject.FindProperty("facebookHashtag").stringValue = EditorGUILayout.TextArea(oldFacebookHashtag, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            if (GUILayout.Button("Share to Facebook", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareToFacebook();
            GUI.enabled = originalGUIEnabled;
        }

        void Email()
        {
            Share.Platform platform = Share.Platform.Email;
            if (!((ShareScript)target).showSharedMethod[(int)platform])
                return;

            ((ShareScript)target).showSharedMethodSettings[(int)platform] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showSharedMethodSettings[(int)platform], "Email");
            if (((ShareScript)target).showSharedMethodSettings[(int)platform])
            {
                EditorGUILayout.LabelField("Email Address");
                string oldEmailAddress = serializedObject.FindProperty("emailAddress").stringValue;
                serializedObject.FindProperty("emailAddress").stringValue = EditorGUILayout.TextArea(oldEmailAddress, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Subject");
                string oldEmailSubject = serializedObject.FindProperty("emailSubject").stringValue;
                serializedObject.FindProperty("emailSubject").stringValue = EditorGUILayout.TextArea(oldEmailSubject, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));

                EditorGUILayout.LabelField("Body");
                string oldEmailBody = serializedObject.FindProperty("emailBody").stringValue;
                serializedObject.FindProperty("emailBody").stringValue = EditorGUILayout.TextArea(oldEmailBody, GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 50));
            }

            bool originalGUIEnabled = GUI.enabled;
            GUI.enabled &= !processing;
            if (GUILayout.Button("Share by Email", GUILayout.MinHeight(40)))
                ((ShareScript)target).ShareByEmail();
            GUI.enabled = originalGUIEnabled;
        }

        void CustomUrls()
        {
            SerializedProperty elementIterator = serializedObject.FindProperty("customUrls").Copy();
            elementIterator.NextVisible(true); // Skip list attribute
            elementIterator.NextVisible(true); // Skip array size
            for (int i = 0; i < ((ShareScript)target).customUrls.Count; ++i)
            {
                CustomUrl customUrl = ((ShareScript)target).customUrls[i];
                ((ShareScript)target).showCustomUrlSettings[i] = CustomEditorGUILayout.BoldFoldout(((ShareScript)target).showCustomUrlSettings[i], customUrl.displayName + " Settings");
                if (((ShareScript)target).showCustomUrlSettings[i])
                {
                    EditorGUILayout.PropertyField(elementIterator);
                    elementIterator.NextVisible(false);
                    EditorGUILayout.HelpBox("The parameter values {media_url}, {media_post_url}, {media_width}, or {media_height} will be replaced with the proper values.", MessageType.Info);
                }

                bool originalGUIEnabled = GUI.enabled;
                GUI.enabled &= !processing;
                if (GUILayout.Button("Share to " + customUrl.displayName, GUILayout.MinHeight(40)))
                    ((ShareScript)target).ShareToCustomUrl(customUrl);
                GUI.enabled = originalGUIEnabled;
            }
        }

        #region Helpers
        protected void CheckOrRequestMediaPath()
        {
            string currentMediaPath = serializedObject.FindProperty("mediaToUploadPath").stringValue;
            if (currentMediaPath == "")
                OpenFilePanelForMediaPath();
        }

        protected void OpenFilePanelForMediaPath()
        {
            string currentMediaPath = ((ShareScript)target).mediaToUploadPath;
            string[] acceptedFileTypes = ShareScript.acceptedFileTypes;
            string newMediaToUploadPath = EditorUtility.OpenFilePanel("Path to Media to Share", currentMediaPath, string.Join(",", acceptedFileTypes));
            if (newMediaToUploadPath.Length > 0)
                ((ShareScript)target).mediaToUploadPath = newMediaToUploadPath;
            serializedObject.ApplyModifiedProperties();

            EditorGUIUtility.ExitGUI();
        }
        #endregion
    }
}