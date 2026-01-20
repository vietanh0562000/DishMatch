namespace PuzzleGames.Highlight
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class TutBtnInGame : TutorialObjectInGame, INextStep
    {
        private Button button;

        UnityAction action;

        public TutBtnInGame(Button button) : base(button.GetComponent<RectTransform>()) { this.button = button; }

        public void AddAction(Action nextStep)
        {
            action = () => nextStep?.Invoke();
            button.transform.SetAsLastSibling();
            button.onClick.AddListener(action);
        }

        public override void Hide()
        {
            base.Hide();
            button.onClick.RemoveListener(action);
        }
    }
}