namespace ChuongCustom.ScreenManager
{
    using System;
    using Core.Utilities.Extension;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using ZBase.UnityScreenNavigator.Core.Activities;

    public class BaseActivity : Activity, IScreenPresenter
    {
        private bool _isActive;

        protected bool IsActive => _isActive;

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Identifier = gameObject.name;
        }
#endif
        public virtual void   Init()      { }
        public virtual void   FirstLook() { }

        public Action OnClosed { get; set; }

        public override void DidEnter(Memory<object> args)
        {
            _isActive = true;
            base.DidEnter(args);
        }

        public override void DidExit(Memory<object> args)
        {
            OnClosed?.Invoke();
            OnClosed  = null;
            _isActive = false;
            base.DidExit(args);
        }

        protected virtual void Update()
        {
            if (Input.GetMouseButtonDown(0) && _isActive && EventSystem.current.IsPointerOverGameObject())
            {
                CloseView();
            }
        }

        protected virtual void CloseView()
        {
            if (IsTransitioning)
            {
                return;
            }
            
            var popupAtt = this.GetCustomAttribute<PopupAttribute>();

            WindowManager.Instance.CloseActivity(popupAtt.namePath);
        }
    }

    public abstract class BaseActivity<TData> : BaseActivity, IBindData<TData>
        where TData : IScreenData
    {
        private TData _screenData;

        protected TData ScreenData => this._screenData;

        public void BindData(TData data) { this._screenData = data; }

        public override void Init() { Init(this._screenData); }

        protected abstract void Init(TData data);
    }
}