using ZBase.UnityScreenNavigator.Core.Screens;

namespace ChuongCustom
{
    using System;
    using Cysharp.Threading.Tasks;

    public abstract class BaseScreen : Screen, IScreenPresenter
    {
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Identifier = gameObject.name;
        }
#endif
        public virtual void   Init()      { }
        public virtual void   FirstLook() { }
        public         Action OnClosed    { get; set; }

        protected void CloseView() { WindowManager.Instance.CloseCurrentWindow(); }
        public override UniTask WillPopExit(Memory<object> args)
        {
            OnClosed?.Invoke();
            OnClosed = null;
            return base.WillPopExit(args);
        }
    }

    public abstract class BaseScreen<TData> : BaseScreen, IBindData<TData>
        where TData : IScreenData
    {
        private TData _baseScreenData;
        public  void  BindData(TData data) { _baseScreenData = data; }

        public override void Init() { Init(_baseScreenData); }

        protected abstract void Init(TData data);
    }
}