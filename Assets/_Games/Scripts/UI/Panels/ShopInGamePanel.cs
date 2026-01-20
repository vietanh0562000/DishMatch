using System;
using ChuongCustom;
using Cysharp.Threading.Tasks;
using BasePuzzle.PuzzlePackages;
using PuzzleGames;
using UnityEngine;
using UnityEngine.UI;

[Popup("ShopInGamePanel")]
public class ShopInGamePanel : BasePopup
{
    [SerializeField] private ScrollRect _rect;
    [SerializeField] private GameObject _moreOffer, _goldPacks;
    [SerializeField] private GoldTextUI _goldTextUI;
    public override void Init()
    {
        base.Init();
        
        _goldTextUI.Push();
        _goldTextUI.UpdateUI();
        _moreOffer.SetActive(true);
        _goldPacks.SetActive(false);
        
        _rect.verticalNormalizedPosition = 1;
        
        GameManager.Instance.Stop();
    }

    public override UniTask WillPopExit(Memory<object> args)
    {
        _goldTextUI.Pop();
        GameManager.Instance.Continue();
        return base.WillPopExit(args);
    }
}