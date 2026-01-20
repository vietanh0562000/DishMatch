using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace BasePuzzle.PuzzlePackages.Core
{
    public static class PoolHolder
    {
        private static Transform _transform;

        public static Transform PoolTransform
        {
            get
            {
                if (_transform == null)
                {
                    _transform = new GameObject("Pools").transform;
                }

                return _transform;
            }
        }
    }

    public static class PrefabPool<T> where T : MonoBehaviour
    {
        // private static Transform _parent;
        private static Dictionary<T, PoolGeneric<T>> _poolInfoDict;
        private static Transform _infosHolder;

        private static Transform InfosHolder
        {
            get
            {
                if (_infosHolder != null) return _infosHolder;

                _infosHolder = new GameObject($"{typeof(T).Name}").transform;
                _infosHolder.SetParent(PoolHolder.PoolTransform);

                return _infosHolder;
            }
        }


        public static bool IsCreated(T prefab)
        {
            return _poolInfoDict != null && _poolInfoDict.TryGetValue(prefab, out _);
        }

        /// <summary>
        /// Hàm này có thể tạo ra nhiều pool cho nhiều prefab khác nhau của cùng type T.
        /// </summary>
        /// <param name="prefab">Prefab cần tạo pool</param>
        /// <param name="capacity">Số lượng object được tạo sẵn</param>
        /// <param name="maxSize">Số lượng object tối đa được quản lý bởi pool.
        /// Nếu vượt quá số lượng sễ vẫn tạo object mới nhưng chúng không được quản lý bởi pool (chỉ Destroy và Instantiate) </param>
        /// <param name="collectionCheck">Có kiểm tra tính hợp lệ của object hay không. (object có đúng là được lấy từ pool hay không)</param>
        /// <param name="parent">Object cha dùng để chứa các object sau khi spawn.</param>
        public static PoolGeneric<T> Create(
            T prefab, int capacity, int maxSize, bool collectionCheck, Transform parent = null)
        {
            _poolInfoDict ??= new Dictionary<T, PoolGeneric<T>>();

            if (_poolInfoDict.TryGetValue(prefab, out var poolGeneric))
            {
                Debug.Log(
                    $"Pool cho object [{prefab.name}] đã được tạo trước đó. Việc gọi lại hàm này là không cần thiết.");
                return poolGeneric;
            }

            var pool = new ObjectPool<T>(
                () =>
                {
                    var obj = Object.Instantiate(prefab, parent);
                    return obj;
                },
                t => t.gameObject.SetActive(true),
                t =>
                {
                    t.gameObject.SetActive(false);
                },
                t =>
                {
                    Object.DestroyImmediate(t.gameObject);
                },
                collectionCheck, capacity, maxSize
            );

            poolGeneric = new PoolGeneric<T>(pool);
            _poolInfoDict.Add(prefab, poolGeneric);
            
            var poolInfo = InfosHolder.AddComponent<PoolInfo>();
            poolInfo.Initialize(prefab, capacity, maxSize, collectionCheck, () =>
            {
                _poolInfoDict.Remove(prefab);
            });
            
            return poolGeneric;
        }

        /// <summary>
        /// Nếu số lượng pool của Type T là 1 thì gọi hàm này sẽ không cần phải truyền prefab
        /// </summary>
        public static T Spawn()
        {
            switch (_poolInfoDict.Count)
            {
                case 0:
                    Debug.LogWarning(
                        $"Pool cho type [{typeof(T)}] chưa được khởi tạo. Hãy tạo pool trước khi gọi.");
                    return null;
                case > 1:
                    Debug.LogWarning($"Có nhiều hơn 1 pool thuộc Type [{typeof(T)}], hãy gọi hàm Spawn(T prefab) thay cho hàm này.");
                    return null;
                default:
                    return _poolInfoDict.Values.First().Spawn();
            }
        }

        /// <summary>
        /// Nếu số lượng pool của Type T vượt quá 1 thì bắt buộc phải gọi hàm này mới lấy được đúng pool để spawn object.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static T Spawn(T prefab)
        {
            if (!prefab)
            {
                Debug.LogError($"Prefab truyền vào không được phép null.");
                return null;
            }
            
            if (_poolInfoDict.TryGetValue(prefab, out var poolInfo))
                return poolInfo.Spawn();

            Debug.LogWarning(
                $"Không thể spawn object [{prefab.name}] khi chưa tạo pool cho nó. Gọi hàm Create để tạo pool trước khi sử dụng.");
            return null;
        }

        public static T Spawn(T prefab, Vector3 pos, Quaternion rot, Transform parent)
        {
            var obj = Spawn(prefab);
            obj.transform.SetParent(parent, false);
            obj.transform.SetPositionAndRotation(pos, rot);
            return obj;
        }
        
        /// <summary>
        /// Nếu số lượng pool của Type T là 1 thì gọi hàm này sẽ không cần phải truyền prefab
        /// </summary>
        public static void Release(T releaseObject)
        {
            switch (_poolInfoDict.Count)
            {
                case 0:
                    Debug.LogWarning(
                        $"Pool cho type [{typeof(T)}] chưa được khởi tạo. Hãy tạo pool trước khi gọi.");
                    return;
                case > 1:
                    Debug.LogWarning($"Có nhiều hơn 1 pool thuộc Type [{typeof(T)}], hãy gọi hàm Release(T prefab) thay cho hàm này.");
                    return;
                default:
                    _poolInfoDict.Values.First().Release(releaseObject);
                    return;
            }
        }

        /// <summary>
        /// Nếu số lượng pool của Type T vượt quá 1 thì bắt buộc phải gọi hàm này mới lấy được đúng pool để release object.
        /// </summary>
        /// <param name="releaseObject">Object muốn trả lại pool.</param>
        /// <param name="prefabObject">Prefab của object muốn trả lại pool</param>
        /// <returns></returns>
        public static void Release(T releaseObject, T prefabObject)
        {
            if (_poolInfoDict.TryGetValue(prefabObject, out var poolInfo))
            {
                poolInfo.Release(releaseObject);
                return;
            }

            Debug.LogWarning(
                $"Không thể release object [{releaseObject.name}] khi chưa tạo pool cho nó. Gọi hàm Create để tạo pool trước khi sử dụng.");
        }

        /// <summary>
        /// Destroy toàn bộ inactive object để giải phóng bộ nhớ
        /// </summary>
        /// <param name="prefab"></param>
        public static void Clear(T prefab)
        {
            if (_poolInfoDict.TryGetValue(prefab, out var poolInfo))
            {
                poolInfo.Clear();
                return;
            }
            
            Debug.LogWarning(
                $"Không thể clear Pool: [{prefab.name}] vì pool này chưa được khởi tạo.");
        }

        public static void ClearAll()
        {
            foreach (var poolInfo in _poolInfoDict.Values)
            {
                poolInfo.Clear();
            }
            
            _poolInfoDict.Clear();
        }
    }
}