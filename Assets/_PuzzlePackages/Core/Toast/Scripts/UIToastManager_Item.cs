using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIToastManager_Item : MonoBehaviour
{
    public RectTransform rectTransform;
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI txtDes;

    public void SetItem(string strDes)
    {
        txtDes.SetText(strDes);

        //Tween
        rectTransform.DOScale(1, 0.25f).From(0);
        canvasGroup.DOFade(1, 0.25f).From(0);
    }

    public void ResetPos()
    {
       
    }

    public void Out(float delay, Action OnComplete)
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0, 0.25f).From(1).SetDelay(delay).OnComplete(() =>
        {
            gameObject.SetActive(false);
            rectTransform.DOAnchorPos3DY(0, 0);
            OnComplete?.Invoke();
        });
    }
}
