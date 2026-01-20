namespace PuzzleGames
{
    using UnityEngine;

    public class TutorialCanvas : Singleton<TutorialCanvas>
    {
        [SerializeField] private GameObject raycastPanel;
        [SerializeField] private GameObject blackBackground;
        
        [SerializeField] private HighlightObject highlightPrefab;
        [SerializeField] private TextObject      textPrefab;
        [SerializeField] private ImageObject     imagePrefab;
        [SerializeField] private ButtonObject    buttonPrefab;

        public GameObject RaycastPanel => raycastPanel;
        public GameObject BlackBackground => blackBackground;
        
        public HighlightObject CreateArrow()
        {
            var go = Instantiate(highlightPrefab, transform);
            return go;
        }

        public TextObject CreateText()
        {
            var go = Instantiate(textPrefab, transform);
            return go;
        }

        public ImageObject CreateImage()
        {
            var go = Instantiate(imagePrefab, transform);
            return go;
        }

        public ButtonObject CreateButton()
        {
            var go = Instantiate(buttonPrefab, transform);
            return go;
        }
    }
}