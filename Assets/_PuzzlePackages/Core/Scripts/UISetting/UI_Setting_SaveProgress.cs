using System;
using System.Collections;
using System.Collections.Generic;
using ChuongCustom;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Socials.Profile;
using UnityEngine;
using UnityEngine.UI;

[Popup("UIPopup_Setting_SaveProgress")]
public class UI_Setting_SaveProgress : BasePopup
{
    public static Action OnCloseUI;

    public RectTransform rectBG;
    public RectTransform rectDesNotSign;
    public RectTransform rectDesSigned;

    public RectTransform rectFacebook;
    public Button btnSignInFacebook;
    public Button btnSignOutFacebook;

    public RectTransform rectGoogle;
    public Button btnSignInGoogle;
    public Button btnSignOutGoogle;

    public RectTransform rectApple;
    public Button btnSignInApple;
    public Button btnSignOutApple;

    private void Start()
    {
        btnSignInFacebook.onClick.RemoveAllListeners();
        btnSignInFacebook.onClick.AddListener(() => 
        {
            BindDataController.instance.SignIn(BindDataType.Facebook);
        });

        btnSignInGoogle.onClick.RemoveAllListeners();
        btnSignInGoogle.onClick.AddListener(() => 
        {
            BindDataController.instance.SignIn(BindDataType.Google);
        });

        btnSignInApple.onClick.RemoveAllListeners();
        btnSignInApple.onClick.AddListener(() => 
        {
            BindDataController.instance.SignIn(BindDataType.Apple);
        });

        btnSignOutFacebook.onClick.RemoveAllListeners();
        btnSignOutFacebook.onClick.AddListener(SignOut);

        btnSignOutGoogle.onClick.RemoveAllListeners();
        btnSignOutGoogle.onClick.AddListener(SignOut);

        btnSignOutApple.onClick.RemoveAllListeners();
        btnSignOutApple.onClick.AddListener(SignOut);

        void SignOut()
        {
            UIManager.Instance.OpenPopup(ProfileAssetPaths.GetPath(AssetIDs.PROFILE_CONFIRM_ACTION), p =>
            {
                var strTitle = LocalizationHelper.GetTranslation("bind_data_are_you_sure_title");
                var strContent = LocalizationHelper.GetTranslation("bind_data_are_you_sure_des");
                p.GetComponent<UIPopup_ConfirmAction>().UpdateUI(strTitle, strContent, () =>
                {
                    BindDataController.instance.CancelBindData();
                });
            });
        }
    }
    private void OnEnable()
    {
        UpdateUI();
        BindDataController.instance.onSignIn += OnSignIn;
        BindDataController.instance.onSignInFail += OnSignInFail;
        BindDataController.instance.onSignOut += UpdateUI;
    }

    
    private void OnDisable()
    {
        OnCloseUI?.Invoke();
        BindDataController.instance.onSignIn -= OnSignIn;
        BindDataController.instance.onSignInFail -= OnSignInFail;
        BindDataController.instance.onSignOut -= UpdateUI;
    }

    private void OnSignIn(BindDataType bindDataType)
    {
        UpdateUI();
        GetComponent<UIPopup>().OnClick_CloseThisPopup();
    }

    private void OnSignInFail(BindDataType bindDataType)
    {
        GetComponent<UIPopup>().OnClick_CloseThisPopup();
    }

    private void UpdateUI()
    {
        rectFacebook.gameObject.SetActive(false);
        rectGoogle.gameObject.SetActive(false);
        rectApple.gameObject.SetActive(false);
        btnSignInFacebook.gameObject.SetActive(false);
        btnSignOutFacebook.gameObject.SetActive(false);
        btnSignInGoogle.gameObject.SetActive(false);
        btnSignOutGoogle.gameObject.SetActive(false);
        btnSignInApple.gameObject.SetActive(false);
        btnSignOutApple.gameObject.SetActive(false);

        var state = BindDataController.instance.GetBindDataState();
        if(state == BindDataState.None)
        {
            rectBG.sizeDelta = new Vector2(rectBG.sizeDelta.x, 575);
            rectDesNotSign.gameObject.SetActive(true);
            rectDesSigned.gameObject.SetActive(false);

            rectFacebook.gameObject.SetActive(true);
            //rectGoogle.gameObject.SetActive(true);
            //rectApple.gameObject.SetActive(true);
            btnSignInFacebook.gameObject.SetActive(true);
            //btnSignInGoogle.gameObject.SetActive(true);
            //btnSignInApple.gameObject.SetActive(true);
        }
        else
        {
            rectBG.sizeDelta = new Vector2(rectBG.sizeDelta.x, 575);
            rectDesNotSign.gameObject.SetActive(false);
            rectDesSigned.gameObject.SetActive(true);

            switch (state)
            {
                case BindDataState.Facebook:
                    rectFacebook.gameObject.SetActive(true);
                    btnSignOutFacebook.gameObject.SetActive(true);
                    break;
                case BindDataState.Google:
                    rectGoogle.gameObject.SetActive(true);
                    btnSignOutGoogle.gameObject.SetActive(true);
                    break;
                case BindDataState.Apple:
                    rectApple.gameObject.SetActive(true);
                    btnSignOutApple.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
