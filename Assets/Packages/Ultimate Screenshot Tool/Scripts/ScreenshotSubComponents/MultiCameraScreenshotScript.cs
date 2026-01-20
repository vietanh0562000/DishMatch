using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TRS.CaptureTool
{
    public class MultiCameraScreenshotScript : ScreenshotSubComponentScript
    {
        public List<Camera> cameras = new List<Camera>();

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.MultiCamera; } }
#endif

        public void TakeMultiCameraScreenshots(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) return;
#endif
            StartCoroutine(CaptureMultiCamerasCoroutine(save));
        }

        public IEnumerator CaptureMultiCamerasCoroutine(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) yield break;
#endif
            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            Rect cutoutRect = (screenshotScript.useCutout && screenshotScript.cutoutScript != null) ? screenshotScript.cutoutScript.rect : new Rect(0, 0, currentResolution.width, currentResolution.height);
            screenshotScript.screenshotsInProgressOverride = true;
            int originalScreenshotIndex = screenshotScript.fileSettings.count;
            bool originalIncludeCamera = screenshotScript.fileSettings.includeCamera;
            CaptureMode originalCaptureMode = screenshotScript.captureMode;
            screenshotScript.captureMode = CaptureMode.Camera;
            screenshotScript.fileSettings.includeCamera = true;
            for (int i = 0; i < cameras.Count; ++i)
            {
                Camera cameraToUse = cameras[i];
                if (cameraToUse == null || !cameraToUse.enabled) continue;
                screenshotScript.fileSettings.SetCount(originalScreenshotIndex);

                // Need to force wait for updates here to catch a corner case. 
                // (If original canvas size is in list of resolutions, then TakeScreenshotWithCameras will resize back to it at end of method.
                // the next run will think the resolution has already been set/adjusted, and it won't wait for adjustments)

                yield return StartCoroutine(screenshotScript.TakeScreenshotWithCameras(new Camera[] { cameraToUse }, currentResolution, cutoutRect, save));
            }
            screenshotScript.screenshotsInProgressOverride = false;
            screenshotScript.captureMode = originalCaptureMode;
            screenshotScript.fileSettings.includeCamera = originalIncludeCamera;
            screenshotScript.fileSettings.SetCount(originalScreenshotIndex + 1);
            screenshotScript.fileSettings.SaveCount();
        }

#if UNITY_EDITOR
        public override bool CheckForRequiredProperties()
        {
            if (cameras.Count <= 0)
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