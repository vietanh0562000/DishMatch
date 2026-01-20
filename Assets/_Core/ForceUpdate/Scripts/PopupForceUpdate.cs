using System;
using BasePuzzle.Core.Scripts.ABTesting.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupForceUpdate : MonoBehaviour
{
    public GameObject groupOkCancel;
    public GameObject groupOk;

    public TMP_Text textTitle;
    public TMP_Text textUpdate;
    public TMP_Text textUpdate1;
    public TMP_Text textCancel;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ShowOkCancel()
    {
        groupOkCancel.SetActive(true);
        groupOk.SetActive(false);
    }

    public void ShowOkOnly()
    {
        groupOkCancel.SetActive(false);
        groupOk.SetActive(true);
    }

    public void ButtonUpdate()
    {
        ForceUpdateConfig config = ServerConfig.Instance<ForceUpdateConfig>();
#if UNITY_ANDROID
        if (config.f_core_popupUpdate_url_store_android == "")
        {
            Application.OpenURL("market://details?id=" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_android);
        }
#elif UNITY_IOS
        if (config.f_core_popupUpdate_url_store_ios == "")
        {
            Application.OpenURL("itms-apps://itunes.apple.com/app/" + Application.identifier);
        }
        else
        {
            Application.OpenURL(config.f_core_popupUpdate_url_store_ios);
        }
#endif
    }

    public void ButtonCancel()
    {
        gameObject.SetActive(false);
    }
}