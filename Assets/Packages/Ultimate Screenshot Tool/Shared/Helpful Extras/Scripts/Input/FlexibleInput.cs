using UnityEngine;
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace TRS.CaptureTool.Extras
{
    public class FlexibleInput : MonoBehaviour
    {
        public static bool AnyKey()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null) return Keyboard.current.anyKey.isPressed;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.anyKey;
#else
    return false;
#endif
        }

        public static bool AnyKeyDown()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null) return Keyboard.current.anyKey.wasPressedThisFrame;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.anyKeyDown;
#else
    return false;
#endif
        }

        public static bool Click()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) return Mouse.current.leftButton.isPressed;

            foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended && touch.phase != UnityEngine.InputSystem.TouchPhase.Canceled)
                    return true;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetMouseButton(0);
#else
    return false;
#endif
        }

        public static bool LeftMouseButton()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) return Mouse.current.leftButton.isPressed;
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetMouseButton(0);
#else
    return false;
#endif
        }

        public static Vector3 ClickPosition()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) return Mouse.current.position.ReadValue();

            foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Ended && touch.phase != UnityEngine.InputSystem.TouchPhase.Canceled)
                    return touch.screenPosition;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#else
    return Vector3.zero;
#endif
        }

        public static Vector3 MousePosition()
        {
#if UNITY_2019_2_OR_NEWER && ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) return Mouse.current.position.ReadValue();
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.mousePosition;
#else
    return Vector3.zero;
#endif
        }
    }
}