using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TRS.CaptureTool.Extras
{
    public class UIStatus
    {
        public static bool InputFieldFocused()
        {
            if (EventSystem.current == null)
                return false;
            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;
            if (selectedObject == null)
                return false;

            InputField inputField = selectedObject.GetComponent<InputField>();
            return inputField != null && inputField.isFocused;
        }
    }
}