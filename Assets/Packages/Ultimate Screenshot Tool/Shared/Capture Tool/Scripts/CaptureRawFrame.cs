using UnityEngine;
#if UNITY_2019_1_OR_NEWER
using Unity.Collections;
using UnityEngine.Rendering;
#endif
using System.Collections.Generic;

namespace TRS.CaptureTool
{
    /** Note these methods require the rect to be set with a width and height. */
    public static class CaptureRawFrame
    {
#if UNITY_2019_1_OR_NEWER
        public static RawFrameData AsyncCapture(Resolution captureResolution, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            return new AsyncRawFrameData(captureResolution, rect, resizeResolution, solidify, renderTextureFormat);
        }
#endif

        public static RawFrameData ScreenCapture(Rect rect, Resolution resizeResolution, bool solidify, TextureFormat textureFormat = TextureFormat.ARGB32)
        {
            textureFormat = (solidify && (textureFormat == TextureFormat.ARGB32 || textureFormat == TextureFormat.RGBA32)) ? TextureFormat.RGB24 : textureFormat;
            Texture2D screenCapture = new Texture2D((int)rect.width, (int)rect.height, textureFormat, false);
            screenCapture.ReadPixels(rect, 0, 0, false);

            Rect fullScreenCaptureRect = new Rect(0, 0, screenCapture.width, screenCapture.height);
            return new TextureRawFrameData(screenCapture, fullScreenCaptureRect, resizeResolution, solidify);
        }

        public static RawFrameData AltScreenCapture(Rect rect, Resolution resizeResolution, bool solidify, ScreenCapture.StereoScreenCaptureMode stereoCaptureMode)
        {
            return new TextureRawFrameData(UnityEngine.ScreenCapture.CaptureScreenshotAsTexture(stereoCaptureMode), rect, resizeResolution, solidify);
        }

        public static RawFrameData AltScreenCapture(Rect rect, Resolution resizeResolution, bool solidify, int scale = 1)
        {
            return new TextureRawFrameData(UnityEngine.ScreenCapture.CaptureScreenshotAsTexture(scale), rect, resizeResolution, solidify);
        }

        public static RawFrameData CameraRenderTexture(Camera camera, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            Resolution resolution = new Resolution { width = camera.pixelWidth, height = camera.pixelHeight };
            return CameraRenderTexture(resolution, camera, rect, resizeResolution, solidify, renderTextureFormat);
        }

        public static RawFrameData CameraRenderTexture(Resolution resolution, Camera camera, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            RenderTexture renderTexture = RenderTextureExtensions.TempCameraRenderTexture(camera, resolution.width, resolution.height, 0, renderTextureFormat);

            TextureFormat textureFormat = Texture2DExtensions.TextureFormatForRenderTextureFormat(renderTexture.format);
            Texture2D resultTexture = new Texture2D((int)rect.width, (int)rect.height, textureFormat, false);
            RenderTexture originalRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            resultTexture.ReadPixels(rect, 0, 0, false);
            RenderTexture.active = originalRenderTexture;

            RenderTexture.ReleaseTemporary(renderTexture);

            Rect fullScreenCaptureRect = new Rect(0, 0, resultTexture.width, resultTexture.height);
            return new TextureRawFrameData(resultTexture, fullScreenCaptureRect, resizeResolution, solidify);
        }

        public static RawFrameData CamerasRenderTexture(Camera[] cameras, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            Resolution resolution = new Resolution { width = Camera.main.pixelWidth, height = Camera.main.pixelHeight };
            return CamerasRenderTexture(resolution, cameras, rect, resizeResolution, solidify, renderTextureFormat);
        }

        public static RawFrameData CamerasRenderTexture(Resolution resolution, Camera[] cameras, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            RenderTexture renderTexture = RenderTextureExtensions.TempCamerasRenderTexture(cameras, resolution.width, resolution.height, 0, renderTextureFormat);

            TextureFormat textureFormat = Texture2DExtensions.TextureFormatForRenderTextureFormat(renderTexture.format);
            Texture2D resultTexture = new Texture2D((int)rect.width, (int)rect.height, textureFormat, false);
            RenderTexture originalRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            resultTexture.ReadPixels(rect, 0, 0, false);
            RenderTexture.active = originalRenderTexture;

            RenderTexture.ReleaseTemporary(renderTexture);

            Rect fullScreenCaptureRect = new Rect(0, 0, resultTexture.width, resultTexture.height);
            return new TextureRawFrameData(resultTexture, fullScreenCaptureRect, resizeResolution, solidify);
        }

        public static RawFrameData AllCamerasRenderTexture(Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            Resolution resolution = new Resolution { width = Camera.main.pixelWidth, height = Camera.main.pixelHeight };
            return AllCamerasRenderTexture(resolution, rect, resizeResolution, solidify, renderTextureFormat);
        }

        public static RawFrameData AllCamerasRenderTexture(Resolution resolution, Rect rect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32)
        {
            Camera[] allCameras = Camera.allCameras;
            List<Camera> allActiveCameras = new List<Camera>(allCameras.Length);
            for (int i = 0; i < allCameras.Length; ++i)
            {
                Camera camera = allCameras[0];
                if (camera.isActiveAndEnabled && camera.cameraType != CameraType.SceneView)
                    allActiveCameras.Add(camera);
            }

            return CamerasRenderTexture(resolution, allActiveCameras.ToArray(), rect, resizeResolution, solidify, renderTextureFormat);
        }
    }
}