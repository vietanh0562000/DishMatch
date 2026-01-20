using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif
#if UNITY_2019_1_OR_NEWER
using Unity.Collections;
using UnityEngine.Rendering;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
    [CanEditMultipleObjects]
    [DisallowMultipleComponent]
#endif
    [System.Serializable]
    public sealed class ScreenshotScript : CaptureScript
    {
        public const string version = "3.55";
        const string TRS_SCREENSHOTS_MAX_INSTANCES_KEY = "TRS_SCREENSHOTS_MAX_INSTANCES_KEY";

        public static System.Action<ScreenshotScript> WillTakeMultipleScreenshots;
        public static System.Action<ScreenshotScript, int, int, int> WillTakeScreenshot; // screenshot script, width, height, scale
        public static System.Action<ScreenshotScript, RawFrameData> RawFrameTaken;
        public static System.Action<ScreenshotScript, Texture2D> ScreenshotTaken;
        public static System.Action<ScreenshotScript, Texture2D> WillSaveScreenshot;
        public static System.Action<ScreenshotScript, string> ScreenshotSaved; // screenshot script, file path
        public static System.Action<ScreenshotScript> MultipleScreenshotsTaken;

        [System.Serializable]
        public enum ResolutionSelect
        {
            GameViewResolution,
            DefaultResolution,
            CameraResolution
        };

        static bool maxInstancesLoaded;
        static int cachedMaxInstances = 1;
        static List<ScreenshotScript> instances = new List<ScreenshotScript>();

        public List<ScreenshotResolution> screenshotResolutions = new List<ScreenshotResolution>();
        public bool hasScreenshotResolutions { get { return screenshotResolutions.Count > 0; } }
        public List<ScreenshotResolutionTransformation> screenshotResolutionTransformations = new List<ScreenshotResolutionTransformation>();


        public bool getFileNameFromScreenshotResolution;

        public bool captureWithHDRIfPossible;
        public bool canCaptureWithHDR
        {
            get
            {
                if (captureMode != CaptureMode.ScreenCapture) return true;
                if (screenCaptureConfig == null) return true;
#if UNITY_2021_3_OR_NEWER
                if (!screenCaptureConfig.useAltScreenCapture) return HDROutputSettings.main != null && HDROutputSettings.main.available;
#endif
                return false;
            }
        }

        public bool captureWithHDR { get { return captureWithHDRIfPossible && canCaptureWithHDR; } }

        RenderTextureFormat renderTextureFormat { get { return captureWithHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.ARGB32; } }
        TextureFormat textureFormat { get { return captureWithHDR ? TextureFormat.RGBAHalf : TextureFormat.ARGB32; } }

        public bool saveRawScreenshot = true;
        public LiveFrame[] liveFrames = new LiveFrame[0];
        public ScreenshotResolutionLiveFrame[] screenshotResolutionLiveFrames = new ScreenshotResolutionLiveFrame[0];
        public bool hasLiveFrames { get { return liveFrames.Length > 0 || screenshotResolutionLiveFrames.Length > 0; } }
        LiveFrame activeLiveFrame;
        bool capturingLiveFrame { get { return activeLiveFrame != null; } }

        public HotKeySet takeSingleScreenshotKeySet = new HotKeySet
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.F,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.F,
#endif
            alt = true,
        };
        public HotKeySet takeAllScreenshotsKeySet = new HotKeySet
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.V,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.V,
#endif
            alt = true,
        };

        public bool playScreenshotAudio
        {
            get
            {
#if UNITY_EDITOR
                return playScreenshotAudioInEditor;
#else
                return playScreenshotAudioInGame;
#endif
            }
        }
        public bool playScreenshotAudioInGame = true;
        public AudioSource screenshotAudioSource;

        public ScreenshotFileSettings fileSettings = new ScreenshotFileSettings();
        public Texture2D lastScreenshotTexture;

        // Advance Settings
        public ResolutionSelect gameViewScreenshotResolution { get { return captureMode != CaptureMode.Camera ? ResolutionSelect.GameViewResolution : ResolutionSelect.CameraResolution; } }
        public ResolutionSelect sceneViewScreenshotResolution = ResolutionSelect.CameraResolution;

        public bool _screenshotsInProgress;
        public bool screenshotsInProgress { get { return _screenshotsInProgress || screenshotsInProgressOverride; } }
        public bool screenshotsInProgressOverride;

        protected override bool useCanvasesAdjuster
        {
            get
            {
                return base.useCanvasesAdjuster && captureMode == CaptureMode.Camera;
            }
        }

        #region Editor variables
        public delegate IEnumerator CaptureAndSaveRoutine(bool save = true);
