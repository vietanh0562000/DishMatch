using TMPro;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    [RequireComponent(typeof(TMP_InputField))]
    public class IntergerInputField : MonoBehaviour
    {
        [SerializeField] private int _min, _max;

        private TMP_InputField _inputField;
        
        private void Awake()
        {
            _inputField = GetComponent<TMP_InputField>();
            _inputField.onEndEdit.AddListener(OnValueChanged);
            _inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        }

        private void OnDestroy()
        {
            _inputField.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(string value)
        {
            if (!int.TryParse(value, out int num)) return;
            if (num < _min) num = _min;
            if (num > _max) num = _max;

            _inputField.text = num.ToString();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.IntegerNumber;
        }
#endif
    }
}
