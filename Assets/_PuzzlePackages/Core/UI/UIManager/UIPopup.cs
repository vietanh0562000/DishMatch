using DG.Tweening;
using System;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPopup : UIBase
{
    public bool isFullScreen;
    public bool isHalfFullScreen;

    [Header("The Watcher")]
    public bool DeactivateWhileInvisible = true;

    private CanvasGroup canvasGroup;
    private RectTransform viewport;

    /// <summary>
    /// Change the visibilty of the menu by playing the desired animation.
    /// </summary>
    /// <param name="visible">Should this menu be visible or not?</param>
    /// <param name="trivial">If true, sounds won't play and events won't fire</param>
    public override void ChangeVisibility(bool visible, bool trivial = false)
    {
        if (!Initialized)
            InitializeElements();

        if (visible)
        {
            ChangeVisibilityImmediate(visible, true);
            gameObject.SetActive(true);
        }

        Visible = visible;

        if (!trivial)
        {
            if (visible)
            {
                if (OnShow != null)
                    OnShow.Invoke();
            }
            else if (!visible)
            {
                if (OnHide != null)
                    OnHide.Invoke();
            }
        }

        if (DeactivateWhileInvisible)
        {
            if (!visible)
                Invoke("DeactivateMe", 0);
            else
                CancelInvoke("DeactivateMe");
        }
    }

    /// <summary>
    /// Change the visibilty of the menu instantly without playing animation.
    /// </summary>
    /// <param name="visible">Should this menu be visible or not?</param>
    /// <param name="trivial">If true, sounds won't play and events won't fire</param>
    public override void ChangeVisibilityImmediate(bool visible, bool trivial = false)
    {
        if (!Initialized)
            InitializeElements();

        Visible = visible;

        if (!trivial)
        {
            if (visible)
            {
                if (OnShow != null)
                    OnShow.Invoke();
            }
            else if (!visible)
            {
                if (OnHide != null)
                    OnHide.Invoke();
            }
        }

        if (DeactivateWhileInvisible && !visible)
            DeactivateMe(false);
    }

    private void InitializeElements()
    {
        if (Initialized) return;

        OnInit?.Invoke();

        Initialized = true;
    }

    public void OnClick_CloseThisPopup()
    {
        UIManager.Instance.ClosePopup(this);
    }

    public void OnClick_CloseThisPopup(Action onHide)
    {
        DOVirtual.DelayedCall(0.125f, () => onHide?.Invoke());
        UIManager.Instance.ClosePopup(this);
    }

    public void ShowTween()
    {
        Init();
        canvasGroup.interactable = true;
        viewport.DOScale(1, 0.15f).From(1.05f);
        AudioController.PlaySound(SoundKind.UIShowPopup);
    }

    public void HideTween()
    {
        Init();
        canvasGroup.interactable = false;
    }

    private bool init;

    private void Init()
    {
        if (!init)
        {
            init = true;

            canvasGroup = GetComponent<CanvasGroup>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals("Viewport"))
                {
                    viewport = transform.GetChild(i).GetComponent<RectTransform>();
                    break;
                }
            }
        }
    }
}