#if UNITY_EDITOR
        public System.WeakReference[] subComponents = new System.WeakReference[(int)SubComponentType.Size];
        public enum SubComponentType
        {
            ScreenshotSeries,
            MultiLanguage,
            Burst,
            GameObject,
            CutoutSet,
            MultiCamera,
            MultiCameraGameObject,
            Size
        }

        public ScreenshotSubComponentScript GetSubComponent(SubComponentType subComponentType)
        {
            System.WeakReference weakSubComponent = subComponents[(int)subComponentType];
            if (weakSubComponent == null || !weakSubComponent.IsAlive)
            {
                subComponents[(int)subComponentType] = null;
                return null;
            }

            return subComponents[(int)subComponentType].Target as ScreenshotSubComponentScript;
        }

        public void SetSubComponent(SubComponentType subComponentType, ScreenshotSubComponentScript subComponent)
        {
            if (subComponent == null)
            {
                subComponents[(int)subComponentType] = null;
                return;
            }

            subComponent.subWindowMode = true;
            subComponent.hiddenMode = subComponent.gameObject == gameObject;
            subComponents[(int)subComponentType] = new System.WeakReference(subComponent);
        }

        public void RefreshSubComponents()
        {
            ScreenshotSubComponentScript[] subComponentScripts = GetComponents<ScreenshotSubComponentScript>();
            foreach (ScreenshotSubComponentScript subComponentScript in subComponentScripts)
            {
                switch(subComponentScript.GetType().Name)
                {
                    case "ScreenshotSeriesScript":
                        SetSubComponent(SubComponentType.ScreenshotSeries, subComponentScript);
                        continue;
                    case "GameObjectScreenshotScript":
                        SetSubComponent(SubComponentType.GameObject, subComponentScript);
                        continue;
                    case "MultiLangScreenshotScript":
                        SetSubComponent(SubComponentType.MultiLanguage, subComponentScript);
                        continue;
                    case "ScreenshotBurstScript":
                        SetSubComponent(SubComponentType.Burst, subComponentScript);
                        continue;
                    case "CutoutScreenshotSetScript":
                        SetSubComponent(SubComponentType.CutoutSet, subComponentScript);
                        continue;
                    case "MultiCameraScreenshotScript":
                        SetSubComponent(SubComponentType.MultiCamera, subComponentScript);
                        continue;
                    case "MultiCameraGameObjectScreenshotScript":
                        SetSubComponent(SubComponentType.MultiCameraGameObject, subComponentScript);
                        continue;
                }
            }
        }

        [UnityEngine.Serialization.FormerlySerializedAs("adjustDelay")]
        public bool adjustScreenshotResolutionDelay;
        public bool playScreenshotAudioInEditor = true;
        [SerializeField]
        bool shareResolutionsBetweenPlatforms = true;
        [SerializeField] // Serialized to preserve values
        ScreenshotResolutionSet[] screenshotResolutionsForType = new ScreenshotResolutionSet[20]; // Leaving some wiggle room
#pragma warning disable 0414
        public bool showScreenshotResolutionTransformations;
        [SerializeField]
        public Texture2D textureToEdit;
        [SerializeField]
        int screenshotsEditorRefreshHack;
        [SerializeField]
        bool showButtonSettings;
        [SerializeField]
        bool showSceneCaptureButton = true;
        [SerializeField]
        public bool showDownloadFramesButton = true;
        [SerializeField]
        bool showLiveFramesCaptureButton = true;
        [SerializeField]
        bool showAllResolutionLiveFramesCaptureButton = true;
        [SerializeField]
        bool showResolutionList = true;
        [SerializeField]
        bool showResolutionSettings;
        [SerializeField]
        bool showLiveFrames = true;
        [SerializeField]
        bool showScreenshotResolutionLiveFrames;
