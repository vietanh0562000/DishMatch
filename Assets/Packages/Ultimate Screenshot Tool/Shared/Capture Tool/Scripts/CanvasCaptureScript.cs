using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool
{
    public class CanvasCaptureScript : MonoBehaviour
    {
        [Tooltip("The rect transform to capture. Must be within Overlay Canvas. It'll be temporarily swapped over to this canvas and swapped back. The canvas this rect transform is in currently and the destination canvas should match as much as possible. Especially components like CanvasScaler.")]
        public RectTransform rectTransformToCapture;

        [Tooltip("A camera to render the canvas on. It'll be configured automatically. This camera should be used for nothing else.")]
        public Camera canvasCamera;
        [Tooltip("The canvas to capture. It'll be configured automatically. This canvas should be used for nothing else.")]
        public Canvas canvas;

        private void Start()
        {
            if (canvasCamera == null) canvasCamera = GetComponent<Camera>();
            if (canvas == null) canvas = GetComponent<Canvas>();

            SetUp();
        }

        // Status: Working
        // Method: Transfer rect transform to otherwise empty canvas while preserving screen size.
        //         Keep the rect transform as is and use cutout to capture the same area.
        //
        // Pros:   Matches exactly what's on screen.
        // Cons:   No size input means it may need to be resized.

        public Texture2D ExactCapture()
        {
            canvasCamera.enabled = true;
            Rect originalRect = rectTransformToCapture.RectForCurrentResolution();
            Transform originalRectTransformToCaptureParent = rectTransformToCapture.parent;
            rectTransformToCapture.SetParent(canvas.transform, true);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = canvasCamera;

            RawFrameData rawFrameData = CaptureRawFrame.CameraRenderTexture(canvasCamera, originalRect, ResolutionExtensions.EmptyResolution(), false);
            Texture2D capturedTexture = rawFrameData.Process();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rectTransformToCapture.SetParent(originalRectTransformToCaptureParent, true);
            canvasCamera.enabled = false;

            return capturedTexture;
        }

        // Status: Generally working - positioning a bit different
        // Method: Transfer rect transform to otherwise empty canvas without preserving screen size.
        //         
        //
        // Pros:   Doesn't exactly match what's on screen.
        // Cons:   Captures the right resolution, so it doesn't need to be scaled.
        //
        public Texture2D Capture(Resolution captureResolution)
        {
            canvasCamera.enabled = true;
            Transform originalRectTransformToCaptureParent = rectTransformToCapture.parent;
            rectTransformToCapture.SetParent(canvas.transform, false);
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = canvasCamera;

            Rect captureRect = new Rect(0, 0, captureResolution.width, captureResolution.height);
            RawFrameData rawFrameData = CaptureRawFrame.CameraRenderTexture(captureResolution, canvasCamera, captureRect, ResolutionExtensions.EmptyResolution(), false);
            Texture2D capturedTexture = rawFrameData.Process();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            rectTransformToCapture.SetParent(originalRectTransformToCaptureParent, false);
            canvasCamera.enabled = false;

            return capturedTexture;
        }

        // Try to ensure we only get this canvas while also allowing a transparent background.
        void SetUp()
        {
            float distance = 1f;
            canvasCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
            canvasCamera.clearFlags = CameraClearFlags.Nothing;
            canvasCamera.farClipPlane = distance * 1.1f;

            canvas.gameObject.layer = LayerMask.NameToLayer("UI");
            canvas.planeDistance = distance;
        }
    }
}