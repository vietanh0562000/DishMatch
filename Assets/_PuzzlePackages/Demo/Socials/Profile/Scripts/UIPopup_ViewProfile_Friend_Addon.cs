using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Socials.Profile;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_ViewProfile_Friend_Addon : MonoBehaviour
{
    public RectTransform rectAddFriend;
    public Button btnAddFriend;

    public RectTransform rectRemoveFriend;
    public Button btnRemoveFriend;

    public RectTransform rectPending;

    private UserInfoDetail c_data;

    private void OnEnable()
    {
        //FriendDataController.instance.onSendFriendRequest += UpdateUI;
        //FriendDataController.instance.onResponRequest += UpdateUI;
        //FriendDataController.instance.onRemoveFriend += UpdateUI;
    }

    private void OnDisable()
    {
        //FriendDataController.instance.onResponRequest -= UpdateUI;
       // FriendDataController.instance.onSendFriendRequest -= UpdateUI;
       // FriendDataController.instance.onRemoveFriend -= UpdateUI;
    }

    private void UpdateUI()
    {
        UpdateUI(c_data);
    }

    public void UpdateUI(UserInfoDetail data)
    {
        //Cache
        c_data = data;

        rectAddFriend.gameObject.SetActive(false);
        rectRemoveFriend.gameObject.SetActive(false);

        if (data == null) return;

        var isMine = AccountManager.instance.Code == data.code;
        var status = data.GetFriendStatus();

        rectAddFriend.gameObject.SetActive(!isMine &&
                                           (status == FriendStatus.NOT_FRIEND || status == FriendStatus.PENDING));
        btnAddFriend.onClick.RemoveAllListeners();
        //btnAddFriend.onClick.AddListener(() => { FriendDataController.instance.SendFriendRequest(data); });
        rectRemoveFriend.gameObject.SetActive(!isMine && status == FriendStatus.FRIEND);
        btnRemoveFriend.onClick.RemoveAllListeners();
        btnRemoveFriend.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPopup(ProfileAssetPaths.GetPath(AssetIDs.PROFILE_CONFIRM_ACTION), p =>
            {
                var strTitle = LocalizationHelper.GetTranslation("remove_friend");
                var strContent = LocalizationHelper.GetTranslation("team_info_remove_friends_des")
                    .Replace("%{0}", $"<color=#ffe500>{data.name}</color>");
                p.GetComponent<UIPopup_ConfirmAction>().UpdateUI(strTitle, strContent, () =>
                {
                    c_data.friendStatus = 0;
                   // FriendDataController.instance.RemoveFriend(data.code);
                });
            });
        });

        rectPending.gameObject.SetActive(!isMine && status == FriendStatus.REQUESTED);
    }
}