using UnityEngine;

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    public static class ScreenExtensions
    {
        public static bool resolutionUpdatesComplete { get; private set; }

        // Subscribe to perform an update on resolution change.
        // Note: This only captures resolution changes caused by this script.
        //        Use AnyResolutionChangeUpdateScript for other resolution updates.
        public static event System.Action<int, int> ResolutionUpdated;

        // Subscribe to perform an action after all other resolution updates have been made.
        public static event System.Action<int, int> ResolutionUpdatesComplete;

        public static Resolution CurrentResolution()
        {
#if UNITY_EDITOR
            Vector2 size = GameView.CurrentSize();
            return new Resolution { width = (int)size.x, height = (int)size.y, refreshRate = Screen.currentResolution.refreshRate };
#else
            return new Resolution { width = Screen.width, height = Screen.height, refreshRate = Screen.currentResolution.refreshRate };
#endif
        }

        public static Rect SafeArea()
        {
#if !UNITY_EDITOR && UNITY_2019_1_2_OR_NEWER
            return Screen.safeArea;
#else
            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            Resolution noRefreshRateResolution = new Resolution { width = currentResolution.width, height = currentResolution.height };
            if (AdditionalResolutions.safeAreaForResolution.ContainsKey(noRefreshRateResolution))
                return AdditionalResolutions.safeAreaForResolution[noRefreshRateResolution];
            return new Rect(0, 0, currentResolution.width, currentResolution.height);
#endif
        }

        public static void UpdateResolution(Resolution resolution)
        {
            UpdateResolution(resolution.width, resolution.height);
        }

        public static void UpdateResolution(Resolution resolution, bool fullScreen)
        {
            UpdateResolution(resolution.width, resolution.height, fullScreen);
        }

        public static void UpdateResolution(int width, int height)
        {
            bool inFullScreen = Screen.fullScreen;
            UpdateResolution(width, height, inFullScreen);
        }

        public static void UpdateResolution(int width, int height, bool fullScreen)
        {
#if !UNITY_EDITOR
            if(fullScreen)
                Debug.LogError("Resolution cannot be modified while in full screen mode.");
#endif

            resolutionUpdatesComplete = false;

#if UNITY_EDITOR
            GameView.SetSize(GameView.GameViewSizeType.FixedResolution, width, height);
#else
		    Screen.SetResolution(width, height, fullScreen);
#endif

            ((System.Action)(() =>
            {
                if (ResolutionUpdated != null)
                    ResolutionUpdated(width, height);
                if (ResolutionUpdatesComplete != null)
                    ResolutionUpdatesComplete(width, height);
                resolutionUpdatesComplete = true;
            })).PerformAfterCoroutine<WaitForEndOfFrame>();
        }
    }
}