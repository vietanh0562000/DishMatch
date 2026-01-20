
using UnityEngine;

namespace TRS.CaptureTool
{
    public static class RenderTextureExtensions
    {
        // https://docs.unity3d.com/ScriptReference/Camera.Render.html
        public static RenderTexture TempCameraRenderTexture(Camera camera, int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
            return RenderCameraToRenderTexture(camera, renderTexture);
        }

        public static RenderTexture CameraRenderTexture(Camera camera, int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, depthBuffer, format);
            return RenderCameraToRenderTexture(camera, renderTexture);
        }

        public static RenderTexture RenderCameraToRenderTexture(Camera camera, RenderTexture renderTexture)
        {
            RenderTexture originalTargetTexture = camera.targetTexture;
            camera.targetTexture = renderTexture;
            camera.Render();
            camera.targetTexture = originalTargetTexture;

            return renderTexture;
        }

        public static RenderTexture TempCamerasRenderTexture(Camera[] cameras, int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default, RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default, int antiAliasing = 1, RenderTextureMemoryless memorylessMode = RenderTextureMemoryless.None, VRTextureUsage vrUsage = VRTextureUsage.None, bool useDynamicScale = false)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, depthBuffer, format, readWrite, antiAliasing, memorylessMode, vrUsage, useDynamicScale);
            return RenderCamerasToRenderTexture(cameras, renderTexture);
        }

        public static RenderTexture CamerasRenderTexture(Camera[] cameras, int width, int height, int depthBuffer = 0, RenderTextureFormat format = RenderTextureFormat.Default)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, depthBuffer, format);
            return RenderCamerasToRenderTexture(cameras, renderTexture);
        }

        public static RenderTexture RenderCamerasToRenderTexture(Camera[] cameras, RenderTexture renderTexture)
        {
            for (int i = 0; i < cameras.Length; ++i)
            {
                Camera camera = cameras[i];
                RenderTexture originalCameraTargetTexture = camera.targetTexture;
                camera.targetTexture = renderTexture;
                camera.Render();
                camera.targetTexture = originalCameraTargetTexture;
            }

            return renderTexture;
        }

        public static RenderTextureFormat RenderTextureFormatForTextureFormat(TextureFormat textureFormat)
        {
            switch (textureFormat)
            {
                case TextureFormat.RGBAFloat:
                    return RenderTextureFormat.ARGBFloat;
                case TextureFormat.RGBAHalf:
                    return RenderTextureFormat.ARGBHalf;
                case TextureFormat.RGBA64:
                    return RenderTextureFormat.ARGBInt;
                case TextureFormat.RGBA32:
                case TextureFormat.ARGB32:
                    return RenderTextureFormat.ARGB32;
                default:
                    Debug.LogError("No RenderTextureFormat Found for TextureFormat: " + textureFormat);
                    return RenderTextureFormat.ARGB32;
            }
        }

        public static int BitsPerPixelForFormat(RenderTextureFormat format)
        {
            switch (format)
            {
                case RenderTextureFormat.ARGB32:
                    return 32;
                case RenderTextureFormat.BGRA32:
                    return 32;
                case RenderTextureFormat.ARGBHalf:
                    return 64;
                case RenderTextureFormat.RGB565:
                    return 16;
                case RenderTextureFormat.ARGB4444:
                    return 16;
                case RenderTextureFormat.ARGB1555:
                    return 16;
                case RenderTextureFormat.ARGB2101010:
                    return 32;
                case RenderTextureFormat.ARGB64:
                    return 64;
                case RenderTextureFormat.ARGBFloat:
                    return 128;
                case RenderTextureFormat.ARGBInt:
                    return 128;
                case RenderTextureFormat.RGB111110Float:
                    return 32;
                case RenderTextureFormat.RGBAUShort:
                    return 64;
                case RenderTextureFormat.R8:
                    return 8;
                case RenderTextureFormat.R16:
                    return 16;
                case RenderTextureFormat.RHalf:
                    return 16;
                case RenderTextureFormat.RFloat:
                    return 32;
                case RenderTextureFormat.RInt:
                    return 32;
                case RenderTextureFormat.RG16:
                    return 16;
                case RenderTextureFormat.RG32:
                    return 32;
                case RenderTextureFormat.RGHalf:
                    return 32;
                case RenderTextureFormat.RGFloat:
                    return 64;
                case RenderTextureFormat.RGInt:
                    return 64;
                default:
                    return -1;
            }
        }

        // This method may not be accurate in all cases. It should be accurate while in editor. MipMaps seem fairly inconsistent.
        public static long SizeInBytes(this RenderTexture texture)
        {
#if UNITY_EDITOR
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture);
#else
            long calculatedTextureSize;
            long bitsPerPixel = BitsPerPixelForFormat(texture.format);
            if (texture.mipmapCount > 1)
            {
                // Do not round to nearest word length when using mip maps.
                long bytesPerPixel = (bitsPerPixel + texture.depth) / 8;
                calculatedTextureSize = (long)((float)texture.width * (float)texture.height * (float)bytesPerPixel * 1.33333333333f);
            }
            else
            {
                long bytesPerPixel = Mathf.CeilToInt((float)(bitsPerPixel + texture.depth) / 16.0f) * 2;
                calculatedTextureSize = texture.width * texture.height * bytesPerPixel;
            }

#if UNITY_EDITOR
            // Debugging code used to test this method.
            long profilerSize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(texture);
            if (calculatedTextureSize < profilerSize) {
                string debugOutput = "Render texture with size: " + texture.width + "x" + texture.height + " format: " + texture.format + " mipmap count: " + texture.mipmapCount +  " has profiler size: " + profilerSize + " (" + (profilerSize / (texture.width * texture.height)) + " bpp) vs. calculated size: " + calculatedTextureSize + " (" + (calculatedTextureSize / (texture.width * texture.height)) + " bpp)";
                Debug.LogError("Render texture size calculation is underestimated. Please contact with debug output:  " + debugOutput);
            }
            //if (calculatedTextureSize != profilerSize)
            //    Debug.LogError("Render texture size calculation is incorrect.");
#endif

            return calculatedTextureSize;
#endif
        }

        public static long EstimatedSizeInBytes(int width, int height)
        {
            return width * height * (BitsPerPixelForFormat(RenderTextureFormat.ARGB32) / 8);
        }
    }
}