#pragma warning restore 0414
#endif
        #endregion

        public void TakeLiveFrameScreenshots(bool save = true)
        {
            StartCoroutine(TakeLiveFrameScreenshotsCoroutine(saveRawScreenshot, liveFrames, save));
        }

        public void TakeLiveFrameScreenshotsWithTexture(Texture2D textureToFrame, bool save = true)
        {
            StartCoroutine(TakeLiveFrameScreenshotsWithTextureCoroutine(textureToFrame, liveFrames, save));
        }

        public IEnumerator TakeLiveFrameScreenshotsCoroutine(bool save = true)
        {
            yield return StartCoroutine(TakeLiveFrameScreenshotsCoroutine(saveRawScreenshot, liveFrames, save));
        }

        public IEnumerator TakeLiveFrameScreenshotsCoroutine(bool saveRawScreenshot, LiveFrame[] liveFrames, bool save = true)
        {
            yield return StartCoroutine(TakeSingleScreenshotCoroutine(saveRawScreenshot));
            yield return StartCoroutine(TakeLiveFrameScreenshotsWithTextureCoroutine(lastScreenshotTexture, liveFrames, save));
        }

        public IEnumerator TakeLiveFrameScreenshotsWithTextureCoroutine(Texture2D textureToFrame, LiveFrame[] liveFrames, bool save = true)
        {
            int originalCount = fileSettings.count;
            bool originalScreenshotsInProgressOverride = screenshotsInProgressOverride;
            screenshotsInProgressOverride = true;

            bool resolutionChanged = false;
            Resolution originalResolution = ScreenExtensions.CurrentResolution();
            for (int i = 0; i < liveFrames.Length; ++i)
            {
                LiveFrame liveFrame = liveFrames[i];
                if (!liveFrame.active) continue;
                if (liveFrame.activeOnlyIfGameObjectActive && !liveFrame.gameObjectRequiredToBeActive.activeInHierarchy) continue;

                activeLiveFrame = liveFrame;
                GameObject[] originalTempEnabledObjects = tempEnabledObjects;
                GameObject[] originalTempDisabledObjects = tempDisabledObjects;
                Texture originalDestinationTexture = liveFrame.textureDestination.texture;

                tempEnabledObjects = liveFrame.tempEnabledObjects;
                tempDisabledObjects = liveFrame.tempDisabledObjects;
                liveFrame.textureDestination.texture = textureToFrame;

                fileSettings.SetCount(originalCount);
                fileSettings.frameDescription = liveFrame.name;

                Resolution currentResolution = ScreenExtensions.CurrentResolution();
                if (liveFrame.overrideResolution)
                {
                    bool resolutionIsDifferent = !liveFrame.resolution.IsSameSizeAs(currentResolution);
                    if (resolutionIsDifferent)
                    {
                        resolutionChanged = true;
                        liveFrame.originalResolution = currentResolution;
                        ScreenExtensions.UpdateResolution(liveFrame.resolution);
                        yield return new WaitForResolutionUpdates();
                        currentResolution = liveFrame.resolution;
                    }
                }

                bool originalUseCutout = useCutout;
                useCutout = false;
                PrepareToCapture();

                Rect cutoutRect = new Rect(0, 0, currentResolution.width, currentResolution.height);
                yield return StartCoroutine(TakeScreenshotWithCameras(cameras, currentResolution, cutoutRect, save, screenCaptureConfig, liveFrame.transformations, true));
                fileSettings.frameDescription = null;

                RestoreAfterCapture();
                useCutout = originalUseCutout;
                tempEnabledObjects = originalTempEnabledObjects;
                tempDisabledObjects = originalTempDisabledObjects;
                liveFrame.textureDestination.texture = originalDestinationTexture;

                activeLiveFrame = null;
            }

            if (resolutionChanged)
                ScreenExtensions.UpdateResolution(originalResolution);

            screenshotsInProgressOverride = originalScreenshotsInProgressOverride;
        }

        /** Take screenshots of at all the listed resolutions. */
        public void TakeAllScreenshots(bool save = true)
        {
            StartCoroutine(TakeAllScreenshotsCoroutine(save));
        }

        public void TakeAllScreenshotsWithLiveFrames(bool save = true)
        {
            StartCoroutine(TakeAllScreenshotsWithLiveFramesCoroutine(save));
        }

        public IEnumerator TakeAllScreenshotsCoroutine(bool save = true)
        {
            return FlexibleTakeAllScreenshotsCoroutine(false, save);
        }

        public IEnumerator TakeAllScreenshotsWithLiveFramesCoroutine(bool save = true)
        {
            return FlexibleTakeAllScreenshotsCoroutine(true, save);
        }

        public IEnumerator FlexibleTakeAllScreenshotsCoroutine(bool withLiveFrames = false, bool save = true)
        {
            _screenshotsInProgress = true;
#if UNITY_EDITOR
            PrepareGameViewResolutions();
#endif
            if (WillTakeMultipleScreenshots != null)
                WillTakeMultipleScreenshots(this);

            CleanUpCameraList();
            PrepareToCapture();
            ScreenshotResolution[] resolutionsToUse = screenshotResolutions.ToArray();

            bool fixedResolutionDevice = false;
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            fixedResolutionDevice = true;
#endif


#if UNITY_EDITOR
            int originalSelectedSizeIndex = GameView.GetSelectedSizeIndex();
#else
            Resolution originalResolution = ScreenExtensions.CurrentResolution();
#endif
            if (realtimeDelay)
                yield return new WaitForSecondsRealtime(delayBeforeCapture);
            else
                yield return new WaitForSeconds(delayBeforeCapture);


            // GameObjects added or removed prior to capture only update after 2 frames in Unity 2019.2. (This wait and the one in the loop.)
#if UNITY_2019_2
                yield return new WaitForEndOfFrame();
#endif

            int originalScreenCaptureConfigScale = screenCaptureConfig != null ? screenCaptureConfig.altScreenCaptureModeScale : 1;
            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            if (resolutionsToUse.Length < 1 || fixedResolutionDevice)
                resolutionsToUse = new ScreenshotResolution[] { new ScreenshotResolution(true, NameForResolution(currentResolution), currentResolution.width, currentResolution.height, 1) };
            for (int i = 0; i < resolutionsToUse.Length; ++i)
            {
                ScreenshotResolution screenshotResolution = resolutionsToUse[i];
                if (!screenshotResolution.active)
                    continue;

                currentResolution = ScreenExtensions.CurrentResolution();
                bool resolutionIsDifferent = !screenshotResolution.unscaledResolution.IsSameSizeAs(currentResolution);
                if (resolutionIsDifferent)
                {
                    ScreenExtensions.UpdateResolution(screenshotResolution.unscaledResolution);
                    if (screenshotResolution.waitForUpdates)
                        yield return new WaitForResolutionUpdates();
                }

                if (screenshotResolution.delay > 0)
                {
                    if (realtimeDelay)
                        yield return new WaitForSecondsRealtime(screenshotResolution.delay);
                    else
                    {
                        if (stopTimeDuringCapture && Application.isPlaying)
                            Time.timeScale = 1f;
                        yield return new WaitForSeconds(screenshotResolution.delay);
                        if (stopTimeDuringCapture && Application.isPlaying)
                            Time.timeScale = 0f;
                    }
                }

                bool screenCaptureMode = captureMode == CaptureMode.ScreenCapture;
                if (screenCaptureConfig != null) screenCaptureConfig.altScreenCaptureModeScale = screenshotResolution.scale;
                Rect cutoutRect = (useCutout && cutoutScript != null) ? cutoutScript.rect : new Rect(0, 0, screenshotResolution.unscaledResolution.width, screenshotResolution.unscaledResolution.height);

                List<TextureTransformation> allCaptureTransformations = new List<TextureTransformation>(captureTransformations);
                foreach (ScreenshotResolutionTransformation screenshotResolutionTransformation in screenshotResolutionTransformations)
                {
                    if (!screenshotResolutionTransformation.active) continue;

                    if (string.Equals(screenshotResolutionTransformation.screenshotResolutionName, screenshotResolution.name))
                        allCaptureTransformations.Add(screenshotResolutionTransformation.textureTransformation);
                }

                bool saveScreenshot = save && (!withLiveFrames || saveRawScreenshot);
                yield return StartCoroutine(TakeScreenshotWithCameras(cameras, screenshotResolution.unscaledResolution, cutoutRect, saveScreenshot, screenCaptureConfig, allCaptureTransformations.ToArray(), true));

                if (withLiveFrames)
                {
                    RestoreAfterCapture();

                    List<LiveFrame> allLiveFrames = new List<LiveFrame>(liveFrames);
                    foreach (ScreenshotResolutionLiveFrame screenshotResolutionLiveFrame in screenshotResolutionLiveFrames)
                    {
                        if (string.Equals(screenshotResolutionLiveFrame.screenshotResolutionName, screenshotResolution.name))
                            allLiveFrames.Add(screenshotResolutionLiveFrame.liveFrame);
                    }

                    yield return StartCoroutine(TakeLiveFrameScreenshotsWithTextureCoroutine(lastScreenshotTexture, allLiveFrames.ToArray(), save));

                    PrepareToCapture();
                }
            }
            if (screenCaptureConfig != null) screenCaptureConfig.altScreenCaptureModeScale = originalScreenCaptureConfigScale;

            if (save)
            {
                fileSettings.IncrementCount();
                fileSettings.SaveCount();
            }

#if UNITY_EDITOR
            if (GameView.GetSelectedSizeIndex() != originalSelectedSizeIndex)
                GameView.SetSelecedSizeIndex(originalSelectedSizeIndex);
#else
            currentResolution = ScreenExtensions.CurrentResolution();
            if (currentResolution.width != originalResolution.width || currentResolution.height != originalResolution.height)
                ScreenExtensions.UpdateResolution(originalResolution);
#endif
            RestoreAfterCapture();
            if (MultipleScreenshotsTaken != null)
                MultipleScreenshotsTaken(this);

#if UNITY_EDITOR
            screenshotsEditorRefreshHack = 10;
#endif
            _screenshotsInProgress = false;
        }

