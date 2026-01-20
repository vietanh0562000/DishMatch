using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TRS.CaptureTool.Extras
{
    [ExecuteInEditMode]
    public class DebugInfoScript : MonoBehaviour
    {
        public Text debugInfoText;

        public bool includeCustomText;
        public bool includeProject;
        public bool includeScene = true;
        public bool includeVersion;
        public bool includeBuild;
        public bool includeDate = true;

        public string customText = "";
        public string dateFormat = "MMMM dd";

        const string CONNECTING_TEXT = " - ";
        public string debugText
        {
            get
            {
                string fullDebugText = "";

                if (includeCustomText)
                    fullDebugText += CONNECTING_TEXT + customText;

                if (includeProject)
                    fullDebugText += CONNECTING_TEXT + Application.productName;

                if (includeScene)
                    fullDebugText += CONNECTING_TEXT + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + " ";

                if (includeVersion)
                    fullDebugText += CONNECTING_TEXT + "v" + Application.version + " ";

                if (includeBuild)
                    fullDebugText += CONNECTING_TEXT + "b" + BuildVersion() + " ";

                if (includeDate)
                    fullDebugText += CONNECTING_TEXT + System.DateTime.Now.ToString(dateFormat);

                if (fullDebugText.StartsWith(CONNECTING_TEXT, System.StringComparison.Ordinal) && fullDebugText.Length > CONNECTING_TEXT.Length)
                    fullDebugText = fullDebugText.Substring(CONNECTING_TEXT.Length);

                return fullDebugText;
            }
        }

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField]
        bool debugInfoSettingsChanged;
#pragma warning restore 0414
#endif

        void Awake()
        {
            debugInfoText.text = debugText;
        }

        const string BUILD_KEY = "TRS_Build";

#if UNITY_EDITOR
        public static void UpdateBuildVersion()
        {
#if UNITY_ANDROID
            string build = PlayerSettings.Android.bundleVersionCode.ToString();
#elif UNITY_IOS
            string build = PlayerSettings.iOS.buildNumber;
#elif UNITY_STANDALONE && UNITY_2017_OR_NEWER
            string build = PlayerSettings.macOS.buildNumber;
#else
            string build = "NA";
#endif

            PlayerPrefs.SetString(BUILD_KEY, build);
            PlayerPrefs.Save();
        }
#endif

        public static string BuildVersion()
        {
            return PlayerPrefs.GetString(BUILD_KEY);
        }
    }
}