using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BasePuzzle.PuzzlePackages.Profile;
using BasePuzzle.PuzzlePackages.Socials.Profile;

public class UIPopup_ViewProfile : MonoBehaviour
{
    [Sirenix.OdinInspector.ReadOnly] public int Code;

    public RectTransform viewport;
    public RectTransform rectLoading;

    [Header("Info")] public Image avatar;
    public Text txtName;
    public TextMeshProUGUI txtId;
    public RectTransform rectTeam;
    public Button btnInfoTeam;

    public Button btnCopyID;
    public RectTransform rectTweenCopyID;

    [Header("Addon")] public UIPopup_ViewProfile_Edit_Addon editAddon;
    public UIPopup_ViewProfile_Friend_Addon friendAddon;

    private void OnEnable()
    {
        UIPopup_Profile.onSaveUI += OnSaveUIInfo;
    }

    private void OnDisable()
    {
        UIPopup_Profile.onSaveUI -= OnSaveUIInfo;

        viewport.gameObject.SetActive(false);
        rectLoading.gameObject.SetActive(false);
        friendAddon.UpdateUI(null);
    }

    private void OnRefresh_ViewProfile()
    {
        UpdateUI(Code);
    }

    private void OnSaveUIInfo()
    {
        var data = UserInfoController.instance.GetMyUserInfoDetail();
        UpdateUI(data);
        friendAddon.UpdateUI(data);
        editAddon.UpdateUI(data);
    }

    public static void OpenUI(int code)
    {
        UIManager.Instance.OpenPopup(ProfileAssetPaths.GetPath(AssetIDs.PROFILE_VIEW_PROFILE),
            p => { p.GetComponent<UIPopup_ViewProfile>().UpdateUI(code); });
    }

    public void UpdateUI(int code)
    {
        //Cache
        Code = code;

        viewport.gameObject.SetActive(false);
        rectLoading.gameObject.SetActive(true);
        friendAddon.UpdateUI(null);

        //Tự động đóng Popup nếu phản hồi quá lâu
        var strKey = "ProfileInfo_Responsive_Fail";

        {
            DOTween.Kill(strKey);
            DOVirtual.DelayedCall(7.5f, ForceClosePopup).SetId(strKey);
        }

        UserInfoController.instance.GetUserinfoDetail(code, data =>
        {
            if (gameObject.activeInHierarchy)
            {
                DOTween.Kill(strKey);
                UpdateUI(data);
                friendAddon.UpdateUI(data);
                editAddon.UpdateUI(data);
            }
        });
    }

    private void UpdateUI(UserInfoDetail data)
    {
        if (data == null)
        {
            ForceClosePopup();
            return;
        }

        viewport.gameObject.SetActive(true);
        rectLoading.gameObject.SetActive(false);

        UISpriteController.Instance.SetImageAvatar(data, avatar);

        txtName.gameObject.SetActive(true);
        txtName.text = data.name;
        txtId.text = "ID: " + data.code;

        // var dataTeam = data.teamInfo;
        // if (dataTeam == null)
        // {
        //     btnInfoTeam.onClick.RemoveAllListeners();
        //     btnInfoTeam.onClick.AddListener(() => { UIToastManager.Instance.Show("No Team"); });
        // }
        // else
        // {
        //     btnInfoTeam.onClick.RemoveAllListeners();
        //     btnInfoTeam.onClick.AddListener(() =>
        //     {
        //         // UIManager.Instance.OpenPopup("UIPopup_Clan_TeamInfo",
        //         //     p => { p.GetComponent<UIPopup_Clan_TeamInfo>().UpdateUI(dataTeam.code); });
        //     });
        // }

        btnCopyID.onClick.RemoveAllListeners();
        btnCopyID.onClick.AddListener(() =>
        {
            try
            {
                GUIUtility.systemCopyBuffer = data.code.ToString();
                rectTweenCopyID.gameObject.SetActive(true);
            }
            catch
            {
                Debug.LogError("Cannot Copy ID !");
            }
        });
        rectTweenCopyID.gameObject.SetActive(false);
    }

    private void ForceClosePopup()
    {
        if (gameObject.activeInHierarchy)
        {
            GetComponent<UIPopup>().OnClick_CloseThisPopup();
        }
    }
}