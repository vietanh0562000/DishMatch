using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ScrollSnapExtension;
using System;
using ChuongCustom;
using BasePuzzle.PuzzlePackages.Settings;
using TMPro;

[Popup("UIPopup_Profile", closeWhenClickOnBackdrop = true)]
public class UIPopup_Profile : BasePopup
{
    public static Action onSaveUI;

    public static int s_SelectIdAvatar;
    public static int s_SelectIdFrameAvatar;

    private static Action s_OnSelectIdAvatar;
    private static Action s_OnSelectFrameAvatar;

    public static void SelectIdAvatar(int id)
    {
        s_SelectIdAvatar = id;
        s_OnSelectIdAvatar?.Invoke();
    }

    public static void SelectIdFrameAvatar(int id)
    {
        s_SelectIdFrameAvatar = id;
        s_OnSelectFrameAvatar?.Invoke();
    }

    public static bool IsChangeData()
    {
        var userInfo = UserInfoController.instance.UserInfo;
        return s_SelectIdAvatar != userInfo.avatar_id || s_SelectIdFrameAvatar != userInfo.frame_id;
    }

    [Header("Tab")] public Button[] btnSelectTabs;
    public RectTransform[] rectBtnSelectActives;
    public RectTransform[] tabs;

    [Header("Profile")] [SerializeField] private Image      _avatar;
    [SerializeField]                     private Image      rectFrameAvatar;
    [SerializeField]                     private InputField _inputField;

    [Header("Scroll Rect Avatar")] [SerializeField]
    private ScrollRectEnsureVisible _scroll;

    [SerializeField] private AvatarSelect _avatarSelectPrefab;
    [SerializeField] private GameObject _avatarContainer;
    [Header("Scroll Rect Frame Avatar")] public RectTransform rectContentScrollFrame;
    public List<UIPopup_Profile_SelectFrameAvatar_Item> itemFrameAvatars;
    // public Button btnFacebook;

    public Text       txtId;
    public GameObject textIDObj;
    public Button     btnCopyID;
    
    private List<AvatarSelect> _avatarSelects;
    private AvatarSelect _avatarSpecialSelect;

    public override void Init() { }
    
    private void Awake()
    {
        //Khởi tạo
        var numAvatar = UISpriteController.Instance.Avatar.GetCount();
        var avatar = UserInfoController.instance.UserInfo.avatar_id;

        _avatarSelects = new List<AvatarSelect>(numAvatar);

        for (int i = 0; i < numAvatar; i++)
        {
            var a = i;
            var cloneAvatarSelect = Instantiate(_avatarSelectPrefab, _avatarContainer.transform);
            cloneAvatarSelect.Setup(a);
            cloneAvatarSelect.SetAvatar(a);
            cloneAvatarSelect.gameObject.SetActive(true);
            _avatarSelects.Add(cloneAvatarSelect);
        }

        //Special Avatar
        {
            var indexAvatarSpecial = -1;
            _avatarSpecialSelect = Instantiate(_avatarSelectPrefab, _avatarContainer.transform);
            _avatarSpecialSelect.Setup(indexAvatarSpecial);
            _avatarSelects.Add(_avatarSpecialSelect);
            _avatarSpecialSelect.gameObject.SetActive(true);
            _avatarSpecialSelect.gameObject.SetActive(false);
        }

        {
            var numFrame = UserInfoController.instance.frames.Count - 1;
            for (int i = 0; i < numFrame; i++)
            {
                var cloneFrame = Instantiate(itemFrameAvatars[0], rectContentScrollFrame);
                cloneFrame.transform.localScale = Vector3.one;
                // cloneFrame.gameObject.SetActive(true);
                itemFrameAvatars.Add(cloneFrame);
            }
        }

        for (int i = 0; i < btnSelectTabs.Length; i++)
        {
            var a = i;
            btnSelectTabs[a].onClick.RemoveAllListeners();
            btnSelectTabs[a].onClick.AddListener(() => { OpenTab(a); });
        }

        void OpenTab(int index)
        {
            for (int i = 0; i < btnSelectTabs.Length; i++)
            {
                var isActive = i == index;
                rectBtnSelectActives[i].gameObject.SetActive(isActive);
                tabs[i].gameObject.SetActive(isActive);
            }
        }
        
        if (AccountManager.instance.Code > 0)
        {
            textIDObj.SetActive(true);
            txtId.text = $"ID: {AccountManager.instance.Code}";
        }
        else
        {
            textIDObj.SetActive(false);
        }
        
        btnCopyID.onClick.AddListener(() =>
        {
            GUIUtility.systemCopyBuffer = AccountManager.instance.Code.ToString();
            UIToastManager.Instance.Show("Copied ID to clipboard");
        });
    }

    public void SaveInfo()
    {
        //Để tên trống thì không làm gì cả
        if (string.IsNullOrEmpty(_inputField.text))
        {
            UIToastManager.Instance.Show("profile edit noti blank name");
        }
        else
        {
            if (IsChangeData() || !_inputField.text.Equals(UserInfoController.instance.UserInfo.name))
            {
                UserInfoController.instance.SetName(_inputField.text);
                UserInfoController.instance.SetAvatar(s_SelectIdAvatar);
                UserInfoController.instance.SetFrame(s_SelectIdFrameAvatar);
                onSaveUI?.Invoke();
            }

            CloseView();
        }
    }

