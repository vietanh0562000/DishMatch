namespace PuzzleGames
{
    using DG.Tweening;
    using TMPro;
    using UnityEngine;

    public class TextObject : TutorialObject
    {
        public string content;

        public void SetContent(string tutContent) { content = tutContent; }

        public override void CreateUI(TutorialCanvas canvas)
        {
            transform.DOKill();
            transform.DOScale(1, 0.3f).From(0);
            gameObject.SetActive(true);
            transform.SetParent(canvas.transform);
            var label = transform.GetComponentInChildren<TMP_Text>();
            label.text = content;
        }

        public void SetSize(Vector2 size)
        {
            var rect = GetComponent<RectTransform>();
            rect.sizeDelta = size;
        }
    }
}