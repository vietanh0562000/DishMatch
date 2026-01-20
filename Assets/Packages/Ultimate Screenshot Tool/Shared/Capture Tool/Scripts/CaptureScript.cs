using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

using TRS.CaptureTool.Extras;
using TRS.CaptureTool.Share;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public enum CaptureMode
    {
#if UNITY_2019_1_OR_NEWER
        Async,
#endif
        ScreenCapture,
        Camera,
    };

    [System.Serializable]
    public class ScreenCaptureConfig
    {
        public bool useAltScreenCapture;
        public int altScreenCaptureModeScale = 1;
        public bool useStereoScreenCaptureMode;
        public ScreenCapture.StereoScreenCaptureMode stereoScreenCaptureMode = ScreenCapture.StereoScreenCaptureMode.LeftEye;
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    [CanEditMultipleObjects]
    [DisallowMultipleComponent]
#endif
    [System.Serializable]
    public abstract class CaptureScript : MonoBehaviour
    {
        [SerializeField]
        CaptureMode __captureMode = CaptureMode.ScreenCapture;
        public CaptureMode captureMode
        {
            get { return __captureMode; }
            set
            {
                __captureMode = value;
#if UNITY_2019_1_OR_NEWER && (UNITY_IOS || UNITY_WEBGL) && !UNITY_EDITOR
                if(__captureMode == CaptureMode.Async)
                {
                    Debug.LogError("Async mode does not function on iOS or WebGL. Switching to ScreenCapture mode.");
                    __captureMode = CaptureMode.ScreenCapture;
                }
#endif
                CaptureModeChanged();
            }
        }

        //public bool scriptableRenderPipelineActive;
        //public bool customRenderPipelineActive;
        //public bool universalRenderPipelineActive;
        //public bool hdRenderPipelineActive;
        //public bool builtInRenderPipelineActive;
        public GameObject[] tempEnabledObjects = new GameObject[0];
        public GameObject[] tempDisabledObjects = new GameObject[0];
        public float delayBeforeCapture;
        public bool realtimeDelay = true;
        public bool stopTimeDuringCapture = true;
        public float prePauseTimeScale = 1f;

        public bool captureAllCameras = true;
        public Camera[] cameras = new Camera[0];

        public bool overrideBackground;
        public Camera backgroundCamera;
        public Color backgroundColor;

        public bool useCutout;
        public CutoutScript cutoutScript;
        public RectTransform[] cutoutAdjustedRectTransforms = new RectTransform[0];
        Transform[] originalCutoutAdjustedParents;
        bool[] originalCutoutAdjustedActive;
        bool[] originalCutoutChildrenActive;

        public HotKeySet previewCutoutKeySet = new HotKeySet {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.C,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.C,
#endif
            alt = true,
        };

        public HotKeySet pauseKeySet = new HotKeySet
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            key = UnityEngine.InputSystem.Key.None,
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            keyCode = KeyCode.None,
#endif
            alt = true,
        };

        public TextureTransformation[] captureTransformations = new TextureTransformation[0];
        [UnityEngine.Serialization.FormerlySerializedAs("transformations")]
        public TextureTransformation[] editTransformations = new TextureTransformation[0];

        public bool autoSwitchRenderMode = true;
        [UnityEngine.Serialization.FormerlySerializedAs("overlayCamera")]
        public Camera uiCamera;
        public bool overridePlaneDistance;
        public float planeDistanceOverride;

#pragma warning disable 0649
        public ScreenCaptureConfig screenCaptureConfig = new ScreenCaptureConfig();
#pragma warning restore 0649

        public string lastSaveFilePath;

        public bool showInGameMouse;
        public bool hideOriginalMouse;
        public MouseFollowScript mouseFollowScript;

        // Used to automatically update when scene switches
#pragma warning disable 0649
        [SerializeField]
        protected bool dontDestroyOnLoad = true;
#pragma warning restore 0649
        public abstract int GetMaxInstances();
        public abstract void SetMaxInstances(int newValue);
        public bool autoUpdateCameras = true;
        public bool autoUpdateCamerasByTag;
        public string[] camerasNameOrTags;
        [UnityEngine.Serialization.FormerlySerializedAs("overlayCameraNameOrTag")]
        public string uiCameraNameOrTag;
        public string backgroundCameraNameOrTag;

        protected virtual bool useCanvasesAdjuster
        {
            get
            {
                return autoSwitchRenderMode && uiCamera != null;
            }
        }

#region Editor variables
#if UNITY_EDITOR
        public void RefreshShareScript()
        {
            SetShareScript(GetComponent<ShareScript>());
        }

        public System.WeakReference weakShareScript;
        public void SetShareScript(ShareScript shareScript)
        {
            if (shareScript == null)
            {
                weakShareScript = null;
                return;
            }

            shareScript.subWindowMode = true;
            shareScript.hiddenMode = shareScript.gameObject == gameObject;
            weakShareScript = new System.WeakReference(shareScript);
        }

        public bool editorWindowMode;
        [SerializeField]
        float timeScaleOverride = 1f;

        // These values are necessary to persist inspector state such as which dropdowns are open
#pragma warning disable 0414
        [SerializeField]
        int selectedTabIndex;
        [SerializeField]
        bool showTiming;
        [SerializeField]
        bool showEdit = true;
        [SerializeField]
        bool backgroundCameraSelected;
        [SerializeField]
        bool showCameraList;
        [SerializeField, UnityEngine.Serialization.FormerlySerializedAs("showBackground")]
        bool showBackground;
        [SerializeField]
        bool showUICamera;
        [SerializeField]
        bool showCutout;
        [SerializeField]
        bool showEnabledObjects;
        [SerializeField]
        bool showDisabledObjects;
        [SerializeField]
        bool showCaptureTransformations = true;
        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("showTransformations")]
        bool showEditTransformations = true;
        [SerializeField]
        bool showSaveSettings = true;
        [SerializeField]
        bool showHotKeys;
        [SerializeField]
        bool showAudioSettings;
        [SerializeField]
        bool showMouseSettings;
        [SerializeField]
        bool showDontDestroyOnLoadSettings;
        [SerializeField]
        bool showAdvancedSettings;
        [SerializeField]
        bool showSupportSettings;
        [SerializeField]
        bool uploadingEditorLogs;
        [SerializeField]
        string editorLogsUrl;
