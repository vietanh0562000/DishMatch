using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting_SetNickname : MonoBehaviour
{
    public TMP_InputField inputName;
    public Button btnContinue;

    private void OnEnable()
    {
        inputName.text = UserInfoController.instance.UserInfo.name;
        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(() =>
        {
            GetComponent<UIPopup>().OnClick_CloseThisPopup(() =>
            {
                UserInfoController.instance.SetName(inputName.text);
            });
        });
    }
}
