namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using BasePuzzle.PuzzlePackages;
    using UnityEngine.UI;

    [Popup("MechanicUnlockPanel")]
    public class MechanicUnlockPanel : BaseScreen
    {
        public MechanicVisual Visual;
        public Button         Button;
        public MechanicSO     MechanicSo;

        protected override void Awake()
        {
            base.Awake();
            Button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton() { CloseView(); }

        public void SetUpWithMechanicID(int mechanicID)
        {
            var mData = MechanicSo.GetData(mechanicID);
            Visual.SetVisual(mData);
        }

        public override void DidPopExit(Memory<object> args)
        {
            
            base.DidPopExit(args);
        }
    }
}