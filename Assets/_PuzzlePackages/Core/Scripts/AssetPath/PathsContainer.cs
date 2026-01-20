using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BasePuzzle.PuzzlePackages.Core
{
    public enum AssetCategory
    {
        Popup,
        Prefab,
        ScriptableObject
    }

    public class PathsContainer : ScriptableObject
    {
        [Serializable]
        private struct PathInfo
        {
            [UnityEngine.UI.Extensions.ReadOnly, SerializeField]
            private string _id;

            [UnityEngine.UI.Extensions.ReadOnly, SerializeField]
            private string _path;

            public string ID => _id;
            public string Path => _path;

            public PathInfo(string id, string path)
            {
                _id = id;
                _path = path;
            }
        }

        //Todo: viết custom editor để không cho thêm, sửa, xóa list trên inspector.
        [SerializeField] private List<PathInfo> _popupPathInfos = new List<PathInfo>();
        [SerializeField] private List<PathInfo> _prefabPathInfos = new List<PathInfo>();
        [SerializeField] private List<PathInfo> _scriptableObjectPathInfos = new List<PathInfo>();

        [SerializeField] private Dictionary<string, string> _dictionary;

        public Dictionary<string, string> Paths
        {
            get
            {
                _dictionary = new Dictionary<string, string>();

                foreach (var info in _popupPathInfos)
                {
                    _dictionary.Add(info.ID, info.Path);
                }

                foreach (var info in _prefabPathInfos)
                {
                    _dictionary.Add(info.ID, info.Path);
                }

                foreach (var info in _scriptableObjectPathInfos)
                {
                    _dictionary.Add(info.ID, info.Path);
                }

                return _dictionary;
            }
        }

        public void Add(AssetCategory category, string id, Object obj)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogError($"{typeof(PathsContainer)} > Id không được null.");
                return;
            }

            if (obj == null)
            {
                Debug.LogError($"{typeof(PathsContainer)} > Object truyền vào không được null.");
                return;
            }

            var path = GetPath(category, obj);
            GetPathInfos(category).Add(new PathInfo(id, path));
        }

        private List<PathInfo> GetPathInfos(AssetCategory category)
        {
            return category switch
            {
                AssetCategory.Popup => _popupPathInfos,
                AssetCategory.Prefab => _prefabPathInfos,
                _ => _scriptableObjectPathInfos
            };
        }

        private static string GetPath(AssetCategory category, Object obj)
        {
            return category switch
            {
                AssetCategory.ScriptableObject => GetScriptableObjectPath(obj),
                _ => GetPrefabPath(obj)
            };
        }

        private static string GetPrefabPath(Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
#endif
            return string.Empty;
        }

        private static string GetScriptableObjectPath(Object obj)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(obj);
#endif
            return string.Empty;
        }

        public void ResetData()
        {
            _popupPathInfos.Clear();
            _prefabPathInfos.Clear();
            _scriptableObjectPathInfos.Clear();
        }
    }
}