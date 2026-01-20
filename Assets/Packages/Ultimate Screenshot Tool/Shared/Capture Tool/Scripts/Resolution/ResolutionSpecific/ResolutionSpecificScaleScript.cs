using UnityEngine;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ResolutionScalePair
    {
        public Vector2 resolution;
        public Vector3 scale;

        public ResolutionScalePair(Vector2 resolution, Vector3 scale)
        {
            this.resolution = resolution;
            this.scale = scale;
        }
    }

    [ExecuteInEditMode]
    public class ResolutionSpecificScaleScript : ResolutionSpecificScript
    {
        public ResolutionScalePair[] resolutionScalePairs;

        public override void UpdateForResolution(int width, int height)
        {
            if (!ValidObject()) return;

            if (resolutionScalePairs == null) return;

            foreach (ResolutionScalePair resolutionScalePair in resolutionScalePairs)
            {
                if (resolutionScalePair.resolution.x == width && resolutionScalePair.resolution.y == height)
                {
                    gameObject.transform.localScale = resolutionScalePair.scale;
                    break;
                }
            }
        }
    }
}