using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class AddressableHelper
    {
        #region Preload/Load/Release assets

        private static readonly Dictionary<string, AsyncOperationHandle> _operations =
            new Dictionary<string, AsyncOperationHandle>();

        private static readonly Dictionary<string, GameObject> _gameObjects = new Dictionary<string, GameObject>();

        private static void Preload<T>(string path) where T : Object
        {
            if (_operations.ContainsKey(path)) return;

            var handle = Addressables.LoadAssetAsync<T>(path);
            _operations.Add(path, handle);
        }

        private static async Task<T> Load<T>(string path) where T : Object
        {
            Debug.Log($"{typeof(AddressableHelper)} > Load đối tượng với đường dẫn {path}");
            if (!_operations.TryGetValue(path, out AsyncOperationHandle handle))
            {
                handle = Addressables.LoadAssetAsync<T>(path);
                _operations.Add(path, handle);
            }

            await handle.Task;

            if (handle.IsValid())
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result as T;
                }

                Debug.LogError($"Không thể load được asset: {path}");
                Addressables.Release(handle);
                _operations.Remove(path);
                return null;
            }

            // Handle đã bị release, load lại asset
            Debug.LogWarning($"Asset: [{path}] đã bi release trước đó. Đang load lại...");
            _operations.Remove(path);
            return await Load<T>(path);
        }

        public static void PreloadPrefab(string path)
        {
            Preload<GameObject>(path);
        }

        public static async Task<T> LoadPrefab<T>(string path, Transform parent = null) where T : Component
        {
            var obj = await Load<GameObject>(path);
            if (obj == null) return null;
            
            // DebugDictionary();
            
            if (_gameObjects.TryGetValue(path, out GameObject go))
            {
                if (go != null) return go.GetComponent<T>();
                Debug.LogError($"Asset: [{path}] đã được load trước đó nhưng chưa được release");
                return null;
            }

            go = Object.Instantiate(obj, parent);
            go.SetActive(false);
            _gameObjects.Add(path, go);

            return go.GetComponent<T>();
        }
        
        public static void PreloadScriptableObject<T>(string path) where T : ScriptableObject
        {
            Preload<T>(path);
        }

        public static async Task<T> LoadScriptableObject<T>(string path) where T : ScriptableObject
        {
            return await Load<T>(path);
        }

        public static void Release(string path)
        {
            if (_gameObjects.TryGetValue(path, out GameObject go))
            {
                _gameObjects.Remove(path);
            }
            
            if (_operations.TryGetValue(path, out AsyncOperationHandle handle))
            {
                // Release asset và xóa khỏi dictionary
                Addressables.Release(handle);
                _operations.Remove(path);
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy asset cần release hoăc asset đã đươc release trước đó: {path}");
            }
        }

        private static void DebugDictionary()
        {
        // private static readonly Dictionary<string, GameObject> _gameObjects = new Dictionary<string, GameObject>();
        Debug.LogError($"AdrresableHelpep Start Debug: ===========================");
        
        foreach (var key in _gameObjects.Keys)
        {
            Debug.LogError($"key: {key}/n");
        }

        foreach (var value in _gameObjects.Values)
        {
            Debug.LogError($"value: {value}/n");
        }
        
        Debug.LogError($"AdrresableHelpep Finish Debug: ===========================");
        }

        #endregion

#if UNITY_EDITOR
        private static UnityEditor.AddressableAssets.Settings.AddressableAssetSettings CreateDefaultSettings()
        {
            UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings = UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.Create(
                UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.kDefaultConfigFolder,
                UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.kDefaultConfigAssetName, true, true);
            UnityEditor.AssetDatabase.SaveAssets();
            return UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
        }

        private static UnityEditor.AddressableAssets.Settings.AddressableAssetGroup GetOrCreateGroup(string groupName)
        {
            UnityEditor.AddressableAssets.Settings.AddressableAssetSettings settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            UnityEditor.AddressableAssets.Settings.AddressableAssetGroup group = settings.FindGroup(groupName);

            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas);
            }

            return group;
        }
#endif

        public static void MakeAssetAddressable(Object obj, string groupName)
        {
#if UNITY_EDITOR
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
                settings = CreateDefaultSettings();

            var group = GetOrCreateGroup(groupName);
            settings.CreateOrMoveEntry(UnityEditor.AssetDatabase.AssetPathToGUID(path), group);

            UnityEditor.EditorUtility.SetDirty(settings);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}