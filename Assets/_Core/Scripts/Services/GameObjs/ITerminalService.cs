namespace BasePuzzle.Core.Scripts.Services.GameObjs
{
    public interface ITerminalService
    {
        /// <summary>
        /// Called after all other FMainObj.OnGameStop.
        /// Called in main thread.
        /// </summary>
        void OnPostStop();
    }
}