using UnityEngine.Events;
using System.Collections;
using UnityEngine;

public class UIMenu : UIBase
{
    [Header("The Watcher")]
    public float hidingTime;
    public bool DeactivateWhileInvisible = true;
    public UIMenu PreviousMenu;
    public UIMenu NextMenu;

    [Header("Override Back - Ignore Previous Menu")]
    public bool isOverrideBack;
    public UnityEvent onOverrideBack;

    /// <summary>
    /// Change the visibilty of this menu by playing the desired animation.
    /// </summary>
    /// <param name="visible">Should this menu be visible or not?</param>
    /// <param name="trivial">If true, sounds won't play and events won't fire</param>
    public override void ChangeVisibility(bool visible, bool trivial = false)
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

        //If set to deactivate while invisible, then wait until the total hiding time passes and deactivate.
        if (DeactivateWhileInvisible)
        {
            if (!visible)
                Invoke("DeactivateMe", hidingTime);
            else
                CancelInvoke("DeactivateMe");
        }
    }

    /// <summary>
    /// Change the visibilty of this menu instantly without playing animation.
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

    /// <summary>
    /// Initialize UIElements, call this first if you plan to change visibility in the same frame this menu is activated at.
    /// </summary>
    private void InitializeElements()
    {
        if (Initialized) return;

        OnInit?.Invoke();

        Initialized = true;
    }
}