#if UNITY_EDITOR
        public void TakeSceneViewScreenshot(bool save = true)
        {
            StartCoroutine(SafeTakeSceneViewScreenshot(save));
        }

        IEnumerator SafeTakeSceneViewScreenshot(bool save = true)
        {
            CaptureMode originalCaptureMode = captureMode;
            captureMode = CaptureMode.Camera;

            Camera sceneViewCamera = SceneView.lastActiveSceneView.camera;
            Resolution resolution = ResolutionForResolutionSelect(sceneViewScreenshotResolution, sceneViewCamera);
            Rect cutoutRect = (useCutout && cutoutScript != null) ? cutoutScript.rect : new Rect(0, 0, resolution.width, resolution.height);
            yield return StartCoroutine(TakeScreenshotWithCameras(new Camera[] { sceneViewCamera }, resolution, cutoutRect, save, null, captureTransformations));

            captureMode = originalCaptureMode;
        }
#endif

        public void TakeSingleScreenshot(bool save = true)
        {
            StartCoroutine(TakeSingleScreenshotCoroutine(save));
        }

        public IEnumerator TakeSingleScreenshotCoroutine(bool save = true)
        {
            CleanUpCameraList();

            Camera firstCamera = cameras.Length > 0 ? cameras[0] : null;
            Resolution resolution = ResolutionForResolutionSelect(gameViewScreenshotResolution, firstCamera);
            Rect cutoutRect = (useCutout && cutoutScript != null) ? cutoutScript.rect : new Rect(0, 0, resolution.width, resolution.height);
            yield return StartCoroutine(TakeScreenshotWithCameras(cameras, resolution, cutoutRect, save, screenCaptureConfig, captureTransformations));
        }

        // Convenience method to skip the often unnecessary camerasToUse.
        public IEnumerator TakeScreenshot(Resolution resolution, Rect cutoutRect, bool save = true, ScreenCaptureConfig screenCaptureConfigToUse = null, TextureTransformation[] transformations = null, bool calledFromLoop = false)
        {
            return TakeScreenshotWithCameras(null, resolution, cutoutRect, save, screenCaptureConfigToUse, transformations, calledFromLoop);
        }

        // CamerasToUse is only necessary with the RenderTexture render mode. To use all cameras, pass in Camera.allCameras 
        public IEnumerator TakeScreenshotWithCameras(Camera[] camerasToUse, Resolution resolution, Rect cutoutRect, bool save = true, ScreenCaptureConfig screenCaptureConfigToUse = null, TextureTransformation[] transformations = null, bool calledFromLoop = false)
        {
            if (!calledFromLoop)
            {
                _screenshotsInProgress = true;
                ValidateCaptureMode(camerasToUse != null && camerasToUse.Length > 0);
                PrepareToCapture();
            }

            string cameraName = (camerasToUse != null && camerasToUse.Length > 0 && camerasToUse[0] != null) ? camerasToUse[0].name : "CameraName";

            string resolutionName = NameForResolution(resolution);
            string resolutionString = new ScreenshotResolution(true, resolutionName, resolution).resolutionString;

            bool screenCaptureMode = captureMode == CaptureMode.ScreenCapture;

            int scale = screenCaptureMode && screenCaptureConfigToUse != null && screenCaptureConfigToUse.useAltScreenCapture && !screenCaptureConfigToUse.useStereoScreenCaptureMode ? screenCaptureConfigToUse.altScreenCaptureModeScale : 1;
            if (WillTakeScreenshot != null)
                WillTakeScreenshot(this, resolution.width, resolution.height, scale);
            if (delayBeforeCapture > 0) yield return new WaitForSecondsRealtime(delayBeforeCapture);


            bool asyncCaptureMode = false;
#if UNITY_2019_1_OR_NEWER
            asyncCaptureMode = captureMode == CaptureMode.Async;
#endif
            // Skip WaitForEndOfFrame here as it occurs within the AsyncScreenshot method.
            if (!asyncCaptureMode)
                yield return new WaitForEndOfFrame();

            // Avoid destroying the texture that will be displayed in the live frame.
            if (!capturingLiveFrame)
                MonoBehaviourExtended.FlexibleDestroy(lastScreenshotTexture);

            bool solidify = fileSettings.fileType == ScreenshotFileSettings.FileType.PNG && !fileSettings.allowTransparency;
#if UNITY_2019_1_OR_NEWER
            if (captureMode == CaptureMode.Async)
                yield return StartCoroutine(AsyncScreenshot(cutoutRect, ResolutionExtensions.EmptyResolution(), solidify, transformations));
            else
#endif
                lastScreenshotTexture = Screenshot(camerasToUse, cutoutRect, ResolutionExtensions.EmptyResolution(), solidify, screenCaptureConfigToUse, transformations);

#if UNITY_EDITOR
            textureToEdit = lastScreenshotTexture;
#endif

            if (save)
            {
                string fullFilePath = null;
                if (useCutout && cutoutScript != null)
                    fullFilePath = fileSettings.FullFilePath(cutoutScript.name, fileSettings.FileNameWithCaptureDetails(cameraName, resolutionString));
                else if (getFileNameFromScreenshotResolution)
                {
                    // Standardize to the directory separator character (different on different OS)
                    resolutionName = resolutionName.Replace('/', System.IO.Path.DirectorySeparatorChar);
                    resolutionName = resolutionName.Replace('\\', System.IO.Path.DirectorySeparatorChar);

                    int lastSeparatorIndex = resolutionName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                    if (lastSeparatorIndex >= 0)
                    {
                        string folder = resolutionName.Substring(0, lastSeparatorIndex + 1);
                        string fileName = resolutionName.Substring(lastSeparatorIndex + 1);
                        fullFilePath = fileSettings.FullFilePath(folder, fileSettings.MinimalFileName(fileName));
                    }
                    else
                        fullFilePath = fileSettings.FullFilePath("", fileSettings.MinimalFileName(resolutionName));
                }
                else
                    fullFilePath = fileSettings.FullFilePathWithCaptureDetails(cameraName, resolutionName, resolutionString);
                Save(lastScreenshotTexture, fullFilePath, false);
                if (!calledFromLoop)
                {
                    fileSettings.IncrementCount();
                    fileSettings.SaveCount();
                }
            }

            if (!calledFromLoop)
            {
                RestoreAfterCapture();
                _screenshotsInProgress = false;
            }
        }

