namespace PuzzleGames
{
    using BasePuzzle.PuzzlePackages.Core;

    public interface IResourceUI
    {
        public EndPointInfo EndPoint { get; }

        void EnableCanvas();
        void OnReachUI(bool isLast);
        void UpdateUI();
        void Push();
        void Pop();
    }
}