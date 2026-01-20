using UnityEngine;

namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TutorialHighlight : MonoBehaviour
    {
        public Image bg;

        private Material overlayMaterial;

        void Awake() { overlayMaterial = bg.material; }

        /// <summary>
        /// Tính rectangle khoét dựa trên một RectTransform (UI trên Canvas).
        /// </summary>
        public void UpdateHoleForUI(RectTransform targetRectUI)
        {
            bg.gameObject.SetActive(true);

            if (overlayMaterial == null || targetRectUI == null) return;

            // 1. Lấy góc world của targetRectUI
            Vector3[] worldCorners = new Vector3[4];
            targetRectUI.GetWorldCorners(worldCorners);

            // 2. Tính min/max normalized UV (0..1) dựa trên Screen
            Vector2 minUV = new Vector2(1, 1);
            Vector2 maxUV = new Vector2(0, 0);

            for (int i = 0; i < 4; i++)
            {
                Vector3 worldPt  = worldCorners[i];
                Vector2 screenPt = RectTransformUtility.WorldToScreenPoint(null, worldPt);
                Vector2 uv       = new Vector2(screenPt.x / Screen.width, screenPt.y / Screen.height);

                minUV.x = Mathf.Min(minUV.x, uv.x);
                minUV.y = Mathf.Min(minUV.y, uv.y);
                maxUV.x = Mathf.Max(maxUV.x, uv.x);
                maxUV.y = Mathf.Max(maxUV.y, uv.y);
            }

            // 3. Tính centerNormalized và sizeNormalized
            Vector2 centerUV = (minUV + maxUV) * 0.5f;
            Vector2 sizeUV = new Vector2(
                Mathf.Clamp01(maxUV.x - minUV.x),
                Mathf.Clamp01(maxUV.y - minUV.y)
            );

            // 4. Gán vào material
            overlayMaterial.SetVector("_HoleCenter", new Vector4(centerUV.x, centerUV.y, 0, 0));
            overlayMaterial.SetVector("_HoleSize", new Vector4(sizeUV.x, sizeUV.y, 0, 0));
        }

        /// <summary>
        /// Tính rectangle khoét dựa trên một điểm world (Vector3) và holeSize normalized.
        /// targetTransform3D.position chính là center world.
        /// </summary>
        public void UpdateHoleForWorld(Vector3 position, Vector2 holeSize)
        {
            bg.gameObject.SetActive(true);
            if (overlayMaterial == null) return;

            Camera cam = Camera.main;
            if (cam == null) return;

            Vector3 worldPos = position;
            Vector3 screenPt = cam.WorldToScreenPoint(worldPos);

            // Chuyển sang normalized UV
            Vector2 centerUV = new Vector2(screenPt.x / Screen.width, screenPt.y / Screen.height);

            // holeSize (width,height) bạn set sẵn normalized
            Vector2 sizeUV = holeSize;

            overlayMaterial.SetVector("_HoleCenter", new Vector4(centerUV.x, centerUV.y, 0, 0));
            overlayMaterial.SetVector("_HoleSize", new Vector4(sizeUV.x, sizeUV.y, 0, 0));
        }

        public void Hide() { bg.gameObject.SetActive(false); }
    }
}