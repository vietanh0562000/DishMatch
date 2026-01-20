using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    public class SaveScreenshotUIScript : MonoBehaviour
    {
        [Tooltip("Must be set to the GameObject containing the panel with the screenshot save UI.")]
        public GameObject panel;

        [Tooltip("Optional. If not set, will use the ScreenshotScript from the ScreenshotTaken action.")]
        public ScreenshotScript screenshotScript;

        [Tooltip("If set, will activate this GameObject automatically when a screenshot is taken.")]
        public bool displayAutomatically = true;

        [Tooltip("If set, will deactivate this GameObject automatically when a screenshot is saved.")]
        public bool hideOnSave = true;

        [Tooltip("If set, will unlock the cursor and set to visible to enable interactions with the panel and reset to original status when closing the panel.")]
        public bool unlockCursor = true;

        public Button saveButton;
        public Button cancelButton;

        protected Texture2D screenshot;
        protected CursorLockMode originalCursorLockMode;
        protected bool originalCursorVisibility;

        void Awake()
        {
            saveButton.onClick.AddListener(SaveScreenshot);
            cancelButton.onClick.AddListener(Cancel);

            //if (GetComponentInParent<Canvas>() == null)
            //    Debug.LogError("This UI panel must be the child of a Canvas.");

            ScreenshotScript.ScreenshotTaken += ScreenshotTaken;
        }

        void OnDestroy()
        {
            saveButton.onClick.RemoveListener(SaveScreenshot);
            cancelButton.onClick.RemoveListener(Cancel);

            ScreenshotScript.ScreenshotTaken -= ScreenshotTaken;
        }

        void ScreenshotTaken(ScreenshotScript screenshotScript, Texture2D screenshotTaken)
        {
            if (this == null)
            {
                ScreenshotScript.ScreenshotTaken -= ScreenshotTaken;
                DestroyImmediate(this);
                return;
            }

            if (displayAutomatically)
            {
                if (unlockCursor)
                {
                    originalCursorLockMode = Cursor.lockState;
                    originalCursorVisibility = Cursor.visible;
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                panel.SetActive(true);
            }

            if (this.screenshotScript == null) this.screenshotScript = screenshotScript;
            screenshot = screenshotTaken;
        }

        public virtual void SaveScreenshot()
        {
            screenshotScript.lastScreenshotTexture = screenshot;
#if UNITY_EDITOR
            screenshotScript.textureToEdit = screenshot;
#endif

            screenshotScript.Save(screenshot, "", false);
            screenshotScript.fileSettings.IncrementCount();
            screenshotScript.fileSettings.SaveCount();

            if (hideOnSave) HidePanel();
        }

        public void Cancel()
        {
            HidePanel();
        }

        void HidePanel()
        {
            if (unlockCursor)
            {
                Cursor.lockState = originalCursorLockMode;
                Cursor.visible = originalCursorVisibility;
            }
            panel.SetActive(false);
        }
    }
}