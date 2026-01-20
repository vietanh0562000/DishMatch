using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ChuongCustom;
using TMPro;

[Popup("UIPopup_Setting_SaveProgress_Status")]
public class UI_Setting_SaveProgress_Status : BasePopup
{
    public TextMeshProUGUI txtTitle;

    public RectTransform rectFacebook;
    public RectTransform rectFacebookSuccess;
    public RectTransform rectFacebookFail;

    public RectTransform rectBtnContinue;
    public RectTransform rectBtnTryAgain;

    public Button btnContinue;
    public Button btnTryAgain;

    public void UpdateUI(bool isSuccess, BindDataType bindDataType, Action onClick)
    {
        txtTitle.SetText(isSuccess ? "bind_account_success" : "bind_account_fail");

        rectFacebook.gameObject.SetActive(false);
        rectFacebookSuccess.gameObject.SetActive(isSuccess);
        rectFacebookFail.gameObject.SetActive(!isSuccess);

        switch (bindDataType)
        {
            case BindDataType.Facebook:
                rectFacebook.gameObject.SetActive(true);
                break;
            case BindDataType.Google:
                break;
            case BindDataType.Apple:
                break;
        }

        rectBtnContinue.gameObject.SetActive(isSuccess);
        rectBtnTryAgain.gameObject.SetActive(!isSuccess);

        btnContinue.onClick.RemoveAllListeners();
        btnContinue.onClick.AddListener(() =>
        {
            GetComponent<UIPopup>().OnClick_CloseThisPopup();
            onClick?.Invoke();
        });

        btnTryAgain.onClick.RemoveAllListeners();
        btnTryAgain.onClick.AddListener(() =>
        {
            GetComponent<UIPopup>().OnClick_CloseThisPopup();
            onClick?.Invoke();
        });
    }
}