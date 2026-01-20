using System;
using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{ 
    [HideInInspector]
    public bool Visible;

    [Header("Events")]
    public UnityEvent OnInit;
    public UnityEvent OnShow;
    public UnityEvent OnHide;

    protected bool Initialized;

    /// <summary>
    /// Change the visibilty of the object by playing the desired animation.
    /// </summary>
    /// <param name="visible">Should this element be visible?</param>
    /// <param name="trivial">If true, sounds won't play and events won't fire</param>
    public virtual void ChangeVisibility(bool visible, bool trivial = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Change the visibilty of the object instantly without playing animation.
    /// </summary>
    /// <param name="visible">Should this element be visible?</param>
    /// <param name="trivial">If true, sounds won't play and events won't fire</param>
    public virtual void ChangeVisibilityImmediate(bool visible, bool trivial = false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Change the visibilty of the object by playing the desired animation.
    /// </summary>
    /// <param name="visible">Should this element be visible?</param>
    public void ChangeVisibility(bool visible)
    {
        ChangeVisibility(visible, false);
    }

    /// <summary>
    /// Change the visibilty of the object instantly without playing animation.
    /// </summary>
    /// <param name="visible">Should this element be visible?</param>
    public void ChangeVisibilityImmediate(bool visible)
    {
        ChangeVisibilityImmediate(visible, false);
    }

    /// <summary>
    /// Switch the visibility of the object by playing the desired animation.
    /// </summary>
    public virtual void SwitchVisibility()
    {
        ChangeVisibility(!Visible);
    }

    /// <summary>
    /// Switch the visibility of the object instantly without playing animation.
    /// </summary>
    public virtual void SwitchVisibilityImmediate()
    {
        ChangeVisibilityImmediate(!Visible);
    }

    /// <summary>
    /// Deactivate this element's Game Object.
    /// </summary>
    /// <param name="forceInvisibility">Make sure the element is completly invisible by changing the visiblity immediately.</param>
    protected void DeactivateMe(bool forceInvisibility)
    {
        if (forceInvisibility)
            ChangeVisibilityImmediate(false, true);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Deactivate this element's Game Object.
    /// </summary>
    protected void DeactivateMe()
    {
        gameObject.SetActive(false);
    }
}