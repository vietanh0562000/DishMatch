using UnityEngine;

namespace TRS.CaptureTool
{
    /** Disables screenshot resolutions that aren't a similar aspect ratio as the current resolution at launch time. */
    public class SimilarScreenshotResolutionsOnlyScript : MonoBehaviour
    {
        public ScreenshotScript screenshotScript;

        public float minWideScreenCutoff = 1.51f;

        void Awake()
        {
            if (screenshotScript == null)
                screenshotScript = GetComponent<ScreenshotScript>();
        }

        void Start()
        {
            UpdateResolutions();
        }

        public void UpdateResolutions()
        {
            Resolution screenResolution = ScreenExtensions.CurrentResolution();
            float screenAspectRatio = (float)screenResolution.width / (float)screenResolution.height;
            foreach (ScreenshotResolution resolution in screenshotScript.screenshotResolutions)
            {
                if (screenResolution.width == screenResolution.height)
                    resolution.active = resolution.width == resolution.height;
                else if (screenResolution.width < screenResolution.height)
                    resolution.active = resolution.width < resolution.height;
                else
                {
                    float aspectRatio = (float)resolution.width / (float)resolution.height;
                    if (screenAspectRatio >= minWideScreenCutoff)
                        resolution.active = aspectRatio > minWideScreenCutoff;
                    else
                        resolution.active = 1.0f < aspectRatio && aspectRatio < minWideScreenCutoff;
                }
            }
        }
    }
}