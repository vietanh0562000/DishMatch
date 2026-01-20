using System.Collections.Generic;
using UnityEngine;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class TempDataHandler
    {
        #region Object Type

        private static readonly Dictionary<string, object> _objDict = new Dictionary<string, object>();

        public static void Set<T>(string key, T data) where T : class
        {
            if (!_objDict.ContainsKey(key))
            {
                _objDict.Add(key, data);
                return;
            }

            _objDict[key] = data;
        }
        
        public static T Get<T>(string key) where T : class
        {
            if (_objDict.TryGetValue(key, out var value)) return value as T;
            
            Debug.LogError($"Data với key: {key} chưa được tạo");
            return null;
        }

        #endregion

        #region int

        private static readonly Dictionary<string, int> _intDict = new Dictionary<string, int>();

        public static void Set(string key, int data)
        {
            if (!_intDict.ContainsKey(key))
            {
                _intDict.Add(key, data);
                return;
            }

            _intDict[key] = data;
        }
        
        public static int Get(string key, int defautlValue)
        {
            if (_intDict.TryGetValue(key, out var value)) return value;
            
            Debug.LogError($"Data với key: {key} chưa được tạo");
            return defautlValue;
        }

        #endregion

        #region float

        private static readonly Dictionary<string, float> _floatDict = new Dictionary<string, float>();

        public static void Set(string key, float data)
        {
            if (!_floatDict.ContainsKey(key))
            {
                _floatDict.Add(key, data);
                return;
            }

            _floatDict[key] = data;
        }
        
        public static float Get(string key, float defautlValue)
        {
            if (_floatDict.TryGetValue(key, out var value)) return value;
            
            Debug.LogError($"Data với key: {key} chưa được tạo");
            return defautlValue;
        }

        #endregion
        
        #region bool
        private static readonly Dictionary<string, bool> _boolDict = new Dictionary<string, bool>();

        public static void Set(string key, bool data)
        {
            if (!_boolDict.ContainsKey(key))
            {
                _boolDict.Add(key, data);
                return;
            }

            _boolDict[key] = data;
        }
        
        public static bool Get(string key, bool defautlValue)
        {
            if (_boolDict.TryGetValue(key, out var value)) return value;
            
            Debug.LogError($"Data với key: {key} chưa được tạo");
            return defautlValue;
        }
        #endregion

    }
}
