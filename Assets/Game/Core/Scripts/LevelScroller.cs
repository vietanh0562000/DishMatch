using BasePuzzle.PuzzlePackages.Core;
using TMPro;
using UnityEngine;

namespace PuzzleGames
{
    using System;
    using System.Collections;
    using Newtonsoft.Json;
    using UnityEngine.Serialization;
    using UnityEngine.UI;

    public class LevelScroller : MonoBehaviour
    {
        [SerializeField] private ScrollingLevelItem[] _levelItems;
        [SerializeField] private RectTransform        _scrollRectContent;
        [SerializeField] private Sprite[]             _unlockBgSprite;
        [SerializeField] private Color[]              _colorText;
        [SerializeField] private Color[]              _colorStar;
        [SerializeField] private Sprite               _lockBgSprite;
        [SerializeField] private Image               _starImg;
        [SerializeField] private TMP_Text             _btnPlayTxt;

        [SerializeField] private GameObject[] _skullIcon;

        //[SerializeField] private GameObject[] _fxDiff;
        [SerializeField] private GameObject _levelPassed;
        [SerializeField] private ScrollRect _rect;
        [SerializeField] private float      _scrollBackSpeed  = 5f;
        [SerializeField] private float      _timeToAutoScroll = 1f;

        private float _originalHighlightHeight;

        private Coroutine scrollCoroutine;

        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(_timeToAutoScroll);
            /*foreach (var fx in _fxDiff)
            {
                fx.SetActive(false);
            }*/
        }

        public void UpdateUI(int currentLevel, LevelDifficulty difficulty)
        {
            /*var localizedLevel = LocalizationHelper.GetTranslation(LocalizationTerm.LEVEL_NUMBER)
                .Replace1(currentLevel.ToString());

            _btnPlayTxt.text = localizedLevel + $"\n{GetLocalizedDifficulty(difficulty)}";*/

            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    _btnPlayTxt.text = $"Level {currentLevel}";
                    break;
                case LevelDifficulty.Normal:
                    _btnPlayTxt.text = $"Level {currentLevel}";
                    break;
                case LevelDifficulty.Hard:
                    _btnPlayTxt.text = $"<size=50>Hard</size>\nLevel {currentLevel}";
                    break;
                case LevelDifficulty.VeryHard:
                    _btnPlayTxt.text = $"<size=50>Super Hard</size>\nLevel {currentLevel}";
                    break;
            }
            
            _starImg.color = GetColorStar(difficulty);

            foreach (var icon in _skullIcon)
            {
                icon.SetActive(difficulty == LevelDifficulty.VeryHard);
            }

            for (int i = 0; i < _levelItems.Length; i++)
            {
                var level = currentLevel + i;

                var levelJson = LoadLevelManager.instance.ReadLevelData(level);
               // var levelData = JsonConvert.DeserializeObject<TxtLevelData>(levelJson);

                var diff = LevelDifficulty.Easy;

                _levelItems[i].SetLevel(GetBackgroundSprite(diff), level, GetColorText(diff));

                _levelItems[i].ShowLockIcon(i != 0);
            }

            //SetDifficultyFx(difficulty).SetActive(true);
            //_levelPassed.SetActive(true);
        }

        private string GetLocalizedDifficulty(LevelDifficulty difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Normal:
                    return LocalizationHelper.GetTranslation(LocalizationTerm.NORMAL);
                case LevelDifficulty.Hard:
                    return LocalizationHelper.GetTranslation(LocalizationTerm.HARD);
                case LevelDifficulty.VeryHard:
                    return LocalizationHelper.GetTranslation(LocalizationTerm.VERY_HARD);
                default:
                    return string.Empty;
            }
        }

        private Sprite GetBackgroundSprite(LevelDifficulty? difficulty)
        {
            if (difficulty == null) return _lockBgSprite;

            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _unlockBgSprite[0];
                case LevelDifficulty.Normal:
                    return _unlockBgSprite[0];
                case LevelDifficulty.Hard:
                    return _unlockBgSprite[1];
                case LevelDifficulty.VeryHard:
                    return _unlockBgSprite[2];
                default:
                    return _unlockBgSprite[0];
            }
        }

        private Color GetColorText(LevelDifficulty? difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _colorText[0];
                case LevelDifficulty.Normal:
                    return _colorText[0];
                case LevelDifficulty.Hard:
                    return _colorText[1];
                case LevelDifficulty.VeryHard:
                    return _colorText[2];
                default:
                    return _colorText[0];
            }
        }
        
        private Color GetColorStar(LevelDifficulty? difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _colorStar[0];
                case LevelDifficulty.Normal:
                    return _colorStar[0];
                case LevelDifficulty.Hard:
                    return _colorStar[1];
                case LevelDifficulty.VeryHard:
                    return _colorStar[2];
                default:
                    return _colorStar[0];
            }
        }

        /*private GameObject SetDifficultyFx(LevelDifficulty difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _fxDiff[0];
                case LevelDifficulty.Normal:
                    return _fxDiff[0];;
                case LevelDifficulty.Hard:
                    return _fxDiff[1];
                case LevelDifficulty.VeryHard:
                    return _fxDiff[2];;
                default:
                    return _fxDiff[0];
            }
        }*/

        public void OnDrag()
        {
            if (scrollCoroutine != null)
                StopCoroutine(scrollCoroutine);
        }

        public void OnEndDrag()
        {
            if (scrollCoroutine != null)
                StopCoroutine(scrollCoroutine);

            scrollCoroutine = StartCoroutine(SmoothScrollToTop());
        }

        private IEnumerator SmoothScrollToTop()
        {
            yield return _waitForSeconds;

            while (_rect.verticalNormalizedPosition > 0f)
            {
                _rect.verticalNormalizedPosition = Mathf.Lerp(
                    _rect.verticalNormalizedPosition, 0f, Time.deltaTime * _scrollBackSpeed);
                yield return null;
            }

            _rect.verticalNormalizedPosition = 0f;
        }
    }
}