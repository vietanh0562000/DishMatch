namespace PuzzleGames
{
    public class QuitButton : InGameBtn
    {
        protected override void OnClickButton()
        {
            WindowManager.Instance.OpenWindow<LevelFailedPanel>
                (onLoaded: panel => { panel.SetQuitPanel(); });
        }
    }
}