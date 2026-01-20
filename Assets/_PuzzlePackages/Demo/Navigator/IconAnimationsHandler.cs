using System;
using Spine.Unity;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Demo
{
    public class IconAnimationsHandler : MonoBehaviour
    {
        [SerializeField] private IconAnimation[] _animations;

        public void OnNewTabSelected(int oldIndex, int newIndex)
        {
            AudioController.PlaySound(SoundKind.UIClickButton);
            _animations[oldIndex].PlayBackward();
            _animations[newIndex].PlayForward();
        }
    }

    [Serializable]
    public class IconAnimation
    {
        [SerializeField] private SkeletonGraphic _skeleton;
        
        [SpineAnimation(dataField = "_skeleton"), SerializeField]
        private string _playForward;

        [SpineAnimation(dataField = "_skeleton"), SerializeField]
        private string _playBackward;
        
        public void PlayForward()
        {
            if (_skeleton.AnimationState.GetCurrent(0).Animation.Name == _playForward) return;
            
            _skeleton.Initialize(false);
            _skeleton.AnimationState.SetAnimation(0, _playForward, false);
        }

        public void PlayBackward()
        {
            if (_skeleton.AnimationState.GetCurrent(0).Animation.Name == _playBackward) return;
            
            _skeleton.Initialize(false);
            _skeleton.AnimationState.SetAnimation(0, _playBackward, false);
        }
    }
}
