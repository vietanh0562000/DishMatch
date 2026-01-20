using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ScrollRectCullingItem : MonoBehaviour
{
    // public Action OnDimensionChange;
    // private void OnRectTransformDimensionsChange()
    // {
    //     OnDimensionChange?.Invoke();
    // }
    
    private RectTransform _rectTransform;
    public int Index { get; private set; }

    public RectTransform RectTransform => _rectTransform ? _rectTransform : _rectTransform = GetComponent<RectTransform>();

    public void SetIndex(int index)
    {
        Index = index;
    }
}
