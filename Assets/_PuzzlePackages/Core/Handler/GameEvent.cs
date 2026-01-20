using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BasePuzzle.PuzzlePackages.Core
{
    /// <summary>
    /// Simple Event System
    /// </summary>
    /// <author>minhddv@onesoft.com.vn</author>
    public static class GameEvent<T>
    {
        private static readonly Dictionary<string, HashSet<Action<T>>> _listenerDict = new();

        /// <summary>
        /// Register an event.
        /// </summary>
        /// <param name="eventName">Event name. Better to using a constant string.</param>
        /// <param name="action">The action is called when event should be raised</param>
        /// <param name="listener">The listener component. This parameter should be null if the listener is not a component. </param>
        public static void Register(string eventName, Action<T> action, Component listener)
        {
            if (_listenerDict.ContainsKey(eventName) && _listenerDict[eventName].Contains(action))
            {
                Debug.LogError("This action has already registered for this event");
                return;
            }

            if (_listenerDict.TryGetValue(eventName, out var value))
            {
                value.Add(action);
            }
            else
            {
                var hashSet = new HashSet<Action<T>> { action };
                _listenerDict.Add(eventName, hashSet);
            }

#if UNITY_EDITOR
            if (listener != null)
                GameEventViewer.OnRegisterEvent(eventName, listener);
#endif
        }

        /// <summary>
        /// This action cannot be null.
        /// <param name="listener">The listener component. This parameter should be null if the listener is not a component. </param>
        /// </summary>
        public static void Unregister(string eventName, Action<T> action, Component listener)
        {
            if (!_listenerDict.ContainsKey(eventName))
            {
                Debug.LogWarning($"This event: {eventName} has no register yet!");
                return;
            }

            if (!_listenerDict[eventName].Contains(action))
            {
                Debug.LogError($"This event: {eventName} doesn't contain this action!");
                return;
            }

            _listenerDict[eventName].Remove(action);

#if UNITY_EDITOR
            if(listener != null)
                GameEventViewer.OnUnregisterEvent(eventName, listener);
#endif
        }

        public static void Emit(string eventName, T data = default)
        {
            if (!_listenerDict.ContainsKey(eventName))
            {
                Debug.LogError($"This event: {eventName} has no register yet!");
                return;
            }
        
            foreach (var listener in _listenerDict[eventName])
            {
                listener.Invoke(data);
            }
        }
    }

#if UNITY_EDITOR
    public class EventViewer : MonoBehaviour
    {
        [SerializeField] private List<Component> _listeners = new();

        public int ListenerCount => _listeners.Count;

        public void AddListener(Component component)
        {
            _listeners.Add(component);
        }

        public void RemoveListener(Component component)
        {
            _listeners.Remove(component);
        }

        public void Destroy()
        {
            if(gameObject != null) Destroy(gameObject);
        }
    }

    public static class GameEventViewer
    {
        private static GameObject _gameObject;
        private static readonly Dictionary<string, EventViewer> _viewers = new();

        public static void OnRegisterEvent(string eventName, Component listener)
        {
            if (_gameObject == null)
            {
                _gameObject = new GameObject("GameEventViewer");
                Object.DontDestroyOnLoad(_gameObject);
            }

            if (_viewers.TryGetValue(eventName, out var viewer1))
            {
                viewer1.AddListener(listener);
            }
            else
            {
                var go = new GameObject(eventName);
                go.transform.SetParent(_gameObject.transform);
                var viewer = go.AddComponent<EventViewer>();
                viewer.AddListener(listener);
                _viewers.Add(eventName, viewer);
            }
        }

        public static void OnUnregisterEvent(string eventName, Component listener)
        {
            _viewers[eventName].RemoveListener(listener);

            if (_viewers[eventName].ListenerCount > 0 || _viewers[eventName] == null) return;
            _viewers[eventName].Destroy();
            _viewers.Remove(eventName);
        }
    }
#endif
}