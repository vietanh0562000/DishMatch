using System;
using com.ootii.Messages;
using DG.Tweening;
using BasePuzzle.PuzzlePackages.Core;
using BasePuzzle.PuzzlePackages.Navigator;
using BasePuzzle.PuzzlePackages.Socials.FreeLives;
using PuzzleGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIResource : MonoBehaviour
{
    [Header("Amount")] public TextMeshProUGUI txtHearth;
    public                    TextMeshProUGUI txtGold;

    [Header("Get More")] public Button btnGetMoreHearth;
    public                      Button btnGetMoreGold;

    [Header("Infinite & Timer")] public RectTransform       rectHearthInfinite;
    public                              RectTransform       rectIconHearthPlus;
    public                              UITimerCountdownTMP timerHearth;

    [Header("Target Fly Item")] public RectTransform iconTargetHearth;
    public                             RectTransform iconTargetGold;

    private void Start()
    {
        btnGetMoreHearth.onClick.RemoveAllListeners();
        btnGetMoreHearth.onClick.AddListener(() =>
        {
            if (UserResourceController.instance.IsInfiHeart() || UserResourceController.instance.IsMaxHeart())
            {
                //Mở popup Free Live
                // UIManager.Instance.OpenPopup(FreeLivesAssetPaths.GetPath(AssetIDs.FREELIVES_RESOURCE_ASSETS));
            }
            else
            {
                //Mở popup mua thêm 
                //Bên trong có Popup Free Live từ Team
                ShowPopupFreeLive();
            }
        });

        btnGetMoreGold.onClick.RemoveAllListeners();

        btnGetMoreGold.onClick.AddListener(() => { Navigator.Instance.MoveToTab(0); });
    }

    public static void ShowPopupFreeLive()
    {
        /*UIManager.Instance.OpenPopup(FreeLivesAssetPaths.GetPath(AssetIDs.FREELIVES_SHOP_LIVE_ASSETS),
            p =>
            {
                p.GetComponent<UIPopup_ShopLives>()
                    // .UpdateUI_Title(LocalizationHelper.GetTranslation("live_getmore_title"));
                    .UpdateUI_Title("Get more");
            });*/
      //  WindowManager.Instance.OpenWindow<UIPopup_ShopLives>(onLoaded: popup => { popup.UpdateUI_Title("Get more"); });
    }


    private void OnEnable()
    {
        DOVirtual.DelayedCall(0.05f, () =>
        {
            UpdateUI_Hearth();
            UpdateUI_GoldAndStar();
        });
    }

    private void OnDisable()
    {
    }

    public void UpdateHeartUI() { UpdateUI_Hearth(); }

    private void UpdateUI_Hearth()
    {
        if (UserResourceController.instance.IsInfiHeart())
        {
            txtHearth.gameObject.SetActive(false);
            rectHearthInfinite.gameObject.SetActive(true);
            rectIconHearthPlus.gameObject.SetActive(false);

            timerHearth.onStopTimer = () => this.Delay(1, UpdateUI_Hearth);
            timerHearth.StartTimer(UserResourceController.instance.GetTargetTimeSpanInfiHearth());
        }
        else
        {
            txtHearth.gameObject.SetActive(true);
            rectHearthInfinite.gameObject.SetActive(false);
            rectIconHearthPlus.gameObject.SetActive(false);

            var rs = UserResourceController.instance.UserResource;
            txtHearth.SetText($"{rs.heart}");

            if (UserResourceController.instance.IsMaxHeart())
            {
                timerHearth.onStopTimer = null;
                timerHearth.Stop();
                timerHearth.GetTimeTextTMP().SetText("Full");
            }
            else
            {
                rectIconHearthPlus.gameObject.SetActive(true);
                timerHearth.onStopTimer = () => this.Delay(1, UpdateUI_Hearth);
                timerHearth.StartTimer(UserResourceController.instance.TimeSpanUpdateHeart);
            }
        }
    }

    public void UpdateGold() { UpdateUI_GoldAndStar(); }

    private void UpdateUI_GoldAndStar() { txtGold.SetText(FormatNumber.FormatKMBNumber(UserResourceController.instance.UserResource.gold)); }
}