    private void OnEnable()
    {
        s_OnSelectIdAvatar += UpdateAvatar;
        s_OnSelectFrameAvatar += UpdateFrameAvatar;
        UI_Setting_SaveProgress.OnCloseUI += UpdateAvatar;

        StartCoroutine(IEDelayForceSrollAvatar());

        IEnumerator IEDelayForceSrollAvatar()
        {
            yield return new WaitForEndOfFrame();
            _inputField.text = UserInfoController.instance.UserInfo.name;
            _scroll.ForceScrollNormalizedToTarget(
                _avatarSelects[Mathf.Max(0, UserInfoController.instance.UserInfo.avatar_id)]
                    .GetComponent<RectTransform>());
        }

        //Default Tab Avatar
        btnSelectTabs[0].onClick?.Invoke();
        UPdateCache();
        UpdateAvatar();
        UpdateFrameAvatar();
        BindDataController.instance.onSignIn += OnSignIn;
        BindDataController.instance.onSignInFail += OnSignInFail;
    }

    private void OnSignIn(BindDataType type)
    {
        s_SelectIdAvatar = UserInfoController.instance.UserInfo.avatar_id;
        _inputField.text = UserInfoController.instance.UserInfo.name;
        UpdateAvatar();
        onSaveUI?.Invoke();
    }

    private void UPdateCache()
    {
        //Cache
        s_SelectIdAvatar = UserInfoController.instance.UserInfo.avatar_id;
        s_SelectIdFrameAvatar = UserInfoController.instance.UserInfo.frame_id;
    }

    private void OnDisable()
    {
        s_OnSelectIdAvatar -= UpdateAvatar;
        s_OnSelectFrameAvatar -= UpdateFrameAvatar;
        UI_Setting_SaveProgress.OnCloseUI -= UpdateAvatar;
        BindDataController.instance.onSignIn -= OnSignIn;
        BindDataController.instance.onSignInFail -= OnSignInFail;
    }

    private void UpdateFrameAvatar()
    {
        rectFrameAvatar.sprite = UISpriteController.Instance.Frame.GetData(s_SelectIdFrameAvatar);

        var dataFrameAvatar = UserInfoController.instance.frames;
        for (int i = 0; i < itemFrameAvatars.Count; i++)
        {
            var a = i;
            var isActive = a < dataFrameAvatar.Count;
            itemFrameAvatars[a].gameObject.SetActive(isActive);
            if (isActive)
            {
                itemFrameAvatars[a].SetItemData(dataFrameAvatar[a]);
            }
        }

        for (int i = 0; i < itemFrameAvatars.Count; i++)
        {
            if (itemFrameAvatars[i].IsUnlock)
            {
                itemFrameAvatars[i].transform.SetAsFirstSibling();
            }
            else
            {
                itemFrameAvatars[i].transform.SetAsLastSibling();
            }
        }
    }

    private void OnSignInFail(BindDataType bindDataType)
    {
        WindowManager.Instance.OpenWindow<UI_Setting_SaveProgress_Status>(onLoaded: popup =>
        {
            popup.UpdateUI(false, bindDataType, () =>
            {
                switch (bindDataType)
                {
                    case BindDataType.Facebook:
                        BindDataController.instance.SignIn(BindDataType.Facebook);
                        break;
                    case BindDataType.Google:
                        BindDataController.instance.SignIn(BindDataType.Google);
                        break;
                    case BindDataType.Apple:
                        BindDataController.instance.SignIn(BindDataType.Apple);
                        break;
                }
            });
        });
        
        /*UIManager.Instance.OpenPopup(SettingsAssetPaths.GetPath(AssetIDs.SETTINGS_SAVE_PROGRESS_STATUS_ASSETS), (p) =>
        {
            p.GetComponent<UI_Setting_SaveProgress_Status>().UpdateUI(false, bindDataType, () =>
            {
                switch (bindDataType)
                {
                    case BindDataType.Facebook:
                        BindDataController.instance.SignIn(BindDataType.Facebook);
                        break;
                    case BindDataType.Google:
                        BindDataController.instance.SignIn(BindDataType.Google);
                        break;
                    case BindDataType.Apple:
                        BindDataController.instance.SignIn(BindDataType.Apple);
                        break;
                }
            });
        });*/
    }

    private void UpdateAvatar()
    {
        if (s_SelectIdAvatar >= 0)
        {
            UISpriteController.Instance.SetImageAvatar(s_SelectIdAvatar, _avatar);
        }
        else
        {
            UISpriteController.Instance.SetMyFacebookAvatar(UserInfoController.instance.UserInfo, _avatar);
        }

        var count = _avatarSelects.Count;
        for (int i = 0; i < count; i++)
        {
            _avatarSelects[i].SetSelected(s_SelectIdAvatar);
        }

        var isHasSpecialAvatar = UserInfoController.instance.HasSpecialAvatar();
        _avatarSpecialSelect.gameObject.SetActive(isHasSpecialAvatar);
        if (_avatarSpecialSelect.gameObject.activeInHierarchy)
        {
            _avatarSpecialSelect.SetAvatar(UserInfoController.instance.UserInfo);
            _avatarSpecialSelect.transform.SetAsFirstSibling();
        }

        var state = BindDataController.instance.GetBindDataState();
        // btnFacebook.gameObject.SetActive(state == BindDataState.None &&
        //                                  !UserInfoController.instance.HasSpecialAvatar());
        // if (btnFacebook.gameObject.activeInHierarchy)
        // {
        //     btnFacebook.transform.SetAsFirstSibling();
        //     btnFacebook.onClick.RemoveAllListeners();
        //     btnFacebook.onClick.AddListener(() => { BindDataController.instance.SignIn(BindDataType.Facebook); });
        // }
    }
}