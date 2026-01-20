using UnityEngine;
using System.Collections.Generic;

public class SwipeDetector : MonoBehaviour
{
    public List<GameObject> tabPanels; // Kéo các GameObject Panel của từng tab vào đây từ Inspector
    public int currentTabIndex = 0;

    void Start()
    {
        // Hiển thị tab đầu tiên khi bắt đầu
        ShowTab(currentTabIndex);
    }

    public void ShowTab(int index)
    {
        if (index < 0 || index >= tabPanels.Count)
        {
            Debug.LogWarning("Tab index out of range.");
            return;
        }

        // Ẩn tất cả các tab
        for (int i = 0; i < tabPanels.Count; i++)
        {
            if (tabPanels[i] != null)
            {
                tabPanels[i].SetActive(false);
            }
        }

        // Hiển thị tab được chọn
        if (tabPanels[index] != null)
        {
            tabPanels[index].SetActive(true);
            currentTabIndex = index;
            // Optional: Cập nhật trạng thái active/inactive của các button chọn tab trực tiếp (nếu có)
        }
    }

    public void GoToNextTab()
    {
        int nextIndex = (currentTabIndex + 1) % tabPanels.Count; // % để quay vòng
        ShowTab(nextIndex);
        Debug.LogError($"{typeof(SwipeDetector)} > {nextIndex}");
    }

    public void GoToPreviousTab()
    {
        int prevIndex = (currentTabIndex - 1 + tabPanels.Count) % tabPanels.Count; // Thêm tabPanels.Count để xử lý số âm khi quay vòng
        ShowTab(prevIndex);
    }

    // Optional: Thêm hàm để các button chọn tab trực tiếp gọi
    public void SelectTabByIndex(int index)
    {
        ShowTab(index);
    }
}