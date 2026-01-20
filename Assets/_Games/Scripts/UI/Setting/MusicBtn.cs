namespace PuzzleGames
{
    public class MusicBtn : SettingButton
    {
        protected override bool ChangeSetting()
        {
            var enableMusic = GetCurrentSetting();
            AudioDataController.instance.SetMusic(enableMusic ? 0 : 1);

            return !enableMusic;
        }
        protected override bool GetCurrentSetting() { return AudioDataController.instance.IsActiveMusic(); }
    }
}