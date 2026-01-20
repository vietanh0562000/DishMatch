namespace PuzzleGames
{
    using UnityEngine;
    using UnityEngine.UI;

    public class ImageObject : TutorialObject
    {
        private Image img;

        public Sprite sprite;

        private void Start() { img = GetComponent<Image>(); }

        public override void CreateUI(TutorialCanvas canvas)
        {
            gameObject.SetActive(true);
            transform.SetParent(canvas.transform);
            img.sprite = sprite;
        }
    }
}