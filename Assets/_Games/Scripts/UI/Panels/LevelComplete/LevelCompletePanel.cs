using System;
using System.Collections;
using ChuongCustom;
using com.ootii.Messages;
using DG.Tweening;
using PuzzleGames;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Popup("LevelCompletePanel")]
public class LevelCompletePanel : BaseScreen
{
    public Button btnAds;
    public Button btnReroll;
    public Button btnTapToContinue;
    public Button btnContinue;

    public TMP_Text txtCoinReward;

    public SkeletonGraphic spineComplete;
    public WheelOfFortune  WheelOfFortune;

    public override void Init()
    {
        MessageDispatcher.SendMessage(EventID.DISABLE_BTN_SHOP, 0);
        
        OnShowLevelComplete();
    }

    public override void DidPushEnter(Memory<object> args)
    {
        spineComplete.gameObject.SetActive(true);
        spineComplete.AnimationState.ClearTracks();
        spineComplete.AnimationState.SetAnimation(0, "appear", false);
        spineComplete.AnimationState.AddAnimation(0, "idle", true, 0);

        base.DidPushEnter(args);
    }

    private Tween _onIncreaseGold;
    private void OnVisualIncreaseGold(int targetGold)
    {
        var gold = WheelOfFortune.BaseReward;
        
        _onIncreaseGold.Kill();
        _onIncreaseGold = DOTween.To(() => gold, x =>
        {
            gold = x;

            txtCoinReward.SetText($"+{gold} <sprite name=\"coin\">");

        }, targetGold, 1.5f).OnComplete(() =>
        {
            AudioController.PlaySound(SoundKind.UIUpgradeGoldPack);
            txtCoinReward.SetText($"+{WheelOfFortune.Reward}<sprite name=\"coin\">");
            txtCoinReward.transform.localScale = Vector3.one;
            txtCoinReward.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
        }).SetDelay(1f);
    }

    private void OnRollEnded(bool isGreater, bool isEnd)
    {
        if (!isGreater)
        {
            txtCoinReward.SetText($"+{WheelOfFortune.Reward}<sprite name=\"coin\">");
            txtCoinReward.transform.localScale = Vector3.one;
            txtCoinReward.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
        }
        
        btnContinue.gameObject.SetActive(isEnd);
        btnReroll.gameObject.SetActive(!isEnd);
        btnTapToContinue.gameObject.SetActive(!isEnd);
    }

    private void OnClickEndRoll(bool canReroll)
    {
        _onIncreaseGold.Kill();
        btnReroll.gameObject.SetActive(canReroll);
        btnTapToContinue.gameObject.SetActive(canReroll);
        btnContinue.gameObject.SetActive(!canReroll);

        txtCoinReward.SetText($"+{WheelOfFortune.Reward}<sprite name=\"coin\">");
        txtCoinReward.transform.localScale = Vector3.one;
        txtCoinReward.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
    }

    private void OnShowLevelComplete()
    {
        WheelOfFortune.SetAction(OnRollEnded, OnClickEndRoll, OnVisualIncreaseGold);
        txtCoinReward.SetText("");

        btnAds.onClick.RemoveAllListeners();
        btnAds.onClick.AddListener(() =>
        {
            WheelOfFortune.ShowAdsToGetMultipleCoin();
            btnAds.gameObject.SetActive(false);
            btnTapToContinue.gameObject.SetActive(false);
        });
        btnReroll.onClick.RemoveAllListeners();
        btnReroll.onClick.AddListener(() =>
        {
            WheelOfFortune.OnRerollClick();
            btnReroll.gameObject.SetActive(false);
            btnTapToContinue.gameObject.SetActive(false);
        });
        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(ButtonContinue);
        btnTapToContinue.onClick.RemoveAllListeners();
        btnTapToContinue.onClick.AddListener(ButtonContinue);

        StartCoroutine(Wait1s());

        IEnumerator Wait1s()
        {
            if (!UserResourceController.instance.IsInfiHeart())
            {
                UserResourceController.instance.AddHeart(1);
            }

            yield return new WaitForSecondsRealtime(0.3f);
            AudioController.PlaySound(SoundKind.InGamePreVictory);

            btnAds.gameObject.SetActive(false);
            btnContinue.gameObject.SetActive(false);
            txtCoinReward.SetText($"+{WheelOfFortune.Reward}<sprite name=\"coin\">");
            if (WheelOfFortune.CanShowMultipleFortune)
            {
                WheelOfFortune.ShowWheelOfFortune();

                btnAds.transform.localScale = Vector3.zero;
                btnAds.gameObject.SetActive(true);
                btnAds.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetUpdate(true).OnComplete(() =>
                {
                    btnAds.transform.DOScale(Vector3.one * 1.1f, 1.5f).From(1).SetEase(Ease.InOutBack).SetLoops(-1, LoopType.Yoyo);
                });

                btnTapToContinue.transform.localScale = Vector3.zero;
                btnTapToContinue.gameObject.SetActive(true);
                btnTapToContinue.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetUpdate(true);
            }
            else
            {
                btnContinue.transform.localScale = Vector3.zero;
                btnContinue.gameObject.SetActive(true);
                btnContinue.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetUpdate(true);
            }
        }
    }

    public void ButtonContinue()
    {
        AudioController.Instance.StopMusic();
        ResourceType.Gold.Manager()?.Add(WheelOfFortune.Reward);
        FlyManager.Instance.ShowFly(ResourceType.Gold, WheelOfFortune.Reward
            , txtCoinReward.transform.parent.GetComponent<RectTransform>());
        StartCoroutine(ContinueGame());

        btnAds.gameObject.SetActive(false);
        btnReroll.gameObject.SetActive(false);
        btnContinue.gameObject.SetActive(false);
        btnTapToContinue.gameObject.SetActive(false);
    }

    private IEnumerator ContinueGame()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        
        CloseView();
    }

    public override void DidPopExit(Memory<object> args)
    {
        GameManager.Instance.EndGame(WheelOfFortune.BaseReward < WheelOfFortune.Reward);
        base.DidPopExit(args);
    }
}