using UnityEngine;

namespace PuzzleGames
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using ChuongCustom;
	using ChuongCustom.ScreenManager;
	using Cysharp.Threading.Tasks;
	using DG.Tweening;
	using UnityEngine.UI;

	[Popup("RewardsBundlePanel")]
	public class RewardsBundlePanel : BaseActivity
	{
		[SerializeField] private DOTweenAnimation[] _tweenAnimations;
		[SerializeField] private Button             _claimBtn;
		[SerializeField] private GameObject         _rewardInfoGo;
		[SerializeField] private RectTransform      _rectHolder;
		[SerializeField] private LayoutByQuantity   _layoutByQuantity;
		[SerializeField] private Sprite             _noAds;
		[SerializeField] private float              _startShowAfter = 0.05f;
		[SerializeField] private float              _delay;

		private List<RectTransform> _rewardInfos    = new();
		private List<RewardFlyInfo> _rewardFlyInfos = new();

		private RectTransform _adsInfo;
		private Action     _onClosed;

		protected override void Awake()
		{
			base.Awake();
			
			_claimBtn.onClick.RemoveAllListeners();
			_claimBtn.onClick.AddListener(CloseView);
		}

		public void OnClosed(Action action) { _onClosed = action; }

		public void SetPurchasePackage(StandardPackBundle package)
		{
			ClearRewardInfos();

			foreach (var resource in package.Resources)
			{
				AddRewardInfo(resource);
			}

			if (package.RemoveAds)
			{
				AddAdsRewardInfo();
			}

			foreach (var infiResource in package.InfiResources)
			{
				AddRewardInfo(infiResource);
			}

			_layoutByQuantity.Layouts(_rewardInfos);
		}

		private void ClearRewardInfos()
		{
			_rewardInfos.Clear();
			_rewardFlyInfos.Clear();
			foreach (Transform go in _rectHolder)
			{
				Destroy(go.gameObject);
			}
		}

		private void AddRewardInfo(ResourceValue resource)
		{
			var rewardInfo = Instantiate(_rewardInfoGo, _rectHolder).GetComponent<RewardInfo>();
			rewardInfo.SetType(resource.type);
			rewardInfo.SetIcon(resource.type.Manager().GetIconWithAmount(resource.value));
			rewardInfo.SetValue(resource);
			rewardInfo.gameObject.SetActive(false);

			var rewardRect = rewardInfo.GetComponent<RectTransform>();
			_rewardInfos.Add(rewardRect);
			_rewardFlyInfos.Add(new RewardFlyInfo(resource, rewardRect));
		}

		private void AddAdsRewardInfo()
		{
			var rewardInfo = Instantiate(_rewardInfoGo, _rectHolder).GetComponent<RewardInfo>();
			_adsInfo = rewardInfo.GetComponent<RectTransform>();
			_rewardInfos.Add(_adsInfo);
			rewardInfo.SetIcon(_noAds);
			rewardInfo.gameObject.SetActive(false);
		}

		public override void Init()
		{
			base.Init();

			foreach (var tween in _tweenAnimations)
			{
				tween.DORestart();
			}
			StartCoroutine(Wait1s());

			IEnumerator Wait1s()
			{
				yield return new WaitForSecondsRealtime(0.3f);
				AudioController.PlaySound(SoundKind.InGamePreVictory);
			}
		}

		public override void DidEnter(Memory<object> args)
		{
			StartCoroutine(CoShowRewards());
			
			IEnumerator CoShowRewards()
			{
				yield return new WaitForSecondsRealtime(_startShowAfter);
				foreach (var reward in _rewardInfos)
				{
					yield return new WaitForSecondsRealtime(_delay);
					reward.gameObject.SetActive(true);
					reward.transform.localScale = Vector3.one;
					reward.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3).SetUpdate(true);
				}
			}
			
			base.DidEnter(args);
		}

		public override UniTask WillExit(Memory<object> args)
		{
			_ = FlyManager.Instance.ShowFlyExist(_rewardFlyInfos);

			foreach (var rewardFlyInfo in _rewardFlyInfos)
			{
				rewardFlyInfo.Rect.GetComponent<RewardInfo>().DisableBackground();
			}
			
			return base.WillExit(args);
		}

		public override void DidExit(Memory<object> args)
		{
			_onClosed?.Invoke();
			base.DidExit(args);
		}

		protected override void Update() { }
	}
}