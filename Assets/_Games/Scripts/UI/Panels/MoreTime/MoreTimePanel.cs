using System;
using ChuongCustom;
using DG.Tweening;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using PuzzleGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Popup("MoreTimePanel")]
public class MoreTimePanel : BasePopup
{
	private int _price = 500;

	public FailedPackage    failedPackage;
	public RectTransform    popupRect;
	public InGameGoldTextUI goldTextUI;
	public InGameHeartTextUI heartTextUI;
	public TextMeshProUGUI  textContinue;
	public Button           btnContinue;

	[FoldoutGroup("Text Material")] [SerializeField]
	private Material _canBuy, _cantBuy;

	public override void Init()
	{
		base.Init();

		goldTextUI.Push();
		heartTextUI.Push();
		_price = ServerConfig.Instance<ValueRemoteConfig>().revivePrice;
		
		if (!LevelDataController.instance.IsAvailableForBuySupportPack())
		{
			popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, 50);
			failedPackage.Hide();
		}
		else
		{
			popupRect.anchoredPosition = new Vector2(popupRect.anchoredPosition.x, 270);
			failedPackage.Show(() =>
			{
				goldTextUI.Pop();
				heartTextUI.Pop();
				CloseView();
				GameManager.Instance.ReviveGame();
				LevelDataController.instance.ResetLostBuy();
			});
		}

		ShowTextContinue();
		btnContinue.onClick.RemoveAllListeners();
		btnContinue.onClick.AddListener(OnclickRevive);

		UserResourceController.onAddGold -= i => { ShowTextContinue(); };
		UserResourceController.onAddGold += i => { ShowTextContinue(); };
	}

	public override void DidPopEnter(Memory<object> args)
	{
		UserResourceController.onAddGold -= i => { ShowTextContinue(); };

		base.DidPopExit(args);
	}

	private void OnclickRevive()
	{
		if (UserResourceController.instance.UserResource.gold >= _price)
		{
			UserResourceController.instance.MinusGold(_price, "revive_in_game", "revive_in_game");
			GameManager.Instance.ReviveGame();
			goldTextUI.UpdateUI();
			goldTextUI.Pop();
			heartTextUI.Pop();
			CloseView();
		}
		else
		{
			//open shop
			WindowManager.Instance.OpenWindow<ShopInGamePanel>();
		}
	}

	private void ShowTextContinue()
	{
		if (UserResourceController.instance.UserResource.gold >= _price)
		{
			textContinue.fontMaterial = _canBuy;
			textContinue.text         = "<sprite name=\"coin\"> " + _price;
		}
		else
		{
			textContinue.fontMaterial = _cantBuy;
			textContinue.text         = "<sprite name=\"coin\"> <color=\"red\">" + _price + "</color>";
		}
	}

	public void ClosePanel()
	{
		WindowManager.Instance.CloseCurrentWindow(false);
		WindowManager.Instance.OpenWindow<LevelFailedPanel>(onLoaded: panel => { panel.SetFailedPanel(); });
	}
}