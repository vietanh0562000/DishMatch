using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Share
{
    public class DemoUploadScript : MonoBehaviour
    {
        public ShareScript shareScript;
        public string noMediaToShareText = "No Media to Upload";

        Button button;
        Text buttonText;

        int numPeriods;
        float timeSinceLastUpdate;
        const int maxNumPeriods = 3;
        const float timeBetweenPeriodUpdates = 0.25f;

        void Awake()
        {
            button = GetComponent<Button>();
            buttonText = GetComponentInChildren<Text>();
        }

        void Update()
        {
            button.interactable = !string.IsNullOrEmpty(shareScript.mediaToUploadPath) && !shareScript.uploadingToServer;
            if (string.IsNullOrEmpty(shareScript.mediaToUploadPath))
                buttonText.text = noMediaToShareText;
            else
                buttonText.text = shareScript.uploadingToServer ? "Uploading" + new string('.', numPeriods) : "Upload to Imgur";

            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= timeBetweenPeriodUpdates)
            {
                timeSinceLastUpdate = 0;
                ++numPeriods;
                if (numPeriods > maxNumPeriods)
                    numPeriods = 0;
            }
        }

        public void DemoUpload()
        {
            System.Action<string, string> onComplete = (mediaUrl, mediaPostUrl) => { if (string.IsNullOrEmpty(mediaPostUrl)) { return; }  Application.OpenURL(mediaPostUrl); };
            shareScript.UploadToImgur(onComplete);
        }
    }
}