using System.Threading.Tasks;
using UnityEngine;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    public class UIFading : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        
        private float _fadeDuration;

        public void Setup(UITransitionConfig config)
        {
            _fadeDuration = config.FadeDuration;
        }
        
        public async Task FadeOutAsync()
        {
            gameObject.SetActive(true);
            float timer = 0f;
            while (timer < _fadeDuration)
            {
                _canvasGroup.alpha = Mathf.Lerp(0, 1, timer / _fadeDuration);
                timer += Time.unscaledDeltaTime;
                await Task.Yield();
            }

            _canvasGroup.alpha = 1;
        }

        public async Task FadeInAsync()
        {
            float timer = 0f;
            while (timer < _fadeDuration)
            {
                _canvasGroup.alpha = Mathf.Lerp(1, 0, timer / _fadeDuration);
                timer += Time.unscaledDeltaTime;
                await Task.Yield();
            }

            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }
}