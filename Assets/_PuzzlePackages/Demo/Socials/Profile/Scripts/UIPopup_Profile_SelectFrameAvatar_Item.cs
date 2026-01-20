
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Profile_SelectFrameAvatar_Item : MonoBehaviour
{
    public Image rectFrameAvatar;

    // public RectTransform rectLock;
    public RectTransform rectSelected;
    public Button btnSelect;

    [Sirenix.OdinInspector.ReadOnly] public bool IsUnlock;

    public void SetItemData(int index)
    {
        rectFrameAvatar.sprite = UISpriteController.Instance.Frame.GetData(index);

        // IsUnlock = UserInfoController.instance.IsUnlockFrame(index);
        // rectLock.gameObject.SetActive(!IsUnlock);

        rectSelected.gameObject.SetActive(index == UIPopup_Profile.s_SelectIdFrameAvatar);
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() =>
        {
            // if (UserInfoController.instance.IsUnlockFrame(index))
            // {
            UIPopup_Profile.SelectIdFrameAvatar(index);
            // }
            // else
            // {
            //     Toast(index);
            // }        
            AudioController.PlaySound(SoundKind.UIClickButton);
        });
    }
/*
    private void Toast(int index)
    {
        if(UserInfoController.instance.IsFrameMasterPass(index))
        {
            UIToastManager.Instance.Show(LocalizationManager.GetTranslation("profile_frame_locked_bp_info"));
            return;
        }

        if(UserInfoController.instance.IsFrameScoopSquad(index))
        {
            UIToastManager.Instance.Show(LocalizationManager.GetTranslation("profile_frame_locked_scoopsquad_info"));
            return;
        }

        if(UserInfoController.instance.IsFrameDigging(index))
        {
            UIToastManager.Instance.Show(LocalizationManager.GetTranslation("profile_frame_locked_digging_trig_or_dig_info"));
            return;
        }

        if(UserInfoController.instance.IsFrameSingleDay(index))
        {
            UIToastManager.Instance.Show(LocalizationManager.GetTranslation("profile_frame_locked_single_day_info"));
            return;
        }

        UIToastManager.Instance.Show(LocalizationManager.GetTranslation("profile_not_unlock"));
    }
    */
}