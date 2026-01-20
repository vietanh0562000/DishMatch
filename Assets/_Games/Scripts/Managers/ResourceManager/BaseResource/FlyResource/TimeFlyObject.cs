namespace PuzzleGames
{
    using TMPro;
    using UnityEngine;

    public class TimeFlyObject : AFlyObject
    {
        public TextMeshProUGUI numberTMP;

        public override void ShowVisual(int num) { numberTMP.SetText($"{num.DisplayTimeWithMinutes()}"); }

        public override int NumberToFly => 1;
    }
}