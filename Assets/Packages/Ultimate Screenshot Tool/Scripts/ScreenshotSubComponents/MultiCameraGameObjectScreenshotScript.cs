using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    [RequireComponent(typeof(GameObjectScreenshotScript))]
    [RequireComponent(typeof(MultiCameraScreenshotScript))]
    public class MultiCameraGameObjectScreenshotScript : ScreenshotSubComponentScript
    {
        public GameObjectScreenshotScript gameObjectScreenshotScript;
        public MultiCameraScreenshotScript multiCameraScreenshotScript;

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.MultiCameraGameObject; } }
#endif

        protected override void Awake()
        {
            base.Awake();

            if (gameObjectScreenshotScript == null)
                gameObjectScreenshotScript = GetComponentInParent<GameObjectScreenshotScript>();
            if (gameObjectScreenshotScript == null)
                gameObjectScreenshotScript = GetComponentInChildren<GameObjectScreenshotScript>();

            if (multiCameraScreenshotScript == null)
                multiCameraScreenshotScript = GetComponentInParent<MultiCameraScreenshotScript>();
            if (multiCameraScreenshotScript == null)
                multiCameraScreenshotScript = GetComponentInChildren<MultiCameraScreenshotScript>();
        }

        public void TakeMultiCameraGameObjectScreenshots(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) return;
#endif
            StartCoroutine(CaptureMultiCameraGameObjectsCoroutine(save));
        }

        public IEnumerator CaptureMultiCameraGameObjectsCoroutine(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) yield break;
#endif
            screenshotScript.screenshotsInProgressOverride = true;
            List<ScreenshotResolution> originalScreenshotResolutions = screenshotScript.screenshotResolutions;
            for (int i = 0; i < gameObjectScreenshotScript.pairs.Count; ++i)
            {
                GameObjectScreenshotScript.GameObjectResolutionPair pair = gameObjectScreenshotScript.pairs[i];
                screenshotScript.screenshotResolutions = new List<ScreenshotResolution>() { pair.resolution };

                bool gameObjectWasActive = pair.gameObject.activeSelf;
                pair.gameObject.SetActive(true);
                yield return StartCoroutine(multiCameraScreenshotScript.CaptureMultiCamerasCoroutine(save));
                pair.gameObject.SetActive(gameObjectWasActive);
            }
            screenshotScript.screenshotsInProgressOverride = false;
            screenshotScript.screenshotResolutions = originalScreenshotResolutions;
            screenshotScript.fileSettings.SaveCount();
        }

#if UNITY_EDITOR
        public override bool CheckForRequiredProperties()
        {
            if (gameObjectScreenshotScript.pairs.Count <= 0)
            {
                Debug.LogError("Game Object and Resolution pairs must be set in GameObject Screenshot Settings to use this feature.");
                LogRemovalTip();
                return false;
            }

            if (multiCameraScreenshotScript.cameras.Count <= 0)
            {
                Debug.LogError("Cameras must be set in Multi Camera Screenshot Settings to use this feature.");
                LogRemovalTip();
                return false;
            }

            return true;
        }
#endif
    }
}