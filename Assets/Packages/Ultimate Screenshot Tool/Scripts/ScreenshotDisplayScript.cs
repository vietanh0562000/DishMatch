using System.Collections.Generic;
using UnityEngine;

// Displays screenshots as a stack of framed pictures.

namespace TRS.CaptureTool
{
    public class ScreenshotDisplayScript : MonoBehaviour
    {
        const int STACK_SIZE = 3;
        const float MAX_ROTATION = 20f;
        const float MAX_X_POSITION_DIFF = 5f;
        const float MAX_Y_POSITION_DIFF = 5f;

        public GameObject framedScreenshotPrefab;

        Queue<GameObject> framedScreenshots = new Queue<GameObject>();

        void OnEnable()
        {
            ScreenshotScript.WillSaveScreenshot += UpdateScreenshot;
        }

        void OnDisable()
        {
            ScreenshotScript.WillSaveScreenshot -= UpdateScreenshot;
        }

        void UpdateScreenshot(ScreenshotScript screenshotScript, Texture2D texture)
        {
            if (framedScreenshots.Count >= STACK_SIZE)
                Destroy(framedScreenshots.Dequeue());

            GameObject framedScreenshot = Instantiate(framedScreenshotPrefab, transform);
            RectTransform frameScreenshotRectTransform = (RectTransform)framedScreenshot.transform;
            frameScreenshotRectTransform.localScale = Vector3.one;

            float xPosition = Random.Range(-MAX_X_POSITION_DIFF, MAX_X_POSITION_DIFF);
            float yPosition = Random.Range(-MAX_Y_POSITION_DIFF, MAX_Y_POSITION_DIFF);
            frameScreenshotRectTransform.anchoredPosition3D = new Vector3(xPosition, yPosition, 0);

            float rotation = Random.Range(-MAX_ROTATION, MAX_ROTATION);
            framedScreenshot.transform.Rotate(new Vector3(0, 0, rotation));
        }
    }
}