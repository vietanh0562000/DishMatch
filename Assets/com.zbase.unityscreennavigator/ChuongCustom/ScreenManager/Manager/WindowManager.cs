using System;
using System.Reflection;
using ChuongCustom;
using ChuongCustom.ScreenManager;
using UnityEngine;

[DefaultExecutionOrder(-999)]
public class WindowManager : Singleton<WindowManager>
{
    [SerializeField] private WindowsContainer windowsContainer;

    public void OpenWindow<TPresenter, TData>(TData data, bool shouldInitialize = true, Action<TPresenter> onLoaded = null)
        where TPresenter : IScreenPresenter
        where TData : IScreenData
    {
        OpenWindow<TPresenter>(shouldInitialize, presenter =>
        {
            var dataPresenter = presenter as IBindData<TData>;
            dataPresenter?.BindData(data);
            onLoaded?.Invoke(presenter);
        });
    }

    public void OpenTooltip<TPresenter>(Transform rootTrans, bool shouldInitialize = true, Action<TPresenter> onLoaded = null)
        where TPresenter : BaseTooltip
    {
        OpenWindow<TPresenter>(shouldInitialize, presenter =>
        {
            presenter.BindTransform(rootTrans);
            onLoaded?.Invoke(presenter);
        });
    }

    public void OpenWindow<TPresenter>(bool shouldInitialize = true, Action<TPresenter> onLoaded = null) where TPresenter : IScreenPresenter
    {
        var type = typeof(TPresenter);

        var popupAtt = typeof(TPresenter).GetCustomAttribute<PopupAttribute>();

        if (popupAtt != null)
        {
            this.windowsContainer.Push(type, popupAtt,
                onLoad: o =>
                {
                    var presenter = o.GetComponent<TPresenter>();
                    onLoaded?.Invoke(presenter);

                    if (!shouldInitialize) return;
                    presenter?.Init();
                });
        }
        else
        {
            Debug.LogError($"The {type.Name} don't have attribute. Must add PopupAttribute!!");
        }
    }
    
    public void OpenWindow(Type presenterType, bool shouldInitialize = true, Action<IScreenPresenter> onLoaded = null)
    {
        if (!typeof(IScreenPresenter).IsAssignableFrom(presenterType))
        {
            Debug.LogError($"The type {presenterType.Name} does not implement IScreenPresenter");
            return;
        }

        var popupAtt = presenterType.GetCustomAttribute<PopupAttribute>();

        if (popupAtt != null)
        {
            this.windowsContainer.Push(presenterType, popupAtt,
                onLoad: o =>
                {
                    var presenter = o.GetComponent<IScreenPresenter>();
                    onLoaded?.Invoke(presenter);

                    if (!shouldInitialize) return;
                    presenter?.Init();
                });
        }
        else
        {
            Debug.LogError($"The {presenterType.Name} don't have attribute. Must add PopupAttribute!!");
        }
    }

    public void CloseCurrentWindow(bool playAnimation = true) { this.windowsContainer.CloseCurrentWindow(playAnimation); }

    public void CloseActivity(string path, bool playAnimation = true)
    {
        windowsContainer.ActivityContainer.Hide(path, playAnimation);
    }
}