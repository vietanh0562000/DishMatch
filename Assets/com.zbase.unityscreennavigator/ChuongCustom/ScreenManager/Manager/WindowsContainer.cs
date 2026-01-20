using System.Collections.Generic;
using System.Linq;
using ChuongCustom.Utils;
using ZBase.UnityScreenNavigator.Core;
using ZBase.UnityScreenNavigator.Core.Activities;
using ZBase.UnityScreenNavigator.Core.Modals;
using ZBase.UnityScreenNavigator.Core.Screens;
using ZBase.UnityScreenNavigator.Core.Views;
using ZBase.UnityScreenNavigator.Core.Windows;

namespace ChuongCustom
{
    using System;
    using UnityEngine;
    
    [DefaultExecutionOrder(-999)]
    [RequireComponent(typeof(RectTransform), typeof(Canvas))]
    public class WindowsContainer : WindowContainerManager
    {
        [SerializeField] private ModalContainer    modalContainer;
        [SerializeField] private ScreenContainer   screenContainer;
        [SerializeField] private ActivityContainer activityContainer;

        public ModalContainer    ModalContainer    => modalContainer;
        public ScreenContainer   ScreenContainer   => screenContainer;
        public ActivityContainer ActivityContainer => activityContainer;

        [SerializeField] private UnityScreenNavigatorSettings unityScreenNavigatorSettings;

        [SerializeField] private WindowContainerSettings windowContainerSettings;

        public UnityScreenNavigatorSettings UnityScreenNavigatorSettings => unityScreenNavigatorSettings;

        public WindowContainerSettings WindowContainerSettings => windowContainerSettings;

        private readonly ScreenAssetLoader        _assetLoader    = new();
        private readonly Dictionary<Type, string> _cachedWindows  = new Dictionary<Type, string>();
        private readonly Stack<Type>              _openingWindows = new Stack<Type>();

        protected sealed override void Awake()
        {
            if (unityScreenNavigatorSettings == false)
            {
                throw new NullReferenceException(nameof(unityScreenNavigatorSettings));
            }

            if (windowContainerSettings == false)
            {
                throw new NullReferenceException(nameof(windowContainerSettings));
            }

            UnityScreenNavigatorSettings.DefaultSettings = unityScreenNavigatorSettings;
        }

        protected sealed override void Start()
        {
            var configs = windowContainerSettings.Containers.Span;

            foreach (var config in configs)
            {
                switch (config.containerType)
                {
                    case WindowContainerType.Modal:
                    {
                        if (modalContainer)
                            modalContainer.UpdateSetting(config, this, unityScreenNavigatorSettings);
                        else
                            modalContainer = ModalContainer.Create(config, this, unityScreenNavigatorSettings);

                        break;
                    }
                    case WindowContainerType.Screen:
                    {
                        if (screenContainer)
                            screenContainer.UpdateSetting(config, this, unityScreenNavigatorSettings);
                        else
                            screenContainer = ScreenContainer.Create(config, this, unityScreenNavigatorSettings);
                        break;
                    }

                    case WindowContainerType.Activity:
                    {
                        if (activityContainer)
                            activityContainer.UpdateSetting(config, this, unityScreenNavigatorSettings);
                        else
                            activityContainer = ActivityContainer.Create(config, this, unityScreenNavigatorSettings);
                        break;
                    }
                }
            }
        }

        public void CloseCurrentWindow(bool playAnimation = true)
        {
            if (IsInTransaction)
                return;

            if (!_openingWindows.TryPop(out var windowType))
            {
                Debug.LogWarning($"There is no window to close");
                return;
            }

            if (windowType.IsSubclassOf(typeof(Modal)))
            {
                ModalContainer.Pop(playAnimation);
            }
            else if (windowType.IsSubclassOf(typeof(ZBase.UnityScreenNavigator.Core.Screens.Screen)))
            {
                ScreenContainer.Pop(playAnimation);
            }
            else
            {
                Debug.LogError($"View type {windowType} is not supported");
            }
        }

        public void Push(Type viewType, PopupAttribute popupAtt, Action<GameObject> onLoad)
        {
            if (IsInTransaction)
                return;

            if (_openingWindows.Contains(viewType))
            {
                Debug.LogWarning($"Window {viewType.Name} is opening, can not push!!");
                return;
            }

            _cachedWindows.TryAdd(viewType, popupAtt.namePath);
            _openingWindows.Push(viewType);

            var viewOption = new ViewOptions(popupAtt.namePath, popupAtt.showAnim, (view, args) => {
                onLoad?.Invoke(view.gameObject);
            });

            _assetLoader.SetLoadType(popupAtt.loadAddressable);

            if (viewType.IsSubclassOf(typeof(Modal)))
            {
                if (popupAtt.closeWhenClickOnBackdrop)
                    ModalContainer.Push(new ModalOptions(viewOption, closeWhenClickOnBackdrop: true));
                else
                    ModalContainer.Push(viewOption);
            }
            else if (viewType.IsSubclassOf(typeof(ZBase.UnityScreenNavigator.Core.Screens.Screen)))
            {
                ScreenContainer.Push(viewOption);
            }
            else if (viewType.IsSubclassOf(typeof(Activity)))
            {
                ActivityContainer.Show(viewOption);
                _openingWindows.Pop();
            }
            else
            {
                Debug.LogError($"View type {viewType.Name} is not supported");
                _openingWindows.Pop();
                _cachedWindows.Remove(viewType);
            }
        }

        private bool IsInTransaction => modalContainer.IsInTransition || screenContainer.IsInTransition;
    }
}