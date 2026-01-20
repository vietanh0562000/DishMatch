using System.Collections;
using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class ScreenshotBurstScript : ScreenshotSubComponentScript
    {
        public static System.Action WillTakeScreenshotBurst;
        public static System.Action ScreenshotBurstTaken;

        public HotKeySet takeScreenshotBurstKeySet = new HotKeySet {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.B,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.B,
#endif
            alt = true,
        };

        public int burstSize = 10;
        public bool useTimeDelay = false;
        public bool realtimeDelay = true;
        public float delay = 0.1f;
        public int skipFrames = 0;

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.Burst; } }

#pragma warning disable 0414
        [SerializeField]
        bool showTakeAllResolutionBurstCaptureButton = true;
#pragma warning restore 0414
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        void Update()
        {
            if (FlexibleInput.AnyKey() && !UIStatus.InputFieldFocused())
            {
                bool takeScreenshotBurst = takeScreenshotBurstKeySet.MatchesInput();
                if (screenshotScript.screenshotsInProgress && takeScreenshotBurst)
                {
                    Debug.Log("Screenshots already in progress.");
                    return;
                }

                if (takeScreenshotBurst)
                    TakeScreenshotBurst(true);
            }
        }
#endif

        public void TakeScreenshotBurst(bool save = true)
        {
            StartCoroutine(TakeScreenshotBurstCoroutine(screenshotScript.TakeSingleScreenshotCoroutine, save));
        }

        public void TakeAllScreenshotBurst(bool save = true)
        {
            StartCoroutine(TakeScreenshotBurstCoroutine(screenshotScript.TakeAllScreenshotsCoroutine, save));
        }

        public IEnumerator TakeScreenshotBurstCoroutine(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
            if (WillTakeScreenshotBurst != null)
                WillTakeScreenshotBurst();

            screenshotScript.screenshotsInProgressOverride = true;

            for (int i = 0; i < burstSize; ++i)
            {
                yield return StartCoroutine(captureAndSaveRoutine(save));

                if(useTimeDelay)
                {
                    if(realtimeDelay)
                        yield return new WaitForSecondsRealtime(delay);
                    else
                        yield return new WaitForSeconds(delay);
                } else
                {
                    for (int j = 0; j < skipFrames + 1; ++j)
                        yield return new WaitForEndOfFrame();
                }
            }

            screenshotScript.screenshotsInProgressOverride = false;

            if (ScreenshotBurstTaken != null)
                ScreenshotBurstTaken();
        }
    }
}