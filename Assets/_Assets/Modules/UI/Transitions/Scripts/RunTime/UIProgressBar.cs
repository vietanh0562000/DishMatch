using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    using UnityEngine.UIElements.Experimental;

    internal class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _progressBar, _progressBarBG, _background;
        [SerializeField] private Slider _slider;

        private float _currentValue, _animationSpeed;
        private List<PausePoint> _pausePoints = new List<PausePoint>();

        internal void Setup(UITransitionConfig config)
        {
            _animationSpeed = config.AnimationSpeed;
            _pausePoints = config.PausePoints;
        }

        internal async Task ProgressBarIn()
        {
            gameObject.SetActive(true);
            foreach (var pausePoint in _pausePoints)
            {
                var rand = Random.Range(pausePoint.position - pausePoint.offset, pausePoint.position + pausePoint.offset);
                await AnimateProgressToAsync(rand);
            }
        }

        internal async Task ProgressBarOut()
        {
            await AnimateProgressToAsync(1);
            await Task.Delay(100);
            gameObject.SetActive(false);
        }

        private async Task AnimateProgressToAsync(float target)
        {
            while (_currentValue < target - 0.001f)
            {
                _currentValue      = Mathf.MoveTowards(_currentValue, target, Time.unscaledDeltaTime * _animationSpeed);
                _slider.value = Easing.OutCubic(_currentValue);
                await Task.Yield();
            }

            _currentValue      = target;
            _slider.value = Easing.OutCubic(_currentValue);
        }
    }
}