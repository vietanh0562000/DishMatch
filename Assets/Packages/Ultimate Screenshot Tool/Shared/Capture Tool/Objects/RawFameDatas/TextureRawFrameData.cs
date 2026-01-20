using UnityEngine;
using TRS.CaptureTool.Extras;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class TextureRawFrameData : RawFrameData
    {
        [SerializeField]
        Texture2D rawTexture;

        public TextureRawFrameData(Texture2D rawTexture, Rect cutout, Resolution resizeResolution, bool solidify) : base(cutout, resizeResolution, solidify)
        {
            this.rawTexture = rawTexture;
        }

        public override Texture2D Process(bool apply = true)
        {
            if (processed) return processedTexture;

            if (rawTexture != null)
                processedTexture = ProcessRawTexture();
            else
                // If you're hitting this error, you may be deleting the processedTexture elsewhere or using a destroyed RawDataFrame.
                throw new UnityException("Attempting to process frame, but TextureRawFrameData has no RenderTexture.");

            if (resizeResolution.HasSize())
                processedTexture = processedTexture.Resize(resizeResolution);
            if (apply) processedTexture.Apply(false);
            return processedTexture;
        }

        Texture2D ProcessRawTexture()
        {
            Texture2D resultTexture = rawTexture.Cutout(cutout, false);
            if (!solidify || resultTexture.format == TextureFormat.RGB24)
            {
                if (rawTexture != resultTexture && destroyOriginal)
                    MonoBehaviourExtended.FlexibleDestroy(rawTexture);

                if (destroyOriginal) rawTexture = null;
                return resultTexture;
            }

            Texture2D solidTexture;
            if (resultTexture.format == TextureFormat.ARGB32 || resultTexture.format == TextureFormat.RGBA32)
            {
                solidTexture = new Texture2D(resultTexture.width, resultTexture.height, TextureFormat.RGB24, false);
                solidTexture.SetPixels(resultTexture.GetPixels());
            }
            else
                solidTexture = resultTexture.Solidify();

            if (destroyOriginal)
            {
                if(solidTexture != resultTexture)
                    MonoBehaviourExtended.FlexibleDestroy(resultTexture);
                rawTexture = null;
            }
            return solidTexture;
        }

        public override long SizeInBytes()
        {
            long bytes = base.SizeInBytes();
            if (rawTexture != null)
                bytes += rawTexture.SizeInBytes();
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

            if (rawTexture != null)
            {
                MonoBehaviourExtended.FlexibleDestroy(rawTexture);
                rawTexture = null;
            }
        }
    }
}
