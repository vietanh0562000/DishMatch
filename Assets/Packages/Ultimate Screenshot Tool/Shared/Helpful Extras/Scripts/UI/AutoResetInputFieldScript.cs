using UnityEngine;
using UnityEngine.UI;

namespace TRS.CaptureTool.Extras
{
    public class AutoResetInputFieldScript : MonoBehaviour
    {
        public string defaultPlaceholder = "Enter text...";

        InputField inputField;
        Text placeholder;

        void Awake()
        {
            inputField = GetComponent<InputField>();
            placeholder = inputField.placeholder.GetComponent<Text>();
        }

        void OnEnable()
        {
            inputField.text = "";
            placeholder.text = defaultPlaceholder;
        }
    }
}