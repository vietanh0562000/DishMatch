using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace UnityToolbag
{
    // Modification - Tangled Reality Studios LLC - 8/18/18 - Add ExecuteInEditMode
    [ExecuteInEditMode]

    /// <summary>
    /// A system for dispatching code to execute on the main thread.
    /// </summary>
    [AddComponentMenu("UnityToolbag/Dispatcher")]
    public class Dispatcher : MonoBehaviour
    {
        private static Dispatcher _instance;

        // We can't use the behaviour reference from other threads, so we use a separate bool
        // to track the instance so we can use that on the other threads.
        private static bool _instanceExists;

        private static Thread _mainThread;
        private static object _lockObject = new object();
        private static readonly Queue<Action> _actions = new Queue<Action>();

        /// <summary>
        /// Gets a value indicating whether or not the current thread is the game's main thread.
        /// </summary>
        public static bool isMainThread
        {
            get
            {
                return Thread.CurrentThread == _mainThread;
            }
        }

        // Modification - Tangled Reality Studios LLC - 8/18/18 - Added Prepare funciton
        public static void Prepare(GameObject gameObject = null)
        {
            if (!_instanceExists)
            {
                if (gameObject == null)
                    gameObject = new GameObject { hideFlags = HideFlags.HideAndDontSave };
                else
                    _instance = gameObject.GetComponent<Dispatcher>();
                if (_instance == null)
                    _instance = gameObject.AddComponent<Dispatcher>();
                _instanceExists = true;
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        /// Modification - Tangled Reality Studios LLC - 4/22/22 - Make isMainThread check first.
        public static void InvokeAsync(Action action)
        {
            if (isMainThread)
            {
                // Don't bother queuing work on the main thread; just execute it.
                action();
                return;
            }

            if (!_instanceExists)
            {
                Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
                return;
            }

            lock (_lockObject)
            {
                _actions.Enqueue(action);
            }
        }

        /// <summary>
        /// Queues an action to be invoked on the main game thread and blocks the
        /// current thread until the action has been executed.
        /// </summary>
        /// <param name="action">The action to be queued.</param>
        /// Modification - Tangled Reality Studios LLC - 4/22/22 - Handle !_instanceExists differently.
        public static void Invoke(Action action)
        {
            if (!_instanceExists)
            {
                Debug.LogError("No Dispatcher exists in the scene. Actions will not be invoked!");
                return;
            }

            bool hasRun = false;

            InvokeAsync(() =>
            {
                action();
                hasRun = true;
            });

            // Lock until the action has run
            while (!hasRun)
            {
                Thread.Sleep(5);
            }
        }

        void Awake()
        {
            if (_instance)
            {
                DestroyImmediate(this);
            }
            else
            {
                _instance = this;
                _instanceExists = true;
                _mainThread = Thread.CurrentThread;
            }
        }

        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                _instanceExists = false;
            }
        }

        void Update()
        {
            lock (_lockObject)
            {
                while (_actions.Count > 0)
                {
                    _actions.Dequeue()();
                }
            }
        }
    }
}
