namespace PuzzleGames
{
    using System;
    using DG.Tweening;
    using TMPro;
    using UnityEngine.UI;

    public class ButtonObject : TutorialObject, INextStep
    {
        public Button Button;

        public string content;
        public float  delay;
        
        public override void CreateUI(TutorialCanvas canvas)
        {
            gameObject.SetActive(true);

            if (delay > 0)
            {
                Button.interactable = false;
                DOVirtual.DelayedCall(delay, () => { Button.interactable = true; });
            }

            transform.SetParent(canvas.transform);
            transform.SetAsLastSibling();

            var label = transform.GetComponentInChildren<TMP_Text>();

            if (label != null) label.text = content;
        }
        public void AddAction(Action nextStep)
        {
            transform.SetAsLastSibling();
            Button.onClick.AddListener(() => nextStep?.Invoke());
        }
    }
}