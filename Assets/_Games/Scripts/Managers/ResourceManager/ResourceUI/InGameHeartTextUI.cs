using UnityEngine;

namespace PuzzleGames
{
	using System;
	using DG.Tweening;
	using TMPro;
	using UnityEngine.UI;

	public class InGameHeartTextUI : AResourceUI
	{
		public                      ParticleSystem  fx;
		[Header("Get More")] public Button          btnGetMoreHearth;
		public                      TextMeshProUGUI txtHearth;

		[Header("Infinite & Timer")] public RectTransform       rectHearthInfinite;
		public                              RectTransform       rectIconHearthPlus;
		public                              UITimerCountdownTMP timerHearth;
		public                              Canvas              canvasOrder;

		public Action onLastUpdate;

		private void UpdateUIHeart()
		{
			if (UserResourceController.instance.IsInfiHeart())
			{
				txtHearth.gameObject.SetActive(false);
				rectHearthInfinite.gameObject.SetActive(true);
				rectIconHearthPlus.gameObject.SetActive(false);

				timerHearth.onStopTimer = () => this.Delay(1, UpdateUI);
				timerHearth.StartTimer(UserResourceController.instance.GetTargetTimeSpanInfiHearth());
			}
			else
			{
				txtHearth.gameObject.SetActive(true);
				rectHearthInfinite.gameObject.SetActive(false);
				rectIconHearthPlus.gameObject.SetActive(false);

				var rs = UserResourceController.instance.UserResource;
				txtHearth.SetText($"{rs.heart}");

				if (UserResourceController.instance.IsMaxHeart())
				{
					timerHearth.onStopTimer = null;
					timerHearth.Stop();
					timerHearth.GetTimeTextTMP().SetText("Full");
				}
				else
				{
					rectIconHearthPlus.gameObject.SetActive(true);
					timerHearth.onStopTimer = () => this.Delay(1, UpdateUI);
					timerHearth.StartTimer(UserResourceController.instance.TimeSpanUpdateHeart);
				}
			}
		}

		public override ResourceType Type => ResourceType.Heart;

		protected override void Start()
		{
			base.Start();
			btnGetMoreHearth.onClick.RemoveAllListeners();
			btnGetMoreHearth.onClick.AddListener(() =>
			{
				if (UserResourceController.instance.IsInfiHeart() || UserResourceController.instance.IsMaxHeart())
				{
				}
				else
				{
					ShowPopupFreeLive();
				}
			});
		}

		private void ShowPopupFreeLive()
		{
			WindowManager.Instance.OpenWindow<RefillPanel>(onLoaded: p => { p.SetInHome(true); });
		}

		public override void OnReachUI(bool isLast)
		{
			if (fx)
				fx.Play();
			
			HapticController.instance.Play();
			
			if (isLast)
			{
				UpdateUI();
				onLastUpdate?.Invoke();

				DOVirtual.DelayedCall(0.5f, () =>
				{
					if (canvasOrder.overrideSorting)
					{
						canvasOrder.overrideSorting = false;
					}
				});
			}
			
			AudioController.PlaySound(SoundKind.UIRecivedItem);
		}

		public override void EnableCanvas()
		{
			if (!canvasOrder.overrideSorting)
			{
				canvasOrder.overrideSorting = true;
			}
		}

		public override void UpdateUI() { UpdateUIHeart(); }
	}
}