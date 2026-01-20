using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TRS.CaptureTool.Extras;
namespace TRS.CaptureTool
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RawImage))]
    [RequireComponent(typeof(RectTransform))]
    public class CutoutScript : MonoBehaviour
    {
        [SerializeField]
        bool _preview;
        public bool preview
        {
            get { return _preview; }
            set { _preview = value; UpdateCutoutGraphic(); }
        }
        bool temporarilyHidden;

        public Rect rect
        {
            get
            {
                transform.rotation = Quaternion.identity;
                return ((RectTransform)transform).RectForCurrentResolution();
            }
        }

        public bool clickToSelectPivot;
        Graphic cutoutGraphic;
        CanvasScaler canvasScaler;

        void Awake()
        {
            cutoutGraphic = GetComponent<RawImage>();
            canvasScaler = GetComponentInParent<CanvasScaler>();
        }

#if UNITY_EDITOR
        void OnEnable()
        {
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }
#endif

        public void Show()
        {
            temporarilyHidden = false;
            UpdateCutoutGraphic();
        }

        public void Hide()
        {
            temporarilyHidden = true;
            UpdateCutoutGraphic();
        }

        void UpdateCutoutGraphic()
        {
            if (cutoutGraphic != null)
                cutoutGraphic.enabled = _preview && !temporarilyHidden;
        }

        public Rect RectForResolution(Resolution resolution)
        {
            Resolution currentResolution = ScreenExtensions.CurrentResolution();
            if (resolution.IsSameSizeAs(currentResolution))
                return rect;

            Vector2 scale = transform.lossyScale;
            if (canvasScaler != null && canvasScaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
            {
                // Remove influence of current scale.
                scale *= 1.0f / ScreenSizeScaleForResolution(canvasScaler, currentResolution);
                scale *= ScreenSizeScaleForResolution(canvasScaler, resolution);

                Debug.Log("Adjusted ScaleWithScreenSize CanvasScaler to target resolution. Remove CanvasScaler or move the CutoutScript on new canvas to avoid.");
            }

            return ((RectTransform)transform).RectForResolution(resolution, scale);
        }

        float ScreenSizeScaleForResolution(CanvasScaler canvasScaler, Resolution resolution)
        {
            const int kLogBase = 2;
            switch (canvasScaler.screenMatchMode)
            {
                case CanvasScaler.ScreenMatchMode.Expand:
                    return Mathf.Min(resolution.width / canvasScaler.referenceResolution.x, resolution.height / canvasScaler.referenceResolution.y);
                case CanvasScaler.ScreenMatchMode.Shrink:
                    return Mathf.Max(resolution.width / canvasScaler.referenceResolution.x, resolution.height / canvasScaler.referenceResolution.y);
                case CanvasScaler.ScreenMatchMode.MatchWidthOrHeight:
                default:
                    float logWidth = Mathf.Log(resolution.width / canvasScaler.referenceResolution.x, kLogBase);
                    float logHeight = Mathf.Log(resolution.height / canvasScaler.referenceResolution.y, kLogBase);
                    float logWeightedAverage = Mathf.Lerp(logWidth, logHeight, canvasScaler.matchWidthOrHeight);
                    return Mathf.Pow(kLogBase, logWeightedAverage);

            }
        }

        void Update()
        {
            if (FlexibleInput.Click() && preview && clickToSelectPivot)
            {
                Vector2 mousePosition = FlexibleInput.ClickPosition();
#if UNITY_EDITOR
                // Skip clicks within the Editor windows. Still seems to glitch on occassion.
                if (mousePosition.x < 0 || mousePosition.x > Screen.width || mousePosition.y < 0 || mousePosition.y > Screen.height)
                    return;
#endif

                Resolution resolution = ScreenExtensions.CurrentResolution();
                RectTransform rectTransform = cutoutGraphic.rectTransform;

                float centerX = mousePosition.x / resolution.width;
                float centerY = mousePosition.y / resolution.height;
                float halfWidth = (rectTransform.anchorMax.x - rectTransform.anchorMin.x) / 2f;
                float halfHeight = (rectTransform.anchorMax.y - rectTransform.anchorMin.y) / 2f;

                rectTransform.anchoredPosition = new Vector2(0f, 0f);

                if (rectTransform.anchorMin.x != rectTransform.anchorMax.x)
                {
                    rectTransform.offsetMin = new Vector2(0f, rectTransform.offsetMin.y);
                    rectTransform.offsetMax = new Vector2(0f, rectTransform.offsetMax.y);
                }

                if (rectTransform.anchorMin.y != rectTransform.anchorMax.y)
                {
                    rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
                    rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);
                }

                rectTransform.anchorMin = new Vector2(centerX - halfWidth, centerY - halfHeight);
                rectTransform.anchorMax = new Vector2(centerX + halfWidth, centerY + halfHeight);

#if UNITY_EDITOR
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
            }
        }
    }
}