namespace PuzzleGames
{
    using com.ootii.Messages;
    using UnityEngine;
    using UnityEngine.UI;

    public class PlayerAvatar : MonoBehaviour
    {
        [Space] [SerializeField] private Image _avatar;
        [SerializeField]         private Image _frame;

        private void Awake()
        {
            MessageDispatcher.AddListener(EventID.UPDATE_FRAME_AVATAR, OnUpdateFrameAvatar, true);
            OnUpdateFrameAvatar(null);
        }

        private void OnDestroy() { MessageDispatcher.RemoveListener(EventID.UPDATE_FRAME_AVATAR, OnUpdateFrameAvatar, true); }

        private void OnUpdateFrameAvatar(IMessage rmessage)
        {
            var data = UserInfoController.instance.GetMyUserInfoDetail();
            UISpriteController.Instance.SetImageAvatar(data, _avatar);
            _frame.sprite = UISpriteController.Instance.Frame.GetData(UserInfoController.instance.UserInfo.frame_id);
        }
    }
}