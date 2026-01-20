namespace PuzzleGames
{
    using com.ootii.Messages;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIHomeProfile : MonoBehaviour
    {
        public Image avatar;

        private void OnEnable()
        {
            OnSaveUIInfo();
            UIPopup_Profile.onSaveUI += OnSaveUIInfo;
        }

        private void OnDisable() { UIPopup_Profile.onSaveUI -= OnSaveUIInfo; }

        private void OnSaveUIInfo()
        {
            var data = UserInfoController.instance.GetMyUserInfoDetail();
            UISpriteController.Instance.SetImageAvatar(data, avatar);
            MessageDispatcher.SendMessage(EventID.UPDATE_FRAME_AVATAR, 0);
        }
    }
}