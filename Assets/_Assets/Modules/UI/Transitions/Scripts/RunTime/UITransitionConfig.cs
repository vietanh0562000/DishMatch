using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasePuzzle.Modules.UI.Transition.Runtime
{
    [CreateAssetMenu(fileName = "UITransitionConfig",
        menuName = "Scriptable Objects/Falcon/Modules/UI/UITransition/Config")]
    public class UITransitionConfig : ScriptableObject
    {
        public const string SETTINGS_PATH = "Assets/Resources/Falcon/Modules/UI/Transition";
        public const string SETTINGS_NAME = "UITransitionConfig";

        [Header("Progress Bar:"), Space(5)]
        [SerializeField] private float _animationSpeed = 3;
        [SerializeField, Space(6)] private List<PausePoint> _pausePoints;
        
        [Header("Iris Wipe:"), Space(7)]
        [SerializeField, Range(0.5f, 1f)] private float _wipeInDurationBG = 0.62f;
        [SerializeField, Range(0.2f, 1f)] private float _wipeOutDurationBG = 0.256f;
        [SerializeField, Range(0.25f, 0.5f)] private float _wipeInDurationLogo = 0.3f;
        [SerializeField, Range(0.25f, 0.5f)] private float _wipeOutDurationLogo = 0.35f;
        [SerializeField, Range(0, 1)] private float _wipeInDelayScaleLogo = 0.47f;
        [SerializeField] private AnimationCurve _logoScaleInCurve, _logoScaleOutCurve;

        [Header("Fading:"), Space(10)]
        [SerializeField] private float _fadeDuration;

        public float AnimationSpeed => _animationSpeed;
        public List<PausePoint> PausePoints => _pausePoints;
        
        public float WipeInDurationBG => _wipeInDurationBG;
        public float WipeOutDurationBG => _wipeOutDurationBG;
        public float WipeInDurationLogo => _wipeInDurationLogo;
        public float WipeOutDurationLogo => _wipeOutDurationLogo;
        public float WipeInDelayScaleLogo => _wipeInDelayScaleLogo;
        public AnimationCurve LogoScaleInCurve => _logoScaleInCurve;
        public AnimationCurve LogoScaleOutCurve => _logoScaleOutCurve;
        
        public float FadeDuration => _fadeDuration;
    }

    [Serializable]
    public struct PausePoint
    {
        [Range(0, 0.95f)]
        public float position;
        
        [Range(0, 0.4f)]
        public float offset;
    }
}