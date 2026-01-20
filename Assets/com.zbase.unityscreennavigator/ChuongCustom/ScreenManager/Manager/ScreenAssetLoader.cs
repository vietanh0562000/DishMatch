using ZBase.UnityScreenNavigator.Foundation.AssetLoaders;

namespace ChuongCustom
{
    using Object = UnityEngine.Object;

    public class ScreenAssetLoader : IAssetLoader
    {
        private bool _isLoadAddressable;

        private readonly AddressableAssetLoader _addressableLoader = new();
        private readonly ResourcesAssetLoader   _resourcesLoader   = new();

        public void SetLoadType(bool loadAddressable)
        {
            this._isLoadAddressable = loadAddressable;
        }
        public AssetLoadHandle<T> Load<T>(string key) where T : Object
        {
            return this._isLoadAddressable ? _addressableLoader.Load<T>(key) : _resourcesLoader.Load<T>(key);
        }
        public AssetLoadHandle<T> LoadAsync<T>(string key) where T : Object
        {
            return this._isLoadAddressable ? _addressableLoader.LoadAsync<T>(key) : _resourcesLoader.LoadAsync<T>(key);
        }
        public void Release(AssetLoadHandleId handle)
        {
            _addressableLoader.Release(handle);
            _resourcesLoader.Release(handle);
        }
    }
}