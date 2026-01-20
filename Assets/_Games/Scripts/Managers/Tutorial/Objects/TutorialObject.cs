namespace PuzzleGames
{
    using UnityEngine;

    public abstract class TutorialObject : MonoBehaviour, ITutorialObject
    {
        public abstract void CreateUI(TutorialCanvas canvas);
        public          void Hide() { Destroy(gameObject); }
    }
}