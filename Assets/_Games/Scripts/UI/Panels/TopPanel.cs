using System;
using System.Collections;
using com.ootii.Messages;
using PuzzleGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{
    public TextMeshProUGUI _levelTMP, _goldTMP;
    public Image           _levelImg, _goldImg, _retryImg;

    public InGameDifficultyUiSo DifficultySprites;

    private void Awake() { MessageDispatcher.AddListener(EventID.SET_LEVEL_DIFFICULTY, SetLevelDiff, true); }

    private IEnumerator Start()
    {
        yield return null;
        _levelTMP.SetText($"Lv.{LevelManager.Instance.currentLevelToLog}");
    }

    public void SetLevelDiff(IMessage rmessange)
    {
        if (rmessange != null)
        {
            var levelDiff = (LevelDifficulty)rmessange.Data;

            var mat = DifficultySprites.GetMaterialDifficulty("TopPanel", levelDiff);
            _levelTMP.fontMaterial = mat;
            _goldTMP.fontMaterial  = mat;


            var sprite = DifficultySprites.GetSpriteDifficulty("TopBG", levelDiff);
            _levelImg.sprite = sprite;
            _goldImg.sprite  = sprite;

            _retryImg.sprite = DifficultySprites.GetSpriteDifficulty("RetryButton", levelDiff);
        }
    }

    private void OnDestroy() { MessageDispatcher.RemoveListener(EventID.SET_LEVEL_DIFFICULTY, SetLevelDiff, true); }
}