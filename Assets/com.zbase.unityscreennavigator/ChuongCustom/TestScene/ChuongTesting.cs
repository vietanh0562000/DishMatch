namespace ChuongCustom.ScreenManager.TestScene
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class ChuongTesting : MonoBehaviour
    {
        [Button]
        public void Failed() { }
        [Button]
        public void Back() { WindowManager.Instance.CloseCurrentWindow(); }
    }
}