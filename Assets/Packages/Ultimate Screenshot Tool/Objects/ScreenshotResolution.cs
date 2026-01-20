using UnityEngine;
using System.Collections.Generic;

namespace TRS.CaptureTool
{
    [System.Serializable]
    public class ScreenshotResolutionSet
    {
        public string name;
        public List<ScreenshotResolution> screenshotResolutions;

        public ScreenshotResolutionSet(string name, List<ScreenshotResolution> screenshotResolutions)
        {
            this.name = name;
            this.screenshotResolutions = screenshotResolutions;
        }
    }

    [System.Serializable]
    public class ScreenshotResolution
    {
        public bool active;
        public string name;
        public int width;
        public int height;
        public int scale;
        public float delay;
        public bool waitForUpdates;
        public Resolution unscaledResolution
        {
            get
            {
                return new Resolution { width = width, height = height };
            }
        }

        public Resolution scaledResolution
        {
            get
            {
                return new Resolution { width = width * scale, height = height * scale };
            }
        }

        public string resolutionString { get { return width * scale + "x" + height * scale; } }

        public ScreenshotResolution(bool active, string name, Resolution resolution, int scale = 1, float delay = 0f, bool waitForFrame = true) : this(active, name, resolution.width, resolution.height, scale, delay, waitForFrame) { }
        public ScreenshotResolution(bool active, string name, int width, int height, int scale = 1, float delay = 0f, bool waitForFrame = true)
        {
            this.active = active;
            this.name = name;
            this.width = width;
            this.height = height;
            this.scale = scale;
            this.delay = delay;
            this.waitForUpdates = waitForFrame;
        }

        public void ScaleToScreen()
        {
            Resolution screenResolution = Screen.currentResolution;
            bool requiresScaling = width > screenResolution.width || height > screenResolution.height;
            if (!requiresScaling)
                return;

            int newScale = 1;
            int newWidth = width;
            int newHeight = height;
            do
            {
                if (newWidth % 2 == 0 && newHeight % 2 == 0)
                {
                    newWidth /= 2;
                    newHeight /= 2;
                    newScale *= 2;
                }
                else
                    break;
            } while (newWidth > screenResolution.width || newHeight > screenResolution.height);

            int maxDimension = Mathf.Max(newWidth, newHeight);

            int maybePrime = 3;
            while (maybePrime * maybePrime <= maxDimension && (newWidth > screenResolution.width || newHeight > screenResolution.height))
            {
                if (newWidth % maybePrime == 0 && newHeight % maybePrime == 0)
                {
                    newWidth /= maybePrime;
                    newHeight /= maybePrime;
                    newScale *= maybePrime;
                }
                else
                    maybePrime += 2;
            }

            bool scaleFound = maybePrime * maybePrime <= maxDimension;
            if (scaleFound)
            {
                scale = newScale;
                width = newWidth;
                height = newHeight;
            }
#if UNITY_EDITOR
            else
                Debug.LogError(width + "x" + height + " cannot be evenly scaled to fit this device.");
#endif
        }
    }
}