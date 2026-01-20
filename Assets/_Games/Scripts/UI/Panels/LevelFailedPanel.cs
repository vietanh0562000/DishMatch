namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using Cysharp.Threading.Tasks;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using BasePuzzle.PuzzlePackages.Core;
    using Spine.Unity;
    using TMPro;
    using UnityEngine;

    [Popup("LevelFailedPanel")]
    public class LevelFailedPanel : BasePopup
    {
        public TextMeshProUGUI title;
        public GameObject      btnHome;
        public GameObject      btnRetry;
        public TextMeshProUGUI txtContent;
        public TextMeshProUGUI txtButtonContent;
        public SkeletonGraphic heartSkeleton;

        private bool isRetryPopup;
        private bool isQuit;

        public override UniTask WillPushEnter(Memory<object> args)
        {
            heartSkeleton.AnimationState.SetAnimation(0, "Idle", true);
            heartSkeleton.AnimationState.AddAnimation(0, "Idle_broken", false, 0.5f);
            return base.WillPushEnter(args);
        }

        public override void Init()
        {
            isQuit = false;
            GameManager.Instance.Stop();
        }

        public override UniTask WillPopExit(Memory<object> args)
        {
            if (!isQuit)
            {
                GameManager.Instance.Continue();
            }
            return base.WillPopExit(args);
        }
        
        public void CloseLevelFailedPanel()
        {
            if (needGoHome)
            {
                Debug.LogError("Go home");
                Home();
                return;
            }

            WindowManager.Instance.CloseCurrentWindow();
        }

        private bool needGoHome;

        public void SetRetryPanel()
        {
            BaseInit();
            isRetryPopup = true;
            btnHome.SetActive(false);
            btnRetry.SetActive(true);
            txtContent.text       = "You will lose 1 life!";
            txtButtonContent.text = "Retry";
        }

        public void SetQuitPanel()
        {
            BaseInit();
            btnHome.SetActive(true);
            btnRetry.SetActive(false);
            txtContent.text       = "You will lose 1 life!";
            txtButtonContent.text = "Leave";
        }

        public void SetFailedPanel()
        {
            BaseInit();
            btnHome.SetActive(false);
            btnRetry.SetActive(true);
            txtContent.text       = "Failed!";
            txtButtonContent.text = "Retry";
            needGoHome            = true;
        }

        private void BaseInit()
        {
            isRetryPopup = false;
            needGoHome   = false;
            title.text   = "Level " + LevelManager.Instance.currentLevelToLog;
        }

        public void ButtonRetry()
        {
            Retry();
        }

        void Retry()
        {
            if (UserResourceController.instance.CanPlayLevel())
            {
                Action loadScene = () =>
                {
                    Destroy(PoolHolder.PoolTransform.gameObject);
                    LoadSceneManager.Instance.LoadScene("GamePlay");
                };

                if (ServerConfig.Instance<ValueRemoteConfig>().showInterstitialWhenRetry)
                {
                    GameManager.Instance.ShowInterstitials(loadScene, loadScene, "retry",
                        LevelManager.Instance.currentLevelToLog);
                }
                else
                {
                    loadScene.Invoke();
                }
            }
            else
            {
                WindowManager.Instance.OpenWindow<RefillPanel>(onLoaded: panel => { panel.SetInHome(false); });
            }
        }

        void Home()
        {
            isQuit = true;
            GameManager.Instance.QuitGame();
        }

        public void ButtonHome()
        {
            Home();
        }
    }
}