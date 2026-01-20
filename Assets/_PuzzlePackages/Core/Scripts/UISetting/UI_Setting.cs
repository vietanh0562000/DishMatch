
using ChuongCustom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Popup("UIPopup_Setting")]
public class UI_Setting : BasePopup
{
    [Header("Music & Sound")] public Button btnSound;
    public Button btnHaptic;
    public Button btnMusic;

    public Sprite[] sprSounds;
    public Sprite[] sprHaptics;
    public Sprite[] sprMusics;

    [Header("Language")] public Button btnLanguage;
    public TextMeshProUGUI txtLanguage;
    public UIPopup popupLanguage;

    [Header("Save Progress")] public Button btnSaveProgress;

    [Header("Extend")] public Button btnCustomCare;
    public Button btnPrivacyPolicy;
    public Button btnTermOfUse;
    public Button btnRestorePurchases;
    public TextMeshProUGUI txtVersion;

    protected override void Start()
    {
        if (AccountManager.instance.Code > 0)
        {
            txtVersion.text = $"ID: {AccountManager.instance.Code} v{Application.version}";
        }
        else
        {
            txtVersion.text = $"v{Application.version}";
        }

        btnCustomCare.onClick.RemoveAllListeners();
        btnCustomCare.onClick.AddListener(() => { FacebookController.OpenFacebookMessenger(); });

        btnSound.onClick.RemoveAllListeners();
        btnSound.onClick.AddListener(() =>
        {
            AudioDataController.instance.SetSound(AudioDataController.instance.IsActiveSound() ? 0 : 1);
            UpdateUI_Setting();
        });

        btnHaptic.onClick.RemoveAllListeners();
        btnHaptic.onClick.AddListener(() =>
        {
            HapticController.instance.SetState(HapticController.instance.IsActive() ? 0 : 1);
            UpdateUI_Setting();
        });

        btnMusic.onClick.RemoveAllListeners();
        btnMusic.onClick.AddListener(() =>
        {
            AudioDataController.instance.SetMusic(AudioDataController.instance.IsActiveMusic() ? 0 : 1);
            UpdateUI_Setting();
        });

        btnLanguage.onClick.RemoveAllListeners();
        btnLanguage.onClick.AddListener(() => { UIManager.Instance.OpenPopup(popupLanguage); });

        btnSaveProgress.onClick.RemoveAllListeners();
        btnSaveProgress.onClick.AddListener(() =>
        {
            //UIManager.Instance.OpenPopup(SettingsAssetPaths.GetPath(AssetIDs.SETTINGS_SAVE_PROGRESS_ASSETS));
            WindowManager.Instance.OpenWindow<UI_Setting_SaveProgress>();
        });

        btnPrivacyPolicy.onClick.RemoveAllListeners();
        btnPrivacyPolicy.onClick.AddListener(() =>
        {
            Application.OpenURL("https://falcongames.com/policy/en/privacy-policy.html");
        });

        btnTermOfUse.onClick.RemoveAllListeners();
        btnTermOfUse.onClick.AddListener(() =>
        {
            Application.OpenURL("https://falcongames.com/policy/en/privacy-policy.html");
        });

        btnRestorePurchases.gameObject.SetActive(false);
        btnRestorePurchases.onClick.RemoveAllListeners();
        btnRestorePurchases.onClick.AddListener(() =>
        {
            // RemoveAdsController.instance.Restore();
        });

#if UNITY_IOS
            btnRestorePurchases.gameObject.SetActive(true);
#endif
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateUI_Setting();
    }

    private void UpdateUI_Setting()
    {
        var iconSound = btnSound.GetComponent<Image>();
        var iconHaptic = btnHaptic.GetComponent<Image>();
        var iconMusic = btnMusic.GetComponent<Image>();

        iconSound.sprite = sprSounds[AudioDataController.instance.IsActiveSound() ? 0 : 1];
        iconHaptic.sprite = sprHaptics[HapticController.instance.IsActive() ? 0 : 1];
        iconMusic.sprite = sprMusics[AudioDataController.instance.IsActiveMusic() ? 0 : 1];

        txtLanguage.text = "English";
    }
}