using System;
using ChuongCustom;
using Cysharp.Threading.Tasks;
using BasePuzzle.PuzzlePackages;
using PuzzleGames;
using UnityEngine;

[Popup("PausePanel", closeWhenClickOnBackdrop = true)]
public class PausePanel : BasePopup
{
    private bool isQuit;

    public override void Init()
    {
        isQuit = false;
        GameManager.Instance.Stop();
    }

    public void ButtonQuit()
    {
        if (GameManager.Instance.IsPlayed)
        {
            WindowManager.Instance.OpenWindow<LevelFailedPanel>(onLoaded: panel => panel.SetQuitPanel());
        }
        else
        {
            Home();
        }
    }

    void Home()
    {
        isQuit = true;
        GameManager.Instance.QuitGame();
    }

    public override UniTask WillPopExit(Memory<object> args)
    {
        if (!isQuit)
        {
            GameManager.Instance.Continue();
        }
        
        return base.WillPopExit(args);
    }

    public void ClickBtnPrivacy() {; }

    public void ClickBtnSupport() { }
}