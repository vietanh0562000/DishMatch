namespace ChuongCustom
{
    using System;

    public interface IScreenPresenter : IInitialize
    {
        void   FirstLook();
        Action OnClosed { get; set; }
    }

    public interface IInitialize
    {
        void Init();
    }
}