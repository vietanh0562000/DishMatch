using UnityEngine;
using System.Collections;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [System.Serializable]
    public class TransformableTexture
    {
        public bool isAsset;
        public bool destroyRawTextureWhenFinalized = true;

        public RawFrameData rawFrameData;
        public Texture2D rawTexture;
        public TextureTransformation[] transformations;

        public Texture2D finalTexture;
        public bool finalized {  get { return finalTexture != null;  } }
        public bool readyToProcess { get { return rawFrameData == null || rawFrameData.readyToProcess; } }

        public TransformableTexture(RawFrameData rawFrameData, TextureTransformation[] transformations)
        {
            this.rawFrameData = rawFrameData;
            this.transformations = transformations;
        }

        public TransformableTexture(Texture2D rawTexture, TextureTransformation[] transformations)
        {
            if(transformations != null && transformations.Length > 0)
            {
                this.rawTexture = rawTexture;
                this.transformations = transformations;
            } else
                this.finalTexture = rawTexture;
        }

        public IEnumerator AsyncProcess()
        {
            while (!readyToProcess) yield return new WaitForEndOfFrame();
            Process();
        }

        public Texture2D Process()
        {
            if (rawTexture != null) return rawTexture;

            if (rawFrameData != null)
            {
                rawTexture = rawFrameData.Process();
                rawFrameData = null;
            }

            return rawTexture;
        }

        public IEnumerator AsyncFinalize()
        {
            while (!readyToProcess) yield return new WaitForEndOfFrame();
            Finalize();
        }

        public Texture2D Finalize()
        {
            if (finalTexture != null) return finalTexture;

            if (rawFrameData == null && rawTexture == null) throw new UnityException("Cannot finalize without either RawFrameData or a raw Texture2D.");

            Process();

            finalTexture = transformations != null && transformations.Length > 0 ? rawTexture.ApplyTransformations(transformations, true) : rawTexture;
            if (destroyRawTextureWhenFinalized && rawTexture != finalTexture) MonoBehaviourExtended.FlexibleDestroy(rawTexture);
            rawTexture = null;

            return finalTexture;
        }

        public long SizeInBytes()
        {
            long bytes = 0;
            if (rawFrameData != null)
                bytes += rawFrameData.SizeInBytes();
            else if (rawTexture != null)
                bytes += rawTexture.SizeInBytes();

            if (finalTexture != null)
                bytes += finalTexture.SizeInBytes();

            return bytes;
        }

        public static long EstimatedSizeInBytes(int rawWidth, int rawHeight, int processedWidth, int processedHeight, int finalWidth, int finalHeight, bool withAlpha, bool processed, bool finalized, bool destroyRawTextureWhenFinalized, RawFrameDataType rawFrameDataType, bool destroyOriginal = true)
        {
            long bytes = 0;
            if (!finalized || !destroyRawTextureWhenFinalized)
                 bytes += RawFrameData.EstimatedSizeInBytes(rawWidth, rawHeight, processedWidth, processedHeight, withAlpha, processed, rawFrameDataType, destroyOriginal);

            if(finalized)
                bytes += Texture2DExtensions.EstimatedSizeInBytes(finalWidth, finalHeight, withAlpha, true);

            return bytes;
        }

        public void UpdateIsAsset()
        {
#if UNITY_EDITOR
            isAsset = (rawTexture != null && UnityEditor.AssetDatabase.Contains(rawTexture)) || (finalTexture != null && UnityEditor.AssetDatabase.Contains(finalTexture));
#endif
        }

        public void Destroy()
        {
            if (rawFrameData != null) rawFrameData.Destroy();
            if (rawTexture != null && !isAsset) MonoBehaviourExtended.FlexibleDestroy(rawTexture);
            if (finalTexture != null && !isAsset) MonoBehaviourExtended.FlexibleDestroy(finalTexture);
        }
    }
}