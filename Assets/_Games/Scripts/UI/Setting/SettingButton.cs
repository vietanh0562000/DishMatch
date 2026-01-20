namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public abstract class SettingButton : MonoBehaviour
    {
        [SerializeField] public Button     _button;
        [SerializeField] public GameObject _objectOff;

        private void Start()
        {
            var b = GetCurrentSetting();
            ButtonEnable(b);

            AddListener(OnClickBtn);
        }

        private void OnClickBtn()
        {
            var b = ChangeSetting();
            ButtonEnable(b);
        }

        protected abstract bool ChangeSetting();

        private void OnDestroy() { RemoveListener(OnClickBtn); }

        protected abstract bool GetCurrentSetting();

        private void ButtonEnable(bool enable) { _objectOff.SetActive(!enable); }

        public void AddListener(UnityAction action) { _button.onClick.AddListener(action); }

        public void RemoveListener(UnityAction action) { _button.onClick.RemoveListener(action); }
    }
}