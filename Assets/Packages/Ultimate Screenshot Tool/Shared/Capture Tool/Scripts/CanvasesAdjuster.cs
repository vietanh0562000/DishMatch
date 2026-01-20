using System.Collections.Generic;

using UnityEngine;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

namespace TRS.CaptureTool
{
    public static class CanvasesAdjuster
    {
        static bool ranSetup;
        static Canvas[] allCanvases;
        static List<GameObject> dontDestroyOnLoadGameObjects = new List<GameObject>();
        public static bool automaticallyTrackDontDestroyOnLoadGameObjects;

        static Camera camera;
        static bool cameraWasEnabled;
        static bool[] canvasModified;
        static bool didOverridePlaneDistance;
        static Camera[] originalRenderCameras;
        static float[] originalCanvasPlaneDistances;

        public static void AddDontDestroyOnLoadGameObject(GameObject gameObject)
        {
            if (!dontDestroyOnLoadGameObjects.Contains(gameObject))
            {
                dontDestroyOnLoadGameObjects.Add(gameObject);
                Setup();
            }
        }

        public static void RemoveDontDestroyOnLoadGameObject(GameObject gameObject)
        {
            bool removedGameObject = dontDestroyOnLoadGameObjects.Remove(gameObject);
            if (removedGameObject)
                Setup();
        }

        public static void SetupIfNecessary()
        {
            bool setupRequired = false;
            if (allCanvases == null || !ranSetup)
                setupRequired = true;
            else
            {
                for(int i = 0; i < allCanvases.Length; ++i)
                {
                    Canvas canvas = allCanvases[i];
                    if (canvas == null)
                    {
                        setupRequired = true;
                        break;
                    }
                }
            }

            if (setupRequired)
                Setup();
        }

        public static void Setup()
        {
#if UNITY_5_4_OR_NEWER
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.isLoaded)
            {
                ranSetup = false;
                return;
            }
            List<GameObject> rootGameObjects = new List<GameObject>(scene.GetRootGameObjects());
#else
            automaticallyTrackDontDestroyOnLoadGameObjects = true;
            List<GameObject> rootGameObjects = new List<GameObject>();
#endif

            // Find DonDestroyOnLoad gameobjects as well (scene.GetRootGameObjects() does not include them) or all if on older version
            if (automaticallyTrackDontDestroyOnLoadGameObjects)
            {
                Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
                for (int i = 0; i < allTransforms.Length; ++i)
                {
                    Transform rootTransform = allTransforms[i].root;
                    if (rootTransform.hideFlags == HideFlags.None && !rootGameObjects.Contains(rootTransform.gameObject))
                        rootGameObjects.Add(rootTransform.gameObject);
                }
            }
            else
            {
                dontDestroyOnLoadGameObjects.RemoveAll(dontDestroyOnLoadGameObject => dontDestroyOnLoadGameObject == null);
                rootGameObjects.AddRange(dontDestroyOnLoadGameObjects);
            }

            List<Canvas> allCanvasesList = new List<Canvas>(rootGameObjects.Count);
            for (int i = 0; i < rootGameObjects.Count; ++i)
            {
                GameObject rootGameObject = rootGameObjects[i];
                allCanvasesList.AddRange(rootGameObject.GetComponentsInChildren<Canvas>(true));
            }
            allCanvases = allCanvasesList.ToArray();

            ranSetup = true;
        }

        public static bool AnyOverlayCanvases()
        {
            SetupIfNecessary();

            for (int i = 0; i < allCanvases.Length; ++i)
            {
                Canvas canvas = allCanvases[i];
                if (canvas.gameObject.activeInHierarchy && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                    return true;
            }

            return false;
        }

        public static void ForceCameraRenderMode(Camera camera, bool overridePlaneDistance = false, float planeDistanceOverride = 0f)
        {
            SetupIfNecessary();

            CanvasesAdjuster.camera = camera;
            cameraWasEnabled = camera.enabled;
            didOverridePlaneDistance = overridePlaneDistance;

            camera.enabled = true;
            canvasModified = new bool[allCanvases.Length];
            originalRenderCameras = new Camera[allCanvases.Length];
            originalCanvasPlaneDistances = new float[allCanvases.Length];
            for (int i = allCanvases.Length - 1; i >= 0; --i)
            {
                if (allCanvases[i].renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvasModified[i] = true;
                    originalRenderCameras[i] = allCanvases[i].worldCamera;
                    originalCanvasPlaneDistances[i] = allCanvases[i].planeDistance;

                    // Order of operations is apparently important here
                    allCanvases[i].renderMode = RenderMode.ScreenSpaceCamera;
                    if (overridePlaneDistance)
                        allCanvases[i].planeDistance = planeDistanceOverride != 0f ? planeDistanceOverride : camera.nearClipPlane * 1.1f;
                    allCanvases[i].worldCamera = camera;
                }
            }
        }

        public static void RestoreOriginalRenderModes()
        {
            for (int i = 0; i < allCanvases.Length; ++i)
            {
                if (canvasModified[i])
                {
                    allCanvases[i].worldCamera = originalRenderCameras[i];
                    if (didOverridePlaneDistance)
                        allCanvases[i].planeDistance = originalCanvasPlaneDistances[i];
                    allCanvases[i].renderMode = RenderMode.ScreenSpaceOverlay;
                }
            }

            camera.enabled = cameraWasEnabled;
        }
    }
}