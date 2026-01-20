using UnityEngine;

namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Gắn component này lên Image đang dùng Shader khoét lỗ ("HighlightMask").
    /// Nếu user click/touch ở bên trong vùng khoét (highlight), IsRaycastLocationValid trả về false ⇒ 
    /// Raycast sẽ “lọt qua” (không bị Image này chặn). 
    /// Ngược lại, nếu click ở ngoài vùng khoét ⇒ trả về true để Image vẫn chặn raycast.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class HolePunchRaycastFilter : MonoBehaviour, ICanvasRaycastFilter
    {
        // Center và Size (normalized 0..1) của vùng khoét. 
        // Thường bạn đã set từ Material/Shader, nhưng cần copy vào đây để tính toán ICRF.
        private Vector2 holeCenter = new Vector2(0.5f, 0.5f);
        private Vector2 holeSize   = new Vector2(0.2f, 0.2f);

        Canvas        _canvas;
        RectTransform _rectTransform;
        
        public Image bg;

        private Material overlayMaterial;
        
        void Awake()
        {
            overlayMaterial = bg.material; 
            _rectTransform  = GetComponent<RectTransform>();
            _canvas         = GetComponentInParent<Canvas>();
            if (_canvas == null)
                Debug.LogError("[HolePunchRaycastFilter] Không tìm thấy Canvas cha!");
        }

        /// <summary>
        /// Hàm này được Unity gọi mỗi khi kiểm tra xem click/touch tại điểm 'sp' (screenPoint) 
        /// có hợp lệ để Image này chặn hay không. 
        /// Trả về false ⇒ cho click xuyên qua; true ⇒ Image chặn.
        /// </summary>
        /// <param name="sp">screen point (pixel) của con trỏ / touch</param>
        /// <param name="eventCamera">camera để chiếu UI</param>
        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            if (_canvas == null || _rectTransform == null)
                return true; // nếu thiếu thành phần, không chặn.

            holeCenter = overlayMaterial.GetVector("_HoleCenter");
            holeSize   = overlayMaterial.GetVector("_HoleSize");
            
            // 1. Chuyển screen point thành local point trong RectTransform (UI coordinate)
            Vector2 localPoint;
            bool insideRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rectTransform, sp, eventCamera, out localPoint);

            if (!insideRect)
            {
                // Nếu click nằm ngoài RectTransform (vì RectTransform có thể không full-screen),
                // thì tùy case, bạn có thể return true (để chặn luôn) hoặc false. 
                // Ở đây giả sử Image full-screen, nên nếu ngoài, vẫn return true cho chặn.
                return true;
            }

            // 2. Chuyển localPoint sang UV (0..1) so với RectTransform kích cỡ full-screen.
            //    Giả sử anchormin = (0,0), anchormax = (1,1) và RectTransform stretch full màn.
            //    Nếu RectTransform không full, ta vẫn có thể chuyển tương đối bằng cách tính size.
            Rect  rect = _rectTransform.rect;
            float ux   = (localPoint.x - rect.xMin) / rect.width; // normalized x (0..1)
            float uy   = (localPoint.y - rect.yMin) / rect.height; // normalized y (0..1)

            // 3. Tính vùng khoét (minUV, maxUV) theo holeCenter, holeSize (UV).
            Vector2 halfSize = holeSize * 0.5f;
            Vector2 minUV    = holeCenter - halfSize;
            Vector2 maxUV    = holeCenter + halfSize;

            // 4. Kiểm tra xem (ux, uy) nằm trong vùng khoét hay không.
            bool insideHole = (ux >= minUV.x && ux <= maxUV.x &&
                               uy >= minUV.y && uy <= maxUV.y);

            if (insideHole)
            {
                // Nếu click nằm trong "lỗ" ⇒ khuyến nghị cho click xuyên qua (không chặn)
                return false;
            }
            else
            {
                // Nếu click nằm ngoài "lỗ" ⇒ chặn (không cho click qua)
                return true;
            }
        }
    }
}