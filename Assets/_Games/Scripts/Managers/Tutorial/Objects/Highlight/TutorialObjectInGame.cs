namespace PuzzleGames
{
    using UnityEngine;

    public class TutorialObjectInGame : ITutorialObject
    {
        private RectTransform target;
        private Transform      cachedParent;

        public TutorialObjectInGame(RectTransform target)
        {
            this.target = target; 
            cachedParent = target.parent;
        }

        public virtual void CreateUI(TutorialCanvas canvas)
        {
            target.SetParent(canvas.transform);;
        }

        public virtual void Hide()
        {
            target.SetParent(cachedParent);
        }
    }
}