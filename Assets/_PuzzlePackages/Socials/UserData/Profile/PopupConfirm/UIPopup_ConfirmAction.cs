using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPopup_ConfirmAction : MonoBehaviour
{
    public TextMeshProUGUI txtTitle;
    public Text txtContent;
    public Button btnYes;
    public Button btnNo;

    public RectTransform rectWarning;
    public Text txtContentWarning;

    private void Start()
    {
        btnNo.onClick.RemoveAllListeners();
        btnNo.onClick.AddListener(() =>
        {
            GetComponent<UIPopup>().OnClick_CloseThisPopup();
        });
    }

    public void UpdateUI(string strTitle, string strContent, Action onClickYes)
    {
        txtTitle.SetText(strTitle);
        txtContent.text = strContent;
        
        btnYes.onClick.RemoveAllListeners();
        btnYes.onClick.AddListener(() =>
        {
            onClickYes?.Invoke();
            GetComponent<UIPopup>().OnClick_CloseThisPopup();
        });

        rectWarning.gameObject.SetActive(false);
    }

    public void UpdateUI_Warning(string strTitleWarning)
    {
        rectWarning.gameObject.SetActive(true);
        txtContentWarning.text = strTitleWarning;
    }
}
