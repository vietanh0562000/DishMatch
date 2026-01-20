using System;
using System.Threading.Tasks;
using ChuongCustom;
using Cysharp.Threading.Tasks;
using UnityEngine;

[Popup("PreShowAds")]
public class PreShowAdsPanel : BaseScreen
{
	[SerializeField] private GameObject _goTakeABreak;
	[SerializeField] private float _delayHide = 2f;

	private Action _onClose;

	public void Setup(Action onClose) { _onClose = onClose; }

	public override void DidPushEnter(Memory<object> args)
	{
		AutoHide().Forget();

		base.DidPushEnter(args);
	}

	private async UniTask AutoHide()
	{
		await UniTask.Delay(TimeSpan.FromSeconds(_delayHide));

		_goTakeABreak.SetActive(false);
		_onClose?.Invoke();
	}
}