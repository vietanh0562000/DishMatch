namespace PuzzleGames
{
    using System;
    using ChuongCustom;
    using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
    using UnityEngine;

    public class RateData
    {
        public bool isRated;
        public int  levelShow;
    }

    [Popup("RatePopup", closeWhenClickOnBackdrop = false)]
    public class RatePopup : BasePopup
    {
        public GameObject rateObj;
        public GameObject thankObj;
        //public StarTween  starTween;

        private bool waitingForRate = false;

        protected override void OnEnable()
        {
            base.OnEnable();
            rateObj.SetActive(true);
            thankObj.SetActive(false);
            // starTween.Init();
        }

        public override void Init()
        {
            base.Init();
            //  starTween.ShowStars();
        }

        private const string IsRatedKey = "rateData";

        public static bool IsRated
        {
            get
            {
                if (!SaveLoadHandler.Exist(IsRatedKey)) return false;

                var rateData = SaveLoadHandler.Load<RateData>(IsRatedKey);

                return rateData?.isRated ?? false;
            }
        }

        public static int LevelShowed
        {
            get
            {
                if (!SaveLoadHandler.Exist(IsRatedKey)) return 0;

                var rateData = SaveLoadHandler.Load<RateData>(IsRatedKey);

                return rateData?.levelShow ?? 0;
            }
        }

        private void Rate()
        {
            thankObj.SetActive(true);
            rateObj.SetActive(false);
            SaveLoadHandler.Save(IsRatedKey, new RateData()
            {
                levelShow = LevelDataController.instance.Level,
                isRated   = true
            });
        }

        public void OnClickRate()
        {
#if UNITY_EDITOR
            Rate();
            return;
#endif

            DirectlyOpen();
            waitingForRate = true;
        }


        private void DirectlyOpen() { Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}"); }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (waitingForRate && hasFocus)
            {
                Rate();
                waitingForRate = false;
            }
        }

        public void OnClickClose()
        {
            SaveLoadHandler.Save(IsRatedKey, new RateData()
            {
                levelShow = LevelDataController.instance.Level,
                isRated   = false
            });

            CloseView();
        }
    }

    public class RateGameCondition : APopupCondition
    {
        public override bool CanStart()
        {
            var levelRate1 = ServerConfig.Instance<ValueRemoteConfig>().levelRate1 + 1;
            var levelRate2 = ServerConfig.Instance<ValueRemoteConfig>().levelRate2 + 1;

            var currentLevel = LevelDataController.instance.Level;

#if UNITY_EDITOR
            return currentLevel == levelRate1;
#endif

            var check1 = (currentLevel == levelRate1 && RatePopup.LevelShowed != levelRate1);
            var check2 = (currentLevel == levelRate2 && RatePopup.LevelShowed != levelRate2);

            return !RatePopup.IsRated && (check1 || check2);
        }

        public override Type ScreenType => typeof(RatePopup);
    }
}