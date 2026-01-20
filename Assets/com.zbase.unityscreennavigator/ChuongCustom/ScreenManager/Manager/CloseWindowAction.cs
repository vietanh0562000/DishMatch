using UnityEngine;

namespace ChuongCustom
{
    public class CloseWindowAction : MonoBehaviour
    {
        public void CloseWindow()
        {
            WindowManager.Instance.CloseCurrentWindow();
        }
    }
}