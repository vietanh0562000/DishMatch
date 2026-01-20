namespace PuzzleGames
{
    using com.ootii.Messages;
    using UnityEngine;
    using UnityEngine.UI;

    public class BottomPanel : MonoBehaviour
    {
        [SerializeField] private RectTransform        _bgTrans;
        [SerializeField] private Image[]              _btnImgs;
        [SerializeField] private InGameDifficultyUiSo _difficultyUiSo;
        private void Awake()
        {
            MessageDispatcher.AddListener(EventID.SHOW_BANNER_ADS, OnShowBanner, true);
            MessageDispatcher.AddListener(EventID.HIDE_BANNER_ADS, OnHideBanner, true);
            MessageDispatcher.AddListener(EventID.SET_LEVEL_DIFFICULTY, OnSetLevelDiff, true);
        }

        private void OnDestroy()
        {
            MessageDispatcher.RemoveListener(EventID.SHOW_BANNER_ADS, OnShowBanner, true);
            MessageDispatcher.RemoveListener(EventID.HIDE_BANNER_ADS, OnHideBanner, true);
            MessageDispatcher.RemoveListener(EventID.SET_LEVEL_DIFFICULTY, OnSetLevelDiff, true);
        }

        private void OnShowBanner(IMessage rmessage)
        {
            var vector2 = _bgTrans.anchoredPosition;
            vector2.y                 = 165;
            _bgTrans.anchoredPosition = vector2;
        }

        private void OnHideBanner(IMessage rmessage)
        {
            var vector2 = _bgTrans.anchoredPosition;
            vector2.y                 = 0;
            _bgTrans.anchoredPosition = vector2;
        }

        private void OnSetLevelDiff(IMessage rmessage)
        {
            if (rmessage != null)
            {
                var difficulty = (LevelDifficulty)rmessage.Data;

                foreach (var img in _btnImgs)
                {
                    img.sprite = _difficultyUiSo.GetSpriteDifficulty("BoosterButton", difficulty);
                }
            }
        }
    }
}