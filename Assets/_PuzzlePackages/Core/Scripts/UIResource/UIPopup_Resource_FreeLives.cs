using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using ChuongCustom;
using BasePuzzle.PuzzlePackages.Navigator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Popup("UIPopup_Resource_FreeLives")]
public class UIPopup_Resource_FreeLives : BasePopup
{
    public TextMeshProUGUI txtTotal;
    public LoopGridView gridScrollView;
    public RectTransform rectLoading;

    [Header("Empty")] public RectTransform rectEmpty;
    public Button btnRequestEmpty;
    public UIPopup popupRequestJoinTeam;

    private bool initGridView;

    private void Start()
    {
        btnRequestEmpty.onClick.RemoveAllListeners();
        btnRequestEmpty.onClick.AddListener(() =>
        {
            // var isHaveTeam = TeamController.instance.TeamInfo != null;
            var isHaveTeam = false;

            //Nếu ở Menu tự cuộn tới Teams
            if (SceneController.instance.CurrentSceneState == SceneState.Menu)
            {
                if (isHaveTeam)
                {
                    //Đóng tất cả popup và cuộn scroll tới My Team
                    UIManager.Instance.CloseAllPopup();
                    Navigator.Instance.MoveToTab(3);
                }
                else
                {
                    //Hiện Popup vào một Team
                    UIManager.Instance.OpenPopup(popupRequestJoinTeam);
                }
            }
            else
            {
                if (isHaveTeam)
                {
                    // UIToastManager.Instance.Show(LocalizationManager.GetTranslation("get_more_live_request_live_team"));
                    UIToastManager.Instance.Show("get_more_live_request_live_team");
                }
                else
                {
                    // UIToastManager.Instance.Show(LocalizationManager.GetTranslation("get_more_live_request_join_team"));
                    UIToastManager.Instance.Show("get_more_live_request_join_team");
                }
            }
        });
    }

    private void OnEnable()
    {
        UserResourceController.instance.onGetFreeLive += UpdateUI_Scroll;
        UserResourceController.instance.onReceiveFreeLive += OnReceiveFreeLive;

        txtTotal.SetText(string.Empty);
        gridScrollView.gameObject.SetActive(false);
        rectEmpty.gameObject.SetActive(false);
        rectLoading.gameObject.SetActive(true);
        UserResourceController.instance.GetFreeLive();
    }

    private void OnDisable()
    {
        UserResourceController.instance.onGetFreeLive -= UpdateUI_Scroll;
        UserResourceController.instance.onReceiveFreeLive -= OnReceiveFreeLive;
    }

    private void OnReceiveFreeLive()
    {
        UpdateTotalLives();
        OnGetFreeLive();
    }

    private void UpdateUI_Scroll()
    {
        UpdateTotalLives();
        OnGetFreeLive();

        //Scroll To First
        gridScrollView.MovePanelToItemByIndex(0);
    }

    private void UpdateTotalLives()
    {
        if (UserResourceController.instance.freeLives != null)
        {
            txtTotal.SetText($"{UserResourceController.instance.freeLives.Count}");
        }
    }

    private List<FreeLiveRowData> c_data = new List<FreeLiveRowData>();

    private void OnGetFreeLive()
    {
        //Cache
        c_data = UserResourceController.instance.freeLives;

        if (c_data == null)
        {
            c_data = new List<FreeLiveRowData>();
        }

        gridScrollView.gameObject.SetActive(true);
        rectEmpty.gameObject.SetActive(c_data.Count == 0);
        rectLoading.gameObject.SetActive(false);

        if (!initGridView)
        {
            initGridView = true;
            gridScrollView.InitGridView(c_data.Count, InitGridView);
        }
        else
        {
            gridScrollView.SetListItemCount(c_data.Count, false);
        }

        gridScrollView.MovePanelToItemByIndex(0);
        gridScrollView.RefreshAllShownItem();
    }

    private LoopGridViewItem InitGridView(LoopGridView view, int itemIndex, int arg3, int arg4)
    {
        if (c_data == null || c_data.Count == 0)
        {
            return null;
        }

        var item = view.NewListViewItem(view.ItemPrefabDataList[0].mItemPrefab.name);
        var itemScript = item.GetComponent<UIPopup_Resource_FreeLives_Item>();
        itemScript.SetItemData(this, c_data[itemIndex]);
        return item;
    }
}