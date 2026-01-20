using DG.Tweening;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Team
{
    public class CircularLoadingIcon : MonoBehaviour
    {
        [SerializeField] private RectTransform _iconRectTransform;
        [SerializeField] private float _speed = 4.1f;
        
        private Tween _tween;

        private void OnEnable()
        {
            StartRotation();
        }

        private void OnDisable()
        {
            StopRotation();
        }

        private void StartRotation()
        {
            if (_tween != null && _tween.IsActive())
            {
                _tween.Kill();
            }

            _tween = _iconRectTransform.DORotate(new Vector3(0f, 0f, 360f), 1f / _speed, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear) 
                .SetLoops(-1, LoopType.Restart);
        }

        private void StopRotation()
        {
            if (_tween == null || !_tween.IsActive()) return;
            _tween.Kill(); 
            _iconRectTransform.localRotation = Quaternion.identity;
        }
    }
}