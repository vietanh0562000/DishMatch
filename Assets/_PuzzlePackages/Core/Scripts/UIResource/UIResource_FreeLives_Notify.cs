using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIResource_FreeLives_Notify : MonoBehaviour
{
    public RectTransform viewport;
    public TextMeshProUGUI txtNumber;

    private void OnEnable()
    {
        UserResourceController.instance.onGetFreeLive += UpdateUI;
        UserResourceController.instance.onReceiveFreeLive += UpdateUI;

        viewport.gameObject.SetActive(false);
        UserResourceController.instance.GetFreeLive();
    }

    private void OnDisable()
    {
        UserResourceController.instance.onGetFreeLive -= UpdateUI;
        UserResourceController.instance.onReceiveFreeLive -= UpdateUI;
    }

    private void UpdateUI()
    {
        var data = UserResourceController.instance.freeLives;
        var isActive = data != null && data.Count > 0;

        viewport.gameObject.SetActive(isActive);
        if (isActive)
        {
            txtNumber.SetText($"{data.Count}");
        }
    }
}
