namespace PuzzleGames
{
    public class SoundBtn : SettingButton
    {
        protected override bool ChangeSetting()
        {
            var enableSound = GetCurrentSetting();
            AudioDataController.instance.SetSound(enableSound ? 0 : 1);

            return !enableSound;
        }
        protected override bool GetCurrentSetting() { return AudioDataController.instance.IsActiveSound(); }
    }
}