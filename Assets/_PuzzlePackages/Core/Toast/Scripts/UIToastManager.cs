using DG.Tweening;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Core.UserData;
using TMPro;
using UnityEngine;

public class UIToastManager : PersistentSingleton<UIToastManager>
{
    public RectTransform rectPanel;
    public TextMeshProUGUI txtContent;

    public TextMeshProUGUI txtIDAndVer;
    private Vector2 _initPos;
    protected override void Awake()
    {
        base.Awake();
        
        rectPanel.gameObject.SetActive(false);
        _initPos = rectPanel.transform.position;
        // AdsManager.onNoRewardAdsToShow += ShowNoAds;

        UpdateInfo();

        UserDataManager.onLoginStateChange += OnLoginStateChange;
    }

    private void UpdateInfo()
    {
        if (AccountManager.instance.Code > 0)
        {
            txtIDAndVer.text = $"ID: {AccountManager.instance.Code} v{Application.version}";
        }
        else
        {
            txtIDAndVer.text = $"v{Application.version}";
        }
    }

    private void OnLoginStateChange(bool obj)
    {
        if (obj)
        {
            UpdateInfo();
        }
    }

    public void ShowNoAds()
    {
        var s = LocalizationHelper.GetTranslation("ads_not_available");
        Show(s);
    }

    public void ShowConnectionError()
    {
        Show("Connection Problem!");
    }

    private Tween _tween;
    private Tween _tween1;
    private Coroutine _coroutine;
    public void Show(string content)
    {
        if (_tween != null && _tween.IsActive())
        {
            _tween.Kill();
        }
        if (_tween1 != null && _tween1.IsActive())
        {
            _tween1.Kill();
        }
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        rectPanel.transform.position = _initPos;
        rectPanel.gameObject.SetActive(true);
        txtContent.SetText(content);

        //Tween
        _tween = txtContent.transform.DOScale(1, 0.2f).From(0.5f).SetUpdate(true);
        _coroutine = this.Delay(1.25f, ()=>
        {
            rectPanel.gameObject.SetActive(false);
        }, ignoreTimeScale: true);

        _tween1 = rectPanel.DOLocalMoveY(rectPanel.transform.localPosition.y + rectPanel.rect.height, 0.25f).SetUpdate(true);
    }
}
