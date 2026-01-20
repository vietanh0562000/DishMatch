
using UnityEngine;
using UnityEngine.UI;

public class UIPopupChangeName : MonoBehaviour
{
    public InputField _inputField;
    public Button btnSave;

    private void Awake()
    {
        btnSave.onClick.RemoveAllListeners();
        btnSave.onClick.AddListener(() =>
        {
            //Để tên trống thì không làm gì cả
            if (string.IsNullOrEmpty(_inputField.text))
            {
                UIToastManager.Instance.Show("profile_edit_noti_blank_name");
            }
            else
            {
                if (UserInfoController.instance.UserInfo.name != _inputField.text)
                {
                    UserInfoController.instance.SetName(_inputField.text);
                }

                GetComponent<UIPopup>().OnClick_CloseThisPopup();
            }
        });
    }
}