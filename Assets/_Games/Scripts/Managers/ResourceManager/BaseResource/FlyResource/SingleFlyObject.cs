namespace PuzzleGames
{
    using UnityEngine;

    public class SingleFlyObject : AFlyObject
    {
        public override void ShowVisual(int num) { }

        public override int NumberToFly => Number;
    }
}