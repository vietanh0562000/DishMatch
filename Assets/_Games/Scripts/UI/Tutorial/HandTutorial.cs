namespace PuzzleGames
{
    using UnityEngine;

    public class HandTutorial : MonoBehaviour
    {
        [SerializeField] private RectTransform handIcon; // Tay trỏ trên canvas (UI)
        [SerializeField] private Canvas        canvas; // Canvas để điều chỉnh vị trí

        /// <summary>
        /// Xuất hiện tay trỏ tại một điểm trên màn hình (Canvas UI - RectTransform).
        /// </summary>
        /// <param name="uiTarget">Vị trí RectTransform trên canvas cần trỏ tới</param>
        public void ShowAtUI(RectTransform uiTarget)
        {
            if (handIcon == null || canvas == null)
            {
                Debug.LogError("HandIcon or Canvas is not assigned.");
                return;
            }

            // Đặt vị trí tay trỏ trùng với RectTransform mục tiêu
            handIcon.gameObject.SetActive(true);
            handIcon.position = uiTarget.position;
        }

        /// <summary>
        /// Xuất hiện tay trỏ tại một điểm trong World Space.
        /// </summary>
        /// <param name="worldTarget">Tọa độ Vector3 trong World để trỏ tới</param>
        public void ShowAtWorld(Vector3 worldTarget)
        {
            if (handIcon == null || canvas == null)
            {
                Debug.LogError("HandIcon or Canvas is not assigned.");
                return;
            }

            // Đặt tay trỏ tại vị trí được chuyển đổi từ World Space sang Canvas Space
            var canvasPosition =  Camera.main.WorldToScreenPoint(worldTarget);
            handIcon.gameObject.SetActive(true);
            handIcon.position = canvasPosition;
        }

        public void Hide() { handIcon.gameObject.SetActive(false); }
    }
}