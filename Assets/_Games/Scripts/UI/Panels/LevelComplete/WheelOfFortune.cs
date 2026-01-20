namespace PuzzleGames
{
	using System;
	using System.Collections;
	using UnityEngine;
	using System.Collections.Generic;
	using DG.Tweening;
	using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
	using BasePuzzle.PuzzlePackages;
	using TMPro;
	using UnityEngine.UI;
	using Random = UnityEngine.Random;

	public class WheelOfFortune : MonoBehaviour
	{
		[SerializeField] private GoldTextUI           _goldTextUI;
		[SerializeField] private List<Sprite>         _golds;
		[SerializeField] private List<ParticleSystem> _fxMultiply;
		[SerializeField] private RectTransform        _multiplyTransform;

		[SerializeField] private Image          _goldImg;
		[SerializeField] private TMP_Text       _txtMultiCoin;
		[SerializeField] private Slider         _multiplierSlider;
		[SerializeField] private GameObject     _fxLoop;
		[SerializeField] private GameObject     _goGoldMove;
		[SerializeField] private AnimationCurve _animationCurve;
		[SerializeField] private float          _duration = 1f;
		[SerializeField] private int            _loop     = 10;

		private readonly List<MultiplierChance> multiplierChances = new();

		private float _currentMultiplier = 1f;
		private bool  _hasRerolled       = false;
		private bool  _isClickRoll       = false;
		private bool  _isSpinning        = false;
		private int   _baseReward;
		private int   _lastFxIndex  = -1;
		private int   _currentIndex = 0;
		private int   _reward;

		private Tweener _tweener;
		private Tweener _animTween;

		private Action<bool, bool> _onRolled;
		private Action<bool> _onClickEndRoll;
		private Action<int>  _onIncrease;

		private readonly WaitForSeconds _waitForSeconds = new WaitForSeconds(0.2f);

		public int BaseReward => _baseReward;
		public int Reward => _reward;

		private void Awake()
		{
			multiplierChances.Clear();
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 1.5f, chance = ServerConfig.Instance<ValueRemoteConfig>().x150Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 2f, chance = ServerConfig.Instance<ValueRemoteConfig>().x200Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 3f, chance = ServerConfig.Instance<ValueRemoteConfig>().x300Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 4f, chance = ServerConfig.Instance<ValueRemoteConfig>().x400Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 5f, chance = ServerConfig.Instance<ValueRemoteConfig>().x500Chance });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 4f, chance = ServerConfig.Instance<ValueRemoteConfig>().x400Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 3f, chance = ServerConfig.Instance<ValueRemoteConfig>().x300Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 2f, chance = ServerConfig.Instance<ValueRemoteConfig>().x200Chance / 2 });
			multiplierChances.Add(new MultiplierChance
				{ multiplier = 1.5f, chance = ServerConfig.Instance<ValueRemoteConfig>().x150Chance / 2 });

			_multiplyTransform.gameObject.SetActive(false);
			_txtMultiCoin.SetText("Roll to get more Gold!");

			_baseReward = ServerConfig.Instance<ValueRemoteConfig>().coinRewardPassLevel;
			_reward    = (int)(_baseReward * _currentMultiplier);
		}

		public void SetAction(Action<bool, bool> onRolled, Action<bool> onEndRoll, Action<int> onIncrease)
		{
			_onRolled       = onRolled;
			_onClickEndRoll = onEndRoll;
			_onIncrease     = onIncrease;
		}

		public bool CanShowMultipleFortune => LevelDataController.instance.GetLevelJustPassed() >=
		                                      ServerConfig.Instance<ValueRemoteConfig>().numLevelToShowRewarded;

		public void ShowWheelOfFortune()
		{
			_multiplyTransform.localScale = Vector3.zero;
			_multiplyTransform.gameObject.SetActive(true);
			_multiplyTransform.DOScale(Vector3.one, 0.5f).From(Vector3.zero).SetUpdate(true)
				.OnComplete(() => { StartCoroutine(CoFxMultiply()); });
		}

		private IEnumerator CoFxMultiply()
		{
			AudioController.Instance.PlayMusic(1, 0.35f);
			int i = 0;
			while (Mathf.Approximately(_currentMultiplier, 1) && !_isClickRoll)
			{
				_fxMultiply[i].Play();
				i = i == _fxMultiply.Count - 1 ? 0 : ++i;

				yield return _waitForSeconds;
			}
		}

		public void ShowAdsToGetMultipleCoin()
		{
			AudioController.Instance.StopMusic();

			_isClickRoll = true;
			//show ads
		
		}

		private void ShowMultiplierPanel()
		{
			_tweener.Kill();
			_multiplyTransform.gameObject.SetActive(true);
			_hasRerolled = false;
			RollMultiplier();
		}

		public void Continue()
		{
			_isClickRoll = true;

			GameManager.Instance.EndGame(_baseReward < _reward);
		}
		private void RollMultiplier()
		{
			_txtMultiCoin.SetText("");
			_fxLoop.SetActive(false);
			var sum = 0f;
			for (int i = 0; i < multiplierChances.Count; i++)
			{
				sum += multiplierChances[i].chance;
			}

			float random     = Random.Range(0, sum);
			float cumulative = 0f;

			for (int i = 0; i < multiplierChances.Count; i++)
			{
				cumulative += multiplierChances[i].chance;
				if (random <= cumulative)
				{
					bool isGreater = multiplierChances[i].multiplier > _currentMultiplier;
					_currentMultiplier = multiplierChances[i].multiplier;
					_reward           = (int)(_baseReward * _currentMultiplier);
					SpinToReward(i, isGreater);
					break;
				}
			}
		}

		public void SpinToReward(int index, bool greater)
		{
			if (greater) _goGoldMove.SetActive(false);
			_multiplierSlider.value = 0f;
			_animTween?.Kill();
			_lastFxIndex  = -1;
			_currentIndex = index;
			_isSpinning   = true;

			int   multiplyCount = multiplierChances.Count;
			float totalSteps    = _loop * multiplyCount + index;
			float currentStep   = 0f;

			_animTween = DOTween.To(() => currentStep, x =>
				{
					currentStep = x;
					float floatValue = currentStep % multiplyCount;
					_multiplierSlider.value = floatValue;

					if (floatValue - 8 >= 0)
					{
						currentStep = x + 0.5f;
						floatValue  = currentStep % multiplyCount;
					}

					int visualIndex = Mathf.Clamp(Mathf.RoundToInt(floatValue), 0, multiplyCount - 1);

					if (visualIndex != _lastFxIndex)
					{
						_fxMultiply[visualIndex].Play();
						AudioController.PlaySound(SoundKind.UILuckySpinning);
						_lastFxIndex = visualIndex;
					}
				}, totalSteps, _duration)
				.SetEase(_animationCurve)
				.OnComplete(() =>
				{
					_isSpinning                 = false;
					_fxLoop.transform.position = _fxMultiply[index].transform.position;
					_fxLoop.SetActive(true);
					_multiplierSlider.value = index;

					AudioController.PlaySound(SoundKind.UIRollingStop);

					if (greater)
					{
						StartCoroutine(CoIncreaseGold(_reward, index));
					}
					else
					{
						_goldImg.transform.localScale = Vector3.one;
						_goldImg.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
						_goldImg.sprite = _golds[index];
					}

					_onRolled?.Invoke(greater, _currentMultiplier >= 5f || _hasRerolled);

					_txtMultiCoin.SetText($"You rolled x{multiplierChances[_currentIndex].multiplier} Gold!");
					_txtMultiCoin.transform.localScale = Vector3.one;
					_txtMultiCoin.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
				});
		}

		private IEnumerator CoIncreaseGold(int targetGold, int index)
		{
			_goldTextUI.Push();
			_onIncrease?.Invoke(targetGold);
			FlyManager.Instance.ShowFly(ResourceType.Gold, targetGold,
				_fxMultiply[index].transform.GetComponent<RectTransform>());

			yield return new WaitForSecondsRealtime(2.5f);
			_goldImg.transform.localScale = Vector3.one;
			_goldImg.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
			AudioController.PlaySound(SoundKind.UIUpgradeGoldPack);
			_goldImg.sprite = _golds[index];
			_goGoldMove.SetActive(true);
			_goldTextUI.Pop();
		}
		
		private void OnClickSpinning()
		{
			_isSpinning = false;
			_tweener?.Kill();
			_animTween?.Kill();
			AudioController.PlaySound(SoundKind.UIRollingStop);
			_multiplierSlider.value = _currentIndex;
			_goldImg.sprite         = _golds[_currentIndex];
			_txtMultiCoin.SetText($"You rolled x{multiplierChances[_currentIndex].multiplier} Gold!");
			_txtMultiCoin.transform.localScale = Vector3.one;
			_txtMultiCoin.transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 3);
			_goGoldMove.SetActive(true);
			bool canReroll = _currentMultiplier < 5f && !_hasRerolled;
			_onClickEndRoll?.Invoke(canReroll);
		}

		public void OnRerollClick()
		{
			if (_hasRerolled) return;

			// AdsManager.Instance.ShowRewarded(() =>
			// {
			// 	_hasRerolled = true;
			// 	RollMultiplier();
			// }, () => { UIToastManager.Instance.ShowNoAds(); }, "reroll_multiplier");
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(0) && _isSpinning)
			{
				OnClickSpinning();
			}
		}
	}

	public struct MultiplierChance
	{
		public float multiplier;
		public float chance;
	}
}