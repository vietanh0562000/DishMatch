namespace PuzzleGames
{
    using System;

    public abstract class APopupCondition : ICondition
    {
        public abstract bool CanStart();
        public abstract Type ScreenType { get; }
    }
}