using System;
using BasePuzzle.PuzzlePackages.Core;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleGames
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks.Triggers;
    using DG.Tweening;
    using BasePuzzle.PuzzlePackages;
    using TMPro;

    public class HomeUI : MonoBehaviour
    {
        [SerializeField]           private LevelScroller _levelScroller;
        [SerializeField, Space(6)] private Image         _imgBtnPlay;
        [SerializeField, Space(6)] private TMP_Text      _txtPlay;

        [SerializeField] private List<Sprite> _bgBtn;
        [SerializeField] private List<Material> _textColors;

        private void OnEnable()
        {

            UpdateUI();
        }

        private void UpdateUI()
        {
            var level     = LevelDataController.instance.Level;
            var levelJson = LoadLevelManager.instance.ReadLevelData(level);
           // var levelData = JsonConvert.DeserializeObject<TxtLevelData>(levelJson);

            /*if(levelData==null)
                return;  */
            
            _imgBtnPlay.sprite   =GetBackgroundSprite(LevelDifficulty.Easy);
            _txtPlay.fontMaterial=GetFontColor(LevelDifficulty.Easy);

            /*if (levelData == null)
            {
                _levelScroller.UpdateUI(level, LevelDifficulty.Easy);
                return;
            }*/

            _levelScroller.UpdateUI(level, LevelDifficulty.Easy);
        }

        private Sprite GetBackgroundSprite(LevelDifficulty difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _bgBtn[0];
                case LevelDifficulty.Normal:
                    return _bgBtn[0];
                case LevelDifficulty.Hard:
                    return _bgBtn[1];
                case LevelDifficulty.VeryHard:
                    return _bgBtn[2];
                default:
                    return null;
            }
        }

        private Material GetFontColor(LevelDifficulty difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulty.Easy:
                    return _textColors[0];
                case LevelDifficulty.Normal:
                    return _textColors[0];
                case LevelDifficulty.Hard:
                    return _textColors[1];
                case LevelDifficulty.VeryHard:
                    return _textColors[2];
                default:
                    return null;
            }
        }

        public void ClickBtnPlay()
        {
            if (!UserResourceController.instance.CanPlayLevel())
            {
                WindowManager.Instance.OpenWindow<RefillPanel>(onLoaded: p => { p.SetInHome(true); });
                return;
            }
            var level = LevelDataController.instance.Level;
            AudioController.PlaySound(SoundKind.UIClickButton);
            LevelLoader.LoadLevel(level);
        }
    }
}