using SuperScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using BasePuzzle.PuzzlePackages.Navigator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_Resource_FreeLives_RequestJoinTeam : MonoBehaviour
{
    public Button btnRequest;

    private void Start()
    {
        btnRequest.onClick.RemoveAllListeners();
        btnRequest.onClick.AddListener(() =>
        {
            //Đóng tất cả popup và cuộn scroll tới My Team
            UIManager.Instance.CloseAllPopup();
            Navigator.Instance.MoveToTab(3);
        });
    }
}