#pragma warning restore 0414
#endif
#endregion

        protected virtual void Awake()
        {
            UpdateMouse();
            SetUpDefaultValues();

#if UNITY_EDITOR
            // Only used in editor.
            ShareScript shareScript = GetComponentInParent<ShareScript>();
            if (shareScript == null)
                shareScript = GetComponentInChildren<ShareScript>();
            if (shareScript != null)
            {
                shareScript.gifsAvailable = ToolInfo.isGifTool;
                SetShareScript(shareScript);
            }

            uploadingEditorLogs = false;
            // Called here as script will likely not be active in editor and otherwise may not get updated prior to build. (Calling in constructor also won't work.
            DebugInfoScript.UpdateBuildVersion();
#endif
        }

        protected virtual void Start()
        {
            RefeshCanvasList();

            // Moved here as OnEnable may be called before Awake on the Dispatcher.
            UnityToolbag.Dispatcher.Prepare(gameObject);
        }

        protected virtual void OnEnable()
        {
            UpdateDontDestroyOnLoad();

#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded += OnSceneLoaded;
#endif
//#if UNITY_2021_3_OR_NEWER
//            UnityEngine.Rendering.RenderPipelineManager.activeRenderPipelineTypeChanged += OnRenderPipelineTypeChanged;
//#endif
        }

        protected virtual void OnDisable()
        {
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
//#if UNITY_2021_3_OR_NEWER
//            UnityEngine.Rendering.RenderPipelineManager.activeRenderPipelineTypeChanged -= OnRenderPipelineTypeChanged;
//#endif
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        protected virtual void OnApplicationFocus(bool hasFocus)
        {
            // Fixes an issue where the Dispatcher disappears on recompile.
            UnityToolbag.Dispatcher.Prepare(gameObject);

            Cursor.visible = !hideOriginalMouse;
        }
#endif

        protected virtual void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
#if UNITY_EDITOR
            if (editorWindowMode)
                return;
#endif
            if (FlexibleInput.AnyKey() && !UIStatus.InputFieldFocused())
            {
                bool skipPauseCheck = !Application.isPlaying;
                if (pauseKeySet.MatchesInput() && !skipPauseCheck)
                {
                    if(Time.timeScale == 0) {
                        Time.timeScale = prePauseTimeScale;
                    } else {
                        prePauseTimeScale = Time.timeScale;
                        Time.timeScale = 0;
                    }
                }
                if (previewCutoutKeySet.MatchesInput())
                    cutoutScript.preview = !cutoutScript.preview;
            }
#endif
        }

