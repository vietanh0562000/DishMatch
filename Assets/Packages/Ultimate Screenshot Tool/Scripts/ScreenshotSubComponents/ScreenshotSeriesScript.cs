using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TRS.CaptureTool
{
    public class ScreenshotSeriesScript : ScreenshotSubComponentScript
    {
        public bool captureInitialScreen = true;
        public List<ButtonInteraction> buttonInteractions = new List<ButtonInteraction>();

        [System.Serializable]
        public struct ButtonInteraction
        {
            public Button button;
            public float animationDelay;
            public bool takePhoto;
        }

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.ScreenshotSeries; } }
#endif

#pragma warning disable 0414
        [SerializeField]
        bool showTakeSeriesCaptureButton = true;
        [SerializeField]
        bool showTakeSeriesLiveFrameCaptureButton = true;
#pragma warning restore 0414

        public void TakeScreenshotSeries(bool save = true)
        {
            StartCoroutine(TakeScreenshotSeriesCoroutine(save));
        }

        public void TakeScreenshotSeriesWithLiveFrames(bool save = true)
        {
            StartCoroutine(FlexibleTakeScreenshotSeriesCoroutine(screenshotScript.TakeAllScreenshotsWithLiveFramesCoroutine, save));
        }

        public IEnumerator TakeScreenshotSeriesCoroutine(bool save = true)
        {
            return FlexibleTakeScreenshotSeriesCoroutine(screenshotScript.TakeAllScreenshotsCoroutine, save);
        }

        public IEnumerator FlexibleTakeScreenshotSeriesCoroutine(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            screenshotScript.screenshotsInProgressOverride = true;

            if (captureInitialScreen)
                yield return StartCoroutine(captureAndSaveRoutine(save));

            foreach (ButtonInteraction buttonInteraction in buttonInteractions)
            {
                if (buttonInteraction.button != null)
                    buttonInteraction.button.onClick.Invoke();

                yield return new WaitForSecondsRealtime(buttonInteraction.animationDelay);

                if (buttonInteraction.takePhoto)
                    yield return StartCoroutine(captureAndSaveRoutine(save));
            }

            screenshotScript.screenshotsInProgressOverride = false;
        }
    }
}