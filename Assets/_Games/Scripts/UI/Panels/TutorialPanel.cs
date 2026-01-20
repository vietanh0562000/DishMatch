namespace PuzzleGames
{
    using ChuongCustom;
    using ChuongCustom.ScreenManager;
    using Core.Utilities.Extension;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.EventSystems;

    [Popup("TutorialPanel", closeWhenClickOnBackdrop = true, showAnim = false)]
    public class TutorialPanel : BaseActivity
    {
        public HandTutorial      handTutorial;
        public TutorialHighlight tutorialHighlight;

        public override void Init()
        {
            tutorialHighlight.UpdateHoleForWorld(Vector3.zero, Vector2.zero);
            
            DOVirtual.DelayedCall(0.5f, () =>
            {
                /*var holePoint = TemporaryBoardVisualize.Instance.GetRandomHole();
                tutorialHighlight.UpdateHoleForWorld(holePoint, new Vector2(0.24f, 0.15f));
                handTutorial.ShowAtWorld(holePoint);*/
            });
        }

        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsActive && !EventSystem.current.IsPointerOverGameObject())
            {
                CloseView();
            }
        }

        protected override void CloseView()
        {
            if (IsTransitioning)
            {
                return;
            }

            var popupAtt = this.GetCustomAttribute<PopupAttribute>();

            WindowManager.Instance.CloseActivity(popupAtt.namePath, false);
        }
    }
}