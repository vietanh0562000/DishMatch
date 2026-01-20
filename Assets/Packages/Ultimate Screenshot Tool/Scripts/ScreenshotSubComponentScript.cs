using UnityEngine;

namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    public class ScreenshotSubComponentScript : MonoBehaviour
    {
        public ScreenshotScript screenshotScript;

#if UNITY_EDITOR
        public bool hiddenMode;
        public bool subWindowMode;
        public bool editorWindowMode;

#pragma warning disable 0414
        [SerializeField]
        bool showSettings = true;
#pragma warning restore 0414
#endif

#if UNITY_EDITOR
        protected virtual ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.Size; } }
#endif

        protected virtual void Awake()
        {
            if (screenshotScript == null)
                screenshotScript = GetComponentInParent<ScreenshotScript>();
            if (screenshotScript == null)
                screenshotScript = GetComponentInChildren<ScreenshotScript>();

#if UNITY_EDITOR
            if (screenshotScript != null)
                screenshotScript.SetSubComponent(componentType, this);
            else
                Debug.LogError("ScreenshotScript not found.");
#endif
        }

#if UNITY_EDITOR
        public virtual bool CheckForRequiredProperties()
        {
            return false;
        }

        protected void LogRemovalTip()
        {
            Debug.Log("To remove this button, remove the " + componentType + " component.");
        }
#endif
    }
}
