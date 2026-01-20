namespace PuzzleGames
{
    public class PauseBtn : InGameBtn
    {
        protected override void OnClickButton()
        {
            WindowManager.Instance.OpenWindow<PausePanel>();
        }
    }
}