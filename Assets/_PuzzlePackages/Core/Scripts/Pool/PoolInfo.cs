using System;
using UnityEngine;
using UnityEngine.Pool;

namespace BasePuzzle.PuzzlePackages.Core
{
    public class PoolGeneric<T> where T : MonoBehaviour
    {
        private readonly ObjectPool<T> _pool;

        public PoolGeneric(ObjectPool<T> pool)
        {
            _pool = pool;
        }

        public T Spawn()
        {
            return _pool.Get();
        }

        public void Release(T element)
        {
            _pool.Release(element);
        }

        public void Clear()
        {
            _pool.Clear();
        }

    }

    public class PoolInfo : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour _prefab;
        [SerializeField] private int _capacity;
        [SerializeField] private int _maxSize;
        [SerializeField] private bool _collectionCheck;

        private bool _isInitialized;
        private Action _onDestroy;

        public void Initialize(MonoBehaviour prefab, int capacity, int maxSize, bool collectionCheck, Action onDestroy)
        {
            if (_isInitialized) return;

            _prefab = prefab;
            _capacity = capacity;
            _maxSize = maxSize;
            _collectionCheck = collectionCheck;
            _onDestroy = onDestroy;

            _isInitialized = true;
        }

        private void OnDestroy()
        {
            _onDestroy?.Invoke();
        }
    }
}