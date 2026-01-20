using ZBase.UnityScreenNavigator.Core.Views;

namespace ChuongCustom
{
    using System;
    using UnityEngine;

    public abstract class APresenter<TView> : IScreenPresenter where TView : IView
    {
        public abstract void   Dispose();
        public abstract void   Init();
        public abstract void   FirstLook();

        public Action OnClosed { get; set; }
    }
}