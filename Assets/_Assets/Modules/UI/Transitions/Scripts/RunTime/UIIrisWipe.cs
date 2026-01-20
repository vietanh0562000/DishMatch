using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    public class UIIrisWipe : MonoBehaviour
    {
        [SerializeField] private Image _background, _logo;
        [SerializeField] private Material _fadeInMat, _fadeOutMat;

        private readonly int _keyCircleIn = Shader.PropertyToID("_CircleHole_Size_1");
        private readonly int _keyCircleOut = Shader.PropertyToID("_CircleHole_Size_1");

        private UITransitionConfig  _config;

        public void Setup(UITransitionConfig config)
        {

            _config = config;
        }
        
        public async Task WipeIn()
        {
            gameObject.SetActive(true);
            _logo.transform.localScale = Vector3.zero;
            _background.material = _fadeInMat;
            _fadeInMat.SetFloat(_keyCircleIn, 0);

            await Task.WhenAll(
                AnimateLogoScale(true, _config.WipeInDurationLogo, _config.WipeInDelayScaleLogo),
                AnimateMaterialFloat(_fadeInMat, _keyCircleIn, 0f, 1f, _config.WipeInDurationBG)
            );
        }

        public async Task WipeOut()
        {
            _logo.transform.localScale = Vector3.one;

            _background.material = _fadeOutMat;
            _fadeOutMat.SetFloat(_keyCircleOut, 1);

            await AnimateLogoScale(false, _config.WipeOutDurationLogo, 0f);
            await AnimateMaterialFloat(_fadeOutMat, _keyCircleOut, 1f, 0f, _config.WipeOutDurationBG);
            gameObject.SetActive(false);
        }


        private async Task AnimateLogoScale(bool isScalingIn, float duration, float delay)
        {
            float elapsedTime = 0f;
            Vector3 startScale = isScalingIn ? _logo.transform.localScale : Vector3.one;
            Vector3 endScale = isScalingIn ? Vector3.one : Vector3.zero;

            if (delay > 0)
                await Task.Delay(TimeSpan.FromSeconds(delay));

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float curvedT = (isScalingIn ? _config.LogoScaleInCurve : _config.LogoScaleOutCurve).Evaluate(t);

                _logo.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, curvedT);
                await Task.Yield();
            }

            _logo.transform.localScale = endScale;
        }



        private async Task AnimateMaterialFloat(Material material, int propertyID, float from, float to, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float currentValue = Mathf.Lerp(from, to, progress);
                material.SetFloat(propertyID, currentValue);
                await Task.Yield();
            }

            material.SetFloat(propertyID, to);
        }
    }
}