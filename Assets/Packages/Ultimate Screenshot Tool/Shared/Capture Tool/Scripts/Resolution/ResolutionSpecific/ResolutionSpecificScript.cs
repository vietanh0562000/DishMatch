using UnityEngine;

namespace TRS.CaptureTool
{
    // Finalizers/Deconstructors (~ deinit methods) are not called consistently.
    // Therefore, the UpdateForResolution/ResolutionChanged method may get called multiple times.
    [ExecuteInEditMode]
    public class ResolutionSpecificScript : MonoBehaviour
    {
        public ResolutionSpecificScript()
        {
            ScreenExtensions.ResolutionUpdated += UpdateForResolution;
            AnyResolutionChangeUpdateScript.ResolutionChanged += ResolutionChanged;
        }

        ~ResolutionSpecificScript()
        {
            ScreenExtensions.ResolutionUpdated -= UpdateForResolution;
            AnyResolutionChangeUpdateScript.ResolutionChanged -= ResolutionChanged;
        }

        public virtual void Start()
        {
            Resolution resolution = ScreenExtensions.CurrentResolution();
            UpdateForResolution(resolution.width, resolution.height);
        }

        void ResolutionChanged(AnyResolutionChangeUpdateScript anyResolutionChangeUpdateScript, int width, int height)
        {
            UpdateForResolution(width, height);
        }

        protected bool ValidObject()
        {
            if (this == null)
            {
                ScreenExtensions.ResolutionUpdated -= UpdateForResolution;
                AnyResolutionChangeUpdateScript.ResolutionChanged -= ResolutionChanged;
                return false;
            }

            return true;
        }

        public virtual void UpdateForResolution(int width, int height)
        {

        }
    }
}