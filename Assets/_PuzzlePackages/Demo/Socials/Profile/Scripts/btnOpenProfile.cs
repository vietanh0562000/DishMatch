using UnityEngine;

public class btnOpenProfile : MonoBehaviour
{
    public void OnClick_Show_UIPopup_ViewProfile()
    {
        // var code = AccountManager.instance.Code;
        // UIPopup_ViewProfile.OpenUI(code);
        WindowManager.Instance.OpenWindow<UIPopup_Profile>();
        AudioController.PlaySound(SoundKind.UIClickButton);
    }
}