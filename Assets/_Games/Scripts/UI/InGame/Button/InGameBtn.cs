namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;

    public abstract class InGameBtn : MonoBehaviour
    {
        public Button _button;

        private void OnValidate() { _button = GetComponent<Button>(); }

        protected virtual void Start() { _button.onClick.AddListener(OnClickButton); }

        protected abstract void OnClickButton();
    }
}