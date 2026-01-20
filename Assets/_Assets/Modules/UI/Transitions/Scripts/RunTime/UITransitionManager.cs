using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    using Sirenix.OdinInspector;

    public class UITransitionManager : MonoBehaviour
    {
        [SerializeField] private UIProgressBar _uiProgressBar;
        [SerializeField] private UIFading      _uiFading;
        [SerializeField] private UIIrisWipe    _uiIrisWipe;

        private void Awake() { Setup(); }

        private void Setup()
        {
            _uiFading.gameObject.SetActive(false);
            _uiIrisWipe.gameObject.SetActive(false);

            var config = Resources.Load<UITransitionConfig>(UITransitionConfig.SETTINGS_NAME);
            if (config == null)
            {
                Debug.LogError(
                    "Bạn phải tạo UITransitionConfig trước. Trên Unity Menu, chọn Falcon > Modules > UI > Transition > Config");
                return;
            }

            _uiProgressBar.Setup(config);
            _uiFading.Setup(config);
            _uiIrisWipe.Setup(config);
        }

        public Task ProgressBarInAsync => _uiProgressBar.ProgressBarIn();

        public async void ProgressBarIn(Action onCompleted)
        {
            try
            {
                await _uiProgressBar.ProgressBarIn();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > ProgressBarIn Error: {e} ");
            }
        }

        public Task ProgressBarOutAsync => _uiProgressBar.ProgressBarOut();

        public async void ProgressBarOut(Action onCompleted)
        {
            try
            {
                await _uiProgressBar.ProgressBarOut();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > ProgressBarOut Error: {e} ");
            }
        }


        public Task IrisWipeInAsync => _uiIrisWipe.WipeIn();

        [Button]
        public async void IrisWipeIn(Action onCompleted)
        {
            try
            {
                await _uiIrisWipe.WipeIn();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > IrisWipeIn Error: {e} ");
            }
        }

        public Task IrisWipeOutAsync => _uiIrisWipe.WipeOut();
        
        [Button]
        public async void IrisWipeOut(Action onCompleted)
        {
            try
            {
                await _uiIrisWipe.WipeOut();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > IrisWipeOut Error: {e} ");
            }
        }

        public Task FadeInAsync => _uiFading.FadeInAsync();
        public async void FadeIn(Action onCompleted)
        {
            try
            {
                await _uiFading.FadeInAsync();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > FadeIn Error: {e} ");
            }
        }

        public Task FadeOutAsync => _uiFading.FadeOutAsync();
        public async void FadeOut(Action onCompleted)
        {
            try
            {
                await _uiFading.FadeOutAsync();
                onCompleted?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"{typeof(UITransitionManager)} > FadeOut Error: {e} ");
            }
        }

        public void DestroyProgressBar() { Destroy(_uiProgressBar.gameObject); }
    }
}