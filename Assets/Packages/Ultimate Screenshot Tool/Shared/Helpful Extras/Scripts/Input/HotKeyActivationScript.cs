using UnityEngine;

namespace TRS.CaptureTool.Extras
{
    public class HotKeyActivationScript : MonoBehaviour
    {
        public HotKeySet hotKeySet;
        public GameObject activatedObject;

        void Update()
        {
            if (FlexibleInput.AnyKey() && !UIStatus.InputFieldFocused())
            {
                if (hotKeySet.MatchesInput())
                    activatedObject.SetActive(!activatedObject.activeSelf);
            }
        }
    }
}