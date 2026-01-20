using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public enum RawFrameDataType
    {
        RawTexture,
        ColorArray,
        RenderTexture,
    }

    [System.Serializable]
    public class RawFrameData
    {
        public bool readyToProcess = true;

        public bool destroyOriginal = true;

        [SerializeField]
        public Texture2D processedTexture;

        public bool processed { get { return processedTexture != null; } }

        protected Rect cutout;
        protected bool solidify;
        protected Resolution resizeResolution;

        protected RawFrameData(Rect cutout, Resolution resizeResolution, bool solidify)
        {
            this.cutout = cutout;
            this.solidify = solidify;
            this.resizeResolution = resizeResolution;
        }

        public RawFrameData(Texture2D processedTexture)
        {
            this.processedTexture = processedTexture;
        }

        public virtual Texture2D Process(bool apply = true)
        {
            return processedTexture;
        }

        public virtual long SizeInBytes()
        {
            long bytes = 0;
            if (processedTexture != null)
                bytes += processedTexture.SizeInBytes();

            return bytes;
        }

        public virtual void Destroy()
        {
            MonoBehaviourExtended.FlexibleDestroy(processedTexture);
            processedTexture = null;
        }

        public static long EstimatedSizeInBytes(int rawWidth, int rawHeight, int processedWidth, int processedHeight, bool withAlpha, bool processed, RawFrameDataType rawFrameDataType, bool destroyOriginal = true)
        {
            long bytes = 0;
            if (processed)
            {
                bytes += Texture2DExtensions.EstimatedSizeInBytes(processedWidth, processedHeight, withAlpha, true);
                if (destroyOriginal)
                    return bytes;
            }

            if (rawFrameDataType == RawFrameDataType.ColorArray)
                bytes += Color32Extensions.EstimatedSizeInBytes(rawWidth, rawHeight);
            else if (rawFrameDataType == RawFrameDataType.RawTexture)
                bytes += Texture2DExtensions.EstimatedSizeInBytes(rawWidth, rawHeight, withAlpha, true);
            else if (rawFrameDataType == RawFrameDataType.RenderTexture)
                bytes += RenderTextureExtensions.EstimatedSizeInBytes(rawWidth, rawHeight);
            return bytes;
        }
    }
}