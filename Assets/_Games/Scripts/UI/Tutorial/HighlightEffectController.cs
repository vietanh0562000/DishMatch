using UnityEngine;
using UnityEngine.UI;

public class HighlightEffect : MonoBehaviour
{
    [SerializeField] private Material highlightMaterial; // Material cho Shader
    [SerializeField] private RectTransform highlightRect; // Vùng highlight

    private Image overlayImage;

    private void Awake()
    {
        overlayImage = GetComponent<Image>();

        if (overlayImage != null && highlightMaterial != null)
        {
            overlayImage.material = new Material(highlightMaterial);
        }
    }

    private void Update()
    {
        if (overlayImage != null && overlayImage.material != null && highlightRect != null)
        {
            RectTransform canvasRect = overlayImage.canvas.GetComponent<RectTransform>();
            Vector2 position = RectTransformUtility.WorldToScreenPoint(Camera.main, highlightRect.position);
            position.x /= canvasRect.sizeDelta.x;
            position.y /= canvasRect.sizeDelta.y;

            overlayImage.material.SetVector("_MaskPosition", new Vector4(position.x, position.y, 0, 0));
            overlayImage.material.SetVector("_MaskSize", (Vector4)highlightRect.sizeDelta);
        }
    }

    /// <summary>
    /// Cập nhật vị trí và kích thước của vùng sáng
    /// </summary>
    /// <param name="targetRect">Kích thước và vị trí của Rect highlight</param>
    public void SetHighlight(RectTransform targetRect)
    {
        highlightRect = targetRect;
    }
}