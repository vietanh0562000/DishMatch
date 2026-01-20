using UnityEngine;

namespace TRS.CaptureTool
{
    [RequireComponent(typeof(RectTransform))]
    public class DemoLogoScaleScript : MonoBehaviour
    {
        [Range(0f, 1f)]
        [Tooltip("Determines how this rect transform is scaled. Approximately similar to Match variable of CanvasScaler in UI Scale Mode 'Scale with Screen Size' and Screen Match Mode 'Match Width or Height'. Note: If re-using this script with text, set Best Fit on the Text Component. Or do your own thing. :)")]
        public float widthToHeightMatchPreference = 0.5f;

        RectTransform rectTransform;
        Vector2 originalSizeDelta;
        Resolution originalResolution;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalSizeDelta = rectTransform.sizeDelta;
            originalResolution = ScreenExtensions.CurrentResolution();
        }

        void OnEnable()
        {
            Resolution resolution = ScreenExtensions.CurrentResolution();
            float scale = ((float)resolution.width / (float)originalResolution.width) * widthToHeightMatchPreference + ((float)resolution.height / (float)originalResolution.height) * (1f - widthToHeightMatchPreference);
            rectTransform.sizeDelta = new Vector2(originalSizeDelta.x * scale, originalSizeDelta.y * scale);
        }
    }
}