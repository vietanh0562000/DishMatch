using UnityEngine;

namespace TRS.CaptureTool
{
    // This class dispatches updates on any resolution change. (Not just those caused by ScreenExtensions.cs).
    // This is at the expense of requiring the script in each scene and checking the resolution once per frame.
    [ExecuteInEditMode]
    public class AnyResolutionChangeUpdateScript : MonoBehaviour
    {
        // Subscribe to perform an update on resolution change.
        // Note: This script must be added to the scene and checks the resolution once per frame.
        //       Use ScreenExtensions for subscription and resolution changes to avoid using this script.
        public static event System.Action<AnyResolutionChangeUpdateScript, int, int> ResolutionChanged;

        // Subscribe to perform an action after all other resolution updates have been made.
        public static event System.Action<AnyResolutionChangeUpdateScript, int, int> ResolutionChangedUpdatesComplete;

        int lastScreenWidth = 0;
        int lastScreenHeight = 0;

        void Update()
        {
            if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
            {
                lastScreenWidth = Screen.width;
                lastScreenHeight = Screen.height;

                if (ResolutionChanged != null)
                    ResolutionChanged(this, lastScreenWidth, lastScreenHeight);
                if (ResolutionChangedUpdatesComplete != null)
                    ResolutionChangedUpdatesComplete(this, lastScreenWidth, lastScreenHeight);
            }
        }
    }
}