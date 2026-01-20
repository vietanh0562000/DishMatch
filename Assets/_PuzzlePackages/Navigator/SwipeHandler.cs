using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class SwipeHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField, Tooltip("Khoảng cách tối thiểu tính là 1 drag hợp lệ")]
    private float _minDistanceHorizontal = 100f, _minDistanceVertical;

    // private const float maxVerticalSwipeRatio = 0.5f; 

    private Vector2 dragStartPosition;
    private bool isDragging = false;

    void Start()
    {
        // Đảm bảo EventSystem tồn tại
        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem found in the scene. UI input will not work.");
            enabled = false;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDragging) return;
        dragStartPosition = eventData.position;
        isDragging = true;
        Debug.Log("Begin Drag at: " + dragStartPosition);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Không cần xử lý gì đặc biệt trong OnDrag cho việc này
        // Unity EventSystem sẽ tự động xử lý việc ngăn click nếu kéo đủ xa (vượt pixelDragThreshold)
        // Debug.Log("Dragging...");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return; // Thoát nếu không phải drag của chúng ta hoặc không có manager

        isDragging = false; // Kết thúc trạng thái kéo
        Vector2 dragEndPosition = eventData.position;
        Vector2 dragVector = dragEndPosition - dragStartPosition;

        Debug.Log("End Drag at: " + dragEndPosition);
        Debug.Log("Drag Vector: " + dragVector);

        float dragDistanceX = Mathf.Abs(dragVector.x);
        float dragDistanceY = Mathf.Abs(dragVector.y);

        // Kiểm tra xem có phải là một cử chỉ vuốt ngang hợp lệ không
        // if (dragDistanceX > _minDistanceHorizontal && dragDistanceX > dragDistanceY / maxVerticalSwipeRatio) 
        // {
        //     Debug.Log("Valid Horizontal Swipe Detected!");
        //     // Vuốt sang trái (Delta X < 0) -> Chuyển sang tab tiếp theo (bên phải)
        //     if (dragVector.x < 0)
        //     {
        //         Debug.Log("Swipe Left - Go to Next Tab");
        //     }
        //     // Vuốt sang phải (Delta X > 0) -> Chuyển sang tab trước đó (bên trái)
        //     else
        //     {
        //         Debug.Log("Swipe Right - Go to Previous Tab");
        //     }
        // }
        // else
        // {
        //     Debug.Log("Drag was not a valid horizontal swipe.");
        // }
    }
}