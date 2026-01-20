using UnityEngine;

namespace PuzzleGames
{
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;

    public class UFOBtn : BoosterBtn
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

            if (TemporaryBoardVisualize.Instance.UseUfo)
            {
                UpdateCount();
                IsActivating                            = false;
                TemporaryBoardVisualize.Instance.UseUfo = false;
                return;
            }*/

            base.OnClickButton();
        }

        public void UnuseBooster()
        {
            UpdateCount();
            IsActivating                            = false;
            //TemporaryBoardVisualize.Instance.UseUfo = false;
        }


        public override void ActivateBooster()
        {
            WindowManager.Instance.OpenWindow<TooltipPanel>(onLoaded: panel => { panel.ShowTooltip(this); });
            IsActivating                            = true;
           // TemporaryBoardVisualize.Instance.UseUfo = true;
        }

        public override int          LevelUnlock => ServerConfig.Instance<ValueRemoteConfig>().levelUnlockBooserUFO;
        public override ResourceType Type        => ResourceType.Powerup_Helidrop;
    }
}