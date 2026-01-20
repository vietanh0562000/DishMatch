namespace PuzzleGames
{
    using System.ComponentModel;

    public enum PowerupKind
    {
        [Description("Add Slot")] AddSlot,
        [Description("Helidrop")] Helidrop,
        [Description("Rainbow Hole")] RainbowHole
    }

    public enum LevelDifficulty
    {
        Easy     = 0,
        Normal   = 1,
        Hard     = 2,
        VeryHard = 3
    }
}