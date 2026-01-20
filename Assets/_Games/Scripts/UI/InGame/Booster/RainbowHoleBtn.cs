using UnityEngine;

namespace PuzzleGames
{
    using com.ootii.Messages;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

    public class RainbowHoleBtn : BoosterBtn
    {
        private bool IsActivating;

        protected override void OnClickButton()
        {
            if (!IsAvailable)
            {
                return;
            }

            /*if (TemporaryBoardVisualize.Instance.ExistStickmanMoving())
            {
                UIToastManager.Instance.Show("Please wait until all stickmen are placed");
                return;
            }
            
            if (TemporaryBoardVisualize.Instance.UseRainbowHole)
            {
                UnuseBooster();
                return;
            }*/

            base.OnClickButton();
        }


        public void UnuseBooster()
        {
            UpdateCount();
            IsActivating                                    = false;
            //TemporaryBoardVisualize.Instance.UseRainbowHole = false;
        }

        public override void ActivateBooster()
        {
            WindowManager.Instance.OpenWindow<TooltipPanel>(onLoaded: panel => { panel.ShowTooltip(this); });
            IsActivating                                    = true;
            //TemporaryBoardVisualize.Instance.UseRainbowHole = true;
        }

        public override int          LevelUnlock => ServerConfig.Instance<ValueRemoteConfig>().levelUnlockBooserRainbow;
        public override ResourceType Type        => ResourceType.Powerup_RainbowHole;
    }
}