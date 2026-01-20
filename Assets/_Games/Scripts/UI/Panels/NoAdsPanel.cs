using System;
using System.Collections;
using ChuongCustom;
using DG.Tweening;
using PuzzleGames;
using UnityEngine;
using UnityEngine.UI;

[Popup("Popup_NoAds", closeWhenClickOnBackdrop = false)]
public class NoAdsPanel : BasePopup
{
    [SerializeField] private ShopPackSO      _shopPack;
    [SerializeField] private PurchaseID      _adsBundle;
    [SerializeField] private PurchasePackage _purchasePackage;
    [SerializeField] private Button          _btnBuy;

    private Action _onclose;

    private bool inGame;

    public void SetInGame()
    {
        inGame = true;
    }
    
    protected override void Awake()
    {
        base.Awake();

        _btnBuy.onClick.RemoveAllListeners();
        _btnBuy.onClick.AddListener(OnClickBtnBuy);
    }

    private void OnClickBtnBuy()
    {
        _purchasePackage.OnSuccess(() =>
        {
            _btnBuy.gameObject.SetActive(false);

            WindowManager.Instance.OpenWindow<RewardsBundlePanel>(onLoaded: panel =>
            {
                panel.OnClosed(() =>
                {
                    if (inGame)
                    {
                        DOVirtual.DelayedCall(5.5f, CloseView);
                    }
                    else
                        CloseView();
                });
                panel.SetPurchasePackage(_purchasePackage._bundle);
            });
        });
        _purchasePackage.ClickBtnPurchase();
    }

    public override void Init()
    {
        base.Init();
        inGame = false;
        _btnBuy.gameObject.SetActive(true);

        var adsBundle = _shopPack.GetStandardPack(_adsBundle);
        _purchasePackage.SetPack(adsBundle);
    }

    public void SetActionOnClose(Action action) { _onclose = action; }

    public override void DidPopExit(Memory<object> args)
    {
        _onclose?.Invoke();

        base.DidPopExit(args);
    }
}