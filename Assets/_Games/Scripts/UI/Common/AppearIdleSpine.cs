namespace PuzzleGames
{
    using Spine.Unity;
    using UnityEngine;

    public class AppearIdleSpine : MonoBehaviour
    {
        [SerializeField] private SkeletonGraphic _skeleton;

        [SpineAnimation(dataField = "_skeleton"), SerializeField]
        private string _appear;

        [SpineAnimation(dataField = "_skeleton"), SerializeField]
        private string _idle;
        
        public void ShowAnim()
        {
            _skeleton.gameObject.SetActive(true);
            _skeleton.AnimationState.ClearTracks();
            _skeleton.AnimationState.SetAnimation(0, _appear, false);
            _skeleton.AnimationState.AddAnimation(0, _idle, true, 0);
        }
    }
}