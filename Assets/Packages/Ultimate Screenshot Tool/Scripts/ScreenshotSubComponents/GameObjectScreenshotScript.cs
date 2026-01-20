using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class GameObjectScreenshotScript : ScreenshotSubComponentScript
    {
        [System.Serializable]
        public class GameObjectResolutionPair
        {
            public GameObject gameObject;
            public ScreenshotResolution resolution;
        }

        public List<GameObjectResolutionPair> pairs = new List<GameObjectResolutionPair>();

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.GameObject; } }
#endif

        public void TakeGameObjectScreenshots(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) return;
#endif
            StartCoroutine(CaptureGameObjectsCoroutine(save));
        }

        public IEnumerator CaptureGameObjectsCoroutine(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) yield break;
#endif
            screenshotScript.screenshotsInProgressOverride = true;
            List<ScreenshotResolution> originalScreenshotResolutions = screenshotScript.screenshotResolutions;
            for (int i = 0; i < pairs.Count; ++i)
            {
                GameObjectResolutionPair pair = pairs[i];
                screenshotScript.screenshotResolutions = new List<ScreenshotResolution>() { pair.resolution };

                bool gameObjectWasActive = pair.gameObject.activeSelf;
                pair.gameObject.SetActive(true);
                yield return StartCoroutine(screenshotScript.TakeAllScreenshotsCoroutine(save));
                pair.gameObject.SetActive(gameObjectWasActive);
            }
            screenshotScript.screenshotsInProgressOverride = false;
            screenshotScript.screenshotResolutions = originalScreenshotResolutions;
            screenshotScript.fileSettings.SaveCount();
        }

#if UNITY_EDITOR
        public override bool CheckForRequiredProperties()
        {
            if (pairs.Count <= 0)
            {
                Debug.LogError("Game Object and Resolution pairs must be set in GameObject Screenshot Settings to use this feature.");
                LogRemovalTip();
                return false;
            }

            return true;
        }
#endif
    }
}