#if UNITY_2019_1_OR_NEWER
        public IEnumerator AsyncScreenshot(Rect cutoutRect, Resolution resizeResolution, bool solidify = true, TextureTransformation[] transformations = null)
        {
            PlayAudioIfNecessary();

            yield return new WaitForEndOfFrame();

            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            if (cutoutRect == Rect.zero)
                cutoutRect = new Rect(0, 0, currentResolution.width, currentResolution.height);
            RawFrameData rawData = CaptureRawFrame.AsyncCapture(currentResolution, cutoutRect, resizeResolution, solidify, renderTextureFormat);

            while (!rawData.readyToProcess)
                yield return new WaitForEndOfFrame();

            Texture2D screenshotTexture = rawData.Process();

            if (transformations != null && transformations.Length > 0)
                screenshotTexture = screenshotTexture.ApplyTransformations(transformations, true);
            if (ScreenshotTaken != null)
                ScreenshotTaken(this, screenshotTexture);

            lastScreenshotTexture = screenshotTexture;
        }
#endif

        // Convenience method to skip the often unnecessary camerasToUse.
        public Texture2D Screenshot(Rect cutoutRect, Resolution resizeResolution, bool solidify = true, ScreenCaptureConfig screenCaptureConfigToUse = null, TextureTransformation[] transformations = null)
        {
            return Screenshot(null, cutoutRect, resizeResolution, solidify, screenCaptureConfigToUse, transformations);
        }

        // CamerasToUse is only necessary with the RenderTexture render mode. To use all cameras, pass in Camera.allCameras 
        public Texture2D Screenshot(Camera[] camerasToUse, Rect cutoutRect, Resolution resizeResolution, bool solidify = true, ScreenCaptureConfig screenCaptureConfigToUse = null, TextureTransformation[] transformations = null)
        {
            PlayAudioIfNecessary();

            Texture2D screenshotTexture = RawScreenshot(camerasToUse, cutoutRect, resizeResolution, solidify, screenCaptureConfigToUse).Process();
            if (transformations != null && transformations.Length > 0)
                screenshotTexture = screenshotTexture.ApplyTransformations(transformations, true);
            if (ScreenshotTaken != null)
                ScreenshotTaken(this, screenshotTexture);
            return screenshotTexture;
        }

        // Convenience method to skip the often unnecessary camerasToUse.
        // Returns a raw frame data object. Allows delaying a small amount of texture processing until later.
        public RawFrameData RawScreenshot(Rect cutoutRect, Resolution resizeResolution, bool solidify = true, ScreenCaptureConfig screenCaptureConfigToUse = null)
        {
            return RawScreenshot(null, cutoutRect, resizeResolution, solidify, screenCaptureConfigToUse);
        }

        // Returns a raw frame data object. Allows delaying a small amount of texture processing until later.
        // CamerasToUse is only necessary with the RenderTexture render mode.
        public RawFrameData RawScreenshot(Camera[] camerasToUse, Rect cutoutRect, Resolution resizeResolution, bool solidify = true, ScreenCaptureConfig screenCaptureConfigToUse = null)
        {
            if (cutoutRect == Rect.zero)
            {
                Resolution currentResolution = ScreenExtensions.CurrentResolution();
                cutoutRect = new Rect(0, 0, currentResolution.width, currentResolution.height);
            }
            else
            {
                if (resizeResolution.HasSize() && (resizeResolution.width > cutoutRect.width || resizeResolution.height > cutoutRect.height))
                    Debug.LogWarning("Warning: resizing to a resolution larger than the cutout resolution. This will cause warping. To skip the resize, use a Resolution with a width or height of 0.");
            }

            RawFrameData rawFrameData = null;
            switch (captureMode)
            {
                case CaptureMode.Camera:
                    if (captureAllCameras)
                        rawFrameData = CaptureRawFrame.AllCamerasRenderTexture(cutoutRect, resizeResolution, solidify, renderTextureFormat);
                    else if (camerasToUse.Length > 1)
                        rawFrameData = CaptureRawFrame.CamerasRenderTexture(camerasToUse, cutoutRect, resizeResolution, solidify, renderTextureFormat);
                    else
                        rawFrameData = CaptureRawFrame.CameraRenderTexture(camerasToUse[0], cutoutRect, resizeResolution, solidify, renderTextureFormat);
                    break;
                case CaptureMode.ScreenCapture:
                    if (screenCaptureConfigToUse != null && screenCaptureConfigToUse.useAltScreenCapture)
                    {
                        if (screenCaptureConfigToUse.useStereoScreenCaptureMode)
                            rawFrameData = CaptureRawFrame.AltScreenCapture(cutoutRect, resizeResolution, solidify, screenCaptureConfigToUse.stereoScreenCaptureMode);
                        else
                            rawFrameData = CaptureRawFrame.AltScreenCapture(cutoutRect, resizeResolution, solidify, screenCaptureConfigToUse.altScreenCaptureModeScale);
                    }
                    else
                        rawFrameData = CaptureRawFrame.ScreenCapture(cutoutRect, resizeResolution, solidify, textureFormat);
                    break;
                default:
                    throw new UnityException("Unhandled capture mode.");
            }

            if (RawFrameTaken != null)
                RawFrameTaken(this, rawFrameData);
            return rawFrameData;
        }

        public void Save(Texture2D texture, string overrideFilePath = "", bool destroyTexture = false)
        {
            System.Action<string, bool> completionBlock = (filePath, savedSuccessfully) =>
            {
                lastSaveFilePath = filePath;
                if (ScreenshotSaved != null)
                    ScreenshotSaved(this, filePath);

#if UNITY_EDITOR
                if (savedSuccessfully)
                    Debug.Log("Screenshot saved: " + filePath);
#endif
            };

            if (WillSaveScreenshot != null)
                WillSaveScreenshot(this, texture);

            string finalFilePath = string.IsNullOrEmpty(overrideFilePath) ? fileSettings.FullFilePath() : overrideFilePath;
            texture.SaveAccordingToFileSettings(fileSettings, finalFilePath, completionBlock);
            if (destroyTexture)
                MonoBehaviourExtended.FlexibleDestroy(texture);
        }

        #region Unity Methods
        protected override void Awake()
        {
            base.Awake();

#if UNITY_2019_1_OR_NEWER && (UNITY_IOS || UNITY_WEBGL)
            if (captureMode == CaptureMode.Async)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Async mode will not function in iOS or WebGL builds. Builds will automatically switch to ScreenCapture mode.");
#else
                Debug.LogError("Async mode does not function on iOS or WebGL. Switching to ScreenCapture mode.");
                captureMode = CaptureMode.ScreenCapture;
#endif
            }
