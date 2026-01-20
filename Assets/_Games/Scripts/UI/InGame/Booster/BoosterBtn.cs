using UnityEngine;
using UnityEngine.UI; // Required for UI elements

namespace PuzzleGames
{
    using System;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages;
    using BasePuzzle.PuzzlePackages.Core;

    public abstract class BoosterBtn : AResourceUI
    {
        public Canvas      canvasOrder;
        public PowerupKind Kind => Type.ToPowerUp();

        public BoosterBtnVisual boosterButton;

        public ParticleSystem fx;

        // Number of available boosters (tracked for this type of booster)
        public int BoosterCount => Type.Manager().GetAmount();

        // Method to be implemented by subclasses for specific booster behavior
        public abstract void ActivateBooster();

        public abstract int LevelUnlock { get; }

        public bool IsAvailable => LevelDataController.instance.Level >= LevelUnlock;

        public bool InTutorial => LevelDataController.instance.Level == LevelUnlock;

        private void OnValidate() { boosterButton = GetComponent<BoosterBtnVisual>(); }

        private Action onClickInTut;

        // Awake is called when the instance is initialized
        private void Awake()
        {
            UpdateVisual();
            UpdateCount();

            if (boosterButton)
            {
                boosterButton.Btn.onClick.AddListener(OnClickButton);
            }
            else
            {
                Debug.LogError($"Button not assigned for {Kind}");
            }
        }

        protected override void Start()
        {
            base.Start();

            DOVirtual.DelayedCall(0.5f, ShowTutorialPanel);
        }

        private void ShowTutorialPanel()
        {
            var isFirstLevel = LevelManager.Instance.currentLevelToLog <= 1;
            
#if ACCOUNT_TEST
            if (InTutorial && !isFirstLevel)
            {
                AdsManager.Instance.HideBanner();
                WindowManager.Instance.OpenWindow<BoosterUnlockPanel>(onLoaded: panel => { panel.ShowTut(this); });
                return;
            }
#endif
            if (InTutorial && PowerUpDataController.instance.IsFree(Kind)
                           && LevelDataController.instance.IsFirstTryLevel && !isFirstLevel)
            {
                WindowManager.Instance.OpenWindow<BoosterUnlockPanel>(onLoaded: panel => { panel.ShowTut(this); });
            }
        }

        private void UpdateVisual()
        {
            var resourceManager = Type.Manager();

            boosterButton.IconImage.sprite = resourceManager.GetIcon();
            //boosterButton.IconImage.SetNativeSize();

            if (!IsAvailable)
            {
                boosterButton.LevelUnlockTMP.SetText($"Lv.{LevelUnlock}");
            }

            boosterButton.SetAvailable(IsAvailable);
        }

        protected virtual void OnClickButton()
        {
            HapticController.instance.Play();
            
            if (!IsAvailable)
            {
                return;
            }

            if (onClickInTut != null)
            {
                onClickInTut.Invoke();
                onClickInTut = null;
            }

            if (BoosterCount > 0)
            {
                ActivateBooster();
                UpdateCount();
            }
            else
            {
                WindowManager.Instance.OpenWindow<BuyBoosterPanel>(onLoaded: panel => { panel.BuyBooster(Kind); });
            }
        }

        protected void MinusPowerUpCount() { Type.Manager()?.Subtract(1); }

        protected virtual void UpdateCount()
        {
            boosterButton.UpdateBoosterCount(BoosterCount);
            boosterButton.AddObj.SetActive(BoosterCount <= 0 && IsAvailable);
        }

        public override void OnReachUI(bool isLast)
        {
            if (fx)
            {
                fx.Stop();
                fx.Play();
            }
            
            if (isLast)
            {
                HapticController.instance.Play();
                UpdateCount();

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    if (canvasOrder.overrideSorting)
                    {
                        canvasOrder.overrideSorting = false;
                    }
                });
            }

            AudioController.PlaySound(SoundKind.UIRecivedItem);
        }

        public override void EnableCanvas()
        {
            if (!canvasOrder.overrideSorting)
            {
                canvasOrder.overrideSorting = true;
            }
        }

        public override void UpdateUI() { UpdateCount(); }

        public void OnClickInTut(Action action) { onClickInTut = action; }
    }
}