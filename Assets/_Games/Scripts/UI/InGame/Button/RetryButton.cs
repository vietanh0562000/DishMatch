namespace PuzzleGames
{
    using Coffee.UIEffects;

    public class RetryButton : InGameBtn
    {
        public UIEffect UIEffect;

        protected override void Start()
        {
            base.Start();
            GameManager.OnGameStart += StartLevel;
            _button.interactable    =  false;
            UIEffect.enabled        =  true;
        }

        void OnDestroy() { GameManager.OnGameStart -= StartLevel; }

        private void StartLevel()
        {
            _button.interactable = true;
            UIEffect.enabled     = false;
        }

        protected override void OnClickButton()
        {
            WindowManager.Instance.OpenWindow<LevelFailedPanel>
                (onLoaded: panel => { panel.SetRetryPanel(); });
        }
    }
}