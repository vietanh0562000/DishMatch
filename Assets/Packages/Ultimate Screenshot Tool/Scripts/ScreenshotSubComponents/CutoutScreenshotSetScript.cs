using System.Collections;
using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public class CutoutScreenshotSetScript : ScreenshotSubComponentScript
    {
        public static System.Action WillTakeCutoutSet;
        public static System.Action CutoutSetTaken;

        public HotKeySet takeCutoutSetKeySet = new HotKeySet {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.N,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.N,
#endif
            alt = true,
    };

        public bool includeOriginalCutout;
        public bool overrideResolution;
        public int resolutionWidth;
        public int resolutionHeight;

        public CutoutScript[] cutoutScripts = new CutoutScript[0];

        public bool overrideCutoutAdjustedRectTransforms;
        public RectTransform[] cutoutAdjustedRectTransforms = new RectTransform[0];

        Resolution originalResolution;
        RectTransform[] originalCutoutAdjustedRectTransforms;

#if UNITY_EDITOR
        protected override ScreenshotScript.SubComponentType componentType { get { return ScreenshotScript.SubComponentType.Burst; } }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        void Update()
        {
            if (FlexibleInput.AnyKey() && !UIStatus.InputFieldFocused())
            {
                bool takeCutoutSet = takeCutoutSetKeySet.MatchesInput();
                if (screenshotScript.screenshotsInProgress && takeCutoutSet)
                {
                    Debug.Log("Screenshots already in progress.");
                    return;
                }

                if (takeCutoutSet)
                    TakeCutoutSet(true);
            }
        }
#endif

        public void TakeCutoutSet(bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) return;
#endif
            StartCoroutine(TakeCutoutSetCoroutine(screenshotScript.TakeSingleScreenshotCoroutine, save));
        }

        public IEnumerator TakeCutoutSetCoroutine(ScreenshotScript.CaptureAndSaveRoutine captureAndSaveRoutine, bool save = true)
        {
#if UNITY_EDITOR
            if (!CheckForRequiredProperties()) yield break;
#endif
            if (WillTakeCutoutSet != null)
                WillTakeCutoutSet();

            screenshotScript.screenshotsInProgressOverride = true;

            if (overrideResolution)
            {
                Resolution resolution = new Resolution { width = resolutionWidth, height = resolutionHeight };
                originalResolution = ScreenExtensions.CurrentResolution();
                bool resolutionIsDifferent = !originalResolution.IsSameSizeAs(resolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(resolution);
                    yield return new WaitForResolutionUpdates();
                }
            }

            if (overrideCutoutAdjustedRectTransforms)
            {
                originalCutoutAdjustedRectTransforms = screenshotScript.cutoutAdjustedRectTransforms;
                screenshotScript.cutoutAdjustedRectTransforms = cutoutAdjustedRectTransforms;
            }

            bool originalPreviewCutoutScript = false;
            CutoutScript originalCutoutScript = screenshotScript.cutoutScript;
            if(originalCutoutScript != null)
            {
                originalPreviewCutoutScript = originalCutoutScript.preview;
                originalCutoutScript.preview = false;
            }

            bool[] originalPreviewCutoutScripts = new bool[cutoutScripts.Length];
            for (int i = 0; i < cutoutScripts.Length; ++i)
            {
                CutoutScript cutoutScript = cutoutScripts[i];
                originalPreviewCutoutScripts[i] = cutoutScript.preview;
                cutoutScript.preview = false;
            }

            if (includeOriginalCutout)
                yield return StartCoroutine(captureAndSaveRoutine(save));

            for (int i = 0; i < cutoutScripts.Length; ++i)
            {
                CutoutScript cutoutScript = cutoutScripts[i];
                screenshotScript.cutoutScript = cutoutScript;
                yield return StartCoroutine(captureAndSaveRoutine(save));
            }

            for (int i = 0; i < cutoutScripts.Length; ++i)
                cutoutScripts[i].preview = originalPreviewCutoutScripts[i];

            screenshotScript.cutoutScript = originalCutoutScript;
            if (originalCutoutScript != null)
                screenshotScript.cutoutScript.preview = originalPreviewCutoutScript;

            if (overrideCutoutAdjustedRectTransforms)
                screenshotScript.cutoutAdjustedRectTransforms = originalCutoutAdjustedRectTransforms;

            if (overrideResolution)
            {
                Resolution resolution = new Resolution { width = resolutionWidth, height = resolutionHeight };
                bool resolutionIsDifferent = !originalResolution.IsSameSizeAs(resolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(originalResolution);
                    yield return new WaitForResolutionUpdates();
                }
            }

            screenshotScript.screenshotsInProgressOverride = false;

            if (CutoutSetTaken != null)
                CutoutSetTaken();
        }

#if UNITY_EDITOR
        public override bool CheckForRequiredProperties()
        {
            if (cutoutScripts.Length <= 0)
            {
                Debug.LogError("Cutout scripts must be set in Cutout Set Settings to use this feature.");
                LogRemovalTip();
                return false;
            }

            return true;
        }
#endif
    }
}