namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using ChuongCustom.ScreenManager;
    using com.ootii.Messages;
    using Core.Utilities.Extension;
    using BasePuzzle.PuzzlePackages;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [Popup("BoosterUnlockPanel", closeWhenClickOnBackdrop = true)]
    public class BoosterUnlockPanel : BaseActivity
    {
        public Image             boosterImage;
        public TextMeshProUGUI   boosterName;
        public Button            claimBtn;
        public HandTutorial      handTutorial;
        public TutorialHighlight tutorialHighlight;
        public GameObject        unlockPanel;

        private PowerUpResource powerUpManager;
        private BoosterBtn      boosterBtn;

        protected override void Awake()
        {
            base.Awake();
            MessageDispatcher.AddListener(EventID.USE_BOOSTER, UseBooster, true);
        }

        protected override void OnDestroy()
        {
            MessageDispatcher.RemoveListener(EventID.USE_BOOSTER, UseBooster, true);
            base.OnDestroy();
        }

        public override void Init()
        {
            claimBtn.onClick.RemoveAllListeners();
            claimBtn.onClick.AddListener(OnClickClaimBtn);
        }

        private void UseBooster(IMessage rMessage)
        {
            if (IsActive)
            {
                CloseView();
            }
        }

        protected override void Update()
        {
            if (Input.GetMouseButtonDown(0) && IsActive && !EventSystem.current.IsPointerOverGameObject())
            {
                CloseView();
            }
        }

        private void OnClickClaimBtn()
        {
            handTutorial.ShowAtUI(boosterBtn.GetComponent<RectTransform>());
            unlockPanel.SetActive(false);
            boosterBtn.OnClickInTut(OnClickBooster);
            tutorialHighlight.UpdateHoleForUI(boosterBtn.GetComponent<RectTransform>());
        }

        private void OnClickBooster()
        {
            switch (powerUpManager.Kind)
            {
                case PowerupKind.AddSlot:
                    CloseView();
                    break;
                case PowerupKind.Helidrop:
                    break;
                case PowerupKind.RainbowHole:
                    break;
            }
        }

        public void ShowTut(BoosterBtn boosterButton)
        {
            tutorialHighlight.Hide();
            handTutorial.Hide();
            unlockPanel.SetActive(true);
            boosterBtn     = boosterButton;
            powerUpManager = (PowerUpResource)boosterButton.Kind.ToResourceType().Manager();
            ShowVisual();
        }
        private void ShowVisual()
        {
            boosterImage.sprite = powerUpManager.GetIcon();
            boosterName.SetText($"{boosterBtn.Kind.GetDescription()}!");
        }

        protected override void CloseView()
        {
            var popupAtt = this.GetCustomAttribute<PopupAttribute>();

            WindowManager.Instance.CloseActivity(popupAtt.namePath, false);
        }

        public override void DidExit(Memory<object> args)
        {
            
            base.DidExit(args);
        }
    }
}