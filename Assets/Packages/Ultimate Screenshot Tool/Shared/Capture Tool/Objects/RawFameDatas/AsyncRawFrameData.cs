using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using Unity.Collections;
using UnityEngine.Rendering;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public class AsyncRawFrameData : RawFrameData
    {
        [SerializeField]
        RenderTexture renderTexture;
        [SerializeField]
        Texture2D rawTexture;

#if UNITY_2019_1_OR_NEWER
        // Full size cutout passed to base init as cutout is already applied here.
        public AsyncRawFrameData(Resolution captureResolution, Rect cutoutRect, Resolution resizeResolution, bool solidify, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32) : base(new Rect(0, 0, cutoutRect.width, cutoutRect.height), resizeResolution, solidify)
        {
            readyToProcess = false;

            Resolution screenResolution = ScreenExtensions.CurrentResolution();
            // Temp render texture used here, so that a flip may be applied before the readback request.
            RenderTexture tempRenderTexture = RenderTexture.GetTemporary(screenResolution.width, screenResolution.height, 0, renderTextureFormat);
            UnityEngine.ScreenCapture.CaptureScreenshotIntoRenderTexture(tempRenderTexture);

            renderTexture = new RenderTexture(captureResolution.width, captureResolution.height, 0, renderTextureFormat);
            Graphics.Blit(tempRenderTexture, renderTexture, new Vector2(1f, -1f), new Vector2(0.0f, 1f));

            RenderTexture.ReleaseTemporary(tempRenderTexture);

            TextureFormat textureFormat = Texture2DExtensions.TextureFormatForRenderTextureFormat(renderTexture.format);
            textureFormat = (solidify && (textureFormat == TextureFormat.ARGB32 || textureFormat == TextureFormat.RGBA32)) ? TextureFormat.RGB24 : textureFormat;
            AsyncGPUReadback.Request(renderTexture, 0, (int)cutoutRect.x, (int)cutoutRect.width, (int)cutoutRect.y, (int)cutoutRect.height, 0, 1, textureFormat, this.AsyncGPUReadbackRequestCallback);
        }

        // Updates request, but does not apply resize resolution yet.
        void AsyncGPUReadbackRequestCallback(AsyncGPUReadbackRequest readbackRequest)
        {
            if (renderTexture == null) return;
            if (readbackRequest.hasError)
            {
                Debug.LogError("Async readback error");
                MonoBehaviourExtended.FlexibleDestroy(renderTexture);
                return;
            }
            TextureFormat textureFormat = Texture2DExtensions.TextureFormatForRenderTextureFormat(renderTexture.format);
            textureFormat = (solidify && (textureFormat == TextureFormat.ARGB32 || textureFormat == TextureFormat.RGBA32)) ? TextureFormat.RGB24 : textureFormat;

            bool needsToSolidify = solidify && textureFormat != TextureFormat.RGB24;
            rawTexture = new Texture2D(readbackRequest.width, readbackRequest.height, textureFormat, false);
            rawTexture.LoadRawTextureData(readbackRequest.GetData<byte>());
            if (needsToSolidify) rawTexture = rawTexture.Solidify();

            MonoBehaviourExtended.FlexibleDestroy(renderTexture);
            readyToProcess = true;
        }
#endif

        public override Texture2D Process(bool apply = true)
        {
            if (processed) return processedTexture;

            if (rawTexture != null)
            {
                processedTexture = rawTexture;
                if(destroyOriginal) rawTexture = null;
            }
            else
                // If you're hitting this error, you may be deleting the processedTexture elsewhere or using a destroyed RawDataFrame.
                throw new UnityException("Attempting to process frame, but TextureRawFrameData has no RenderTexture.");

            if (resizeResolution.HasSize())
                processedTexture = processedTexture.Resize(resizeResolution);
            if (apply) processedTexture.Apply(false);
            return processedTexture;
        }

        public override long SizeInBytes()
        {
            long bytes = base.SizeInBytes();
            if (renderTexture != null)
                bytes += renderTexture.SizeInBytes();
            return bytes;
        }

        public override void Destroy()
        {
            // Don't call base.Destroy() or the original rawTexture could be unintentionally destroyed.
            if (processedTexture != null && (destroyOriginal || processedTexture != rawTexture))
            {
                MonoBehaviourExtended.FlexibleDestroy(processedTexture);
                processedTexture = null;
            }

            if (!destroyOriginal) return;

            if (renderTexture != null)
            {
                MonoBehaviourExtended.FlexibleDestroy(renderTexture);
                renderTexture = null;
            }
        }
    }
}