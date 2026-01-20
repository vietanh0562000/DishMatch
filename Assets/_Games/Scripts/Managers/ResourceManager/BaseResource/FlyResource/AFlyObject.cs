namespace PuzzleGames
{
    using UnityEngine;

    public abstract class AFlyObject : MonoBehaviour
    {
        public RectTransform rectTrans;

        private int number;
        public  int Number => number;

        public void SetData(int num) { this.number = num; }

        public abstract void ShowVisual(int num);
        public abstract int  NumberToFly { get; }
    }
}