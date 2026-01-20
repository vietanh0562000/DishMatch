using UnityEngine;

namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    public class ResolutionSpecificActivationScript : ResolutionSpecificScript
    {
        public Vector2[] activationResolutions;

        public override void UpdateForResolution(int width, int height)
        {
            if (!ValidObject()) return;

            if (activationResolutions == null) return;

            bool shouldBeActive = false;
            foreach(Vector2 resolution in activationResolutions)
            {
                if(resolution.x == width && resolution.y == height)
                {
                    shouldBeActive = true;
                    break;
                }
            }
            gameObject.SetActive(shouldBeActive);
        }
    }
}