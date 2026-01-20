namespace ChuongCustom
{
    public interface IScreenData
    {
    }

    public interface IBindData<in T> where T : IScreenData
    {
        void BindData(T data);
    }
}