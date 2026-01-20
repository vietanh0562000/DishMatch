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

    [Popup("TooltipPanel", closeWhenClickOnBackdrop = true, showAnim = false)]
    public class TooltipPanel : BaseActivity
    {
        public Image           imgIcon;
        public TextMeshProUGUI txtTitle;
        public TextMeshProUGUI txtContent;

        public Action OnClose;

        public void ShowTooltip(UFOBtn boosterBtn)
        {
            ShowBaseTooltip(boosterBtn);

            OnClose = boosterBtn.UnuseBooster;
        }

        public void ShowTooltip(RainbowHoleBtn boosterBtn)
        {
            ShowBaseTooltip(boosterBtn);
            OnClose = boosterBtn.UnuseBooster;
        }

        public void ShowBaseTooltip(BoosterBtn boosterBtn)
        {
            var manager = (PowerUpResource)boosterBtn.Type.Manager();

            imgIcon.sprite  = manager.GetIcon();
            txtTitle.text   = boosterBtn.Kind.GetDescription();
            txtContent.text = manager.Detail;


                OnHideBanner(null);
            
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

        public override void DidExit(Memory<object> args)
        {
            base.DidExit(args);
            OnClose?.Invoke();
        }

        [SerializeField] private RectTransform _bgTrans;
        protected override void Awake()
        {
            MessageDispatcher.AddListener(EventID.SHOW_BANNER_ADS, OnShowBanner, true);
            MessageDispatcher.AddListener(EventID.HIDE_BANNER_ADS, OnHideBanner, true);
        }

        protected override void OnDestroy()
        {
            MessageDispatcher.RemoveListener(EventID.SHOW_BANNER_ADS, OnShowBanner, true);
            MessageDispatcher.RemoveListener(EventID.HIDE_BANNER_ADS, OnHideBanner, true);
        }

        private void OnShowBanner(IMessage rmessage)
        {
            var vector2 = _bgTrans.anchoredPosition;
            vector2.y                 = 165 + 130;
            _bgTrans.anchoredPosition = vector2;
        }

        private void OnHideBanner(IMessage rmessage)
        {
            var vector2 = _bgTrans.anchoredPosition;
            vector2.y                 = 130;
            _bgTrans.anchoredPosition = vector2;
        }
    }
}