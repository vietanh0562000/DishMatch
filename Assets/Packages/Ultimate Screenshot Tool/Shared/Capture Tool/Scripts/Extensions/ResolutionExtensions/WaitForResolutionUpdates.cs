using UnityEngine;

namespace TRS.CaptureTool
{
    // Does not guarantee that it will be EndOfFrame after yield instruction or that ResolutionUpdated event updates will have occurred.
    // Recommended to WaitUntilEndOfFrame after this yield instruction.
    public class WaitForResolutionUpdates : CustomYieldInstruction
    {
        public override bool keepWaiting
        {
            get
            {
                return !ScreenExtensions.resolutionUpdatesComplete;
            }
        }
    }
}