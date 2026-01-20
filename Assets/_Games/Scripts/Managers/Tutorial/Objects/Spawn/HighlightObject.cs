namespace PuzzleGames
{
    public class HighlightObject : TutorialObject
    {
        public override void CreateUI(TutorialCanvas canvas)
        {
            gameObject.SetActive(true);
            transform.SetParent(canvas.transform);
        }
    }
}