#region Helpers
#region Set Up
        public void UpdateMouse()
        {
            if (Application.isPlaying)
                Cursor.visible = !hideOriginalMouse;
            else
                Cursor.visible = true;
            if (mouseFollowScript != null)
                mouseFollowScript.gameObject.SetActive(showInGameMouse);
        }

        protected virtual void SetUpDefaultValues()
        {
            prePauseTimeScale = 1f;
#if UNITY_EDITOR
            //Time.timeScale = 1f;
            timeScaleOverride = 1f;
#endif

            if (cameras == null || cameras.Length == 0 || backgroundCamera == null || uiCamera == null)
            {
                Camera[] allCameras = Camera.allCameras.OrderBy(camera => camera.depth).ToArray(); // Stable sort as opposed to tradional Sort()
                if (cameras == null || cameras.Length == 0)
                    cameras = allCameras;
                // Bit of a hack to handle the case where cameras is initialized with UI Camera, but prefab still needs to initialize with scene cameras
                else if (cameras[0] == null)
                    AddAllActiveCameras();

                if(cameras != null && cameras.Length > 0)
                {
                    if (backgroundCamera == null)
                    {
#if UNITY_EDITOR
                        backgroundCameraSelected = false;
#endif
                        backgroundCamera = cameras[0];
                    }
                    if (uiCamera == null)
                        uiCamera = allCameras[cameras.Length - 1];
                }
            }

            if (cutoutScript == null)
                cutoutScript = GetComponentInChildren<CutoutScript>();

            AnyCameraChanged();
        }

        public void AddAllActiveCameras()
        {
            Camera[] allCameras = Camera.allCameras;
            List<Camera> tempCameras = new List<Camera>(cameras);
            for (int i = 0; i < allCameras.Length; ++i)
            {
                Camera camera = allCameras[i];
                if (!tempCameras.Contains(camera))
                    tempCameras.Add(camera);
            }
            cameras = tempCameras.ToArray();
        }
        #endregion

        #region Prepare/Restore Capture
        Color originalBackgroundColor = Color.white;
        CameraClearFlags originalClearFlags = CameraClearFlags.Skybox;
        bool[] originalTempEnabledObjectsState;
        bool[] originalTempDisabledObjectsState;
        float originalTimeScale;

        protected virtual void PrepareToCapture()
        {
            RemoveNullValues();

            if (overrideBackground)
            {
                if (backgroundCamera != null)
                {
                    originalBackgroundColor = backgroundCamera.backgroundColor;
                    originalClearFlags = backgroundCamera.clearFlags;

                    backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
                    backgroundCamera.backgroundColor = backgroundColor;
                }
                else
                    Debug.LogError("Camera containing background must be set for background override to work.");
            }

            if (useCutout && cutoutScript != null)
            {
                originalCutoutChildrenActive = new bool[cutoutScript.transform.childCount];
                for(int i = 0; i < cutoutScript.transform.childCount; ++i)
                {
                    Transform childTransform = cutoutScript.transform.GetChild(i);
                    originalCutoutChildrenActive[i] = childTransform.gameObject.activeSelf;
                }

                originalCutoutAdjustedActive = new bool[cutoutAdjustedRectTransforms.Length];
                originalCutoutAdjustedParents = new Transform[cutoutAdjustedRectTransforms.Length];
                for (int i = 0; i < cutoutAdjustedRectTransforms.Length; ++i)
                {
                    RectTransform cutoutRectTransform = cutoutAdjustedRectTransforms[i];
                    if (cutoutRectTransform != null)
                    {
                        originalCutoutAdjustedParents[i] = cutoutRectTransform.parent;
                        originalCutoutAdjustedActive[i] = cutoutRectTransform.gameObject.activeSelf;
                    }
                    else
                    {
                        originalCutoutAdjustedParents[i] = null;
                        originalCutoutAdjustedActive[i] = false;
                    }
                }
            }

            originalTempEnabledObjectsState = new bool[tempEnabledObjects.Length];
            for (int i = 0; i < tempEnabledObjects.Length; ++i)
            {
                GameObject tempEnabledObject = tempEnabledObjects[i];
                if (tempEnabledObject == null) continue;

                originalTempEnabledObjectsState[i] = tempEnabledObject.activeSelf;
                tempEnabledObject.SetActive(true);
            }

            originalTempDisabledObjectsState = new bool[tempDisabledObjects.Length];
            for (int i = 0; i < tempDisabledObjects.Length; ++i)
            {
                GameObject tempDisabledObject = tempDisabledObjects[i];
                if (tempDisabledObject == null) continue;

                originalTempDisabledObjectsState[i] = tempDisabledObject.activeSelf;
                tempDisabledObject.SetActive(false);
            }

            if (useCanvasesAdjuster)
            {
                if (uiCamera == null)
                    Debug.LogError("Overlay canvases will not display properly without overlay camera set: Fix in Advanced Settings");
                else
                {
                    if (!CanvasesAdjuster.AnyOverlayCanvases())
                        Debug.LogWarning("Autoswitch render mode is only useful for overlay canvases. Keeping it active may negatively alter resulting media: Fix in Advanced Settings");

                    CanvasesAdjuster.ForceCameraRenderMode(uiCamera, overridePlaneDistance, planeDistanceOverride);
                }

                if (cameras.Length <= 0 || !cameras.Contains(uiCamera))
                    Debug.LogError("The UI Camera is not in the list of cameras. It must be to be displayed. It most likely should be the last camera in the array: Fix in Advanced Settings");
                else if(cameras[cameras.Length - 1] != uiCamera)
                    Debug.LogWarning("As UI Camera renders an overly, it most likely should be the last camera in the array: Fix in Advanced Settings");
            }

            if (cutoutScript != null)
                cutoutScript.Hide();
            else if (useCutout)
                Debug.LogError("Cutout Error: Cannot use cutout without CutoutScript set.");

            if (useCutout && cutoutScript != null)
            {
                for (int i = 0; i < cutoutScript.transform.childCount; ++i) {
                    Transform childTransform = cutoutScript.transform.GetChild(i);
                    childTransform.gameObject.SetActive(true);
                }

                for (int i = 0; i < cutoutAdjustedRectTransforms.Length; ++i)
                {
                    RectTransform cutoutRectTransform = cutoutAdjustedRectTransforms[i];
                    if (cutoutRectTransform != null)
                    {
                        cutoutRectTransform.gameObject.SetActive(true);
                        cutoutRectTransform.SetParent(cutoutScript.transform, false);
                    }
                }
            }

            if (stopTimeDuringCapture && Application.isPlaying)
            {
                originalTimeScale = Time.timeScale;
                Time.timeScale = 0;
            }
        }

        protected virtual void RestoreAfterCapture()
        {
            if (overrideBackground && backgroundCamera != null)
            {
                backgroundCamera.clearFlags = originalClearFlags;
                backgroundCamera.backgroundColor = originalBackgroundColor;
            }

            for (int i = 0; i < originalTempEnabledObjectsState.Length; ++i)
                tempEnabledObjects[i].SetActive(originalTempEnabledObjectsState[i]);

            for (int i = 0; i < originalTempDisabledObjectsState.Length; ++i)
                tempDisabledObjects[i].SetActive(originalTempDisabledObjectsState[i]);

            if (useCanvasesAdjuster)
                CanvasesAdjuster.RestoreOriginalRenderModes();

            if (cutoutScript != null)
                cutoutScript.Show();

            if (useCutout && cutoutScript != null)
            {
                for (int i = 0; i < cutoutAdjustedRectTransforms.Length; ++i)
                {
                    RectTransform cutoutRectTransform = cutoutAdjustedRectTransforms[i];
                    if (cutoutRectTransform != null)
                    {
                        bool wasActive = originalCutoutAdjustedActive[i];
                        Transform originalParent = originalCutoutAdjustedParents[i];
                        cutoutRectTransform.SetParent(originalParent, false);
                        cutoutRectTransform.gameObject.SetActive(wasActive);
                    }
                }

                for (int i = 0; i < cutoutScript.transform.childCount; ++i)
                {
                    Transform childTransform = cutoutScript.transform.GetChild(i);
                    childTransform.gameObject.SetActive(originalCutoutChildrenActive[i]);
                }

                originalCutoutChildrenActive = null;
                originalCutoutAdjustedActive = null;
                originalCutoutAdjustedParents = null;
            }

            if (stopTimeDuringCapture && Application.isPlaying)
                Time.timeScale = originalTimeScale;
        }
