namespace PuzzleGames
{
    using System;

    public interface INextStep
    {
        void AddAction(Action nextStep);
    }
}