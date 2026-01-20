using ZBase.UnityScreenNavigator.Core.Modals;

namespace ChuongCustom
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BasePopup : Modal, IScreenPresenter
    {
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Identifier = gameObject.name;
        }
#endif
        public virtual void   Init()      {}
        public virtual void   FirstLook() { }
        public         Action OnClosed    { get; set; }

        public virtual void CloseView()
        {
            WindowManager.Instance.CloseCurrentWindow();
        }
        public override UniTask WillPopExit(Memory<object> args)
        {
            OnClosed?.Invoke();
            OnClosed = null;
            return base.WillPopExit(args);
        }
    }

    public abstract class BasePopup<TData> : BasePopup, IBindData<TData>
        where TData : IScreenData
    {
        private TData _screenData;

        protected TData ScreenData => this._screenData;

        public void BindData(TData data) { this._screenData = data; }

        public override void Init() { Init(this._screenData); }

        protected abstract void Init(TData data);
    }
}