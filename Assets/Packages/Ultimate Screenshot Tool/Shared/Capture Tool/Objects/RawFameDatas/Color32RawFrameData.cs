using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class Color32RawFrameData : RawFrameData
    {
        [SerializeField]
        Color32[] colors;

        // Preserve dimensions of colors array.
        [SerializeField]
        Resolution recordResolution;

        public Color32RawFrameData(Color32[] colors, Resolution recordResolution, Rect cutout, Resolution resizeResolution, bool solidify) : base(cutout, resizeResolution, solidify)
        {
            this.colors = colors;
            this.recordResolution = recordResolution;
        }

        public override Texture2D Process(bool apply = true)
        {
            if (processed) return processedTexture;

            if (colors != null && colors.Length > 0)
                processedTexture = ProcessColor32Array();
            else
                // If you're hitting this error, you may be deleting the processedTexture elsewhere or using a destroyed RawDataFrame.
                throw new UnityException("Attempting to process frame, but RenderTextureRawFrameData has no RenderTexture.");

            if (resizeResolution.HasSize())
                processedTexture = processedTexture.Resize(resizeResolution);
            if (apply) processedTexture.Apply(false);
            return processedTexture;
        }

        Texture2D ProcessColor32Array()
        {
            Texture2D resultTexture = new Texture2D(recordResolution.width, recordResolution.height, solidify ? TextureFormat.RGB24 : TextureFormat.ARGB32, false);
            resultTexture.SetPixels32(colors);
            resultTexture = resultTexture.Cutout(cutout, false);
            colors = null;

            return resultTexture;
        }

        public override long SizeInBytes()
        {
            long bytes = base.SizeInBytes();
            if (colors != null)
                bytes += colors.SizeInBytes();
            return bytes;
        }

        public override void Destroy()
        {
            base.Destroy();

            colors = null;
        }
    }
}