#endif
        }

        protected override void Start()
        {
            base.Start();

            _screenshotsInProgress = false;
#if UNITY_EDITOR
            UpdateScreenshotResolutions();
#endif
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            fileSettings.SetUp(gameObject.GetInstanceID());
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        protected override void Update()
        {
            base.Update();
#if UNITY_EDITOR
            if (editorWindowMode)
                return;
#endif
            if (FlexibleInput.AnyKey() && !UIStatus.InputFieldFocused())
            {
                bool takeSingleScreenshot = takeSingleScreenshotKeySet.MatchesInput();
                bool takeAllScreenshots = takeAllScreenshotsKeySet.MatchesInput();
                if (screenshotsInProgress && (takeSingleScreenshot || takeAllScreenshots))
                {
                    Debug.LogWarning("Screenshots already in progress.");
                    return;
                }

                if (takeSingleScreenshot)
                    TakeSingleScreenshot(true);
                if (takeAllScreenshots)
                    TakeAllScreenshots(true);
            }
        }
#endif
        #endregion

        #region Helpers
        void ValidateCaptureMode(bool hasCameras)
        {
            if (hasCameras) return;
            if (captureMode != CaptureMode.Camera) return;

            Debug.LogError("Cannot use capture mode render texture without cameras set. Switching to screen capture mode.");
            captureMode = CaptureMode.ScreenCapture;
        }

        void ValidateCutout(Resolution resizeResolution, Rect cutoutRect)
        {
            if (resizeResolution.HasSize() && (resizeResolution.width > cutoutRect.width || resizeResolution.height > cutoutRect.height))
                Debug.LogWarning("Warning: resizing to a resolution larger than the cutout resolution. This will cause warping. To skip the resize, use a Resolution with a width or height of 0.");
        }

        void PlayAudioIfNecessary()
        {
            if (playScreenshotAudio && screenshotAudioSource != null && screenshotAudioSource.clip != null)
                screenshotAudioSource.Play();
        }

        #region Resolution Helpers
#if UNITY_EDITOR
        void PrepareGameViewResolutions()
        {
            for (int i = 0; i < screenshotResolutions.Count; ++i)
            {
                ScreenshotResolution screenshotResolution = screenshotResolutions[i];
                if (!GameView.SizeExists(screenshotResolution.unscaledResolution))
                    GameView.AddTempCustomSize(GameView.GameViewSizeType.FixedResolution, screenshotResolution.unscaledResolution);
            }
        }
#endif

        string NameForResolution(Resolution resolution)
        {
            if (activeLiveFrame != null && activeLiveFrame.overrideResolution)
                resolution = activeLiveFrame.originalResolution;

            for (int i = 0; i < screenshotResolutions.Count; ++i)
            {
                ScreenshotResolution screenshotResolution = screenshotResolutions[i];
                if (screenshotResolution.unscaledResolution.IsSameSizeAs(resolution))
                    return screenshotResolution.name;
            }

#if UNITY_EDITOR
            return AdditionalResolutions.ConvertToStructuredFolderName(GameView.NameForSize(resolution));
#endif
#pragma warning disable 0162
            return "";
#pragma warning restore 0162
        }

        public Resolution ResolutionForResolutionSelect(ResolutionSelect resolutionSelect, Camera camera = null)
        {
            if (resolutionSelect == ResolutionSelect.CameraResolution)
            {
                if (camera == null)
                    throw new UnityException("Camera must be provided to select camera resolution.");
                return new Resolution { width = camera.pixelWidth, height = camera.pixelHeight };
            }
            else if (resolutionSelect == ResolutionSelect.DefaultResolution && screenshotResolutions.Count > 0)
                return new Resolution { width = screenshotResolutions[0].width, height = screenshotResolutions[0].height };
            else
                return ScreenExtensions.CurrentResolution();
        }
        #endregion

        #region Scene Change Helpers
        public override void UpdateDontDestroyOnLoad()
        {
            bool willDestroy = false;
            if (Application.isPlaying && dontDestroyOnLoad && !instances.Contains(this))
            {
                if (instances.Count < GetMaxInstances())
                {
                    instances.Add(this);
                    DontDestroyOnLoad(gameObject);
                }
                else
                {
                    Debug.LogError("Destroying: " + gameObject + " as at least " + instances.Count + " ScreenshotScripts are in the scene with a max instances count of: " + GetMaxInstances() + ". This functionality may be changed under Settings -> DontDestroyOnLoad settings.\n\nThis feature is intended to be used to allow one or more ScreenshotScripts to be kept and re-used even while transitioning between scenes. If intentionally using separate instances in each scene, it's recommended to uncheck DontDestroyOnLoad.");
                    MonoBehaviourExtended.FlexibleDestroy(gameObject);
                    willDestroy = true;
                }
            }

            if (!willDestroy)
                base.UpdateDontDestroyOnLoad();
        }
        #endregion

        #region Clean Up Inputs
        public void ScaleResolutionsToScreen()
        {
            for (int i = 0; i < screenshotResolutions.Count; ++i)
            {
                ScreenshotResolution screenshotResolution = screenshotResolutions[i];
                screenshotResolution.ScaleToScreen();
                screenshotResolutions[i] = screenshotResolution;
            }
        }
        #endregion

        #region Editor Variable Change Updates
#if UNITY_EDITOR
        public void UpdateScreenshotResolutions()
        {
            if (!shareResolutionsBetweenPlatforms)
            {
                GameViewSizeGroupType currentType = GameView.GetCurrentGroupType();
                if (screenshotResolutionsForType[(int)currentType] == null)
                    screenshotResolutions = new List<ScreenshotResolution>(screenshotResolutions);
                else
                    screenshotResolutions = screenshotResolutionsForType[(int)currentType].screenshotResolutions;
            }
        }

        public void ScreenshotResolutionsChanged()
        {
            if (!shareResolutionsBetweenPlatforms)
            {
                GameViewSizeGroupType currentType = GameView.GetCurrentGroupType();
                if (screenshotResolutionsForType[(int)currentType] == null)
                    screenshotResolutionsForType[(int)currentType] = new ScreenshotResolutionSet(currentType.ToString(), new List<ScreenshotResolution>(screenshotResolutions));
                else
                    screenshotResolutionsForType[(int)currentType].screenshotResolutions = new List<ScreenshotResolution>(screenshotResolutions);
            }
        }
#endif
        #endregion

        #region Max Instances Count
        public override int GetMaxInstances()
        {
            if (maxInstancesLoaded)
                return cachedMaxInstances;
            return LoadMaxInstances();
        }

        public override void SetMaxInstances(int newValue)
        {
            maxInstancesLoaded = true;
            cachedMaxInstances = newValue;
            PlayerPrefs.SetInt(TRS_SCREENSHOTS_MAX_INSTANCES_KEY, newValue);
            PlayerPrefs.Save();
        }

        static int LoadMaxInstances()
        {
            if (PlayerPrefs.HasKey(TRS_SCREENSHOTS_MAX_INSTANCES_KEY))
                cachedMaxInstances = PlayerPrefs.GetInt(TRS_SCREENSHOTS_MAX_INSTANCES_KEY);
            else
                cachedMaxInstances = 1;
            maxInstancesLoaded = true;
            return cachedMaxInstances;
        }
        #endregion
        #endregion
    }
}