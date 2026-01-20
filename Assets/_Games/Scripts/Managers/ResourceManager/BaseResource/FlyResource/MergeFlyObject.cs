namespace PuzzleGames
{
    using TMPro;

    public class MergeFlyObject : AFlyObject
    {
        public TextMeshProUGUI numberTMP;

        public override void ShowVisual(int num) { numberTMP.SetText($"x{num}"); }
        public override int  NumberToFly       => 1;
    }
}