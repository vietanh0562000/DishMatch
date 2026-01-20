using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class LoadingUI
    {
        private static RectTransform _loadingUI;
        private static Vector3 _defaultScale = Vector3.one;
        
        public static void SetObjectUI(RectTransform go)
        {
            _loadingUI = go;
            _loadingUI.gameObject.SetActive(false);
        }

        public static void Show(RectTransform parent, Vector3 scale = default)
        {
            if(_loadingUI == null)
            {
                Debug.LogError("Chưa gọi hàm SetObjectUI nên không sử dụng được");
                return;
            }
            
            _loadingUI.SetParent(parent);
            _loadingUI.anchoredPosition = Vector3.zero;
            _loadingUI.sizeDelta = Vector2.zero;
            
            _loadingUI.localScale = scale == default ? Vector3.one : scale;
            _loadingUI.gameObject.SetActive(true);
        }

        public static void Hide()
        {
            if(_loadingUI == null)
            {
                Debug.LogError("Chưa gọi hàm SetObjectUI nên không sử dụng được");
                return;
            }
            
            _loadingUI.gameObject.SetActive(false);
        }
    }
}