#endregion

#region Scene Change Updates
#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            NewSceneLoaded();
        }
#else
        void OnLevelWasLoaded(int level)
        {
            NewSceneLoaded();
        }
#endif

        protected virtual void NewSceneLoaded()
        {
            RemoveNullValues();
            //OnRenderPipelineTypeChanged();

            if (camerasNameOrTags == null)
                AnyCameraChanged();

            if (dontDestroyOnLoad)
            {
                CanvasesAdjuster.Setup();

                if (autoUpdateCameras)
                {
                    bool shouldRemoveNullCameras = false;
                    for (int i = 0; i < cameras.Length; ++i)
                    {
                        GameObject cameraObject = autoUpdateCamerasByTag ? GameObject.FindWithTag(camerasNameOrTags[i]) : GameObject.Find(camerasNameOrTags[i]);
                        if (cameraObject != null)
                            cameras[i] = cameraObject.GetComponent<Camera>();
                        else
                            shouldRemoveNullCameras = true;
                    }

                    GameObject backgroundCameraObject = null;
                    GameObject uiCameraObject = null;
                    if (backgroundCameraNameOrTag != null && backgroundCameraNameOrTag.Length > 0)
                        backgroundCameraObject = autoUpdateCamerasByTag ? GameObject.FindWithTag(backgroundCameraNameOrTag) : GameObject.Find(backgroundCameraNameOrTag);
                    if (uiCameraNameOrTag != null && uiCameraNameOrTag.Length > 0)
                        uiCameraObject = autoUpdateCamerasByTag ? GameObject.FindWithTag(uiCameraNameOrTag) : GameObject.Find(uiCameraNameOrTag);

                    if (backgroundCameraObject != null)
                        backgroundCamera = backgroundCameraObject.GetComponent<Camera>();
                    if (uiCameraObject != null)
                        uiCamera = uiCameraObject.GetComponent<Camera>();

                    if (shouldRemoveNullCameras)
                        CleanUpCameraList();
                }
            }
        }

        public void RefeshCanvasList()
        {
            CanvasesAdjuster.Setup();
        }

        //public virtual void OnRenderPipelineTypeChanged()
        //{
        //    UnityEngine.Rendering.RenderPipelineAsset currentRenderPipeline = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
        //    scriptableRenderPipelineActive = currentRenderPipeline != null;
        //    if (scriptableRenderPipelineActive)
        //    {
        //        universalRenderPipelineActive = currentRenderPipeline.GetType().Name.Equals("UniversalRenderPipelineAsset");
        //        hdRenderPipelineActive = currentRenderPipeline.GetType().Name.Equals("HDRenderPipelineAsset");
        //        customRenderPipelineActive = !universalRenderPipelineActive && !hdRenderPipelineActive;
        //    }
        //    builtInRenderPipelineActive = scriptableRenderPipelineActive;
        //}

        public virtual void AnyCameraChanged()
        {
            camerasNameOrTags = new string[cameras.Length];
            for (int i = 0; i < cameras.Length; ++i)
            {
                Camera camera = cameras[i];
                if (camera == null)
                    camerasNameOrTags[i] = "";
                else if (autoUpdateCamerasByTag)
                    camerasNameOrTags[i] = camera.tag;
                else
                    camerasNameOrTags[i] = camera.name;
            }

            if (autoUpdateCamerasByTag)
            {
                backgroundCameraNameOrTag = backgroundCamera != null ? backgroundCamera.tag : "";
                uiCameraNameOrTag = uiCamera != null ? uiCamera.tag : "";
            }
            else
            {
                backgroundCameraNameOrTag = backgroundCamera != null ? backgroundCamera.name : "";
                uiCameraNameOrTag = uiCamera != null ? uiCamera.name : "";
            }
        }

        public virtual void UpdateDontDestroyOnLoad()
        {
            if (!CanvasesAdjuster.automaticallyTrackDontDestroyOnLoadGameObjects)
            {
                if (dontDestroyOnLoad)
                    CanvasesAdjuster.AddDontDestroyOnLoadGameObject(gameObject);
                else
                    CanvasesAdjuster.RemoveDontDestroyOnLoadGameObject(gameObject);
            }
        }

        #region Clean Up Inputs
        public void RemoveNullValues()
        {
            List<GameObject> tempTempEnabledObjects = new List<GameObject>(tempEnabledObjects);
            tempTempEnabledObjects.RemoveAll(tempEnabledObject => tempEnabledObject == null);
            if (tempEnabledObjects.Length != tempTempEnabledObjects.Count)
                tempEnabledObjects = tempTempEnabledObjects.ToArray();

            List<GameObject> tempTempDisabledObjects = new List<GameObject>(tempDisabledObjects);
            tempTempDisabledObjects.RemoveAll(tempDisabledObject => tempDisabledObject == null);
            if (tempDisabledObjects.Length != tempTempDisabledObjects.Count)
                tempDisabledObjects = tempTempDisabledObjects.ToArray();

            CleanUpCameraList();
        }

        protected void CleanUpCameraList()
        {
            List<Camera> tempCameras = new List<Camera>(cameras);
            tempCameras.RemoveAll(camera => camera == null);
            cameras = tempCameras.OrderBy(camera => camera.depth).ToArray(); // Stable sort as opposed to tradional Sort()
            AnyCameraChanged();
        }
        #endregion
        #endregion

        #region Editor Variable Change Updates
        public virtual void CaptureModeChanged()
        {
            AnyCameraChanged();
        }

#if UNITY_EDITOR
        public virtual void BackgroundCameraChanged()
        {
            backgroundCameraSelected = true;
            AnyCameraChanged();
        }

        public void UICameraChanged()
        {
            AnyCameraChanged();
        }

        public void TimeScaleOverrideChanged()
        {
            Time.timeScale = timeScaleOverride;
        }
#endif
#endregion
#endregion
    }
}